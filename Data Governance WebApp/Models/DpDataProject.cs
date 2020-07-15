using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class DpDataProject
    {
        public DpDataProject()
        {
            DpAgreement = new HashSet<DpAgreement>();
            DpAttachments = new HashSet<DpAttachments>();
            DpDataProjectConversation = new HashSet<DpDataProjectConversation>();
            DpMilestoneChecklistCompleted = new HashSet<DpMilestoneChecklistCompleted>();
            DpMilestoneTasks = new HashSet<DpMilestoneTasks>();
            DpMilestoneTasksCompleted = new HashSet<DpMilestoneTasksCompleted>();
            DpReportAnnotation = new HashSet<DpReportAnnotation>();
            DpTermAnnotation = new HashSet<DpTermAnnotation>();
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

        public virtual User AnalyticsOwner { get; set; }
        public virtual User DataManager { get; set; }
        public virtual User ExecutiveOwner { get; set; }
        public virtual FinancialImpact FinancialImpactNavigation { get; set; }
        public virtual User LastUpdateUserNavigation { get; set; }
        public virtual User OperationOwner { get; set; }
        public virtual StrategicImportance StrategicImportanceNavigation { get; set; }
        public virtual ICollection<DpAgreement> DpAgreement { get; set; }
        public virtual ICollection<DpAttachments> DpAttachments { get; set; }
        public virtual ICollection<DpDataProjectConversation> DpDataProjectConversation { get; set; }
        public virtual ICollection<DpMilestoneChecklistCompleted> DpMilestoneChecklistCompleted { get; set; }
        public virtual ICollection<DpMilestoneTasks> DpMilestoneTasks { get; set; }
        public virtual ICollection<DpMilestoneTasksCompleted> DpMilestoneTasksCompleted { get; set; }
        public virtual ICollection<DpReportAnnotation> DpReportAnnotation { get; set; }
        public virtual ICollection<DpTermAnnotation> DpTermAnnotation { get; set; }
    }
}
