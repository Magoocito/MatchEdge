namespace MatchEdge.Application.Clients.FootyMetrics;

public sealed record FmFixtureRef(
    string FixtureId,
    string Url,
    string? Home,
    string? Away,
    DateTime? KickoffUtc,
    string? Competition);

public enum FmNavigationErrorCode
{
    BrowserNotReady,
    SessionExpired,
    PageNotReady,
    NoData,
    Timeout,
    NavigationBudgetExceeded
}

public sealed class FmNavigationException : Exception
{
    public FmNavigationErrorCode Code { get; }

    public FmNavigationException(FmNavigationErrorCode code, string message, Exception? inner = null)
        : base(message, inner)
    {
        Code = code;
    }
}

public sealed class FmResolverException : Exception
{
    public FmResolverException(string message) : base(message) { }
}

public sealed class FmFixtureNotFoundException : Exception
{
    public FmFixtureNotFoundException(string message) : base(message) { }
}

public sealed record FmReadySignal(FmReadySignalKind Kind, string Pattern);

public enum FmReadySignalKind
{
    ResponsePattern,
    DomSelector
}

public sealed record FmNavOutcome(string Url, string? ResponseBody, string? ResponseUrl);

public sealed record FmTabOutcome(
    string Tab,
    string Status,
    int Signals,
    string? RawPath,
    string? Error);

public sealed record FmSnapshotOutcome(
    string FixtureId,
    IReadOnlyList<FmTabOutcome> Tabs,
    IReadOnlyList<string> RawPaths,
    IReadOnlyList<string> Warnings,
    bool Partial);

public interface IFmFixtureResolver
{
    Task<IReadOnlyList<FmFixtureRef>> ResolveAsync(
        string league,
        DateOnly date,
        CancellationToken ct = default);

    Task<FmFixtureRef> DescribeAsync(string url, CancellationToken ct = default);
}

public interface IFmFixtureSnapshotService
{
    Task<FmSnapshotOutcome> SnapshotAsync(FmFixtureRef fixture, CancellationToken ct = default);
}
