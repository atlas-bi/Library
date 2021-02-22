using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class Term
    {
        public Term()
        {
            DpTermAnnotation = new HashSet<DpTermAnnotation>();
            ReportObjectDocTerms = new HashSet<ReportObjectDocTerms>();
            TermConversation = new HashSet<TermConversation>();
        }

        public int TermId { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string TechnicalDefinition { get; set; }
        public string ApprovedYn { get; set; }
        public DateTime? ApprovalDateTime { get; set; }
        public int? ApprovedByUserId { get; set; }
        public string HasExternalStandardYn { get; set; }
        public string ExternalStandardUrl { get; set; }
        public DateTime? ValidFromDateTime { get; set; }
        public DateTime? ValidToDateTime { get; set; }
        public int? UpdatedByUserId { get; set; }
        public DateTime? LastUpdatedDateTime { get; set; }

        public virtual User ApprovedByUser { get; set; }
        public virtual User UpdatedByUser { get; set; }
        public virtual ICollection<DpTermAnnotation> DpTermAnnotation { get; set; }
        public virtual ICollection<ReportObjectDocTerms> ReportObjectDocTerms { get; set; }
        public virtual ICollection<TermConversation> TermConversation { get; set; }
    }
}
