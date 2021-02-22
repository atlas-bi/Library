using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class MailMessages
    {
        public MailMessages()
        {
            MailConversations = new HashSet<MailConversations>();
            MailFolderMessages = new HashSet<MailFolderMessages>();
            MailRecipients = new HashSet<MailRecipients>();
        }

        public int MessageId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime? SendDate { get; set; }
        public int? MessageTypeId { get; set; }
        public int? FromUserId { get; set; }
        public string MessagePlainText { get; set; }

        public virtual User FromUser { get; set; }
        public virtual MailMessageType MessageType { get; set; }
        public virtual ICollection<MailConversations> MailConversations { get; set; }
        public virtual ICollection<MailFolderMessages> MailFolderMessages { get; set; }
        public virtual ICollection<MailRecipients> MailRecipients { get; set; }
    }
}
