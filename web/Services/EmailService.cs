using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Net;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Atlas_Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _env;

        public EmailService(IConfiguration config, IWebHostEnvironment env, IMemoryCache cache)
        {
            _config = config;
            _env = env;
            _cache = cache;
        }

        public async Task SendAsync(string subject, string body, string sender, string reciever)
        {
            using (var smtp = new SmtpClient())
            {
                smtp.EnableSsl = bool.Parse(_config["AppSettings:smtp_use_ssl"]);

                if (_env.IsDevelopment())
                {
                    smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtp.PickupDirectoryLocation = @"c:\maildump";
                    smtp.EnableSsl = false;
                }

                var message = new System.Net.Mail.MailMessage
                {
                    Subject = subject,
                    From = new MailAddress(
                        _config["AppSettings:smtp_sender_email"],
                        _config["AppSettings:smtp_sender_name"]
                    ),
                    IsBodyHtml = true
                };

                message.To.Add(reciever);

                if (!string.IsNullOrEmpty(sender))
                {
                    message.ReplyToList.Add(sender);
                }
                else
                {
                    message.ReplyToList.Add(_config["AppSettings:smtp_default_reply_email"]);
                }

                AlternateView HTMLEmail = AlternateView.CreateAlternateViewFromString(
                    body,
                    System.Text.Encoding.UTF8,
                    MediaTypeNames.Text.Html
                );

                LinkedResource MyImage = new LinkedResource(
                    _cache.Get("logo_path").ToString(),
                    "image/png"
                )
                {
                    ContentId = "atlas_logo",
                    TransferEncoding = TransferEncoding.Base64,
                };

                MyImage.ContentType.Name = MyImage.ContentId;
                MyImage.ContentLink = new Uri("cid:" + MyImage.ContentId);

                HTMLEmail.LinkedResources.Add(MyImage);

                // The alt added last will be the default.
                // Reminder in case there is ever a need to
                // a plain text alt.
                message.AlternateViews.Add(HTMLEmail);

                if (_config["AppSettings:smtp_password"] != null)
                {
                    var credential = new NetworkCredential
                    {
                        UserName = _config["AppSettings:smtp_username"],
                        Password = _config["AppSettings:smtp_password"],
                    };
                    smtp.Credentials = credential;
                }

                smtp.Host = _config["AppSettings:smtp_server"];
                smtp.Port = Int32.Parse(_config["AppSettings:smtp_port"]);

                await smtp.SendMailAsync(message);
            }
        }
    }
}
