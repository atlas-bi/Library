using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class ReportObjectReportRunTime
    {
        public int Id { get; set; }
        public int? ReportObjectId { get; set; }
        public int? Duration { get; set; }
        public int? Runs { get; set; }
        public DateTime? RunWeek { get; set; }
        public string RunWeekString { get; set; }

        public virtual ReportObject ReportObject { get; set; }
    }
}
