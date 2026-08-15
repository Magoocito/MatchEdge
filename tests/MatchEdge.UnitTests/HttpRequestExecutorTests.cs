using MatchEdge.Infrastructure.Clients;

namespace MatchEdge.UnitTests;

public class HttpRequestExecutorTests
{
    [Fact]
    public void BuildCurlArguments_WithHeaders_IncludesHFlags()
    {
        var url = "https://www.sofascore.com/api/v1/unique-tournament/406/seasons";
        var headers = new Dictionary<string, string>
        {
            ["x-captcha"] = "my_jwt_token",
            ["x-requested-with"] = "e24dd0"
        };

        var result = HttpRequestExecutor.BuildCurlArguments(url, headers);

        Assert.Contains("-H \"x-captcha: my_jwt_token\"", result);
        Assert.Contains("-H \"x-requested-with: e24dd0\"", result);
        Assert.Contains("-s", result);
        Assert.Contains(url, result);
    }

    [Fact]
    public void BuildCurlArguments_WithEmptyHeaders_DoesNotIncludeHFlags()
    {
        var url = "https://www.sofascore.com/api/v1/unique-tournament/406/seasons";
        var headers = new Dictionary<string, string>();

        var result = HttpRequestExecutor.BuildCurlArguments(url, headers);

        Assert.DoesNotContain("-H", result);
        Assert.Contains("-s", result);
        Assert.Contains(url, result);
    }

    [Fact]
    public void BuildCurlArguments_WithSingleHeader_IncludesOneHFlag()
    {
        var url = "https://www.sofascore.com/api/v1/seasons";
        var headers = new Dictionary<string, string>
        {
            ["x-captcha"] = "token123"
        };

        var result = HttpRequestExecutor.BuildCurlArguments(url, headers);

        Assert.Contains("-H \"x-captcha: token123\"", result);
        Assert.DoesNotContain("-H \"x-requested-with", result);
        Assert.Contains("-s", result);
        Assert.Contains(url, result);
    }

    [Fact]
    public void BuildCurlArguments_WithHeaders_CorrectFormat()
    {
        var url = "https://www.sofascore.com/api/v1/seasons";
        var headers = new Dictionary<string, string>
        {
            ["x-captcha"] = "test_token",
            ["x-requested-with"] = "abc123"
        };

        var result = HttpRequestExecutor.BuildCurlArguments(url, headers);

        // Verify format: -s -H "key: value" -H "key: value" "url"
        Assert.Equal(
            $"-s -H \"x-captcha: test_token\" -H \"x-requested-with: abc123\" \"{url}\"",
            result);
    }

    [Fact]
    public void BuildCurlArguments_WithoutHeaders_CorrectFormat()
    {
        var url = "https://www.sofascore.com/api/v1/seasons";

        var result = HttpRequestExecutor.BuildCurlArguments(url, new Dictionary<string, string>());

        Assert.Equal($"-s \"{url}\"", result);
    }

    [Fact]
    public void BuildCurlArguments_WithOnlyEmptyValues_IncludesHFlags()
    {
        var url = "https://www.sofascore.com/api/v1/seasons";
        var headers = new Dictionary<string, string>
        {
            ["x-captcha"] = "",
            ["x-requested-with"] = ""
        };

        // Empty values should still be included (the caller decided to add them)
        // The filtering happens when reading from configuration
        var result = HttpRequestExecutor.BuildCurlArguments(url, headers);

        Assert.Contains("-H \"x-captcha: \"", result);
        Assert.Contains("-H \"x-requested-with: \"", result);
    }

    [Fact]
    public void BuildCurlArguments_SpecialCharactersInToken_EscapedCorrectly()
    {
        var url = "https://www.sofascore.com/api/v1/seasons";
        var headers = new Dictionary<string, string>
        {
            ["x-captcha"] = "token/with=special+chars&more"
        };

        var result = HttpRequestExecutor.BuildCurlArguments(url, headers);

        Assert.Contains("-H \"x-captcha: token/with=special+chars&more\"", result);
    }
}
