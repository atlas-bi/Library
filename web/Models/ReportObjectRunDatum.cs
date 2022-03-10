using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class ReportObjectRunDatum
    {
        public int ReportObjectId { get; set; }
        public long RunId { get; set; }
        public int? RunUserId { get; set; }
        public DateTime? RunStartTime { get; set; }
        public int? RunDurationSeconds { get; set; }
        public string RunStatus { get; set; }
        public DateTime? LastLoadDate { get; set; }

        public virtual ReportObject ReportObject { get; set; }
        public virtual User RunUser { get; set; }
    }
}
