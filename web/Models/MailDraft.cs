using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class MailDraft
    {
        public int DraftId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime? EditDate { get; set; }
        public int? MessageTypeId { get; set; }
        public int? FromUserId { get; set; }
        public string MessagePlainText { get; set; }
        public string Recipients { get; set; }
        public int? ReplyToMessageId { get; set; }
        public int? ReplyToConvId { get; set; }

        public virtual User FromUser { get; set; }
    }
}
