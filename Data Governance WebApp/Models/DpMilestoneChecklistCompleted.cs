using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class DpMilestoneChecklistCompleted
    {
        public int MilestoneChecklistCompletedId { get; set; }
        public int? DataProjectId { get; set; }
        public DateTime? TaskDate { get; set; }
        public int? TaskId { get; set; }
        public int? MilestoneChecklistId { get; set; }
        public bool? ChecklistStatus { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int? CompletionUser { get; set; }

        public virtual User CompletionUserNavigation { get; set; }
        public virtual DpDataProject DataProject { get; set; }
    }
}
