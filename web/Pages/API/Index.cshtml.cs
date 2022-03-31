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

namespace Atlas_Web.Pages.API
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;

        public IndexModel(Atlas_WebContext context, IMemoryCache cache, IConfiguration config)
        {
            _context = context;
            _cache = cache;
            _config = config;
        }

        public class Stuffdata
        {
            public string Key { get; set; }
            public int Cnt { get; set; }
        }

        private sealed class MailRecipientJsonData
        {
            public int UserId { get; set; }
            public string Type { get; set; }
        }

        public List<AdList> AdLists { get; set; }

        public void OnGet()
        {
            ViewData["stuff"] = (
                from u in (
                    from u in _context.Users
                    group u by u.FullnameCalc into grp
                    select new { grp.Key, cnt = grp.Count() }
                )
                where u.cnt > 1
                orderby u.cnt descending
                select new Stuffdata { Key = u.Key, Cnt = u.cnt }
            ).ToList();
            ViewData["summary"] = (
                from u in (
                    from u in _context.Users
                    group u by u.FullnameCalc into grp
                    select new { grp.Key, cnt = grp.Count() }
                )
                where u.cnt > 1
                group u by u.cnt into grp
                orderby grp.Count() descending
                select new Stuffdata { Key = grp.Key.ToString(), Cnt = grp.Count() }
            ).ToList();

            ViewData["alluserrs"] = (
                from u in _context.Users
                select new Stuffdata { Key = u.FullnameCalc, Cnt = 0 }
            ).ToList();
        }

        public async Task<ActionResult> OnPostShareObject(string to, string url, string name)
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            var AllTo = JsonConvert.DeserializeObject<IEnumerable<MailRecipientJsonData>>(to);
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
                return Content("no users specefied");
            }

            await _context.AddRangeAsync(
                Users.Select(
                    x =>
                        new SharedItem
                        {
                            Url = url,
                            ShareDate = DateTime.Now,
                            SharedFromUserId = MyUser.UserId,
                            SharedToUserId = x.UserId,
                            Name = name
                        }
                )
            );
            await _context.SaveChangesAsync();

            await _context.AddRangeAsync(
                GroupUsers.Select(
                    x =>
                        new SharedItem
                        {
                            Url = url,
                            ShareDate = DateTime.Now,
                            SharedFromUserId = MyUser.UserId,
                            SharedToUserId = x.UserId,
                            Name = name
                        }
                )
            );
            await _context.SaveChangesAsync();

            /*var toUser = _context.User.Where(x => x.UserId == id).FirstOrDefault();

            var ToUserFirstname = toUser.FirstnameCalc;
            var myMessage = message ?? "I would like to share the following report with you.";
            var FromUserName = user.FullnameCalc;
            var FromUserEmail = user.Email;

            string body = $@"<!DOCTYPE html>
                        <head>
                            <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                            <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
                            <meta name='viewport' content='width=device-width, initial-scale=1'>
                            <style type='text/css'>
                                body {{ margin:0;padding:0;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;background-color:#FAFAFAFF;font-family: &apos;book antiqua&apos;, palatino, serif; font-size: 16px; color: #404040; }}
                                p {{ margin: 0px; }}
                                .box {{margin - top:30px;}}
                            </style>
                        </head>
                        <body>
                            <div>
                                <div class='box'>
                                    <div >
                                        <p>Hi {ToUserFirstname},</p>
                                        <br>
                                        <p>{myMessage}</p>
                                        <br>
                                        <p><a href='{url}' target='_blank'>{name}</a></p>
                                        <br>
                                        <p>Regards,</p>
                                        <p>{FromUserName}</p>
                                        <p>{FromUserEmail}</p>
                                    </div>
                                </div>
                                <div class='box' style='font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:11px;line-height:1.5;color:#000000;'>
                                    <p><span style='color: #34495e;'>This email automatically sent from <a style='color: #34495e;' title='Atlas of Information Management' href='https://Atlas' target='_blank' rel='noopener'>Atlas</a>, Riverside Healthcare's official Report Library</span></p>
                                </div>
                            </div>
                        </body>
                        </html>";

            var fromEmail = user.Email ?? _config["AppSettings:default_from_email_address"];


            using (var smtp = new SmtpClient())
            {
                // usnig smtp
                var credential = new NetworkCredential
                {
                    UserName = _config["AppSettings:default_from_email_address"],  // replace with valid value
                   // Password = "password"  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = _config["AppSettings:smtp_server"];
                smtp.Port = Int32.Parse(_config["AppSettings:smtp_port"]);
                smtp.EnableSsl = bool.Parse(_config["AppSettings:smtp_use_ssl"]);
                var thismessage = new MailMessage();
                thismessage.To.Add(toUser.Email);
                thismessage.Subject = FromUserName + "shared an Atlas Report with you";
                thismessage.Body = body;
                thismessage.IsBodyHtml = true;
                thismessage.From = new MailAddress(fromEmail);
                await smtp.SendMailAsync(thismessage);

                // using mail drop to file

                smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                smtp.PickupDirectoryLocation = @"r:\mailpickup";
                thismessage = new MailMessage();
                thismessage.To.Add(toUser.Email);
                thismessage.Subject = FromUserName + "shared an Atlas Report with you";
                thismessage.Body = body;
                thismessage.IsBodyHtml = true;
                thismessage.From = new MailAddress(fromEmail);
                await smtp.SendMailAsync(thismessage);

            }*/
            return Content("shared");
        }

        public async Task<ActionResult> OnPostRenderMarkdown()
        {
            var body = await new System.IO.StreamReader(Request.Body)
                .ReadToEndAsync()
                .ConfigureAwait(false);
            var package = JObject.Parse(body);

            var md = package.Value<string>("md");
            try
            {
                ViewData["html"] = await Task.Run(() => Helpers.HtmlHelpers.MarkdownToHtml(md));
            }
            catch
            {
                ViewData["html"] = md;
            }

            return new PartialViewResult { ViewName = "Partials/_Html", ViewData = ViewData };
        }
    }
}
