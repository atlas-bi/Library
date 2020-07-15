using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class UserRoles
    {
        public UserRoles()
        {
            RolePermissionLinks = new HashSet<RolePermissionLinks>();
            UserRoleLinks = new HashSet<UserRoleLinks>();
        }

        public int UserRolesId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RolePermissionLinks> RolePermissionLinks { get; set; }
        public virtual ICollection<UserRoleLinks> UserRoleLinks { get; set; }
    }
}
