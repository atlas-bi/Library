using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class StrategicImportance
    {
        public StrategicImportance()
        {
            DpDataInitiatives = new HashSet<DpDataInitiative>();
            DpDataProjects = new HashSet<DpDataProject>();
        }

        public int StrategicImportanceId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DpDataInitiative> DpDataInitiatives { get; set; }
        public virtual ICollection<DpDataProject> DpDataProjects { get; set; }
    }
}
