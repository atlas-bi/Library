using Xunit;

namespace web.Tests.FunctionTests;

public class CollectionIndexTests : IClassFixture<TestDatabaseFixture>
{
    public CollectionIndexTests(TestDatabaseFixture fixture) => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }

    [Fact]
    public async void OnGetAsync_no_id()
    {
        using var cache = Fixture.CreateCache();
        var config = Fixture.CreateConfig();
        using var context = Fixture.CreateContext();

        var pageModel = new Atlas_Web.Pages.Collections.IndexModel(context, cache);
        await pageModel.OnGetAsync();
    }
}
