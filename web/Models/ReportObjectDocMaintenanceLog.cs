using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class ReportObjectDocMaintenanceLog
    {
        public int ReportObjectId { get; set; }
        public int MaintenanceLogId { get; set; }

        public virtual MaintenanceLog MaintenanceLog { get; set; }
        public virtual ReportObjectDoc ReportObject { get; set; }
    }
}
