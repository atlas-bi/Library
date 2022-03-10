using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class MailMessageType
    {
        public MailMessageType()
        {
            MailMessages = new HashSet<MailMessage>();
        }

        public int MessageTypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<MailMessage> MailMessages { get; set; }
    }
}
