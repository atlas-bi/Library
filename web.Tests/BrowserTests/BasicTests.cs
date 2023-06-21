using Xunit;
using Xunit.Abstractions;
using System.Threading.Tasks;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Collections.Generic;
using BrowserStack;
using MartinCostello.BrowserStack.Automate;
using Newtonsoft.Json.Linq;

namespace web.Tests.BrowserTests;

public class BasicTests : IClassFixture<BrowserFactory<Program>>
{
    private readonly BrowserFactory<Program> _factory;

    private readonly Uri _baseAddress;

    public BasicTests(ITestOutputHelper outputHelper, BrowserFactory<Program> factory)
    {
        OutputHelper = outputHelper;
        _factory = factory;
        _baseAddress = factory.BaseAddress;
    }

    private ITestOutputHelper OutputHelper { get; }

    [Theory]
    [ClassData(typeof(BrowsersTestData))]
    public async Task Test(
        string browser,
        string version,
        string os,
        string os_version,
        string orientation
    )
    {
        var options = new BrowserFixtureOptions()
        {
            Browser = browser,
            BrowserVersion = version,
            OperatingSystem = os,
            OperatingSystemVersion = os_version,
            Orientation = orientation,
            ProjectName = BrowsersTestData.ProjectName(),
            BuildName = BrowsersTestData.BuildName(),
            BrowserStackCredentials = BrowsersTestData.BrowserStackCredentials()
        };

        Local local = new Local();

        List<KeyValuePair<string, string>> bsLocalArgs = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("key", options.BrowserStackCredentials.AccessKey),
            new KeyValuePair<string, string>("verbose", "true"),
        };

        local.start(bsLocalArgs);

        local.isRunning();

        var capabilities = new BrowserFixture(options, OutputHelper);

        List<string> urls =
            new()
            {
                "",
                "Collections/New",
                "Initiatives",
                "Initiatives/New",
                "Terms",
                "Terms/New",
                "Search",
                "Search?Query=test",
                "about_analytics",
                "Settings",
                "Analytics",
                "Users",
                "Users?id=2",
                "tasks"
            };

        IWebDriver driver = new RemoteWebDriver(
            options.BrowserStackEndpoint,
            capabilities.BuildBrowser()
        );
        foreach (string url in urls)
        {
            driver.Navigate().GoToUrl(_baseAddress.ToString() + url);
        }
        // get session details
        string? sessionObject = Convert.ToString(
            ((IJavaScriptExecutor)driver).ExecuteScript(
                "browserstack_executor: {\"action\": \"getSessionDetails\"}"
            )
        );

        Assert.NotNull(sessionObject);

        var session_details = JObject.Parse(sessionObject);

        driver.Quit();
        local.stop();

        var client = new BrowserStackAutomateClient(
            options.BrowserStackCredentials.UserName,
            options.BrowserStackCredentials.AccessKey
        );

        var consoleLogs = await client.GetSessionConsoleLogsAsync(
            session_details.Value<string>("build_hashed_id"),
            session_details.Value<string>("hashed_id")
        );

        // no errors
        //Assert.NotEqual("", consoleLogs);
        Assert.DoesNotContain("ERROR", consoleLogs);
        Assert.DoesNotContain("SEVERE", consoleLogs);
        // Console.Write(consoleLogs);

        var networkLogs = await client.GetSessionNetworkLogsAsync(
            session_details.Value<string>("build_hashed_id"),
            session_details.Value<string>("hashed_id")
        );

        //Console.Write(networkLogs);
        //Assert.NotEqual("", networkLogs);
        // no 500
        Assert.DoesNotContain("\"status\":500", networkLogs);
        Assert.DoesNotContain("\"status\":504", networkLogs);
        Assert.DoesNotContain("\"status\":505", networkLogs);
        Assert.DoesNotContain("\"status\":507", networkLogs);
        Assert.DoesNotContain("\"status\":508", networkLogs);

        // no 400's
        Assert.DoesNotContain("\"status\":400", networkLogs);
        Assert.DoesNotContain("\"status\":401", networkLogs);
        Assert.DoesNotContain("\"status\":402", networkLogs);
        Assert.DoesNotContain("\"status\":403", networkLogs);
        Assert.DoesNotContain("\"status\":404", networkLogs);
        Assert.DoesNotContain("\"status\":405", networkLogs);
        Assert.DoesNotContain("\"status\":406", networkLogs);
        Assert.DoesNotContain("\"status\":407", networkLogs);
        Assert.DoesNotContain("\"status\":408", networkLogs);
        Assert.DoesNotContain("\"status\":409", networkLogs);


    }
}
