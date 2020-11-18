using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class FragilityTag
    {
        public FragilityTag()
        {
            ReportObjectDocFragilityTags = new HashSet<ReportObjectDocFragilityTags>();
        }

        public int FragilityTagId { get; set; }
        public string FragilityTagName { get; set; }

        public virtual ICollection<ReportObjectDocFragilityTags> ReportObjectDocFragilityTags { get; set; }
    }
}
