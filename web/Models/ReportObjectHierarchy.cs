﻿namespace Atlas_Web.Models
{
    public partial class ReportObjectHierarchy
    {
        public int ParentReportObjectId { get; set; }
        public int ChildReportObjectId { get; set; }
        public int? Line { get; set; }
        public DateTime? LastLoadDate { get; set; }

        public virtual ReportObject ChildReportObject { get; set; }
        public virtual ReportObject ParentReportObject { get; set; }
    }
}
