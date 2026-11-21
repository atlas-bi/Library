using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Atlas_Web.Contracts.Api.Search;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SolrNet;
using SolrNet.Commands.Parameters;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class SearchApiControllerTests
{
    [Fact]
    public async Task Search_ReturnsOkWithEmptyHighlights_WhenSolrSendsNoHighlighting()
    {
        var solrMock = new Mock<ISolrReadOnlyOperations<SolrAtlas>>();
        solrMock
            .Setup(x =>
                x.QueryAsync(
                    It.IsAny<ISolrQuery>(),
                    It.IsAny<QueryOptions>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(() =>
            {
                var results = new SolrQueryResults<SolrAtlas>
                {
                    NumFound = 1,
                    FacetFields = new Dictionary<string, ICollection<KeyValuePair<string, int>>>(),
                    Highlights = null,
                    Header = new ResponseHeader { QTime = 2 },
                };
                results.Add(
                    new SolrAtlas
                    {
                        Id = "report-1",
                        AtlasId = 42,
                        Type = "reports",
                        Name = "Census Report",
                    }
                );
                return results;
            });

        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "search-api-null-highlights")
            .Options;
        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer" });
        await context.SaveChangesAsync();

        var service = new SearchApiService(context, solrMock.Object);
        var controller = new SearchApiController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = BuildPrincipal(1, "viewer") },
            },
        };

        var result = await controller.Search(q: "census", cancellationToken: CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<SearchResponseDto>(ok.Value);
        Assert.NotNull(payload.Highlights);
        Assert.Empty(payload.Highlights);
        Assert.Single(payload.Results);
        Assert.Equal("Census Report", payload.Results[0].Name);
        Assert.Equal(1, payload.Total);
    }

    private static ClaimsPrincipal BuildPrincipal(int userId, string username)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new("UserId", userId.ToString()),
            new("Fullname", username),
            new("AdminEnabled", "Y"),
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }
}