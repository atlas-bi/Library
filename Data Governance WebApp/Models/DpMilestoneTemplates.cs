using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class DpMilestoneTemplates
    {
        public DpMilestoneTemplates()
        {
            DpMilestoneTasks = new HashSet<DpMilestoneTasks>();
        }

        public int MilestoneTemplateId { get; set; }
        public string Name { get; set; }
        public int? MilestoneTypeId { get; set; }
        public int? LastUpdateUser { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? Interval { get; set; }

        public virtual User LastUpdateUserNavigation { get; set; }
        public virtual DpMilestoneFrequency MilestoneType { get; set; }
        public virtual ICollection<DpMilestoneTasks> DpMilestoneTasks { get; set; }
    }
}
