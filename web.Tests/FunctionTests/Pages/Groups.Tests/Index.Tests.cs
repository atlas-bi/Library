using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;

namespace web.Tests.FunctionTests;

public class GroupsIndexTests : IClassFixture<TestDatabaseFixture>
{
    public GroupsIndexTests(TestDatabaseFixture fixture) => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }

    [Fact]
    public async Task OnGetAsync_without_id_returns_list_view()
    {
        using var cache = Fixture.CreateCache();
        using var context = Fixture.CreateContext();

        var pageModel = new Atlas_Web.Pages.Groups.IndexModel(context, cache);

        var result = await pageModel.OnGetAsync(null);

        Assert.IsType<PageResult>(result);
        Assert.True(pageModel.IsListView);
        Assert.NotNull(pageModel.Groups);
    }
}
