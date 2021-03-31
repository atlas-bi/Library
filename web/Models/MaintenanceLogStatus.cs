using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class MaintenanceLogStatus
    {
        public MaintenanceLogStatus()
        {
            MaintenanceLogs = new HashSet<MaintenanceLog>();
        }

        public int MaintenanceLogStatusId { get; set; }
        public string MaintenanceLogStatusName { get; set; }

        public virtual ICollection<MaintenanceLog> MaintenanceLogs { get; set; }
    }
}
