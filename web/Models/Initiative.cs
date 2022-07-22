using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class Initiative
    {
        public Initiative()
        {
            Collections = new HashSet<Collection>();
            StarredInitiatives = new HashSet<StarredInitiative>();
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
        public string Hidden { get; set; }

        public virtual User ExecutiveOwner { get; set; }
        public virtual FinancialImpact FinancialImpactNavigation { get; set; }
        public virtual User LastUpdateUserNavigation { get; set; }
        public virtual User OperationOwner { get; set; }
        public virtual StrategicImportance StrategicImportanceNavigation { get; set; }
        public virtual ICollection<Collection> Collections { get; set; }
        public virtual ICollection<StarredInitiative> StarredInitiatives { get; set; }
    }
}
