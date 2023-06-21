using Atlas_Web.Models;
using Atlas_Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Atlas_Web.Services;

using Hangfire;

namespace Atlas_Web.Pages.Mail
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;

        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IEmailService _emailer;

        public IndexModel(
            Atlas_WebContext context,
            IConfiguration config,
            IRazorPartialToStringRenderer renderer,
            IEmailService emailer
        )
        {
            _context = context;
            _config = config;
            _renderer = renderer;
            _emailer = emailer;
        }

        private sealed class MailRecipientJsonData
        {
#pragma warning disable S3459,S1144
            public int UserId { get; set; }
            public string Type { get; set; }
#pragma warning restore S3459,S1144
        }

        public void OnGet()
        {
            // mail is never directory accessed
        }

        public List<UserPreference> Preferences { get; set; }

        public async Task<ActionResult> OnPostSendMail()
        {
            var body = await new System.IO.StreamReader(Request.Body)
                .ReadToEndAsync()
                .ConfigureAwait(false);
            var package = JObject.Parse(body);

            int? DraftId = (int?)package["DraftId"];
            string To = package.Value<string>("To") ?? "";
            string Subject = package.Value<string>("Subject") ?? "";
            string Message = package.Value<string>("Message") ?? "";
            string Text = package.Value<string>("Text") ?? "";
            string Share = package.Value<string>("Share") ?? "";
            string ShareName = package.Value<string>("ShareName") ?? "";
            string ShareUrl = package.Value<string>("ShareUrl") ?? "";

            // List of ID's {Type,UserId}
            var AllTo = JsonConvert.DeserializeObject<IEnumerable<MailRecipientJsonData>>(To);

            // Split out Users
            var UserIds = AllTo
                .Where(x => x.Type != "g" || x.Type is null || x.Type == "")
                .Select(x => Int32.Parse(x.UserId.ToString()))
                .ToList();

            // Split out groups
            var GroupIds = AllTo
                .Where(x => x.Type == "g")
                .Select(x => Int32.Parse(x.UserId.ToString()))
                .ToList();

            // query db for User details
            var UserList = await _context.Users
                .Where(x => UserIds.Contains(x.UserId))
                .ToListAsync();

            var GroupUserList = await _context.UserGroupsMemberships
                .Where(m => GroupIds.Contains(m.GroupId))
                .ToListAsync();

            if (!UserIds.Any() && !GroupUserList.Any())
            {
                return Content("no users specified");
            }

            // insert message
            Atlas_Web.Models.MailMessage newMessage =
                new()
                {
                    Subject = Subject,
                    Message = Message,
                    SendDate = DateTime.Now,
                    FromUserId = User.GetUserId(),
                    MessagePlainText = Text
                };
            await _context.AddAsync(newMessage);
            await _context.SaveChangesAsync();

            // add users
            await _context.AddRangeAsync(
                UserList.Select(
                    x => new MailRecipient { MessageId = newMessage.MessageId, ToUserId = x.UserId }
                )
            );
            await _context.SaveChangesAsync();

            // add group users
            await _context.AddRangeAsync(
                GroupUserList.Select(
                    x =>
                        new MailRecipient
                        {
                            MessageId = newMessage.MessageId,
                            ToUserId = x.UserId,
                            ToGroupId = x.GroupId
                        }
                )
            );
            await _context.SaveChangesAsync();

            if (DraftId >= 0)
            {
                // delete draft
                _context.RemoveRange(_context.MailDrafts.Where(x => x.DraftId == (int)DraftId));
                await _context.SaveChangesAsync();
            }

            // add share to Users
            if (Share == "1")
            {
                var sender = await _context.Users.SingleAsync(x => x.UserId == User.GetUserId());
                foreach (var recipient in UserList)
                {
                    SharedItem newShare =
                        new()
                        {
                            SharedFromUserId = User.GetUserId(),
                            SharedToUserId = recipient.UserId,
                            ShareDate = DateTime.Now,
                            Name = ShareName,
                            Url = ShareUrl
                        };
                    await _context.AddAsync(newShare);
                    await _context.SaveChangesAsync();

                    var UserSetting = await _context.UserSettings
                        .Where(x => x.Name == "share_notification" && x.UserId == recipient.UserId)
                        .Select(x => x.Value)
                        .FirstOrDefaultAsync();

                    if (!string.IsNullOrEmpty(recipient.Email) && UserSetting != "N")
                    {
                        ViewData["Subject"] = $"New share from {sender.FullnameCalc}";
                        ViewData["Body"] = Helpers.HtmlHelpers.MarkdownToHtml(Message, _config);
                        ViewData["Sender"] = sender;
                        ViewData["Receiver"] = recipient;

                        var msgbody = await _renderer.RenderPartialToStringAsync(
                            "_EmailTemplate",
                            ViewData
                        );

                        BackgroundJob.Enqueue<IEmailService>(
                            x =>
                                x.SendAsync(
                                    $"New share from {sender.FullnameCalc}",
                                    HtmlHelpers.MinifyHtml(msgbody),
                                    sender.Email,
                                    recipient.Email
                                )
                        );
                    }
                }

                foreach (var group in GroupUserList)
                {
                    SharedItem newShare =
                        new()
                        {
                            SharedFromUserId = sender.UserId,
                            SharedToUserId = group.UserId,
                            ShareDate = DateTime.Now,
                            Name = ShareName,
                            Url = ShareUrl
                        };
                    await _context.AddAsync(newShare);
                    await _context.SaveChangesAsync();

                    var GroupUserSetting = await _context.UserSettings
                        .Where(x => x.Name == "share_notification" && x.UserId == group.UserId)
                        .Select(x => x.Value)
                        .FirstOrDefaultAsync();

                    // iddea = use the group email address when possible. here the group is already expanded.
                    if (!string.IsNullOrEmpty(group.User.Email) && GroupUserSetting != "N")
                    {
                        ViewData["Subject"] = $"New share from {sender.FullnameCalc}";
                        ViewData["Body"] = Helpers.HtmlHelpers.MarkdownToHtml(Message, _config);
                        ViewData["Sender"] = sender;
                        ViewData["Receiver"] = group.User;

                        var msgbody = await _renderer.RenderPartialToStringAsync(
                            "_EmailTemplate",
                            ViewData
                        );
                        await _emailer.SendAsync(
                            $"New share from {sender.FullnameCalc}",
                            HtmlHelpers.MinifyHtml(msgbody),
                            sender.Email,
                            group.User.Email
                        );
                    }
                }
            }
            return Content("Successfully shared.");
        }
    }
}
