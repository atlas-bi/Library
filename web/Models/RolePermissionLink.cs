using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class RolePermissionLink
    {
        public int RolePermissionLinksId { get; set; }
        public int? RoleId { get; set; }
        public int? RolePermissionsId { get; set; }

        public virtual UserRole Role { get; set; }
        public virtual RolePermission RolePermissions { get; set; }
    }
}
