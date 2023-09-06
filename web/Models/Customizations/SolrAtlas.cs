using SolrNet.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    public class SolrAtlas
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("name")]
        public string Name { get; set; }

        [SolrField("type")]
        public string Type { get; set; }

        [SolrField("description")]
        public ICollection<string> Description { get; set; }

        [SolrField("report_type")]
        public string ReportType { get; set; }

        [SolrField("report_type_id")]
        public int ReportTypeId { get; set; }

        [SolrField("email")]
        public string Email { get; set; }

        [SolrField("documented")]
        public string Documented { get; set; }

        [SolrField("atlas_id")]
        public int AtlasId { get; set; }

        [SolrField("certification")]
        public ICollection<string> Certification { get; set; }

        [SolrField("epic_master_file")]
        public string EpicMasterFile { get; set; }

        [SolrField("epic_record_id")]
        public string EpicRecordId { get; set; }

        [SolrField("run_url")]
        public string ReportObjectUrl { get; set; }

        [SolrField("source_server")]
        public string SourceServer { get; set; }

        [SolrField("server_path")]
        public string ReportServerPath { get; set; }

        [SolrField("epic_template")]
        public string EpicTemplateId { get; set; }

        [SolrField("executive_visibility")]
        public string ExecutiveVisiblity { get; set; }

        [SolrField("enabled_for_hyperspace")]
        public string EnabledForHyperspace { get; set; }

        [SolrField("group_type")]
        public string GroupType { get; set; }
    }
}
