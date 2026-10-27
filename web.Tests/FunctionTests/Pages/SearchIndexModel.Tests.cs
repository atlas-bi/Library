using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace web.Tests.FunctionTests.Pages;

public class SearchIndexModelTests
{
    [Fact]
    public void BuildOptionalSearchString_ReturnsMatchAllForEmptySearch()
    {
        var query = new QueryCollection();

        var result = Atlas_Web.Pages.Search.IndexModel.BuildOptionalSearchString(null, query);

        Assert.Equal("*:*", result);
    }

    [Fact]
    public void BuildOptionalSearchString_UsesSearchStringWhenProvided()
    {
        var query = new QueryCollection();

        var result = Atlas_Web.Pages.Search.IndexModel.BuildOptionalSearchString("a", query);

        Assert.Equal(
            "name:(a)^12 OR name_split:(a)^6 OR description:(a)^5 OR description_split:(a)^3 OR (a)",
            result
        );
    }

    [Fact]
    public void ResolveSearchQuery_UsesQAliasWhenQueryMissing()
    {
        var requestQuery = new QueryCollection(
            new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                ["q"] = "census",
            }
        );

        var result = Atlas_Web.Pages.Search.IndexModel.ResolveSearchQuery(null, requestQuery);

        Assert.Equal("census", result);
    }

    [Fact]
    public void ResolveSearchQuery_PrefersQueryOverQAlias()
    {
        var requestQuery = new QueryCollection(
            new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                ["q"] = "census",
            }
        );

        var result = Atlas_Web.Pages.Search.IndexModel.ResolveSearchQuery("Patient Flow", requestQuery);

        Assert.Equal("Patient Flow", result);
    }
}
