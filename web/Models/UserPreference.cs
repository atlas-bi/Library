using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class UserPreference
    {
        public int UserPreferenceId { get; set; }
        public string ItemType { get; set; }
        public int? ItemValue { get; set; }
        public int? ItemId { get; set; }
        public int? UserId { get; set; }

        public virtual User User { get; set; }
    }
}
