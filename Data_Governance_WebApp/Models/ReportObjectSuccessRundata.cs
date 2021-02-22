using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class ReportObjectSuccessRundata
    {
        public int Id { get; set; }
        public int? ReportObjectId { get; set; }
        public string Name { get; set; }
        public int? RunUserId { get; set; }
        public int? Runs { get; set; }
        public decimal? RunTime { get; set; }
        public string LastRun { get; set; }

        public virtual User RunUser { get; set; }
    }
}
