using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class UserPreferences
    {
        public int UserPreferenceId { get; set; }
        public string ItemType { get; set; }
        public int? ItemValue { get; set; }
        public int? ItemId { get; set; }
        public int? UserId { get; set; }

        public virtual User User { get; set; }
    }
}
