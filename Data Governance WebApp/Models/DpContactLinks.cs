using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class DpContactLinks
    {
        public int LinkId { get; set; }
        public int? InitiativeId { get; set; }
        public int? ContactId { get; set; }

        public virtual DpContact Contact { get; set; }
        public virtual DpDataInitiative Initiative { get; set; }
    }
}
