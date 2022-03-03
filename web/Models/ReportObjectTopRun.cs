using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class ReportObjectTopRun
    {
        public int Id { get; set; }
        public int? ReportObjectId { get; set; }
        public string Name { get; set; }
        public int? RunUserId { get; set; }
        public int? Runs { get; set; }
        public decimal? RunTime { get; set; }
        public string LastRun { get; set; }
        public int? ReportObjectTypeId { get; set; }

        public virtual ReportObject ReportObject { get; set; }
        public virtual User RunUser { get; set; }
    }
}
