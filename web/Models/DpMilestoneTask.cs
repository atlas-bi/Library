using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class DpMilestoneTask
    {
        public DpMilestoneTask()
        {
            DpMilestoneChecklists = new HashSet<DpMilestoneChecklist>();
        }

        public int MilestoneTaskId { get; set; }
        public int? MilestoneTemplateId { get; set; }
        public int? OwnerId { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? LastUpdateUser { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? DataProjectId { get; set; }

        public virtual DpDataProject DataProject { get; set; }
        public virtual User LastUpdateUserNavigation { get; set; }
        public virtual DpMilestoneTemplate MilestoneTemplate { get; set; }
        public virtual User Owner { get; set; }
        public virtual ICollection<DpMilestoneChecklist> DpMilestoneChecklists { get; set; }
    }
}
