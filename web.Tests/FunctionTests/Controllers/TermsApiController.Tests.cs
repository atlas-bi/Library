using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Atlas_Web.Contracts.Api.Terms;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class TermsApiControllerTests
{
    [Fact]
    public async Task GetTerms_ReturnsListSurfaceWithPermissionsAndFlags()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "terms-api-list")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" });
        context.Terms.AddRange(
            new Term
            {
                TermId = 3,
                Name = "Admissions",
                Summary = "Admission summary",
                ApprovedYn = "Y",
                UpdatedByUserId = 1,
            },
            new Term
            {
                TermId = 4,
                Name = "Discharge",
                TechnicalDefinition = "Technical discharge definition",
                ApprovedYn = "N",
                UpdatedByUserId = 1,
            }
        );
        context.StarredTerms.Add(new StarredTerm { StarId = 20, Ownerid = 1, Termid = 3 });
        await context.SaveChangesAsync();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    ["features:enable_user_profile"] = "true",
                    ["features:enable_sharing"] = "true",
                    ["features:enable_feedback"] = "false",
                }
            )
            .Build();

        var service = new TermsApiService(context, config, new MemoryCache(new MemoryCacheOptions()));
        var controller = BuildController(
            service,
            BuildPrincipal(userId: 1, username: "viewer", permissions: new[] { "Create New Terms" })
        );

        var result = await controller.GetTerms();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<TermsListDto>(ok.Value);

        Assert.True(payload.Permissions.CanCreateTerm);
        Assert.True(payload.Features.UserProfilesEnabled);
        Assert.True(payload.Features.SharingEnabled);
        Assert.False(payload.Features.FeedbackEnabled);
        Assert.Equal(2, payload.Items.Count);

        var approved = Assert.Single(payload.Items.Where(x => x.Id == 3));
        Assert.True(approved.IsApproved);
        Assert.True(approved.IsStarred);
        Assert.Equal(1, approved.StarCount);
        Assert.Equal("Admission summary... ", approved.BodyText);

        var unapproved = Assert.Single(payload.Items.Where(x => x.Id == 4));
        Assert.False(unapproved.IsApproved);
        Assert.False(unapproved.IsStarred);
        Assert.Equal("Technical discharge definition... ", unapproved.BodyText);
    }

    [Fact]
    public async Task GetTerm_ReturnsDetailWithApprovalAwarePermissions()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "terms-api-detail")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.AddRange(
            new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" },
            new User { UserId = 2, Username = "approver", FullnameCalc = "Approver Name" }
        );
        context.Terms.Add(
            new Term
            {
                TermId = 10,
                Name = "Census",
                Summary = "Approved summary",
                TechnicalDefinition = "Approved technical definition",
                ApprovedYn = "Y",
                ApprovedByUserId = 2,
                UpdatedByUserId = 1,
            }
        );
        context.StarredTerms.Add(new StarredTerm { StarId = 30, Ownerid = 1, Termid = 10 });
        await context.SaveChangesAsync();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    ["features:enable_user_profile"] = "true",
                    ["features:enable_sharing"] = "true",
                    ["features:enable_feedback"] = "true",
                }
            )
            .Build();

        var service = new TermsApiService(context, config, new MemoryCache(new MemoryCacheOptions()));
        var controller = BuildController(
            service,
            BuildPrincipal(
                userId: 1,
                username: "viewer",
                permissions: new[] { "Create New Terms", "Edit Approved Terms", "Delete Approved Terms", "View Other User" }
            )
        );

        var result = await controller.GetTerm(10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<TermDetailDto>(ok.Value);

        Assert.Equal(10, payload.Id);
        Assert.Equal("Census", payload.Name);
        Assert.True(payload.IsApproved);
        Assert.True(payload.IsStarred);
        Assert.Equal(1, payload.StarCount);
        Assert.True(payload.Permissions.CanCreateTerm);
        Assert.True(payload.Permissions.CanEditTerm);
        Assert.True(payload.Permissions.CanDeleteTerm);
        Assert.False(payload.Permissions.CanApproveTerm);
        Assert.True(payload.Permissions.CanViewUserProfiles);
        Assert.True(payload.Features.UserProfilesEnabled);
        Assert.True(payload.Features.SharingEnabled);
        Assert.True(payload.Features.FeedbackEnabled);
        Assert.Equal("Approver Name", payload.ApprovedBy.FullName);
        Assert.Equal("Viewer Name", payload.LastUpdatedBy.FullName);
    }

    [Fact]
    public async Task GetTermReports_ReturnsVisibleDirectAndInheritedReports()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "terms-api-related-reports")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" });
        context.Terms.Add(new Term { TermId = 15, Name = "Quality", ApprovedYn = "N", UpdatedByUserId = 1 });
        context.ReportObjectTypes.Add(
            new ReportObjectType
            {
                ReportObjectTypeId = 5,
                Name = "SSRS Report",
                ShortName = "SSRS",
                Visible = "Y",
            }
        );
        context.ReportObjects.AddRange(
            new ReportObject
            {
                ReportObjectId = 101,
                Name = "Direct Report",
                DisplayTitle = "Direct Report",
                Description = "Direct body",
                ReportObjectTypeId = 5,
                DefaultVisibilityYn = "Y",
                SourceDb = "warehouse",
                SourceServer = "reports",
                SourceTable = "dbo.direct",
            },
            new ReportObject
            {
                ReportObjectId = 102,
                Name = "Parent Report",
                DisplayTitle = "Parent Report",
                Description = "Parent body",
                ReportObjectTypeId = 5,
                DefaultVisibilityYn = "Y",
                SourceDb = "warehouse",
                SourceServer = "reports",
                SourceTable = "dbo.parent",
            },
            new ReportObject
            {
                ReportObjectId = 103,
                Name = "Hidden Report",
                DisplayTitle = "Hidden Report",
                Description = "Hidden body",
                ReportObjectTypeId = 5,
                DefaultVisibilityYn = "Y",
                SourceDb = "warehouse",
                SourceServer = "reports",
                SourceTable = "dbo.hidden",
            }
        );
        context.ReportObjectDocs.AddRange(
            new ReportObjectDoc
            {
                ReportObjectId = 101,
                Hidden = "N",
                DeveloperDescription = "Direct description",
            },
            new ReportObjectDoc
            {
                ReportObjectId = 102,
                Hidden = "N",
                DeveloperDescription = "Parent description",
            },
            new ReportObjectDoc
            {
                ReportObjectId = 103,
                Hidden = "Y",
                DeveloperDescription = "Hidden description",
            }
        );
        context.ReportObjectDocTerms.Add(new ReportObjectDocTerm { ReportObjectId = 101, TermId = 15 });
        context.ReportObjectDocTerms.Add(new ReportObjectDocTerm { ReportObjectId = 103, TermId = 15 });
        context.ReportObjectHierarchies.Add(
            new ReportObjectHierarchy
            {
                ParentReportObjectId = 102,
                ChildReportObjectId = 101,
            }
        );
        await context.SaveChangesAsync();

        var service = new TermsApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(service, BuildPrincipal(userId: 1, username: "viewer"));

        var result = await controller.GetTermReports(15);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsAssignableFrom<IReadOnlyList<TermRelatedReportDto>>(ok.Value);

        Assert.Equal(2, payload.Count);
        Assert.Contains(payload, x => x.Id == 101 && x.Name == "Direct Report");
        Assert.Contains(payload, x => x.Id == 102 && x.Name == "Parent Report");
        Assert.DoesNotContain(payload, x => x.Id == 103);
    }

    [Fact]
    public async Task CreateTerm_SetsAuditFields_AndApprovesWhenPermitted()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "terms-api-create")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "creator", FullnameCalc = "Creator Name" });
        await context.SaveChangesAsync();

        var service = new TermsApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(
            service,
            BuildPrincipal(
                userId: 1,
                username: "creator",
                permissions: new[] { "Create New Terms", "Approve Terms" }
            )
        );

        var result = await controller.CreateTerm(
            new CreateTermRequestDto
            {
                Name = "Mortality",
                Summary = "Mortality summary",
                TechnicalDefinition = "Mortality technical definition",
                ApprovedYn = "Y",
            }
        );

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var payload = Assert.IsType<TermDetailDto>(created.Value);
        Assert.True(payload.IsApproved);

        var term = Assert.Single(context.Terms);
        Assert.Equal(1, term.UpdatedByUserId);
        Assert.Equal(1, term.ApprovedByUserId);
        Assert.Equal("Y", term.ApprovedYn);
    }

    [Fact]
    public async Task UpdateTerm_ForbidsEditingApprovedTermWithoutPermission()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "terms-api-update-forbid")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "editor", FullnameCalc = "Editor Name" });
        context.Terms.Add(
            new Term
            {
                TermId = 31,
                Name = "Original",
                Summary = "Summary",
                ApprovedYn = "Y",
                UpdatedByUserId = 1,
            }
        );
        await context.SaveChangesAsync();

        var service = new TermsApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(
            service,
            BuildPrincipal(userId: 1, username: "editor", permissions: new[] { "Edit Unapproved Terms" })
        );

        var result = await controller.UpdateTerm(
            31,
            new UpdateTermRequestDto
            {
                Name = "Changed",
                Summary = "Changed summary",
                TechnicalDefinition = "Changed definition",
                ApprovedYn = "Y",
            }
        );

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task DeleteTerm_RemovesLinks_WhenPermissionMatchesApprovalState()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "terms-api-delete")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "deleter", FullnameCalc = "Deleter Name" });
        context.Terms.Add(
            new Term
            {
                TermId = 40,
                Name = "Delete Me",
                ApprovedYn = "N",
                UpdatedByUserId = 1,
            }
        );
        context.ReportObjectDocTerms.Add(new ReportObjectDocTerm { ReportObjectId = 200, TermId = 40 });
        context.CollectionTerms.Add(new CollectionTerm { CollectionId = 201, TermId = 40 });
        await context.SaveChangesAsync();

        var service = new TermsApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(
            service,
            BuildPrincipal(userId: 1, username: "deleter", permissions: new[] { "Delete Unapproved Terms" })
        );

        var result = await controller.DeleteTerm(40);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.Terms);
        Assert.Empty(context.ReportObjectDocTerms);
        Assert.Empty(context.CollectionTerms);
    }

    private static TermsApiController BuildController(ITermsApiService service, ClaimsPrincipal principal)
    {
        return new TermsApiController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal },
            },
        };
    }

    private static ClaimsPrincipal BuildPrincipal(
        int userId,
        string username,
        IEnumerable<string> permissions = null
    )
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new("UserId", userId.ToString()),
            new("Fullname", username),
            new("AdminEnabled", "Y"),
        };

        if (permissions != null)
        {
            claims.AddRange(permissions.Select(permission => new Claim("Permission", permission)));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }
}
