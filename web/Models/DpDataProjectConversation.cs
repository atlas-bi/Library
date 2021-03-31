using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class DpDataProjectConversation
    {
        public DpDataProjectConversation()
        {
            DpDataProjectConversationMessages = new HashSet<DpDataProjectConversationMessage>();
        }

        public int DataProjectConversationId { get; set; }
        public int DataProjectId { get; set; }

        public virtual DpDataProject DataProject { get; set; }
        public virtual ICollection<DpDataProjectConversationMessage> DpDataProjectConversationMessages { get; set; }
    }
}
