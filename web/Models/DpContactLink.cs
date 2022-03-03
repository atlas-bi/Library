using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class DpContactLink
    {
        public int LinkId { get; set; }
        public int? InitiativeId { get; set; }
        public int? ContactId { get; set; }

        public virtual DpContact Contact { get; set; }
        public virtual DpDataInitiative Initiative { get; set; }
    }
}
