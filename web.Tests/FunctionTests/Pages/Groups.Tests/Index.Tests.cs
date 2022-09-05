using Xunit;
using Atlas_Web.Helpers;
using Atlas_Web.Models;
using Atlas_Web.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Atlas_Web.Helpers;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace web.Tests.FunctionTests;

public class GroupsIndexTests : IClassFixture<TestDatabaseFixture>
{
    public GroupsIndexTests(TestDatabaseFixture fixture) => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }

    [Fact]
    public async void OnGetAsync_with_id()
    {
        using var cache = Fixture.CreateCache();
        var config = Fixture.CreateConfig();
        using var context = Fixture.CreateContext();

        var pageModel = new Atlas_Web.Pages.Groups.IndexModel(context, cache, config);

        await pageModel.OnGetAsync(1);
    }

    // [Fact]
    // public async void OnGetActivity_with_id()
    // {
    //     using var cache = Fixture.CreateCache();
    //     var config = Fixture.CreateConfig();
    //     using var context = Fixture.CreateContext();

    //     var pageModel = new Atlas_Web.Pages.Groups.IndexModel(context, cache, config);

    //     await pageModel.OnGetActivity(1);
    // }


}
