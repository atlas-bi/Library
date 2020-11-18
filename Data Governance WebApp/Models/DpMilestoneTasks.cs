using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class DpMilestoneTasks
    {
        public DpMilestoneTasks()
        {
            DpMilestoneChecklist = new HashSet<DpMilestoneChecklist>();
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
        public virtual DpMilestoneTemplates MilestoneTemplate { get; set; }
        public virtual User Owner { get; set; }
        public virtual ICollection<DpMilestoneChecklist> DpMilestoneChecklist { get; set; }
    }
}
