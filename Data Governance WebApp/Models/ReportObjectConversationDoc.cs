using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class ReportObjectConversationDoc
    {
        public ReportObjectConversationDoc()
        {
            ReportObjectConversationMessageDoc = new HashSet<ReportObjectConversationMessageDoc>();
        }

        public int ConversationId { get; set; }
        public int ReportObjectId { get; set; }

        public virtual ReportObject ReportObject { get; set; }
        public virtual ICollection<ReportObjectConversationMessageDoc> ReportObjectConversationMessageDoc { get; set; }
    }
}
