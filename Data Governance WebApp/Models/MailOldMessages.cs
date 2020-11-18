using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class MailOldMessages
    {
        public MailOldMessages()
        {
            MailConversations = new HashSet<MailConversations>();
        }

        public int MessageId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime? SendDate { get; set; }
        public DateTime? ReadDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? MessageTypeId { get; set; }
        public int? FromUserId { get; set; }
        public int? AlertDisplayed { get; set; }

        public virtual User FromUser { get; set; }
        public virtual MailMessageType MessageType { get; set; }
        public virtual ICollection<MailConversations> MailConversations { get; set; }
    }
}
