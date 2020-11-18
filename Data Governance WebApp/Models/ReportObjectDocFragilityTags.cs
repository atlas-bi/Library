using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class ReportObjectDocFragilityTags
    {
        public int ReportObjectId { get; set; }
        public int FragilityTagId { get; set; }

        public virtual FragilityTag FragilityTag { get; set; }
        public virtual ReportObjectDoc ReportObject { get; set; }
    }
}
