using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace MatchEdge.Infrastructure.Clients;

/// <summary>
/// TEMPORARY WORKAROUND — SofaScore anti-bot bypass.
///
/// This executor reads custom headers from configuration and passes them to
/// curl.exe via the -H flag to bypass SofaScore's Cloudflare protection.
///
/// WHY THIS EXISTS:
/// SofaScore's API is protected by Cloudflare bot protection that requires
/// a JavaScript-generated x-captcha JWT token and x-requested-with header.
/// Neither curl.exe nor .NET HttpClient can generate these tokens.
/// The only workaround is to manually copy the headers from a real browser
/// session and provide them here.
///
/// LIMITATIONS (THIS IS A TEMPORARY PATCH):
/// - The x-captcha JWT token expires (typically hours).
/// - The token is tied to the IP address that generated it.
/// - This is NOT a production-ready solution.
/// - If this becomes a frequent problem, evaluate alternative data sources
///   instead of continuing to patch around SofaScore's bot protection.
///
/// SETUP:
/// 1. Open SofaScore in your browser, open DevTools > Network tab.
/// 2. Find any request to sofascore.com/api, copy these headers:
///    - x-captcha (the full JWT value)
///    - x-requested-with (short string)
/// 3. Store via User Secrets:
///      dotnet user-secrets set "SofaScore:Headers:x-captcha" "YOUR_JWT_VALUE"
///      dotnet user-secrets set "SofaScore:Headers:x-requested-with" "YOUR_VALUE"
///    Or via environment variables:
///      SofaScore__Headers__x-captcha=YOUR_JWT_VALUE
///      SofaScore__Headers__x-requested-with=YOUR_VALUE
/// 4. The token will expire — repeat when requests start failing with 403.
/// </summary>
public class HttpRequestExecutor : IHttpRequestExecutor
{
    private readonly Dictionary<string, string> _customHeaders;

    public HttpRequestExecutor(IConfiguration configuration)
    {
        _customHeaders = new Dictionary<string, string>();

        var headersSection = configuration.GetSection("SofaScore:Headers");
        foreach (var child in headersSection.GetChildren())
        {
            if (!string.IsNullOrWhiteSpace(child.Value))
            {
                _customHeaders[child.Key] = child.Value!;
            }
        }
    }

    public async Task<string> ExecuteCurlAsync(string url)
    {
        var arguments = BuildCurlArguments(url, _customHeaders);

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "curl.exe",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var stdout = await process.StandardOutput.ReadToEndAsync();
        var stderr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new HttpRequestException(
                $"curl.exe falló con exit code {process.ExitCode}: {stderr}");
        }

        return stdout;
    }

    /// <summary>
    /// Builds the curl.exe argument string. Separated for testability.
    /// Adds -H flags for each custom header when present.
    /// </summary>
    internal static string BuildCurlArguments(string url, Dictionary<string, string> customHeaders)
    {
        var arguments = new List<string> { "-s" };

        foreach (var (key, value) in customHeaders)
        {
            arguments.Add($"-H \"{key}: {value}\"");
        }

        arguments.Add($"\"{url}\"");

        return string.Join(" ", arguments);
    }
}
