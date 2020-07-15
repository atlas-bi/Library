using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class DpMilestoneLinks
    {
        public int MilestoneLinkId { get; set; }
        public int? DataProjectId { get; set; }
        public int? MilestoneTaskId { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? LastUpdateUser { get; set; }

        public virtual DpDataProject DataProject { get; set; }
        public virtual WebAppUsers LastUpdateUserNavigation { get; set; }
        public virtual DpMilestoneTasks MilestoneTask { get; set; }
    }
}
