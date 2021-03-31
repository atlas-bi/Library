using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class DpMilestoneLinks
    {
        public int MilestoneLinkId { get; set; }
        public int? DataProjectId { get; set; }
        public int? MilestoneTaskId { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? LastUpdateUser { get; set; }

        public virtual DpDataProject DataProject { get; set; }

        public virtual DpMilestoneTask MilestoneTask { get; set; }
    }
}
