using Xunit.Abstractions;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace web.Tests.BrowserTests;

public class BrowserFixture
{
    public BrowserFixture(BrowserFixtureOptions options, ITestOutputHelper outputHelper)
    {
        Options = options;
        OutputHelper = outputHelper;
    }

    private BrowserFixtureOptions Options { get; }

    private ITestOutputHelper OutputHelper { get; }

    public DriverOptions BuildBrowser()
    {
        switch (Options.Browser)
        {
            case "safari":
                OpenQA.Selenium.Safari.SafariOptions safariCapability =
                    new OpenQA.Selenium.Safari.SafariOptions();
                safariCapability.BrowserVersion = Options.BrowserVersion;
                Dictionary<string, object> browserstackSafariOptions = new Dictionary<
                    string,
                    object
                >();
                browserstackSafariOptions.Add("osVersion", Options.OperatingSystemVersion);
                browserstackSafariOptions.Add("os", Options.OperatingSystem);
                browserstackSafariOptions.Add("projectName", Options.ProjectName);
                browserstackSafariOptions.Add("buildName", Options.BuildName);
                browserstackSafariOptions.Add("local", "true");
                browserstackSafariOptions.Add("consoleLogs", "verbose");
                browserstackSafariOptions.Add("networkLogs", "true");
                browserstackSafariOptions.Add("networkLogs", "true");
                browserstackSafariOptions.Add("userName", Options.BrowserStackCredentials.UserName);
                browserstackSafariOptions.Add(
                    "accessKey",
                    Options.BrowserStackCredentials.AccessKey
                );
                safariCapability.AddAdditionalOption("bstack:options", browserstackSafariOptions);
                return safariCapability;

            case "chrome":
                OpenQA.Selenium.Chrome.ChromeOptions chromeCapability =
                    new OpenQA.Selenium.Chrome.ChromeOptions();
                chromeCapability.BrowserVersion = Options.BrowserVersion;
                Dictionary<string, object> browserstackChromeOptions = new Dictionary<
                    string,
                    object
                >();
                browserstackChromeOptions.Add("os", Options.OperatingSystem);
                browserstackChromeOptions.Add("osVersion", Options.OperatingSystemVersion);
                browserstackChromeOptions.Add("local", "true");
                browserstackChromeOptions.Add("projectName", Options.ProjectName);
                browserstackChromeOptions.Add("buildName", Options.BuildName);
                browserstackChromeOptions.Add("consoleLogs", "verbose");
                browserstackChromeOptions.Add("networkLogs", "true");
                browserstackChromeOptions.Add("userName", Options.BrowserStackCredentials.UserName);
                browserstackChromeOptions.Add(
                    "accessKey",
                    Options.BrowserStackCredentials.AccessKey
                );
                chromeCapability.AddAdditionalOption("bstack:options", browserstackChromeOptions);
                return chromeCapability;

            case "firefox":
                OpenQA.Selenium.Firefox.FirefoxOptions firefoxCapability =
                    new OpenQA.Selenium.Firefox.FirefoxOptions();
                firefoxCapability.BrowserVersion = Options.BrowserVersion;
                Dictionary<string, object> browserstackFirefoxOptions = new Dictionary<
                    string,
                    object
                >();
                browserstackFirefoxOptions.Add("osVersion", Options.OperatingSystemVersion);
                browserstackFirefoxOptions.Add("os", Options.OperatingSystem);
                browserstackFirefoxOptions.Add("projectName", Options.ProjectName);
                browserstackFirefoxOptions.Add("buildName", Options.BuildName);
                browserstackFirefoxOptions.Add("local", "true");
                browserstackFirefoxOptions.Add("consoleLogs", "verbose");
                browserstackFirefoxOptions.Add("networkLogs", "true");
                browserstackFirefoxOptions.Add(
                    "userName",
                    Options.BrowserStackCredentials.UserName
                );
                browserstackFirefoxOptions.Add(
                    "accessKey",
                    Options.BrowserStackCredentials.AccessKey
                );
                firefoxCapability.AddAdditionalOption("bstack:options", browserstackFirefoxOptions);
                return firefoxCapability;

            case "edge":
                OpenQA.Selenium.Edge.EdgeOptions edgeCapability =
                    new OpenQA.Selenium.Edge.EdgeOptions();
                edgeCapability.BrowserVersion = Options.BrowserVersion;
                Dictionary<string, object> browserstackEdgeOptions = new Dictionary<
                    string,
                    object
                >();
                browserstackEdgeOptions.Add("osVersion", Options.OperatingSystemVersion);
                browserstackEdgeOptions.Add("os", Options.OperatingSystem);
                browserstackEdgeOptions.Add("projectName", Options.ProjectName);
                browserstackEdgeOptions.Add("buildName", Options.BuildName);
                browserstackEdgeOptions.Add("local", "true");
                browserstackEdgeOptions.Add("consoleLogs", "verbose");
                browserstackEdgeOptions.Add("networkLogs", "true");
                browserstackEdgeOptions.Add("userName", Options.BrowserStackCredentials.UserName);
                browserstackEdgeOptions.Add("accessKey", Options.BrowserStackCredentials.AccessKey);
                edgeCapability.AddAdditionalOption("bstack:options", browserstackEdgeOptions);
                return edgeCapability;

            case "ie":
                OpenQA.Selenium.IE.InternetExplorerOptions ieCapability =
                    new OpenQA.Selenium.IE.InternetExplorerOptions();
                ieCapability.BrowserVersion = Options.BrowserVersion;
                Dictionary<string, object> browserstackIeOptions = new Dictionary<string, object>();
                browserstackIeOptions.Add("osVersion", Options.OperatingSystemVersion);
                browserstackIeOptions.Add("os", Options.OperatingSystem);
                browserstackIeOptions.Add("projectName", Options.ProjectName);
                browserstackIeOptions.Add("buildName", Options.BuildName);
                browserstackIeOptions.Add("local", "true");
                browserstackIeOptions.Add("consoleLogs", "verbose");
                browserstackIeOptions.Add("networkLogs", "true");
                browserstackIeOptions.Add("userName", Options.BrowserStackCredentials.UserName);
                browserstackIeOptions.Add("accessKey", Options.BrowserStackCredentials.AccessKey);
                ieCapability.AddAdditionalOption("bstack:options", browserstackIeOptions);

                return ieCapability;

            default:
                break;
        }
        switch (Options.OperatingSystem)
        {
            case "ios":
                OpenQA.Selenium.Safari.SafariOptions iosCapability =
                    new OpenQA.Selenium.Safari.SafariOptions();
                Dictionary<string, object> browserstackIosOptions = new Dictionary<
                    string,
                    object
                >();

                // for example, iPhone 12
                browserstackIosOptions.Add("deviceName", Options.OperatingSystem);
                // for example 14
                browserstackIosOptions.Add("osVersion", Options.OperatingSystemVersion);
                browserstackIosOptions.Add("realMobile", "true");
                browserstackIosOptions.Add("local", "true");
                browserstackIosOptions.Add("buildName", Options.BuildName);
                browserstackIosOptions.Add("projectName", Options.ProjectName);
                browserstackIosOptions.Add("consoleLogs", "verbose");
                browserstackIosOptions.Add("networkLogs", "true");
                browserstackIosOptions.Add("userName", Options.BrowserStackCredentials.UserName);
                browserstackIosOptions.Add("accessKey", Options.BrowserStackCredentials.AccessKey);

                iosCapability.AddAdditionalOption("bstack:options", browserstackIosOptions);
                return iosCapability;

            case "android":
                OpenQA.Selenium.Chrome.ChromeOptions androidCapability =
                    new OpenQA.Selenium.Chrome.ChromeOptions();

                Dictionary<string, object> browserstackAndroidOptions = new Dictionary<
                    string,
                    object
                >();

                // for example, Samsung Galaxy Tab S5e
                browserstackAndroidOptions.Add("deviceName", Options.OperatingSystem);
                // for example, 9.0
                browserstackAndroidOptions.Add("osVersion", Options.OperatingSystemVersion);
                browserstackAndroidOptions.Add("realMobile", "true");
                browserstackAndroidOptions.Add("projectName", Options.ProjectName);
                browserstackAndroidOptions.Add("buildName", Options.BuildName);
                browserstackAndroidOptions.Add("local", "false");
                browserstackAndroidOptions.Add("consoleLogs", "verbose");
                browserstackAndroidOptions.Add("networkLogs", "true");
                browserstackAndroidOptions.Add(
                    "userName",
                    Options.BrowserStackCredentials.UserName
                );
                browserstackAndroidOptions.Add(
                    "accessKey",
                    Options.BrowserStackCredentials.AccessKey
                );
                androidCapability.AddAdditionalOption("bstack:options", browserstackAndroidOptions);
                return androidCapability;
            default:
                break;
        }

        // if nothing found, use chrome
        OpenQA.Selenium.Chrome.ChromeOptions chromeCapabilityDefault =
            new OpenQA.Selenium.Chrome.ChromeOptions();
        chromeCapabilityDefault.BrowserVersion = Options.BrowserVersion;
        Dictionary<string, object> browserstackChromeOptionsDefault = new Dictionary<
            string,
            object
        >();
        browserstackChromeOptionsDefault.Add("os", Options.OperatingSystem);
        browserstackChromeOptionsDefault.Add("osVersion", Options.OperatingSystemVersion);
        browserstackChromeOptionsDefault.Add("local", "true");
        browserstackChromeOptionsDefault.Add("projectName", Options.ProjectName);
        browserstackChromeOptionsDefault.Add("buildName", Options.BuildName);
        browserstackChromeOptionsDefault.Add("consoleLogs", "verbose");
        browserstackChromeOptionsDefault.Add("networkLogs", "true");
        browserstackChromeOptionsDefault.Add("userName", Options.BrowserStackCredentials.UserName);
        browserstackChromeOptionsDefault.Add(
            "accessKey",
            Options.BrowserStackCredentials.AccessKey
        );
        chromeCapabilityDefault.AddAdditionalOption(
            "bstack:options",
            browserstackChromeOptionsDefault
        );
        return chromeCapabilityDefault;
    }
}
