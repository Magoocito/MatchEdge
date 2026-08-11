using MatchEdge.Infrastructure.Clients;

namespace MatchEdge.UnitTests.Fakes;

public class FakeHttpRequestExecutor : IHttpRequestExecutor
{
    private readonly Func<string, string> _responseFunc;
    public int CallCount { get; private set; }

    public FakeHttpRequestExecutor(Func<string, string> responseFunc)
    {
        _responseFunc = responseFunc;
    }

    public Task<string> ExecuteCurlAsync(string url)
    {
        CallCount++;
        return Task.FromResult(_responseFunc(url));
    }
}
