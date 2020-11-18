using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class DpDataProjectConversationMessage
    {
        public int DataProjectConversationMessageId { get; set; }
        public int? DataProjectConversationId { get; set; }
        public int? UserId { get; set; }
        public string MessageText { get; set; }
        public DateTime? PostDateTime { get; set; }
        public string UserName { get; set; }

        public virtual DpDataProjectConversation DataProjectConversation { get; set; }
        public virtual User User { get; set; }
    }
}
