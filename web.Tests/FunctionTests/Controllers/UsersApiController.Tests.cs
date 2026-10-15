using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Atlas_Web.Contracts.Api.Users;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class UsersApiControllerTests
{
    [Fact]
    public async Task GetSettings_DefaultsShareNotificationsToEnabled()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "users-api-settings-default")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" });
        await context.SaveChangesAsync();

        var service = new UsersApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(service, BuildPrincipal(userId: 1, username: "viewer"));

        var result = await controller.GetSettings();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<UserSettingsDto>(ok.Value);
        Assert.True(payload.ShareNotificationEnabled);
    }

    [Fact]
    public async Task UpdateSettings_PersistsShareNotificationForCurrentUser()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "users-api-settings-update")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.AddRange(
            new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" },
            new User { UserId = 2, Username = "other", FullnameCalc = "Other Name" }
        );
        context.UserSettings.Add(
            new UserSetting { UserId = 2, Name = "share_notification", Value = "Y" }
        );
        await context.SaveChangesAsync();

        var service = new UsersApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(service, BuildPrincipal(userId: 1, username: "viewer"));

        var result = await controller.UpdateSettings(
            new UpdateUserSettingsRequestDto { ShareNotificationEnabled = false }
        );

        Assert.IsType<NoContentResult>(result);
        var setting = Assert.Single(context.UserSettings, x => x.UserId == 1);
        Assert.Equal("share_notification", setting.Name);
        Assert.Equal("N", setting.Value);
        Assert.Equal("Y", Assert.Single(context.UserSettings, x => x.UserId == 2).Value);
    }

    [Fact]
    public async Task UpdateSettings_RejectsMissingShareNotificationValue()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "users-api-settings-invalid")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" });
        await context.SaveChangesAsync();

        var service = new UsersApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(service, BuildPrincipal(userId: 1, username: "viewer"));

        var result = await controller.UpdateSettings(new UpdateUserSettingsRequestDto());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("required", badRequest.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        Assert.Empty(context.UserSettings);
    }

    [Fact]
    public async Task GetUserPage_ReturnsTargetUserAndViewerDrivenFlags()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "users-api-page")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.AddRange(
            new User
            {
                UserId = 1,
                Username = "viewer",
                FullnameCalc = "Viewer Name",
                FirstnameCalc = "Viewer",
            },
            new User
            {
                UserId = 2,
                Username = "target",
                FullnameCalc = "Target Name",
                FirstnameCalc = "Target",
                Email = "target@example.com",
            }
        );
        context.ReportObjectTypes.AddRange(
            new ReportObjectType { ReportObjectTypeId = 10, Name = "Visible A", Visible = "Y" },
            new ReportObjectType { ReportObjectTypeId = 20, Name = "Hidden B", Visible = "N" },
            new ReportObjectType { ReportObjectTypeId = 30, Name = "Visible C", Visible = "Y" }
        );
        await context.SaveChangesAsync();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    ["features:enable_user_profile"] = "true",
                }
            )
            .Build();

        var service = new UsersApiService(context, config, new MemoryCache(new MemoryCacheOptions()));
        var controller = BuildController(
            service,
            BuildPrincipal(
                userId: 1,
                username: "viewer",
                permissions: new[] { "View Other User", "View Groups", "View Site Analytics", "Administrator" }
            )
        );

        var result = await controller.GetUserPage(2);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<UserPageDto>(ok.Value);

        Assert.Equal(2, payload.User.Id);
        Assert.Equal("Target Name", payload.User.FullName);
        Assert.Equal(1, payload.Viewer.Id);
        Assert.False(payload.Viewer.IsCurrentUser);
        Assert.True(payload.Permissions.CanViewOtherUsers);
        Assert.True(payload.Viewer.IsAdministrator);
        Assert.True(payload.Permissions.CanToggleAdminMode);
        Assert.True(payload.Tabs.GroupsVisible);
        Assert.True(payload.Tabs.AnalyticsVisible);
        Assert.Equal(new[] { 10, 30 }, payload.DefaultReportTypeIds);
    }

    [Fact]
    public async Task GetSearchHistory_ReturnsOnlyCurrentUsersHistory()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "users-api-search-history")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.AddRange(
            new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" },
            new User { UserId = 2, Username = "other", FullnameCalc = "Other Name" }
        );
        context.Analytics.AddRange(
            new Analytic
            {
                Id = 1,
                UserId = 1,
                Pathname = "/search",
                Search = "Query=cardiology",
            },
            new Analytic
            {
                Id = 2,
                UserId = 2,
                Pathname = "/search",
                Search = "Query=finance",
            }
        );
        await context.SaveChangesAsync();

        var service = new UsersApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(service, BuildPrincipal(userId: 1, username: "viewer"));

        var result = await controller.GetSearchHistory();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsAssignableFrom<IReadOnlyList<UserSearchHistoryItemDto>>(ok.Value);
        var item = Assert.Single(payload);
        Assert.Equal("cardiology", item.SearchString);
    }

    [Fact]
    public async Task CreateFolder_CreatesFolderForCurrentUser()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "users-api-create-folder")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" });
        await context.SaveChangesAsync();

        var service = new UsersApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(service, BuildPrincipal(userId: 1, username: "viewer"));

        var result = await controller.CreateFolder(
            new CreateUserFavoriteFolderRequestDto { Name = "Saved Reports" }
        );

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var payload = Assert.IsType<UserFavoriteFolderDto>(created.Value);
        Assert.Equal("Saved Reports", payload.Name);

        var folder = Assert.Single(context.UserFavoriteFolders);
        Assert.Equal(1, folder.UserId);
        Assert.Equal("Saved Reports", folder.FolderName);
    }

    [Fact]
    public async Task GetStars_ReturnsFoldersAndFavoriteItems()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "users-api-stars")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" });
        context.UserFavoriteFolders.Add(
            new UserFavoriteFolder { UserFavoriteFolderId = 5, UserId = 1, FolderName = "Pinned" }
        );
        context.StarredSearches.Add(
            new StarredSearch
            {
                StarId = 9,
                Ownerid = 1,
                Folderid = 5,
                Rank = 3,
                Search = "Query=finance",
            }
        );
        await context.SaveChangesAsync();

        var service = new UsersApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(service, BuildPrincipal(userId: 1, username: "viewer"));

        var result = await controller.GetStars(1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<UserStarsDto>(ok.Value);
        var folder = Assert.Single(payload.Folders);
        var item = Assert.Single(payload.Items);

        Assert.True(payload.IsCurrentUser);
        Assert.True(payload.CanEditWorkspace);
        Assert.True(payload.Permissions.CanCreateFolders);
        Assert.True(payload.Permissions.CanReorderFavorites);
        Assert.Equal(1, payload.Summary.TotalCount);
        Assert.False(payload.Summary.ShowUnsortedBucket);
        Assert.False(payload.Filters.ShowQuickFilters);
        Assert.Equal("Pinned", folder.Name);
        Assert.Equal(1, folder.ItemCount);
        Assert.True(folder.CanManage);
        Assert.True(folder.CanReorder);
        Assert.Equal("search", item.Type);
        Assert.Equal(5, item.FolderId);
        Assert.Equal("Pinned", item.FolderName);
        Assert.Equal("/search?Query=finance", item.Url);
        Assert.Equal("finance", item.SearchString);
        Assert.True(item.CanReorder);
    }

    [Fact]
    public async Task GetStars_ReturnsSnippetParityFieldsForReportInitiativeAndTerm()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "users-api-stars-snippet-parity")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.AddRange(
            new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" },
            new User { UserId = 2, Username = "other", FullnameCalc = "Other Name" }
        );
        context.ReportObjectTypes.Add(
            new ReportObjectType
            {
                ReportObjectTypeId = 3,
                Name = "SSRS Report",
                ShortName = "SSRS",
                Visible = "Y",
            }
        );
        context.Tags.Add(
            new Tag { TagId = 4, Name = "Analytics Certified", ShowInHeader = "Y" }
        );
        context.Initiatives.Add(
            new Initiative { InitiativeId = 8, Name = "Quality", Description = "Initiative body" }
        );
        context.Collections.Add(
            new Collection
            {
                CollectionId = 9,
                InitiativeId = 8,
                Name = "Operations",
                Description = "Collection body",
            }
        );
        context.Terms.Add(
            new Term
            {
                TermId = 12,
                Name = "Census",
                Summary = "Approved term summary",
                ApprovedYn = "Y",
            }
        );
        context.ReportObjects.Add(
            new ReportObject
            {
                ReportObjectId = 6,
                Name = "Revenue Report",
                DisplayTitle = "Revenue Snapshot",
                Description = "Report body",
                ReportObjectTypeId = 3,
                ReportObjectType = context.ReportObjectTypes.Local.Single(x => x.ReportObjectTypeId == 3),
                SourceDb = "warehouse",
                SourceTable = "finance.revenue",
                ReportServerPath = "/Finance/Revenue",
                SourceServer = "reports",
            }
        );
        context.ReportObjectDocs.Add(
            new ReportObjectDoc
            {
                ReportObjectId = 6,
                DeveloperDescription = "Developer summary for the report",
                EnabledForHyperspace = "Y",
            }
        );
        context.ReportTagLinks.Add(
            new ReportTagLink
            {
                ReportTagLinkId = 7,
                ReportId = 6,
                TagId = 4,
                ShowInHeader = "Y",
            }
        );
        context.StarredReports.AddRange(
            new StarredReport { StarId = 21, Ownerid = 1, Reportid = 6 },
            new StarredReport { StarId = 22, Ownerid = 2, Reportid = 6 }
        );
        context.StarredInitiatives.Add(
            new StarredInitiative { StarId = 23, Ownerid = 1, Initiativeid = 8 }
        );
        context.StarredTerms.Add(new StarredTerm { StarId = 24, Ownerid = 1, Termid = 12 });
        await context.SaveChangesAsync();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    ["AppSettings:org_domain"] = "example.org",
                    ["features:enable_sharing"] = "true",
                    ["features:enable_request_access"] = "true",
                }
            )
            .Build();

        var service = new UsersApiService(
            context,
            config,
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(
            service,
            BuildPrincipal(userId: 1, username: "viewer", permissions: new[] { "Open In Editor" })
        );

        var result = await controller.GetStars(1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<UserStarsDto>(ok.Value);
        var report = Assert.Single(payload.Items.Where(x => x.Type == "report"));
        var initiative = Assert.Single(payload.Items.Where(x => x.Type == "initiative"));
        var term = Assert.Single(payload.Items.Where(x => x.Type == "term"));

        Assert.Equal("Revenue Snapshot", report.Name);
        Assert.Equal("SSRS", report.TypeLabel);
        Assert.True(report.IsCertified);
        Assert.Equal(2, report.StarCount);
        Assert.True(report.CanOpenProfile);
        Assert.Equal("report-profile-6", report.ProfileTargetId);
        Assert.True(report.CanShare);
        Assert.Equal("report-share-6", report.ShareTargetId);
        Assert.True(report.CanEditInEditor);
        Assert.Equal(
            "reportbuilder:Action=Edit&ItemPath=%2FFinance%2FRevenue&Endpoint=https%3A%2F%2Freports.example.org%3A443%2FReportServer",
            report.EditUrl
        );
        Assert.Equal(
            "https://reports.example.org/Reports/manage/catalogitem/properties/Finance/Revenue",
            report.ManageUrl
        );
        Assert.Contains(report.Tags, x => x.Name == "Analytics Certified" && x.ShowInHeader);
        Assert.Equal("Developer summary for the report... ", report.BodyText);

        Assert.Equal("initiative", initiative.TypeLabel);
        Assert.Contains("Operations", initiative.RelatedCollectionNames);
        Assert.Equal("Initiative body... ", initiative.BodyText);
        Assert.True(initiative.CanShare);

        Assert.Equal("term", term.TypeLabel);
        Assert.True(term.IsApproved);
        Assert.Equal("Approved term summary... ", term.BodyText);
        Assert.True(term.CanOpenProfile);
        Assert.Equal("term-profile-12", term.ProfileTargetId);
    }

    [Fact]
    public async Task UpdateFavoriteFolder_MovesFavoriteIntoFolder()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "users-api-move-favorite")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" });
        context.UserFavoriteFolders.Add(
            new UserFavoriteFolder { UserFavoriteFolderId = 7, UserId = 1, FolderName = "Saved" }
        );
        context.StarredSearches.Add(
            new StarredSearch
            {
                StarId = 11,
                Ownerid = 1,
                Folderid = null,
                Search = "Query=quality",
            }
        );
        await context.SaveChangesAsync();

        var service = new UsersApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(service, BuildPrincipal(userId: 1, username: "viewer"));

        var result = await controller.UpdateFavoriteFolder(
            new UpdateUserFavoriteFolderAssignmentRequestDto
            {
                FavoriteId = 11,
                FavoriteType = "search",
                FolderId = 7,
            }
        );

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(7, context.StarredSearches.Single().Folderid);
    }

    [Fact]
    public async Task RemoveSharedObject_DeletesUsersSharedItem()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "users-api-remove-share")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.AddRange(
            new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" },
            new User { UserId = 2, Username = "other", FullnameCalc = "Other User" }
        );
        context.SharedItems.Add(
            new SharedItem
            {
                Id = 14,
                SharedFromUserId = 1,
                SharedToUserId = 2,
                Name = "Revenue Report",
                Url = "/reports?id=4",
            }
        );
        await context.SaveChangesAsync();

        var service = new UsersApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(service, BuildPrincipal(userId: 1, username: "viewer"));

        var result = await controller.RemoveSharedObject(14);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.SharedItems);
    }

    [Fact]
    public async Task ToggleFavorite_TogglesSearchFavoriteAndReturnsCount()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "users-api-toggle-search-favorite")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" });
        await context.SaveChangesAsync();

        var service = new UsersApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(service, BuildPrincipal(userId: 1, username: "viewer"));

        var first = await controller.ToggleFavorite(
            new ToggleUserFavoriteRequestDto { Type = "search", Search = "Query=finance" }
        );
        var firstOk = Assert.IsType<OkObjectResult>(first.Result);
        var firstPayload = Assert.IsType<ToggleUserFavoriteResponseDto>(firstOk.Value);
        Assert.True(firstPayload.IsStarred);
        Assert.Equal(1, firstPayload.StarCount);

        var second = await controller.ToggleFavorite(
            new ToggleUserFavoriteRequestDto { Type = "search", Search = "Query=finance" }
        );
        var secondOk = Assert.IsType<OkObjectResult>(second.Result);
        var secondPayload = Assert.IsType<ToggleUserFavoriteResponseDto>(secondOk.Value);
        Assert.False(secondPayload.IsStarred);
        Assert.Equal(0, secondPayload.StarCount);
    }

    [Fact]
    public async Task ToggleAdminMode_CreatesAndRemovesAdminDisabledPreference()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "users-api-toggle-admin-mode")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" });
        await context.SaveChangesAsync();

        var service = new UsersApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(
            service,
            BuildPrincipal(userId: 1, username: "viewer", permissions: new[] { "Administrator" })
        );

        var first = await controller.ToggleAdminMode();
        var firstOk = Assert.IsType<OkObjectResult>(first.Result);
        var firstPayload = Assert.IsType<ToggleAdminModeResponseDto>(firstOk.Value);
        Assert.Equal("N", firstPayload.AdminEnabled);
        Assert.Single(context.UserPreferences);

        var second = await controller.ToggleAdminMode();
        var secondOk = Assert.IsType<OkObjectResult>(second.Result);
        var secondPayload = Assert.IsType<ToggleAdminModeResponseDto>(secondOk.Value);
        Assert.Equal("Y", secondPayload.AdminEnabled);
        Assert.Empty(context.UserPreferences);
    }

    [Fact]
    public async Task CreateFolderForUser_AllowsEditorToManageOtherUsersWorkspace()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "users-api-create-folder-other-user")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.AddRange(
            new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" },
            new User { UserId = 2, Username = "target", FullnameCalc = "Target Name" }
        );
        await context.SaveChangesAsync();

        var service = new UsersApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(
            service,
            BuildPrincipal(userId: 1, username: "viewer", permissions: new[] { "Edit Other Users" })
        );

        var result = await controller.CreateFolderForUser(
            2,
            new CreateUserFavoriteFolderRequestDto { Name = "Managed Folder" }
        );

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var payload = Assert.IsType<UserFavoriteFolderDto>(created.Value);
        Assert.Equal("Managed Folder", payload.Name);
        Assert.Equal(2, Assert.Single(context.UserFavoriteFolders).UserId);
    }

    [Fact]
    public async Task ReorderFavoritesForUser_ForbidsEditingOtherUsersOrder()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "users-api-reorder-other-user-forbid")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.AddRange(
            new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" },
            new User { UserId = 2, Username = "target", FullnameCalc = "Target Name" }
        );
        await context.SaveChangesAsync();

        var service = new UsersApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(
            service,
            BuildPrincipal(userId: 1, username: "viewer", permissions: new[] { "Edit Other Users" })
        );

        var result = await controller.ReorderFavoritesForUser(
            2,
            new[]
            {
                new ReorderUserFavoriteItemDto
                {
                    FavoriteId = "11",
                    FavoriteType = "search",
                    FavoriteRank = 1,
                },
            }
        );

        Assert.IsType<ForbidResult>(result);
    }

    private static UsersApiController BuildController(
        IUsersApiService service,
        ClaimsPrincipal principal
    )
    {
        var controller = new UsersApiController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal },
            },
        };

        return controller;
    }

    private static ClaimsPrincipal BuildPrincipal(
        int userId,
        string username,
        IEnumerable<string> permissions = null,
        IEnumerable<string> roles = null
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

        if (roles != null)
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }
}
