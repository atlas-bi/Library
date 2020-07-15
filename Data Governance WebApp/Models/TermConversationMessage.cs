using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class TermConversationMessage
    {
        public int TermConversationMessageId { get; set; }
        public int TermConversationId { get; set; }
        public int UserId { get; set; }
        public string MessageText { get; set; }
        public DateTime PostDateTime { get; set; }
        public string UserName { get; set; }

        public virtual TermConversation TermConversation { get; set; }
        public virtual User User { get; set; }
    }
}
