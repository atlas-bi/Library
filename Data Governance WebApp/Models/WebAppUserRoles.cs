using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class WebAppUserRoles
    {
        public int WebAppUserId { get; set; }
        public int WebAppRoleId { get; set; }

        public virtual WebAppRoles WebAppRole { get; set; }
        public virtual WebAppUsers WebAppUser { get; set; }
    }
}
