using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class UserRoleLinks
    {
        public int UserRoleLinksId { get; set; }
        public int? UserId { get; set; }
        public int? UserRolesId { get; set; }

        public virtual User User { get; set; }
        public virtual UserRoles UserRoles { get; set; }
    }
}
