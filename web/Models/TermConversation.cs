using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class TermConversation
    {
        public TermConversation()
        {
            TermConversationMessages = new HashSet<TermConversationMessage>();
        }

        public int TermConversationId { get; set; }
        public int TermId { get; set; }

        public virtual Term Term { get; set; }
        public virtual ICollection<TermConversationMessage> TermConversationMessages { get; set; }
    }
}
