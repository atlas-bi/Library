using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class WebAppRolePermissions
    {
        public int WebAppRoleId { get; set; }
        public int WebAppPermissionId { get; set; }

        public virtual WebAppPermissions WebAppPermission { get; set; }
        public virtual WebAppRoles WebAppRole { get; set; }
    }
}
