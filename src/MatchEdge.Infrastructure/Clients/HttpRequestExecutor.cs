using System.Diagnostics;

namespace MatchEdge.Infrastructure.Clients;

public class HttpRequestExecutor : IHttpRequestExecutor
{
    public async Task<string> ExecuteCurlAsync(string url)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "curl.exe",
                Arguments = $"-s \"{url}\"",
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
}
