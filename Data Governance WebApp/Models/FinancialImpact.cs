using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class FinancialImpact
    {
        public FinancialImpact()
        {
            DpDataInitiative = new HashSet<DpDataInitiative>();
            DpDataProject = new HashSet<DpDataProject>();
        }

        public int FinancialImpactId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DpDataInitiative> DpDataInitiative { get; set; }
        public virtual ICollection<DpDataProject> DpDataProject { get; set; }
    }
}
