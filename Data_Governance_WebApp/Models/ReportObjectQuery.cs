using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class ReportObjectQuery
    {
        public int ReportObjectQueryId { get; set; }
        public int? ReportObjectId { get; set; }
        public string Query { get; set; }
        public DateTime? LastLoadDate { get; set; }

        public virtual ReportObject ReportObject { get; set; }
    }
}
