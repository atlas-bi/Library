using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class StrategicImportance
    {
        public StrategicImportance()
        {
            DpDataInitiative = new HashSet<DpDataInitiative>();
            DpDataProject = new HashSet<DpDataProject>();
        }

        public int StrategicImportanceId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DpDataInitiative> DpDataInitiative { get; set; }
        public virtual ICollection<DpDataProject> DpDataProject { get; set; }
    }
}
