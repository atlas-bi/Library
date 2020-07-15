using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class ReportObject
    {
        public ReportObject()
        {
            DpReportAnnotation = new HashSet<DpReportAnnotation>();
            ReportManageEngineTickets = new HashSet<ReportManageEngineTickets>();
            ReportObjectConversationDoc = new HashSet<ReportObjectConversationDoc>();
            ReportObjectHierarchyChildReportObject = new HashSet<ReportObjectHierarchy>();
            ReportObjectHierarchyParentReportObject = new HashSet<ReportObjectHierarchy>();
            ReportObjectImagesDoc = new HashSet<ReportObjectImagesDoc>();
            ReportObjectQuery = new HashSet<ReportObjectQuery>();
            ReportObjectRunData = new HashSet<ReportObjectRunData>();
            ReportObjectSubscriptions = new HashSet<ReportObjectSubscriptions>();
            ReportObjectTopRuns = new HashSet<ReportObjectTopRuns>();
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
        public decimal? NullcolumnNumeric { get; set; }

        public virtual User AuthorUser { get; set; }
        public virtual User LastModifiedByUser { get; set; }
        public virtual ReportObjectType ReportObjectType { get; set; }
        public virtual ReportObjectDoc ReportObjectDoc { get; set; }
        public virtual ICollection<DpReportAnnotation> DpReportAnnotation { get; set; }
        public virtual ICollection<ReportManageEngineTickets> ReportManageEngineTickets { get; set; }
        public virtual ICollection<ReportObjectConversationDoc> ReportObjectConversationDoc { get; set; }
        public virtual ICollection<ReportObjectHierarchy> ReportObjectHierarchyChildReportObject { get; set; }
        public virtual ICollection<ReportObjectHierarchy> ReportObjectHierarchyParentReportObject { get; set; }
        public virtual ICollection<ReportObjectImagesDoc> ReportObjectImagesDoc { get; set; }
        public virtual ICollection<ReportObjectQuery> ReportObjectQuery { get; set; }
        public virtual ICollection<ReportObjectRunData> ReportObjectRunData { get; set; }
        public virtual ICollection<ReportObjectSubscriptions> ReportObjectSubscriptions { get; set; }
        public virtual ICollection<ReportObjectTopRuns> ReportObjectTopRuns { get; set; }
    }
}
