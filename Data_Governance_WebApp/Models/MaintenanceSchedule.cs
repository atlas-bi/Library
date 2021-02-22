using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class MaintenanceSchedule
    {
        public MaintenanceSchedule()
        {
            ReportObjectDoc = new HashSet<ReportObjectDoc>();
        }

        public int MaintenanceScheduleId { get; set; }
        public string MaintenanceScheduleName { get; set; }

        public virtual ICollection<ReportObjectDoc> ReportObjectDoc { get; set; }
    }
}
