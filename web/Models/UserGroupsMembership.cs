using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class UserGroupsMembership
    {
        public int MembershipId { get; set; }
        public int? UserId { get; set; }
        public int? GroupId { get; set; }
        public DateTime? LastLoadDate { get; set; }

        public virtual UserGroup Group { get; set; }
        public virtual User User { get; set; }
    }
}
