using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace web.Tests.BrowserTests;

public sealed class BrowsersTestData : IEnumerable<object[]>
{
    public static bool IsRunningInGitHubActions { get; } =
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));

    public static bool UseBrowserStack => BrowserStackCredentials() != default;

    public static (string UserName, string AccessToken) BrowserStackCredentials()
    {
        var root = Directory.GetCurrentDirectory();

        var dotenv = Path.Combine(root, ".env");
        DotEnv.Load(dotenv);

        string? userName = Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME");
        string? accessToken = Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY");

        Assert.NotNull(userName);
        Assert.NotNull(accessToken);

        return (userName, accessToken);
    }

    public static string ProjectName()
    {
        string? project = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");

        if (!string.IsNullOrEmpty(project))
        {
            return project.Split("/")[1];
        }

        return "atlas-bi-library-local";
    }

    public static string BuildName()
    {
        string? run = Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER");

        if (!string.IsNullOrEmpty(run))
        {
            return run;
        }
        return "local";
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        // Find configs here.
        // https://www.browserstack.com/automate/capabilities?tag=selenium-4

        // Find run output here.
        // https://automate.browserstack.com/dashboard/v2/builds/

        // browser configuration that we want to mass test with.
        yield return new[]
        {
            "chrome",
            "latest",
            "Windows",
            "10",
            "landscape", // or portrait
        };

        yield return new[]
        {
            "ie",
            "11",
            "Windows",
            "10", // uses edge in compat mode for IE 11, but this is the only way to get IE 11 console.
            "landscape", // or portrait
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
