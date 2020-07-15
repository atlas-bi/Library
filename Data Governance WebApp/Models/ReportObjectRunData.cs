using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class ReportObjectRunData
    {
        public int ReportObjectId { get; set; }
        public int RunId { get; set; }
        public int? RunUserId { get; set; }
        public DateTime? RunStartTime { get; set; }
        public int? RunDurationSeconds { get; set; }
        public string RunStatus { get; set; }

        public virtual ReportObject ReportObject { get; set; }
        public virtual User RunUser { get; set; }
    }
}
