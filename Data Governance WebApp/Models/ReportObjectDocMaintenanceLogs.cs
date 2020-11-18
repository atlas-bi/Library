using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class ReportObjectDocMaintenanceLogs
    {
        public int ReportObjectId { get; set; }
        public int MaintenanceLogId { get; set; }

        public virtual MaintenanceLog MaintenanceLog { get; set; }
        public virtual ReportObjectDoc ReportObject { get; set; }
    }
}
