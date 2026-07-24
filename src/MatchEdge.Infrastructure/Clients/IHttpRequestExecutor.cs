namespace MatchEdge.Infrastructure.Clients;

public interface IHttpRequestExecutor
{
    Task<string> ExecuteCurlAsync(string url);
}
