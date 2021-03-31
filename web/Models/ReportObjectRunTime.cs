using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class ReportObjectRunTime
    {
        public int Id { get; set; }
        public int? RunUserId { get; set; }
        public int? Runs { get; set; }
        public decimal? RunTime { get; set; }
        public DateTime? RunWeek { get; set; }
        public string RunWeekString { get; set; }

        public virtual User RunUser { get; set; }
    }
}
