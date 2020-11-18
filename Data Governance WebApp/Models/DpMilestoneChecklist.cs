using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class DpMilestoneChecklist
    {
        public int MilestoneChecklistId { get; set; }
        public int? MilestoneTaskId { get; set; }
        public string Item { get; set; }

        public virtual DpMilestoneTasks MilestoneTask { get; set; }
    }
}
