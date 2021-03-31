using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class ReportObjectDocFragilityTag
    {
        public int ReportObjectId { get; set; }
        public int FragilityTagId { get; set; }

        public virtual FragilityTag FragilityTag { get; set; }
        public virtual ReportObjectDoc ReportObject { get; set; }
    }
}
