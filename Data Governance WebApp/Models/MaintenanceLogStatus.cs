using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class MaintenanceLogStatus
    {
        public MaintenanceLogStatus()
        {
            MaintenanceLog = new HashSet<MaintenanceLog>();
        }

        public int MaintenanceLogStatusId { get; set; }
        public string MaintenanceLogStatusName { get; set; }

        public virtual ICollection<MaintenanceLog> MaintenanceLog { get; set; }
    }
}
