using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class FinancialImpact
    {
        public FinancialImpact()
        {
            DpDataInitiatives = new HashSet<DpDataInitiative>();
            DpDataProjects = new HashSet<DpDataProject>();
        }

        public int FinancialImpactId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DpDataInitiative> DpDataInitiatives { get; set; }
        public virtual ICollection<DpDataProject> DpDataProjects { get; set; }
    }
}
