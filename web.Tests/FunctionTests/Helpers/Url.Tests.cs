using Xunit;
using Atlas_Web.Helpers;
using Atlas_Web.Models;
using System;

namespace web.Tests.FunctionTests;

public class ModelHelpersTests : IClassFixture<TestDatabaseFixture>
{
    public ModelHelpersTests(TestDatabaseFixture fixture) => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }

    [Fact]
    public void Test_Helpers_ModelHelpers_RelativeDate()
    {
        Assert.Equal("0 seconds ago", ModelHelpers.RelativeDate(DateTime.Now));

        using var context = Fixture.CreateContext();
        //var controller = new BloggingController(context);
        // var blog = controller.GetBlog("Blog2").Value;
        // Assert.Equal("http://blog2.com", blog.Url);
    }
}

// public class HtmlHelpersTests
// {
//     [Fact]
//     public void Test_Helpers_HtmlHelpers_MarkdownToHtml()
//     {
//         Assert.Equal("<p>asdf</p>\n", HtmlHelpers.MarkdownToHtml("asdf", ));
//         Assert.Equal("<h1 id=\"asdf\">asdf</h1>\n", HtmlHelpers.MarkdownToHtml("# asdf"));
//     }
// }
