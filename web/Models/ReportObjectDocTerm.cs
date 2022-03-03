using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class ReportObjectDocTerm
    {
        public int LinkId { get; set; }
        public int ReportObjectId { get; set; }
        public int TermId { get; set; }

        public virtual ReportObjectDoc ReportObject { get; set; }
        public virtual Term Term { get; set; }
    }
}
