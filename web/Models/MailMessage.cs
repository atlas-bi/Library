namespace Atlas_Web.Models
{
    public partial class MailMessage
    {
        public MailMessage()
        {
            MailConversations = new HashSet<MailConversation>();
            MailFolderMessages = new HashSet<MailFolderMessage>();
            MailRecipients = new HashSet<MailRecipient>();
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
        public virtual ICollection<MailConversation> MailConversations { get; set; }
        public virtual ICollection<MailFolderMessage> MailFolderMessages { get; set; }
        public virtual ICollection<MailRecipient> MailRecipients { get; set; }
    }
}
