using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class TermCodeExamples
    {
        public int TermCodeExampleId { get; set; }
        public int TermId { get; set; }

        public virtual Term Term { get; set; }
    }
}
