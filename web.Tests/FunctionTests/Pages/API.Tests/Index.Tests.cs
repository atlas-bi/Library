using Xunit;

namespace web.Tests.FunctionTests;

public class APIIndexTests : IClassFixture<TestDatabaseFixture>
{
    public APIIndexTests(TestDatabaseFixture fixture) => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }

    // [Fact]
    // public void OnGet() {
    //     using var cache = Fixture.CreateCache();
    //     var config = Fixture.CreateConfig();
    //     using var context = Fixture.CreateContext();

    //     var pageModel = new Atlas_Web.Pages.API.IndexModel(context, cache, config);


    //     pageModel.OnGet();

    // }

    // [Fact]
    // public async void OnPostRenderMarkdown()
    // {
    //     using var context = Fixture.CreateContext();
    //     using var cache = Fixture.CreateCache();
    //     var config = Fixture.CreateConfig();
    //     var pageModel = new Atlas_Web.Pages.API.IndexModel(context, cache, config);

    //     await pageModel.OnPostRenderMarkdown();
    // }
}
