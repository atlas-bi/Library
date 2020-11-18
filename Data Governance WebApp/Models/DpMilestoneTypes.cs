using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class DpMilestoneTypes
    {
        public DpMilestoneTypes()
        {
            DpMilestoneTemplates = new HashSet<DpMilestoneTemplates>();
        }

        public int MilestoneTypeId { get; set; }
        public string Name { get; set; }
        public int? LastUpdateUser { get; set; }
        public DateTime? LastUpdateDate { get; set; }

        public virtual ICollection<DpMilestoneTemplates> DpMilestoneTemplates { get; set; }
    }
}
