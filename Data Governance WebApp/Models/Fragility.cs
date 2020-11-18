using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class Fragility
    {
        public Fragility()
        {
            ReportObjectDoc = new HashSet<ReportObjectDoc>();
        }

        public int FragilityId { get; set; }
        public string FragilityName { get; set; }

        public virtual ICollection<ReportObjectDoc> ReportObjectDoc { get; set; }
    }
}
