using System.Text.Json;
using System.Text.RegularExpressions;
using MatchEdge.Application.Clients.FootyMetrics;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace MatchEdge.Infrastructure.Clients;

public sealed class FmNavigator : IAsyncDisposable
{
    public const int MaxNavigationsPerRun = 40;
    private const int MaxAttempts = 3; // initial + 2 retries
    private static readonly TimeSpan MinGap = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan MaxGap = TimeSpan.FromSeconds(4);

    private readonly FootyMetricsBrowserManager _browserManager;
    private readonly Microsoft.Extensions.Logging.ILogger<FmNavigator> _logger;
    private readonly Random _rng = new();
    private int _navigationsSinceReset;
    private DateTime _lastNavUtc = DateTime.MinValue;

    private SemaphoreSlim _gate => _browserManager.NavigationGate;

    public int NavigationsSinceReset => Volatile.Read(ref _navigationsSinceReset);

    public FmNavigator(
        FootyMetricsBrowserManager browserManager,
        Microsoft.Extensions.Logging.ILogger<FmNavigator> logger)
    {
        _browserManager = browserManager;
        _logger = logger;
    }

    public void ResetRun()
    {
        Interlocked.Exchange(ref _navigationsSinceReset, 0);
    }

    public Task<FmNavOutcome> GoToAsync(
        string url,
        FmReadySignal readySignal,
        TimeSpan? timeout = null,
        CancellationToken ct = default)
        => GoToAsync(url, readySignal, null, timeout, ct);

    /// <param name="captureWhileLocked">
    /// Optional capture executed while the navigation gate is still held (e.g. reading
    /// page HTML). Prevents the background pipeline from navigating between Goto and
    /// capture. Playwright errors here are mapped to PageNotReady so the retry loop applies.
    /// </param>
    public async Task<FmNavOutcome> GoToAsync(
        string url,
        FmReadySignal readySignal,
        Func<FmNavOutcome, CancellationToken, Task<FmNavOutcome>>? captureWhileLocked,
        TimeSpan? timeout = null,
        CancellationToken ct = default)
    {
        var effectiveTimeout = timeout ?? TimeSpan.FromSeconds(20);
        await _gate.WaitAsync(ct);
        try
        {
            if (NavigationsSinceReset >= MaxNavigationsPerRun)
                throw new FmNavigationException(
                    FmNavigationErrorCode.NavigationBudgetExceeded,
                    $"Navigation budget exceeded ({MaxNavigationsPerRun} per run).");

            var sinceLast = DateTime.UtcNow - _lastNavUtc;
            if (_lastNavUtc != DateTime.MinValue && sinceLast < MinGap)
            {
                var extra = RandomGapMs();
                await Task.Delay(MinGap - sinceLast + TimeSpan.FromMilliseconds(extra), ct);
            }

            var page = _browserManager.GetPage();
            if (page == null)
                throw new FmNavigationException(
                    FmNavigationErrorCode.BrowserNotReady, "Browser not started.");

            Exception? lastError = null;
            for (var attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                ct.ThrowIfCancellationRequested();
                if (NavigationsSinceReset >= MaxNavigationsPerRun)
                    throw new FmNavigationException(
                        FmNavigationErrorCode.NavigationBudgetExceeded,
                        $"Navigation budget exceeded ({MaxNavigationsPerRun} per run).");
                try
                {
                    var outcome = await AttemptAsync(page, url, readySignal, effectiveTimeout, ct);
                    _lastNavUtc = DateTime.UtcNow;
                    if (captureWhileLocked != null)
                    {
                        try
                        {
                            outcome = await captureWhileLocked(outcome, ct);
                        }
                        catch (PlaywrightException ex)
                        {
                            throw new FmNavigationException(
                                FmNavigationErrorCode.PageNotReady,
                                $"Post-navigation capture failed for {url}.", ex);
                        }
                    }
                    return outcome;
                }
                catch (FmNavigationException ex) when (
                    ex.Code is FmNavigationErrorCode.Timeout or FmNavigationErrorCode.PageNotReady
                             or FmNavigationErrorCode.NoData
                    && attempt < MaxAttempts)
                {
                    lastError = ex;
                    var backoffMs = attempt * 2000 + RandomGapMs();
                    _logger.LogWarning(
                        "FmNavigator attempt {Attempt}/{Max} failed for {Url}: {Code}. Backoff {Ms} ms.",
                        attempt, MaxAttempts, url, ex.Code, backoffMs);
                    await Task.Delay(backoffMs, ct);
                }
            }

            _lastNavUtc = DateTime.UtcNow;
            throw lastError ?? new FmNavigationException(
                FmNavigationErrorCode.PageNotReady, $"Navigation failed for {url}.");
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<FmNavOutcome> AttemptAsync(
        IPage page,
        string url,
        FmReadySignal readySignal,
        TimeSpan timeout,
        CancellationToken ct)
    {
        TaskCompletionSource<(string Url, string Body)>? responseTcs = null;
        EventHandler<IResponse>? handler = null;

        if (readySignal.Kind == FmReadySignalKind.ResponsePattern)
        {
            var regex = new Regex(readySignal.Pattern, RegexOptions.Compiled);
            responseTcs = new TaskCompletionSource<(string, string)>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            handler = async (_, response) =>
            {
                try
                {
                    if (!regex.IsMatch(response.Url)) return;
                    var body = await response.TextAsync();
                    responseTcs.TrySetResult((response.Url, body ?? string.Empty));
                }
                catch { /* body may be unavailable; keep waiting */ }
            };
            page.Response += handler;
        }

        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            try
            {
                await page.GotoAsync(url, new PageGotoOptions
                {
                    Timeout = (float)timeout.TotalMilliseconds,
                    WaitUntil = WaitUntilState.DOMContentLoaded
                });
            }
            catch (PlaywrightException ex) when (IsTimeout(ex))
            {
                Interlocked.Increment(ref _navigationsSinceReset);
                throw new FmNavigationException(
                    FmNavigationErrorCode.Timeout, $"Navigation timeout for {url}.", ex);
            }

            Interlocked.Increment(ref _navigationsSinceReset);

            var remaining = timeout - sw.Elapsed;
            if (remaining < TimeSpan.FromMilliseconds(500))
                remaining = TimeSpan.FromMilliseconds(500);

            if (responseTcs != null)
            {
                (string respUrl, string body) resp;
                try
                {
                    resp = await responseTcs.Task.WaitAsync(remaining, ct);
                }
                catch (TimeoutException ex)
                {
                    throw new FmNavigationException(
                        FmNavigationErrorCode.Timeout,
                        $"Ready signal (response {readySignal.Pattern}) not seen for {url}.", ex);
                }
                if (string.IsNullOrWhiteSpace(resp.body))
                    throw new FmNavigationException(
                        FmNavigationErrorCode.NoData, $"Empty response body for {url}.");

                await EnsureSessionAliveAsync(page, ct);
                return new FmNavOutcome(page.Url, resp.body, resp.respUrl);
            }

            try
            {
                await page.WaitForSelectorAsync(readySignal.Pattern, new PageWaitForSelectorOptions
                {
                    Timeout = (float)remaining.TotalMilliseconds,
                    State = WaitForSelectorState.Attached
                });
            }
            catch (PlaywrightException ex) when (IsTimeout(ex))
            {
                throw new FmNavigationException(
                    FmNavigationErrorCode.PageNotReady,
                    $"Ready selector '{readySignal.Pattern}' not attached for {url}.", ex);
            }

            await EnsureSessionAliveAsync(page, ct);
            return new FmNavOutcome(page.Url, null, null);
        }
        finally
        {
            if (handler != null) page.Response -= handler;
        }
    }

    private static async Task EnsureSessionAliveAsync(IPage page, CancellationToken ct)
    {
        var currentUrl = page.Url;
        if (LooksLikeLoginUrl(currentUrl))
            throw new FmNavigationException(
                FmNavigationErrorCode.SessionExpired,
                $"Redirected to login: {currentUrl}");

        var text = await page.EvaluateAsync<string>(
            "() => document.body?.innerText?.substring(0, 800) || ''");
        if (LooksLikeSignedOutText(text))
            throw new FmNavigationException(
                FmNavigationErrorCode.SessionExpired,
                "Sign-in markers present in page content.");
    }

    public static bool LooksLikeLoginUrl(string? url) =>
        !string.IsNullOrEmpty(url) && url.Contains("/login", StringComparison.OrdinalIgnoreCase);

    public static bool LooksLikeSignedOutText(string? text) =>
        !string.IsNullOrEmpty(text)
        && text.Contains("sign in", StringComparison.OrdinalIgnoreCase);

    private static bool IsTimeout(PlaywrightException ex) =>
        ex.Message.Contains("Timeout", StringComparison.OrdinalIgnoreCase);

    private int RandomGapMs() => _rng.Next(2000, 4001);

    public async ValueTask DisposeAsync()
    {
        await Task.CompletedTask;
    }
}
