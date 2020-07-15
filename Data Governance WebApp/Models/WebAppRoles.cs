using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class WebAppRoles
    {
        public WebAppRoles()
        {
            WebAppRolePermissions = new HashSet<WebAppRolePermissions>();
            WebAppUserRoles = new HashSet<WebAppUserRoles>();
        }

        public int WebAppRoleId { get; set; }
        public string WebAppRoleName { get; set; }
        public string WebAppRoleDescription { get; set; }

        public virtual ICollection<WebAppRolePermissions> WebAppRolePermissions { get; set; }
        public virtual ICollection<WebAppUserRoles> WebAppUserRoles { get; set; }
    }
}
