using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class UserGroups
    {
        public UserGroups()
        {
            MailRecipients = new HashSet<MailRecipients>();
            ReportGroupsMemberships = new HashSet<ReportGroupsMemberships>();
            UserGroupsMembership = new HashSet<UserGroupsMembership>();
        }

        public int GroupId { get; set; }
        public string AccountName { get; set; }
        public string GroupName { get; set; }
        public string GroupEmail { get; set; }
        public string GroupType { get; set; }
        public string GroupSource { get; set; }
        public DateTime? LastLoadDate { get; set; }
        public string EpicId { get; set; }

        public virtual ICollection<MailRecipients> MailRecipients { get; set; }
        public virtual ICollection<ReportGroupsMemberships> ReportGroupsMemberships { get; set; }
        public virtual ICollection<UserGroupsMembership> UserGroupsMembership { get; set; }
    }
}
