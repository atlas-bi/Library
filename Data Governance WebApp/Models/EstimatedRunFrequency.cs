using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class EstimatedRunFrequency
    {
        public EstimatedRunFrequency()
        {
            ReportObjectDoc = new HashSet<ReportObjectDoc>();
        }

        public int EstimatedRunFrequencyId { get; set; }
        public string EstimatedRunFrequencyName { get; set; }

        public virtual ICollection<ReportObjectDoc> ReportObjectDoc { get; set; }
    }
}
