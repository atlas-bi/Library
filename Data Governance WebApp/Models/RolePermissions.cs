using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class RolePermissions
    {
        public RolePermissions()
        {
            RolePermissionLinks = new HashSet<RolePermissionLinks>();
        }

        public int RolePermissionsId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RolePermissionLinks> RolePermissionLinks { get; set; }
    }
}
