using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class Collection
    {
        public Collection()
        {
            CollectionReports = new HashSet<CollectionReport>();
            CollectionTerms = new HashSet<CollectionTerm>();
            StarredCollections = new HashSet<StarredCollection>();
        }

        public int DataProjectId { get; set; }
        public int? DataInitiativeId { get; set; }
        public string Name { get; set; }
        public string Purpose { get; set; }
        public string Description { get; set; }
        public int? OperationOwnerId { get; set; }
        public int? ExecutiveOwnerId { get; set; }
        public int? AnalyticsOwnerId { get; set; }
        public int? DataManagerId { get; set; }
        public int? FinancialImpact { get; set; }
        public int? StrategicImportance { get; set; }
        public string ExternalDocumentationUrl { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? LastUpdateUser { get; set; }
        public string Hidden { get; set; }

        public virtual User AnalyticsOwner { get; set; }
        public virtual Initiative Initiative { get; set; }
        public virtual User DataManager { get; set; }
        public virtual User ExecutiveOwner { get; set; }
        public virtual FinancialImpact FinancialImpactNavigation { get; set; }
        public virtual User LastUpdateUserNavigation { get; set; }
        public virtual User OperationOwner { get; set; }
        public virtual StrategicImportance StrategicImportanceNavigation { get; set; }
        public virtual ICollection<CollectionReport> CollectionReports { get; set; }
        public virtual ICollection<CollectionTerm> CollectionTerms { get; set; }
        public virtual ICollection<StarredCollection> StarredCollections { get; set; }
    }
}
