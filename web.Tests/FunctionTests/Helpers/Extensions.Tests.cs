using Xunit;
using Atlas_Web.Helpers;
using Atlas_Web.Models;
using System;

namespace web.Tests.FunctionTests;

using System.Collections.Generic;
using System.Linq;

public class UserHelpersTests : IClassFixture<TestDatabaseFixture>
{
    public UserHelpersTests(TestDatabaseFixture fixture) => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }

    // [Fact]
    // public void IsAdmin()
    // {
    //     Assert.False(UserHelpers.IsAdmin(Fixture.CreateContext(), null));

    //     using var context = Fixture.CreateContext();
    //     //var controller = new BloggingController(context);
    //     // var blog = controller.GetBlog("Blog2").Value;
    //     // Assert.Equal("http://blog2.com", blog.Url);

    // }
    // public void GetPreferences() {
    // need cache here
    //     Assert.Equal(new List<UserPreference>(), UserHelpers.GetPreferences(cache, Fixture.CreateContext(), null));


    // }
}
