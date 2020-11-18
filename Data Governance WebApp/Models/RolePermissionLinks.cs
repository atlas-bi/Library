using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class RolePermissionLinks
    {
        public int RolePermissionLinksId { get; set; }
        public int? RoleId { get; set; }
        public int? RolePermissionsId { get; set; }

        public virtual UserRoles Role { get; set; }
        public virtual RolePermissions RolePermissions { get; set; }
    }
}
