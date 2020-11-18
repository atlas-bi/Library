using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class MaintenanceLog
    {
        public MaintenanceLog()
        {
            ReportObjectDocMaintenanceLogs = new HashSet<ReportObjectDocMaintenanceLogs>();
        }

        public int MaintenanceLogId { get; set; }
        public int? MaintainerId { get; set; }
        public DateTime? MaintenanceDate { get; set; }
        public string Comment { get; set; }
        public int? MaintenanceLogStatusId { get; set; }

        public virtual User Maintainer { get; set; }
        public virtual MaintenanceLogStatus MaintenanceLogStatus { get; set; }
        public virtual ICollection<ReportObjectDocMaintenanceLogs> ReportObjectDocMaintenanceLogs { get; set; }
    }
}
