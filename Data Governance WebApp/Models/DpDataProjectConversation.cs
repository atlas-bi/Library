using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class DpDataProjectConversation
    {
        public DpDataProjectConversation()
        {
            DpDataProjectConversationMessage = new HashSet<DpDataProjectConversationMessage>();
        }

        public int DataProjectConversationId { get; set; }
        public int DataProjectId { get; set; }

        public virtual DpDataProject DataProject { get; set; }
        public virtual ICollection<DpDataProjectConversationMessage> DpDataProjectConversationMessage { get; set; }
    }
}
