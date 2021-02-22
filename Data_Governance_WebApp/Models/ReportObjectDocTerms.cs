using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class ReportObjectDocTerms
    {
        public int ReportObjectId { get; set; }
        public int TermId { get; set; }

        public virtual ReportObjectDoc ReportObject { get; set; }
        public virtual Term Term { get; set; }
    }
}
