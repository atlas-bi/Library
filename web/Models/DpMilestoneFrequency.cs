using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class DpMilestoneFrequency
    {
        public DpMilestoneFrequency()
        {
            DpMilestoneTemplates = new HashSet<DpMilestoneTemplate>();
        }

        public int MilestoneTypeId { get; set; }
        public string Name { get; set; }
        public int? LastUpdateUser { get; set; }
        public DateTime? LastUpdateDate { get; set; }

        public virtual User LastUpdateUserNavigation { get; set; }
        public virtual ICollection<DpMilestoneTemplate> DpMilestoneTemplates { get; set; }
    }
}
