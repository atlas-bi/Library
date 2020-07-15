using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class ReportObjectDoc
    {
        public ReportObjectDoc()
        {
            ReportObjectDocFragilityTags = new HashSet<ReportObjectDocFragilityTags>();
            ReportObjectDocMaintenanceLogs = new HashSet<ReportObjectDocMaintenanceLogs>();
            ReportObjectDocTerms = new HashSet<ReportObjectDocTerms>();
        }

        public int ReportObjectId { get; set; }
        public int? OperationalOwnerUserId { get; set; }
        public int? Requester { get; set; }
        public string GitLabProjectUrl { get; set; }
        public string GitLabTreeUrl { get; set; }
        public string GitLabBlobUrl { get; set; }
        public string DeveloperDescription { get; set; }
        public string KeyAssumptions { get; set; }
        public int? OrganizationalValueId { get; set; }
        public int? EstimatedRunFrequencyId { get; set; }
        public int? FragilityId { get; set; }
        public string ExecutiveVisibilityYn { get; set; }
        public int? MaintenanceScheduleId { get; set; }
        public DateTime? LastUpdateDateTime { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public string EnabledForHyperspace { get; set; }
        public string DoNotPurge { get; set; }
        public string Hidden { get; set; }

        public virtual EstimatedRunFrequency EstimatedRunFrequency { get; set; }
        public virtual Fragility Fragility { get; set; }
        public virtual MaintenanceSchedule MaintenanceSchedule { get; set; }
        public virtual User OperationalOwnerUser { get; set; }
        public virtual OrganizationalValue OrganizationalValue { get; set; }
        public virtual ReportObject ReportObject { get; set; }
        public virtual User RequesterNavigation { get; set; }
        public virtual User UpdatedByNavigation { get; set; }
        public virtual ICollection<ReportObjectDocFragilityTags> ReportObjectDocFragilityTags { get; set; }
        public virtual ICollection<ReportObjectDocMaintenanceLogs> ReportObjectDocMaintenanceLogs { get; set; }
        public virtual ICollection<ReportObjectDocTerms> ReportObjectDocTerms { get; set; }
    }
}
