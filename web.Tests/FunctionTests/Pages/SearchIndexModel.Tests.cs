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
}
