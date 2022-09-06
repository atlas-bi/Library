using System;

namespace web.Tests.BrowserTests;

public class BrowserFixtureOptions
{
    // model to hold options
    public string Browser { get; set; } = "chrome";
    public string BrowserVersion { get; set; } = "latest";
    public string Orientation { get; set; } = "landscape";
    public string OperatingSystem { get; set; } = "Windows";
    public string OperatingSystemVersion { get; set; } = "10";
    public string BuildName { get; set; } = "atlas-bi-library-local";
    public string ProjectName { get; set; } = "local";
    public (string UserName, string AccessKey) BrowserStackCredentials { get; set; }
    public Uri BrowserStackEndpoint { get; set; } =
        new("https://hub-cloud.browserstack.com/wd/hub/");
}
