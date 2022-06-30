using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class Tag
    {
        public Tag()
        {
            ReportTagLinks = new HashSet<ReportTagLink>();
        }

        public int TagId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Priority { get; set; }
        public string ShowInHeader { get; set; }
        public virtual ICollection<ReportTagLink> ReportTagLinks { get; set; }
    }
}
