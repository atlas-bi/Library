using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class DpMilestoneChecklist
    {
        public int MilestoneChecklistId { get; set; }
        public int? MilestoneTaskId { get; set; }
        public string Item { get; set; }

        public virtual DpMilestoneTask MilestoneTask { get; set; }
    }
}
