using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class ReportObjectHierarchy
    {
        public int ParentReportObjectId { get; set; }
        public int ChildReportObjectId { get; set; }
        public int? Line { get; set; }

        public virtual ReportObject ChildReportObject { get; set; }
        public virtual ReportObject ParentReportObject { get; set; }
    }
}
