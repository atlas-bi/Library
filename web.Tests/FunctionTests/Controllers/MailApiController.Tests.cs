using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Atlas_Web.Contracts.Api.Mail;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class MailApiControllerTests
{
    [Fact]
    public void MailApi_ExposesBearerMailboxRoutes()
    {
        Assert.Equal(
            "api/mail",
            typeof(MailApiController).GetCustomAttributes<RouteAttribute>().Single().Template
        );
        Assert.Contains(
            typeof(MailApiController).GetCustomAttributes<AuthorizeAttribute>(),
            attribute => attribute.AuthenticationSchemes == "Bearer"
        );

        var getMailbox = typeof(MailApiController).GetMethod(
            "GetMailbox",
            BindingFlags.Instance | BindingFlags.Public
        );
        Assert.NotNull(getMailbox.GetCustomAttributes<HttpGetAttribute>().Single());

        var check = typeof(MailApiController).GetMethod(
            "CheckForMail",
            BindingFlags.Instance | BindingFlags.Public
        );
        Assert.Equal(
            "check",
            check.GetCustomAttributes<HttpPostAttribute>().Single().Template
        );
    }

    [Fact]
    public async Task GetMailbox_ReturnsEmptyInboxForUserWithNoMail()
    {
        await using var context = CreateContext("mail-empty");
        SeedUsers(context);
        await context.SaveChangesAsync();

        var mailbox = await new MailApiService(context).GetMailboxAsync(
            Owner(),
            "inbox",
            CancellationToken.None
        );

        Assert.Empty(mailbox.Messages);
        Assert.Empty(mailbox.Drafts);
        Assert.Equal(0, mailbox.UnreadCount);
        Assert.Equal(0, mailbox.DraftCount);
    }

    [Fact]
    public async Task MailboxOperations_UseSeededRowsAndStayScopedToCurrentUser()
    {
        await using var context = CreateContext("mail-seeded");
        SeedUsers(context);
        var type = new MailMessageType { MessageTypeId = 1, Name = "User" };
        context.MailMessageTypes.Add(type);
        var message = new MailMessage
        {
            MessageId = 10,
            Subject = "One Two Three Four Five Six",
            Message = "<p>Hello</p>",
            MessagePlainText = "alpha beta gamma delta epsilon zeta eta theta iota kappa lambda",
            SendDate = DateTime.Now.AddHours(-3),
            FromUserId = 2,
            MessageTypeId = 1,
            MessageType = type,
        };
        context.MailMessages.Add(message);
        context.MailRecipients.Add(
            new MailRecipient
            {
                Id = 20,
                MessageId = 10,
                Message = message,
                ToUserId = 1,
                AlertDisplayed = 0,
            }
        );
        context.MailRecipients.Add(
            new MailRecipient
            {
                Id = 21,
                MessageId = 10,
                Message = message,
                ToUserId = 2,
                AlertDisplayed = 1,
            }
        );
        context.MailDrafts.Add(
            new MailDraft
            {
                DraftId = 30,
                FromUserId = 1,
                Subject = "Draft Subject Extra Words",
                Message = "draft html",
                MessagePlainText = "draft plain text more words here",
                Recipients = "[{\"UserId\":2}]",
                EditDate = DateTime.Now.AddMinutes(-10),
            }
        );
        context.MailDrafts.Add(
            new MailDraft
            {
                DraftId = 31,
                FromUserId = 2,
                Subject = "Other draft",
                MessagePlainText = "secret",
                Recipients = "[]",
                EditDate = DateTime.Now,
            }
        );
        await context.SaveChangesAsync();

        var service = new MailApiService(context);
        var inbox = await service.GetMailboxAsync(Owner(), "inbox", CancellationToken.None);
        var preview = Assert.Single(inbox.Messages);
        Assert.Equal(20, preview.RecipientId);
        Assert.Equal("One Two Three Four Five", preview.Subject);
        Assert.Equal("alpha beta gamma delta epsilon zeta eta theta iota kappa", preview.Message);
        Assert.False(string.IsNullOrWhiteSpace(preview.SentPreview));
        Assert.Equal(0, preview.Read);
        Assert.Equal(1, inbox.UnreadCount);
        Assert.Equal(1, inbox.DraftCount);

        var drafts = await service.GetMailboxAsync(Owner(), "drafts", CancellationToken.None);
        Assert.Equal(30, Assert.Single(drafts.Drafts).DraftId);

        var check = await service.CheckForMailAsync(Owner(), CancellationToken.None);
        Assert.Equal(1, check.UnreadCount);
        Assert.Equal(1, check.MessageCount);
        Assert.Equal(1, check.DraftCount);
        Assert.Equal("Writer Name", Assert.Single(check.Alerts).From);
        Assert.Equal(1, context.MailRecipients.Single(x => x.Id == 20).AlertDisplayed);

        var detail = await service.GetMessageAsync(Owner(), 20, CancellationToken.None);
        Assert.Equal("One Two Three Four Five Six", detail.Subject);
        Assert.Contains(detail.To, x => x.Id == 1);

        Assert.Null(await service.GetMessageAsync(Owner(), 21, CancellationToken.None));
        Assert.Null(await service.GetDraftAsync(Owner(), 31, CancellationToken.None));

        await service.MarkMessageReadAsync(Owner(), 20, CancellationToken.None);
        Assert.NotNull(context.MailRecipients.Single(x => x.Id == 20).ReadDate);

        var saved = await service.SaveDraftAsync(
            Owner(),
            new MailDraftSaveRequestDto
            {
                DraftId = 30,
                Subject = "Updated draft",
                Message = "html",
                Text = "plain",
                To = "[{\"UserId\":2}]",
            },
            CancellationToken.None
        );
        Assert.Equal(30, saved.DraftId);
        Assert.Equal("Updated draft", context.MailDrafts.Single(x => x.DraftId == 30).Subject);

        var created = await service.SaveDraftAsync(
            Owner(),
            new MailDraftSaveRequestDto
            {
                DraftId = -1,
                Subject = "New",
                Message = "m",
                Text = "t",
                To = "[]",
            },
            CancellationToken.None
        );
        Assert.True(created.DraftId > 0);

        Assert.False(await service.DeleteMessageAsync(Owner(), 21, CancellationToken.None));
        Assert.True(await service.DeleteMessageAsync(Owner(), 20, CancellationToken.None));
        Assert.False(context.MailRecipients.Any(x => x.Id == 20));
        Assert.Single(context.MailRecipientsDeleteds);
        Assert.False(await service.DeleteDraftAsync(Owner(), 31, CancellationToken.None));
        Assert.True(await service.DeleteDraftAsync(Owner(), 30, CancellationToken.None));
        Assert.False(context.MailDrafts.Any(x => x.DraftId == 30));
    }

    [Fact]
    public async Task SendMessage_DeliversToUsersAndGroupsAndRemovesTheDraft()
    {
        await using var context = CreateContext("mail-send");
        SeedUsers(context);
        context.MailDrafts.Add(
            new MailDraft
            {
                DraftId = 30,
                FromUserId = 1,
                Subject = "Draft",
                MessagePlainText = "draft",
                Recipients = "[]",
            }
        );
        context.MailDrafts.Add(
            new MailDraft
            {
                DraftId = 31,
                FromUserId = 1,
                Subject = "Keep",
                MessagePlainText = "keep",
                Recipients = "[]",
            }
        );
        context.UserGroupsMemberships.Add(
            new UserGroupsMembership
            {
                MembershipId = 4,
                GroupId = 7,
                UserId = 2,
                User = context.Users.Local.Single(x => x.UserId == 2),
            }
        );
        await context.SaveChangesAsync();

        var controller = BuildController(context);
        var result = await controller.SendMessage(
            new MailSendRequestDto
            {
                DraftId = 30,
                Subject = "Hello",
                Message = "<p>Hi</p>",
                Text = "Hi",
                Share = true,
                ShareName = "Census",
                ShareUrl = "/reports?id=6",
                To = new[]
                {
                    new MailSendRecipientDto { UserId = 2, Type = "u" },
                    new MailSendRecipientDto { UserId = 7, Type = "g" },
                },
            }
        );

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<MailSendResponseDto>(ok.Value);
        Assert.Equal(2, payload.RecipientCount);
        Assert.Equal(2, payload.ShareCount);
        Assert.Equal(2, context.MailRecipients.Count());
        Assert.Contains(context.MailRecipients, x => x.ToUserId == 2 && x.ToGroupId == null);
        Assert.Contains(context.MailRecipients, x => x.ToUserId == 2 && x.ToGroupId == 7);
        Assert.Equal(2, context.SharedItems.Count(x => x.SharedToUserId == 2));
        Assert.False(context.MailDrafts.Any(x => x.DraftId == 30));
        Assert.True(context.MailDrafts.Any(x => x.DraftId == 31));
    }

    [Fact]
    public async Task SendMessage_RejectsAnEmptyRecipientList()
    {
        await using var context = CreateContext("mail-send-empty");
        SeedUsers(context);
        await context.SaveChangesAsync();

        var result = await BuildController(context).SendMessage(
            new MailSendRequestDto { Subject = "Hello", To = Array.Empty<MailSendRecipientDto>() }
        );

        Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Empty(context.MailMessages);
    }

    private static Atlas_WebContext CreateContext(string name)
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new Atlas_WebContext(options);
    }

    private static void SeedUsers(Atlas_WebContext context)
    {
        context.Users.AddRange(
            new User
            {
                UserId = 1,
                Username = "owner",
                FullnameCalc = "Owner Name",
                FirstnameCalc = "Owner",
            },
            new User
            {
                UserId = 2,
                Username = "writer",
                FullnameCalc = "Writer Name",
                FirstnameCalc = "Writer",
            }
        );
    }

    private static MailApiController BuildController(Atlas_WebContext context)
    {
        var interactions = new InteractionsApiService(
            context,
            new ConfigurationBuilder().Build(),
            null,
            null,
            new MemoryCache(new MemoryCacheOptions()),
            null
        );
        return new MailApiController(new MailApiService(context), interactions)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = Owner() },
            },
        };
    }

    private static ClaimsPrincipal Owner()
    {
        return new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.Name, "owner"),
                    new Claim("UserId", "1"),
                    new Claim("Fullname", "Owner Name"),
                },
                "Test"
            )
        );
    }
}
