using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class TermCodeExamplesHistory
    {
        public int TermCodeExamplesHistoryId { get; set; }
        public int TermId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string FormatAs { get; set; }
        public int UpdatedByUserId { get; set; }
        public DateTime ValidFromDateTime { get; set; }
        public DateTime ValidToDateTime { get; set; }

        public virtual Term Term { get; set; }
        public virtual User UpdatedByUser { get; set; }
    }
}
