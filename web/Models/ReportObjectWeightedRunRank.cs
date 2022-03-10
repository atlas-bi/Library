using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class ReportObjectWeightedRunRank
    {
        public int Reportobjectid { get; set; }
        public decimal? WeightedRunRank { get; set; }

        public virtual ReportObject Reportobject { get; set; }
    }
}
