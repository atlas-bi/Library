using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class SearchReportObjectSearchDatum
    {
        public int Pk { get; set; }
        public int Id { get; set; }
        public string ColumnName { get; set; }
        public string Value { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string EpicMasterFile { get; set; }
        public string DefaultVisibilityYn { get; set; }
        public string OrphanedReportObjectYn { get; set; }
        public int? ReportObjectTypeId { get; set; }
        public int? AuthorUserId { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public decimal? EpicReportTemplateId { get; set; }
        public string SourceServer { get; set; }
        public string SourceDb { get; set; }
        public string SourceTable { get; set; }
        public int Documented { get; set; }
        public int? DocOwnerId { get; set; }
        public int? DocRequesterId { get; set; }
        public int? DocOrgValueId { get; set; }
        public int? DocRunFreqId { get; set; }
        public int? DocFragId { get; set; }
        public string DocExecVis { get; set; }
        public int? DocMainSchedId { get; set; }
        public DateTime? DocLastUpdated { get; set; }
        public DateTime? DocCreated { get; set; }
        public int? DocCreatedBy { get; set; }
        public int? DocUpdatedBy { get; set; }
        public string DocHypeEnabled { get; set; }
        public string DocDoNotPurge { get; set; }
        public string DocHidden { get; set; }
        public string CertificationTag { get; set; }
        public int? TwoYearRuns { get; set; }
        public int? OneYearRuns { get; set; }
        public int? SixMonthsRuns { get; set; }
        public int? OneMonthRuns { get; set; }
    }
}
