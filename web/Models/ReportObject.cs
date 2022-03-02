using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class ReportObject
    {
        public ReportObject()
        {
            DpReportAnnotations = new HashSet<DpReportAnnotation>();
            ReportGroupsMemberships = new HashSet<ReportGroupsMembership>();
            ReportManageEngineTickets = new HashSet<ReportManageEngineTicket>();
            ReportObjectAttachments = new HashSet<ReportObjectAttachment>();
            ReportObjectConversationDocs = new HashSet<ReportObjectConversationDoc>();
            ReportObjectHierarchyChildReportObjects = new HashSet<ReportObjectHierarchy>();
            ReportObjectHierarchyParentReportObjects = new HashSet<ReportObjectHierarchy>();
            ReportObjectImagesDocs = new HashSet<ReportObjectImagesDoc>();
            ReportObjectQueries = new HashSet<ReportObjectQuery>();
            ReportObjectReportRunTimes = new HashSet<ReportObjectReportRunTime>();
            ReportObjectRunData = new HashSet<ReportObjectRunDatum>();
            ReportObjectSubscriptions = new HashSet<ReportObjectSubscription>();
            ReportObjectTagMemberships = new HashSet<ReportObjectTagMembership>();
            ReportObjectTopRuns = new HashSet<ReportObjectTopRun>();
            StarredReports = new HashSet<StarredReport>();
        }

        public int ReportObjectId { get; set; }
        public string ReportObjectBizKey { get; set; }
        public string SourceServer { get; set; }
        public string SourceDb { get; set; }
        public string SourceTable { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DetailedDescription { get; set; }
        public int? ReportObjectTypeId { get; set; }
        public int? AuthorUserId { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string ReportObjectUrl { get; set; }
        public string EpicMasterFile { get; set; }
        public decimal? EpicRecordId { get; set; }
        public string ReportServerCatalogId { get; set; }
        public string DefaultVisibilityYn { get; set; }
        public string OrphanedReportObjectYn { get; set; }
        public decimal? EpicReportTemplateId { get; set; }
        public string ReportServerPath { get; set; }
        public string DisplayTitle { get; set; }
        public DateTime? LastLoadDate { get; set; }
        public string RepositoryDescription { get; set; }
        public string EpicReleased { get; set; }
        public string CertificationTag { get; set; }
        public string Availability { get; set; }
        public int? CertificationTagId { get; set; }
        public int? Runs { get; set; }

        public virtual User AuthorUser { get; set; }
        public virtual User LastModifiedByUser { get; set; }
        public virtual ReportObjectType ReportObjectType { get; set; }
        public virtual ReportObjectDoc ReportObjectDoc { get; set; }
        public virtual ICollection<DpReportAnnotation> DpReportAnnotations { get; set; }
        public virtual ICollection<ReportGroupsMembership> ReportGroupsMemberships { get; set; }
        public virtual ICollection<ReportManageEngineTicket> ReportManageEngineTickets { get; set; }
        public virtual ICollection<ReportObjectAttachment> ReportObjectAttachments { get; set; }
        public virtual ICollection<ReportObjectConversationDoc> ReportObjectConversationDocs { get; set; }
        public virtual ICollection<ReportObjectHierarchy> ReportObjectHierarchyChildReportObjects { get; set; }
        public virtual ICollection<ReportObjectHierarchy> ReportObjectHierarchyParentReportObjects { get; set; }
        public virtual ICollection<ReportObjectImagesDoc> ReportObjectImagesDocs { get; set; }
        public virtual ICollection<ReportObjectQuery> ReportObjectQueries { get; set; }
        public virtual ICollection<ReportObjectReportRunTime> ReportObjectReportRunTimes { get; set; }
        public virtual ICollection<ReportObjectRunDatum> ReportObjectRunData { get; set; }
        public virtual ICollection<ReportObjectSubscription> ReportObjectSubscriptions { get; set; }
        public virtual ICollection<ReportObjectTagMembership> ReportObjectTagMemberships { get; set; }
        public virtual ICollection<ReportObjectTopRun> ReportObjectTopRuns { get; set; }
        public virtual ICollection<StarredReport> StarredReports { get; set; }
    }
}
