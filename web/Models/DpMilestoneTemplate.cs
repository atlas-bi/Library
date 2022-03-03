using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class DpMilestoneTemplate
    {
        public DpMilestoneTemplate()
        {
            DpMilestoneTasks = new HashSet<DpMilestoneTask>();
        }

        public int MilestoneTemplateId { get; set; }
        public string Name { get; set; }
        public int? MilestoneTypeId { get; set; }
        public int? LastUpdateUser { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? Interval { get; set; }

        public virtual User LastUpdateUserNavigation { get; set; }
        public virtual DpMilestoneFrequency MilestoneType { get; set; }
        public virtual ICollection<DpMilestoneTask> DpMilestoneTasks { get; set; }
    }
}
