using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    public interface MailMessages__Metadata { }

    [ModelMetadataType(typeof(MailMessages__Metadata))]
    public partial class MailMessage
    {
        [NotMapped]
        public virtual string SendDate_MessagePreview
        // don't display the time portion if > 24 hrs ago
        {
            get
            {
                if (SendDate == null)
                {
                    return "";
                }
                var timeAgo = DateTime.Now.Subtract(SendDate ?? DateTime.Now);
                if (timeAgo.TotalDays < 1)
                {
                    return (SendDate ?? DateTime.Now).ToString("h:mm tt");
                }
                else if (timeAgo.TotalHours < 2)
                {
                    return "Yesterday";
                }
                else
                {
                    return (SendDate ?? DateTime.Now).ToString("M/d/yy");
                }
            }
        }

        [NotMapped]
        public virtual string SendDate_MessageReader
        // don't display the time portion if > 24 hrs ago
        {
            get
            {
                if (SendDate == null)
                {
                    return "";
                }
                var timeAgo = DateTime.Now.Subtract(SendDate ?? DateTime.Now);
                if (timeAgo.TotalDays < 1)
                {
                    return (SendDate ?? DateTime.Now).ToString("h:mm tt");
                }
                else if (timeAgo.TotalHours < 2)
                {
                    return string.Concat(
                        "Yesterday at ",
                        (SendDate ?? DateTime.Now).ToString("h:mm tt")
                    );
                }
                else
                {
                    return string.Concat(
                        (SendDate ?? DateTime.Now).ToString("M/d/yy"),
                        " at ",
                        (SendDate ?? DateTime.Now).ToString("h:mm tt")
                    );
                }
            }
        }

        [NotMapped]
        public virtual string SmallSubject
        {
            get { return string.Join(" ", Subject.Split(' ').Take(5)); }
        }

        [NotMapped]
        public virtual string SmallMessage
        {
            get
            {
                if (MessagePlainText is null)
                {
                    return "";
                }

                return string.Join(" ", MessagePlainText.Split(' ').Take(10));
            }
        }
    }
}
