using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class ReportObjectDocFragilityTag
    {
        public int LinkId { get; set; }
        public int ReportObjectId { get; set; }
        public int FragilityTagId { get; set; }

        public virtual FragilityTag FragilityTag { get; set; }
        public virtual ReportObjectDoc ReportObject { get; set; }
    }
}
