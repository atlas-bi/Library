using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class FragilityTag
    {
        public FragilityTag()
        {
            ReportObjectDocFragilityTags = new HashSet<ReportObjectDocFragilityTag>();
        }

        public int FragilityTagId { get; set; }
        public string FragilityTagName { get; set; }

        public virtual ICollection<ReportObjectDocFragilityTag> ReportObjectDocFragilityTags { get; set; }
    }
}
