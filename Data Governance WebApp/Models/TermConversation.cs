using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class TermConversation
    {
        public TermConversation()
        {
            TermConversationMessage = new HashSet<TermConversationMessage>();
        }

        public int TermConversationId { get; set; }
        public int TermId { get; set; }

        public virtual Term Term { get; set; }
        public virtual ICollection<TermConversationMessage> TermConversationMessage { get; set; }
    }
}
