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

namespace Atlas_Web.Pages.Mail
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;

        public IndexModel(Atlas_WebContext context, IMemoryCache cache, IConfiguration config)
        {
            _context = context;
            _cache = cache;
            _config = config;
        }

        public class MessageCountData
        {
            public int MessageCount { get; set; }
        }

        public class NewMailAlertData
        {
            public string From { get; set; }
            public string Message { get; set; }
            public string Subject { get; set; }
        }

        public class MailRecipientsData
        {
            public int Id { get; set; }
            public string Fullname { get; set; }
            public string Type { get; set; }
        }

        private sealed class MailRecipientJsonData
        {
#pragma warning disable S3459
            public int UserId { get; set; }
            public string Type { get; set; }
#pragma warning restore S3459
        }

        public class AllMailData
        {
            public int MessId { get; set; }
            public int RepId { get; set; }
            public string From { get; set; }
            public int FromId { get; set; }
            public IEnumerable<MailRecipientsData> To { get; set; }
            public string Message { get; set; }
            public string Subject { get; set; }
            public DateTime Sent { get; set; }
            public int Read { get; set; }
            public string Type { get; set; }
            public int? TypeId { get; set; }
            public int ConvId { get; set; }
            public string Sent_Preview { get; set; }
            public int AlertDisplayed { get; set; }
        }

        public class MessageData
        {
            public int MessId { get; set; }
            public int RepId { get; set; }
            public string From { get; set; }
            public int FromId { get; set; }
            public string Message { get; set; }
            public string Subject { get; set; }
            public string Sent_Read { get; set; }
            public IEnumerable<MailRecipientsData> To { get; set; }
            public string Type { get; set; }
            public int? TypeId { get; set; }
            public int ConvId { get; set; }
        }

        public class DraftData
        {
            public int DraftId { get; set; }
            public string From { get; set; }
            public int FromId { get; set; }
            public string To { get; set; }
            public string Message { get; set; }
            public string Subject { get; set; }
            public string Edited { get; set; }
            public int? MsgId { get; set; }
            public int? ConvId { get; set; }
        }

        public string FirstName { get; set; }

        public void OnGet()
        {
            // mail is never directory accessed
        }

        public List<UserPreference> Preferences { get; set; }

        public async Task<ActionResult> OnPostCheckForMail()
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            FirstName = MyUser.FirstnameCalc;

            // get all mail

            var AllMail = await (
                from m in _context.MailMessages
                join mr in _context.MailRecipients on m.MessageId equals mr.MessageId into tmp
                from mr in tmp.DefaultIfEmpty()
                where mr.ToUserId == MyUser.UserId
                select new AllMailData
                {
                    MessId = mr.Message.MessageId,
                    RepId = mr.Id,
                    From = mr.Message.FromUser.FullnameCalc,
                    FromId = (int)mr.Message.FromUserId,
                    Message = mr.Message.SmallMessage,
                    Subject = mr.Message.SmallSubject,
                    Sent_Preview = mr.Message.SendDate_MessagePreview,
                    Sent = (DateTime)mr.Message.SendDate,
                    Read = mr.ReadDate != null ? 1 : 0,
                    Type = mr.Message.MessageType.Name,
                    TypeId = mr.Message.MessageTypeId,
                    ConvId = mr.Message.MailConversations
                        .Select(x => x.ConversationId)
                        .FirstOrDefault(),
                    AlertDisplayed = (mr.AlertDisplayed ?? 0)
                }
            ).AsNoTracking().ToListAsync();

            // what to do if this is null. can't use "count" in the html
            var NewMail = (
                from m in AllMail
                where m.AlertDisplayed == 0
                select new NewMailAlertData
                {
                    From = m.From,
                    Message = m.Message,
                    Subject = m.Subject
                }
            ).ToList();

            // update flag for alert displayed
            (
                await (
                    from mr in _context.MailRecipients
                    where
                        mr.ToUserId == MyUser.UserId
                        && (mr.AlertDisplayed == 0 || mr.AlertDisplayed == null)
                    select mr
                ).ToListAsync()
            ).ForEach(x => x.AlertDisplayed = 1);

            await _context.SaveChangesAsync();

            if (NewMail is null)
            {
                ViewData["NewMail"] = new List<NewMailAlertData>();
            }
            else
            {
                ViewData["NewMail"] = NewMail;
            }

            var UnreadMessageCount = (
                from m in AllMail
                where m.Read == 0
                group m by 1 into grp
                select new MessageCountData { MessageCount = grp.Count() }
            ).FirstOrDefault();
            if (UnreadMessageCount is null)
            {
                ViewData["UnreadMessageCount"] = 0;
            }
            else
            {
                ViewData["UnreadMessageCount"] = UnreadMessageCount.MessageCount;
            }

            var MessageCount = (
                from m in AllMail
                group m by 1 into grp
                select new MessageCountData { MessageCount = grp.Count() }
            ).FirstOrDefault();

            if (MessageCount is null)
            {
                ViewData["MessageCount"] = 0;
            }
            else
            {
                ViewData["MessageCount"] = MessageCount.MessageCount;
            }
            ViewData["DraftMessageCount"] = _context.MailDrafts
                .Where(x => x.FromUserId == MyUser.UserId)
                .Count();

            ViewData["AllMail"] = new List<AllMailData>();

            var newMessagePreviews = (
                from m in AllMail
                orderby m.Sent descending
                select new AllMailData
                {
                    MessId = m.MessId,
                    RepId = m.RepId,
                    From = m.From,
                    Message = m.Message,
                    Subject = m.Subject,
                    Sent_Preview = m.Sent_Preview,
                    Sent = m.Sent,
                    Read = m.Read,
                    Type = m.Type,
                    TypeId = m.TypeId,
                    ConvId = m.ConvId
                }
            ).ToList();

            if (newMessagePreviews != null && newMessagePreviews.Count > 0)
            {
                ViewData["AllMail"] = newMessagePreviews;
            }

            return new PartialViewResult
            {
                ViewName = "Partials/_CheckForMail",
                ViewData = ViewData
            };
        }

        public async Task<ActionResult> OnGetGetMailbox(int id)
        {
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Remove("Pragma");

            HttpContext.Response.Headers.Add(
                "Cache-Control",
                "no-cache, no-store, must-revalidate"
            );
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            if (MyUser.UserId != id)
            {
                return Content("You are unauthorized bro.");
            }

            FirstName = MyUser.FirstnameCalc;

            var AllMail = await (
                from r in _context.MailRecipients
                where r.ToUserId == MyUser.UserId
                orderby r.Message.SendDate descending
                select new AllMailData
                {
                    MessId = r.Message.MessageId,
                    RepId = r.Id,
                    From = r.Message.FromUser.FullnameCalc,
                    Message = r.Message.SmallMessage,
                    Subject = r.Message.SmallSubject,
                    Sent_Preview = r.Message.SendDate_MessagePreview,
                    Sent = (DateTime)r.Message.SendDate,
                    Read = r.ReadDate != null ? 1 : 0,
                    Type = r.Message.MessageType.Name,
                    TypeId = r.Message.MessageTypeId,
                    ConvId = r.Message.MailConversations
                        .Select(x => x.ConversationId)
                        .FirstOrDefault()
                }
            ).ToListAsync();

            if (AllMail is null)
            {
                ViewData["AllMail"] = new List<AllMailData>();
                ViewData["UnreadMail"] = "";
            }
            else
            {
                ViewData["AllMail"] = AllMail;
                ViewData["UnreadMail"] = AllMail.Count(x => x.Read == 0);
            }
            ViewData["Drafts"] = _context.MailDrafts
                .Where(x => x.FromUserId == MyUser.UserId)
                .Count();

            return new PartialViewResult { ViewName = "Partials/_Mailbox", ViewData = ViewData };
        }

        public async Task<ActionResult> OnPostMarkMessageRead(int id)
        {
            // only update read flag if the auth user has read it.
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            _context.MailRecipients
                .Where(i => i.Id == id && i.ToUserId == MyUser.UserId)
                .ToList()
                .ForEach(
                    x =>
                    {
                        x.ReadDate = DateTime.Now;
                        x.AlertDisplayed = 1;
                    }
                );
            await _context.SaveChangesAsync();
            return Content("ok");
        }

        // to do - add mark message unread

        public async Task<ActionResult> OnGetMessageDetails(int id)
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            var Message = await (
                from r in _context.MailRecipients
                where r.Id == id && r.ToUserId == MyUser.UserId
                select new MessageData
                {
                    MessId = (int)r.MessageId,
                    RepId = r.Id,
                    From = r.Message.FromUser.FullnameCalc,
                    FromId = (int)r.Message.FromUserId,
                    Message = r.Message.Message,
                    Subject = r.Message.Subject,
                    Sent_Read = r.Message.SendDate_MessageReader,
                    To = (
                        from u in _context.MailRecipients.AsEnumerable()
                        where u.MessageId == r.MessageId
                        select new MailRecipientsData
                        {
                            Id = (int)(u.ToGroupId ?? u.ToUserId),
                            Fullname = u.ToGroup.GroupName ?? u.ToUser.FullnameCalc,
                            Type = u.ToGroupId == null ? "" : "group"
                        }
                    ).ToList(),
                    Type = r.Message.MessageType.Name,
                    TypeId = r.Message.MessageTypeId,
                    ConvId = r.Message.MailConversations
                        .Select(x => x.ConversationId)
                        .FirstOrDefault()
                }
            ).FirstOrDefaultAsync();
            if (Message is null)
            {
                ViewData["Message"] = new MessageData();
            }
            else
            {
                ViewData["Message"] = Message;
            }

            return new PartialViewResult
            {
                ViewName = "Partials/_MessageBody",
                ViewData = ViewData
            };
        }

        public async Task<ActionResult> OnGetDraftDetails(int id)
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            var Message = await (
                from r in _context.MailDrafts
                where r.DraftId == id && r.FromUserId == MyUser.UserId
                select new DraftData
                {
                    DraftId = r.DraftId,
                    From = r.FromUser.FullnameCalc,
                    FromId = (int)r.FromUserId,
                    To = r.Recipients,
                    Message = r.Message,
                    Subject = r.Subject,
                    Edited = r.EditDate_MessagePreview,
                    MsgId = r.ReplyToMessageId,
                    ConvId = r.ReplyToConvId
                }
            ).FirstOrDefaultAsync();
            if (Message is null)
            {
                ViewData["Message"] = new DraftData();
            }
            else
            {
                ViewData["Message"] = Message;
            }

            return new PartialViewResult { ViewName = "Partials/_DraftBody", ViewData = ViewData };
        }

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

            var AllTo = JsonConvert.DeserializeObject<IEnumerable<MailRecipientJsonData>>(To);
            var Users = AllTo
                .Where(x => x.Type != "g" || x.Type is null || x.Type == "")
                .Select(x => new { UserId = Int32.Parse(x.UserId.ToString()) });
            var Groups = AllTo
                .Where(x => x.Type == "g")
                .Select(x => new { GroupId = Int32.Parse(x.UserId.ToString()) });

            var GroupUsers = (
                from ulm in _context.UserGroupsMemberships
                where (from g in Groups select g.GroupId).Contains((int)ulm.GroupId)
                select new { ulm.UserId, ulm.GroupId }
            );

            if (!Users.Any() && !GroupUsers.Any())
            {
                return Content("no users specified");
            }

            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            // insert message
            MailMessage newMessage =
                new()
                {
                    Subject = Subject,
                    Message = Message,
                    SendDate = DateTime.Now,
                    FromUserId = MyUser.UserId,
                    MessagePlainText = Text
                };
            _context.Add(newMessage);
            await _context.SaveChangesAsync();
            // add users
            await _context.AddRangeAsync(
                Users.Select(
                    x => new MailRecipient { MessageId = newMessage.MessageId, ToUserId = x.UserId }
                )
            );
            await _context.SaveChangesAsync();
            // add group users
            await _context.AddRangeAsync(
                GroupUsers.Select(
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
            // add share
            if (Share == "1")
            {
                foreach (var user in Users)
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
                    _context.Add(newShare);
                }
                foreach (var user in GroupUsers)
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
                    _context.Add(newShare);
                }
                await _context.SaveChangesAsync();
            }

            return Content("peace");
        }

        public async Task<ActionResult> OnPostDeleteMessage(int id)
        {
            var Old = await _context.MailRecipients.Where(x => x.Id == id).FirstOrDefaultAsync();
            await _context.AddAsync(
                new MailRecipientsDeleted
                {
                    AlertDisplayed = Old.AlertDisplayed,
                    MessageId = Old.MessageId,
                    ReadDate = Old.ReadDate,
                    ToUserId = Old.ToUserId,
                    ToGroupId = Old.ToGroupId
                }
            );
            _context.Remove(Old);
            await _context.SaveChangesAsync();
            return Content("ok");
        }

        public async Task<ActionResult> OnPostDeleteDraft(int id)
        {
            _context.Remove(_context.MailDrafts.Where(x => x.DraftId == id).FirstOrDefault());
            await _context.SaveChangesAsync();
            return Content("ok");
        }

        public async Task<ActionResult> OnPostSaveDraft()
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            var body = await new System.IO.StreamReader(Request.Body)
                .ReadToEndAsync()
                .ConfigureAwait(false);
            var package = JObject.Parse(body);

            int? DraftId = (int?)package["DraftId"];
            string To = package.Value<string>("To") ?? "";
            string Subject = package.Value<string>("Subject") ?? "";
            string Message = package.Value<string>("Message") ?? "";
            string Text = package.Value<string>("Text") ?? "";
            int? MsgId = (int?)package["MsgId"];
            int? ConvId = (int?)package["ConvId"];

            if (DraftId >= 0)
            {
                var Draft = _context.MailDrafts
                    .Where(x => x.DraftId == (int)DraftId)
                    .FirstOrDefault();
                Draft.EditDate = DateTime.Now;
                Draft.Message = Message;
                Draft.MessagePlainText = Text;
                Draft.Recipients = To;
                Draft.Subject = Subject;
                Draft.ReplyToConvId = ConvId;
                Draft.ReplyToMessageId = MsgId;
                await _context.SaveChangesAsync();
                return Content("ok");
            }
            else
            {
                var Draft = new MailDraft
                {
                    EditDate = DateTime.Now,
                    Message = Message,
                    MessagePlainText = Text,
                    Recipients = To,
                    Subject = Subject,
                    ReplyToConvId = ConvId,
                    ReplyToMessageId = MsgId,
                    FromUserId = MyUser.UserId
                };
                await _context.AddAsync(Draft);
                await _context.SaveChangesAsync();
                return Content(Draft.DraftId.ToString());
            }
        }

        public async Task<ActionResult> OnGetMessages(int? folderId, string folder)
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            if (folder == "drafts")
            {
                var AllDrafts = await (
                    from r in _context.MailDrafts
                    where r.FromUserId == MyUser.UserId
                    orderby r.EditDate descending
                    select new DraftData
                    {
                        DraftId = r.DraftId,
                        From = r.FromUser.FullnameCalc,
                        To = r.Recipients,
                        Message = r.SmallMessage,
                        Subject = r.SmallSubject,
                        Edited = r.EditDate_MessagePreview,
                        MsgId = r.ReplyToMessageId,
                        ConvId = r.ReplyToConvId
                    }
                ).ToListAsync();

                if (AllDrafts is null)
                {
                    ViewData["AllDrafts"] = new List<DraftData>();
                }
                else
                {
                    ViewData["AllDrafts"] = AllDrafts;
                }

                return new PartialViewResult
                {
                    ViewName = "Partials/Mailbox/_DraftPreview",
                    ViewData = ViewData
                };
            }
            else if (folder == "inbox")
            {
                var AllMail = await (
                    from r in _context.MailRecipients
                    where r.ToUserId == MyUser.UserId
                    orderby r.Message.SendDate descending
                    select new AllMailData
                    {
                        MessId = r.Message.MessageId,
                        RepId = r.Id,
                        From = r.Message.FromUser.FullnameCalc,
                        Message = r.Message.SmallMessage,
                        Subject = r.Message.SmallSubject,
                        Sent_Preview = r.Message.SendDate_MessagePreview,
                        Sent = (DateTime)r.Message.SendDate,
                        Read = r.ReadDate != null ? 1 : 0,
                        Type = r.Message.MessageType.Name,
                        TypeId = r.Message.MessageTypeId,
                        ConvId = r.Message.MailConversations
                            .Select(x => x.ConversationId)
                            .FirstOrDefault()
                    }
                ).ToListAsync();

                if (AllMail is null)
                {
                    ViewData["AllMail"] = new List<AllMailData>();
                }
                else
                {
                    ViewData["AllMail"] = AllMail;
                }

                return new PartialViewResult
                {
                    ViewName = "Partials/Mailbox/_MessagePreview",
                    ViewData = ViewData
                };
            }
            else if (folder == "deleted")
            {
                return Content("no folders existing");
            }
            else if (folder == "sent")
            {
                return Content("no folders existing");
            }
            else if (folderId >= 0)
            {
                return Content("no folders existing");
            }
            else
            {
                return Content("no folder selected");
            }
        }
    }
}
