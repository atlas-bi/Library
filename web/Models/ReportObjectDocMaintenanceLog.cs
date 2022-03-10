using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class ReportObjectDocMaintenanceLog
    {
        public int LinkId { get; set; }
        public int ReportObjectId { get; set; }
        public int MaintenanceLogId { get; set; }

        public virtual MaintenanceLog MaintenanceLog { get; set; }
        public virtual ReportObjectDoc ReportObject { get; set; }
    }
}
