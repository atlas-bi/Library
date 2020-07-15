using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class UserLdapgroups
    {
        public UserLdapgroups()
        {
            MailRecipients = new HashSet<MailRecipients>();
            UserLdapgroupMembership = new HashSet<UserLdapgroupMembership>();
        }

        public int GroupId { get; set; }
        public string AccountName { get; set; }
        public string GroupName { get; set; }
        public string GroupEmail { get; set; }
        public string GroupType { get; set; }

        public virtual ICollection<MailRecipients> MailRecipients { get; set; }
        public virtual ICollection<UserLdapgroupMembership> UserLdapgroupMembership { get; set; }
    }
}
