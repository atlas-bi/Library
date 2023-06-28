using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    public interface MailDrafts__Metadata { }

    [ModelMetadataType(typeof(MailDrafts__Metadata))]
    public partial class MailDraft
    {
        [NotMapped]
        public virtual string EditDate_MessagePreview
        // don't display the time portion if > 24 hrs ago
        {
            get
            {
                if (EditDate == null)
                {
                    return "";
                }
                var timeAgo = DateTime.Now.Subtract(EditDate ?? DateTime.Now);
                if (timeAgo.TotalDays < 1)
                {
                    return (EditDate ?? DateTime.Now).ToString("h:mm tt");
                }
                else if (timeAgo.TotalHours < 2)
                {
                    return "Yesterday";
                }
                else
                {
                    return (EditDate ?? DateTime.Now).ToString("M/d/yy");
                }
            }
        }

        [NotMapped]
        public virtual string SmallSubject
        {
            get
            {
                if (Subject == null)
                {
                    return "";
                }
                return string.Join(" ", Subject.Split(' ').Take(5));
            }
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
