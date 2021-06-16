using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class DpDataInitiative
    {
        public DpDataInitiative()
        {
            DpContactLinks = new HashSet<DpContactLink>();
            DpDataProjects = new HashSet<DpDataProject>();
        }

        public int DataInitiativeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? OperationOwnerId { get; set; }
        public int? ExecutiveOwnerId { get; set; }
        public int? FinancialImpact { get; set; }
        public int? StrategicImportance { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? LastUpdateUser { get; set; }

        public virtual User ExecutiveOwner { get; set; }
        public virtual FinancialImpact FinancialImpactNavigation { get; set; }
        public virtual User LastUpdateUserNavigation { get; set; }
        public virtual User OperationOwner { get; set; }
        public virtual StrategicImportance StrategicImportanceNavigation { get; set; }
        public virtual ICollection<DpContactLink> DpContactLinks { get; set; }
        public virtual ICollection<DpDataProject> DpDataProjects { get; set; }
    }
}
