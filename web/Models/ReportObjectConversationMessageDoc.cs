using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class ReportObjectConversationMessageDoc
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public string MessageText { get; set; }
        public DateTime PostDateTime { get; set; }
        public string Username { get; set; }

        public virtual ReportObjectConversationDoc Conversation { get; set; }
        public virtual User User { get; set; }
    }
}
