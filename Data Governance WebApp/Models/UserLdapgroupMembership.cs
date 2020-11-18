using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class UserLdapgroupMembership
    {
        public int MembershipId { get; set; }
        public int? UserId { get; set; }
        public int? GroupId { get; set; }

        public virtual UserLdapgroups Group { get; set; }
        public virtual User User { get; set; }
    }
}
