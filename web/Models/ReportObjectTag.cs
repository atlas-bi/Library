using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class ReportObjectTag
    {
        public ReportObjectTag()
        {
            ReportObjectTagMemberships = new HashSet<ReportObjectTagMembership>();
        }

        public int TagId { get; set; }
        public decimal? EpicTagId { get; set; }
        public string TagName { get; set; }

        public virtual ICollection<ReportObjectTagMembership> ReportObjectTagMemberships { get; set; }
    }
}
