using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class WebAppPermissions
    {
        public WebAppPermissions()
        {
            WebAppRolePermissions = new HashSet<WebAppRolePermissions>();
        }

        public int WebAppPermissionId { get; set; }
        public string WebAppPermissionName { get; set; }
        public string WebAppPermissionDescription { get; set; }

        public virtual ICollection<WebAppRolePermissions> WebAppRolePermissions { get; set; }
    }
}
