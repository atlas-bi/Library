using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using Atlas_Web.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace web.Tests.FunctionTests.Authorization;

public class DemoAuthHandlerTests
{
    [Fact]
    public async Task AuthenticateAsync_UsesConfiguredDemoAdminUsername()
    {
        var options = new DemoSchemeOptions { Username = "local-admin" };
        var optionsMonitor = new Mock<IOptionsMonitor<DemoSchemeOptions>>();
        optionsMonitor.Setup(x => x.CurrentValue).Returns(options);
        optionsMonitor.Setup(x => x.Get(It.IsAny<string>())).Returns(options);

        var handler = new DemoAuthHandler(
            optionsMonitor.Object,
            NullLoggerFactory.Instance,
            UrlEncoder.Default,
            new SystemClock()
        );

        await handler.InitializeAsync(
            new AuthenticationScheme("Demo", "Demo", typeof(DemoAuthHandler)),
            new DefaultHttpContext()
        );

        var result = await handler.AuthenticateAsync();

        Assert.True(result.Succeeded);
        Assert.Equal(
            "local-admin",
            result.Principal?.Claims.Single(c => c.Type == ClaimTypes.Name).Value
        );
    }
}
