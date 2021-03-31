using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class MaintenanceLog
    {
        public MaintenanceLog()
        {
            ReportObjectDocMaintenanceLogs = new HashSet<ReportObjectDocMaintenanceLog>();
        }

        public int MaintenanceLogId { get; set; }
        public int? MaintainerId { get; set; }
        public DateTime? MaintenanceDate { get; set; }
        public string Comment { get; set; }
        public int? MaintenanceLogStatusId { get; set; }

        public virtual User Maintainer { get; set; }
        public virtual MaintenanceLogStatus MaintenanceLogStatus { get; set; }
        public virtual ICollection<ReportObjectDocMaintenanceLog> ReportObjectDocMaintenanceLogs { get; set; }
    }
}
