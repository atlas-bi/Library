using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class ReportFilesApiControllerTests
{
    [Fact]
    public async Task GetCube_ReplacesServerAndCatalogInTemplate()
    {
        var root = Path.Combine(Path.GetTempPath(), "atlas-cube-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(root, "wwwroot", "defaults"));
        await File.WriteAllTextAsync(
            Path.Combine(root, "wwwroot", "defaults", "Cube.xml"),
            "Data Source=server;Initial Catalog=Catalog_Name"
        );

        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase("report-files-cube")
            .Options;
        await using var context = new Atlas_WebContext(options);
        context.ReportObjects.Add(
            new ReportObject
            {
                ReportObjectId = 7,
                Name = "Patient Flow",
                SourceServer = "OLAP01",
                SourceDb = "Catalog",
                SourceTable = "Cube",
            }
        );
        await context.SaveChangesAsync();

        var controller = BuildController(context, root, allowRun: true);
        var result = await controller.GetCube(7);

        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/plain", file.ContentType);
        Assert.Equal("Patient Flow.odc", file.FileDownloadName);
        Assert.Equal(
            "Data Source=OLAP01;Initial Catalog=Patient Flow",
            Encoding.ASCII.GetString(file.FileContents)
        );
    }

    [Fact]
    public async Task GetCube_ReturnsNotFoundWhenReportIsMissing()
    {
        var root = Path.Combine(Path.GetTempPath(), "atlas-cube-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(root, "wwwroot", "defaults"));
        await File.WriteAllTextAsync(
            Path.Combine(root, "wwwroot", "defaults", "Cube.xml"),
            "server"
        );
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase("report-files-cube-missing")
            .Options;
        await using var context = new Atlas_WebContext(options);
        var controller = BuildController(context, root, allowRun: true);

        var result = await controller.GetCube(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAttachment_ReturnsPdfWhenRunIsAllowed()
    {
        var root = Path.Combine(Path.GetTempPath(), "atlas-crystal-" + Guid.NewGuid().ToString("N"));
        var pdfPath = Path.Combine(root, "report.pdf");
        Directory.CreateDirectory(root);
        var bytes = Encoding.ASCII.GetBytes("%PDF-1.4");
        await File.WriteAllBytesAsync(pdfPath, bytes);

        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase("report-files-crystal")
            .Options;
        await using var context = new Atlas_WebContext(options);
        context.ReportObjectAttachments.Add(
            new ReportObjectAttachment
            {
                ReportObjectAttachmentId = 4,
                Name = "Census.pdf",
                Path = pdfPath,
                ReportObject = new ReportObject
                {
                    ReportObjectId = 3,
                    Name = "Census",
                    SourceServer = "OLAP01",
                    SourceDb = "Catalog",
                    SourceTable = "Cube",
                },
            }
        );
        await context.SaveChangesAsync();

        var controller = BuildController(context, root, allowRun: true);
        var result = await controller.GetAttachment(4);

        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/pdf", file.ContentType);
        Assert.Equal(bytes, file.FileContents);
        Assert.Contains("Census.pdf", controller.Response.Headers.ContentDisposition.ToString());
    }

    [Fact]
    public async Task GetAttachment_ForbidsWhenRunIsDenied()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase("report-files-crystal-denied")
            .Options;
        await using var context = new Atlas_WebContext(options);
        context.ReportObjectAttachments.Add(
            new ReportObjectAttachment
            {
                ReportObjectAttachmentId = 5,
                Name = "Hidden.pdf",
                Path = "missing.pdf",
                ReportObject = new ReportObject
                {
                    ReportObjectId = 8,
                    Name = "Hidden",
                    SourceServer = "OLAP01",
                    SourceDb = "Catalog",
                    SourceTable = "Cube",
                },
            }
        );
        await context.SaveChangesAsync();

        var controller = BuildController(context, Path.GetTempPath(), allowRun: false);
        var result = await controller.GetAttachment(5);

        Assert.IsType<ForbidResult>(result);
    }

    private static ReportFilesApiController BuildController(
        Atlas_WebContext context,
        string contentRoot,
        bool allowRun
    )
    {
        var service = new ReportFileApiService(
            context,
            new StubAuthorizationService(allowRun),
            contentRoot
        );
        return new ReportFilesApiController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity("Bearer")),
                },
            },
        };
    }

    private sealed class StubAuthorizationService : IAuthorizationService
    {
        private readonly bool _allow;

        public StubAuthorizationService(bool allow)
        {
            _allow = allow;
        }

        public Task<AuthorizationResult> AuthorizeAsync(
            ClaimsPrincipal user,
            object? resource,
            IEnumerable<IAuthorizationRequirement> requirements
        )
        {
            return Task.FromResult(_allow ? AuthorizationResult.Success() : AuthorizationResult.Failed());
        }

        public Task<AuthorizationResult> AuthorizeAsync(
            ClaimsPrincipal user,
            object? resource,
            string policyName
        )
        {
            return Task.FromResult(_allow ? AuthorizationResult.Success() : AuthorizationResult.Failed());
        }
    }
}
