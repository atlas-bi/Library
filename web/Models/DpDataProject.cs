using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class DpDataProject
    {
        public DpDataProject()
        {
            DpAgreements = new HashSet<DpAgreement>();
            DpAttachments = new HashSet<DpAttachment>();
            DpDataProjectConversations = new HashSet<DpDataProjectConversation>();
            DpMilestoneChecklistCompleteds = new HashSet<DpMilestoneChecklistCompleted>();
            DpMilestoneTasks = new HashSet<DpMilestoneTask>();
            DpMilestoneTasksCompleteds = new HashSet<DpMilestoneTasksCompleted>();
            DpReportAnnotations = new HashSet<DpReportAnnotation>();
            DpTermAnnotations = new HashSet<DpTermAnnotation>();
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
        public virtual DpDataInitiative DataInitiative { get; set; }
        public virtual User DataManager { get; set; }
        public virtual User ExecutiveOwner { get; set; }
        public virtual FinancialImpact FinancialImpactNavigation { get; set; }
        public virtual User LastUpdateUserNavigation { get; set; }
        public virtual User OperationOwner { get; set; }
        public virtual StrategicImportance StrategicImportanceNavigation { get; set; }
        public virtual ICollection<DpAgreement> DpAgreements { get; set; }
        public virtual ICollection<DpAttachment> DpAttachments { get; set; }
        public virtual ICollection<DpDataProjectConversation> DpDataProjectConversations { get; set; }
        public virtual ICollection<DpMilestoneChecklistCompleted> DpMilestoneChecklistCompleteds { get; set; }
        public virtual ICollection<DpMilestoneTask> DpMilestoneTasks { get; set; }
        public virtual ICollection<DpMilestoneTasksCompleted> DpMilestoneTasksCompleteds { get; set; }
        public virtual ICollection<DpReportAnnotation> DpReportAnnotations { get; set; }
        public virtual ICollection<DpTermAnnotation> DpTermAnnotations { get; set; }
        public virtual ICollection<StarredCollection> StarredCollections { get; set; }
    }
}
