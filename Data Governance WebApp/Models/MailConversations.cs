using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class MailConversations
    {
        public int ConversationId { get; set; }
        public int MessageId { get; set; }

        public virtual MailMessages Message { get; set; }
    }
}
