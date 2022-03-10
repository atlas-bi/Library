using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class ReportObjectConversationDoc
    {
        public ReportObjectConversationDoc()
        {
            ReportObjectConversationMessageDocs = new HashSet<ReportObjectConversationMessageDoc>();
        }

        public int ConversationId { get; set; }
        public int ReportObjectId { get; set; }

        public virtual ReportObject ReportObject { get; set; }
        public virtual ICollection<ReportObjectConversationMessageDoc> ReportObjectConversationMessageDocs { get; set; }
    }
}
