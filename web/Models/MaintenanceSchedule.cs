using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class MaintenanceSchedule
    {
        public MaintenanceSchedule()
        {
            ReportObjectDocs = new HashSet<ReportObjectDoc>();
        }

        public int MaintenanceScheduleId { get; set; }
        public string MaintenanceScheduleName { get; set; }

        public virtual ICollection<ReportObjectDoc> ReportObjectDocs { get; set; }
    }
}
