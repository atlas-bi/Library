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

namespace web.Tests;

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
