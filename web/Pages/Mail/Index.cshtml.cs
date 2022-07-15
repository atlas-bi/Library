using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas_Web.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Hangfire;

namespace Atlas_Web.Pages.Mail
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IEmailService _emailer;

        public IndexModel(
            Atlas_WebContext context,
            IMemoryCache cache,
            IConfiguration config,
            IRazorPartialToStringRenderer renderer,
            IEmailService emailer
        )
        {
            _context = context;
            _cache = cache;
            _config = config;
            _renderer = renderer;
            _emailer = emailer;
        }

        private sealed class MailRecipientJsonData
        {
#pragma warning disable S3459
            public int UserId { get; set; }
            public string Type { get; set; }
#pragma warning restore S3459
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
                .Where(m => GroupIds.Contains((int)m.GroupId))
                .ToListAsync();

            if (!UserIds.Any() && !GroupUserList.Any())
            {
                return Content("no users specified");
            }

            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            // insert message
            Atlas_Web.Models.MailMessage newMessage =
                new()
                {
                    Subject = Subject,
                    Message = Message,
                    SendDate = DateTime.Now,
                    FromUserId = MyUser.UserId,
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
                foreach (var user in UserList)
                {
                    SharedItem newShare =
                        new()
                        {
                            SharedFromUserId = MyUser.UserId,
                            SharedToUserId = user.UserId,
                            ShareDate = DateTime.Now,
                            Name = ShareName,
                            Url = ShareUrl
                        };
                    await _context.AddAsync(newShare);
                    await _context.SaveChangesAsync();

                    var UserSetting = user.UserSettings
                        .Where(x => x.Name == "share_notification")
                        .Select(x => x.Value)
                        .FirstOrDefault();

                    if (!string.IsNullOrEmpty(user.Email) && UserSetting != "0")
                    {
                        ViewData["Subject"] = $"New share from {MyUser.FullnameCalc}";
                        ViewData["Body"] = Helpers.HtmlHelpers.MarkdownToHtml(Message, _config);
                        ViewData["Sender"] = MyUser;
                        ViewData["Reciever"] = user;
                    }

                    var msgbody = await _renderer.RenderPartialToStringAsync(
                        "_EmailTemplate",
                        ViewData
                    );

                    BackgroundJob.Enqueue<IEmailService>(
                        x =>
                            x.SendAsync(
                                $"New share from {MyUser.FullnameCalc}",
                                HtmlHelpers.MinifyHtml(msgbody),
                                MyUser.Email,
                                user.Email
                            )
                    );
                }

                foreach (var group in GroupUserList)
                {
                    SharedItem newShare =
                        new()
                        {
                            SharedFromUserId = MyUser.UserId,
                            SharedToUserId = group.UserId,
                            ShareDate = DateTime.Now,
                            Name = ShareName,
                            Url = ShareUrl
                        };
                    await _context.AddAsync(newShare);
                    await _context.SaveChangesAsync();

                    // iddea = use the group email address when possible. here the group is already expanded.
                    if (!string.IsNullOrEmpty(group.User.Email))
                    {
                        ViewData["Subject"] = $"New share from {MyUser.FullnameCalc}";
                        ViewData["Body"] = Helpers.HtmlHelpers.MarkdownToHtml(Message, _config);
                        ViewData["Sender"] = MyUser;
                        ViewData["Reciever"] = group.User;

                        var msgbody = await _renderer.RenderPartialToStringAsync(
                            "_EmailTemplate",
                            ViewData
                        );
                        await _emailer.SendAsync(
                            $"New share from {MyUser.FullnameCalc}",
                            HtmlHelpers.MinifyHtml(msgbody),
                            MyUser.Email,
                            group.User.Email
                        );
                    }
                }
            }
            return Content("Successfully shared.");
        }
    }
}
