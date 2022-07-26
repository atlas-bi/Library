namespace Atlas_Web.Models
{
    public partial class UserGroup
    {
        public UserGroup()
        {
            MailRecipients = new HashSet<MailRecipient>();
            ReportGroupsMemberships = new HashSet<ReportGroupsMembership>();
            StarredGroups = new HashSet<StarredGroup>();
            UserGroupsMemberships = new HashSet<UserGroupsMembership>();
            GroupRoleLinks = new HashSet<GroupRoleLink>();
        }

        public int GroupId { get; set; }
        public string AccountName { get; set; }
        public string GroupName { get; set; }
        public string GroupEmail { get; set; }
        public string GroupType { get; set; }
        public string GroupSource { get; set; }
        public DateTime? LastLoadDate { get; set; }
        public string EpicId { get; set; }

        public virtual ICollection<MailRecipient> MailRecipients { get; set; }
        public virtual ICollection<ReportGroupsMembership> ReportGroupsMemberships { get; set; }
        public virtual ICollection<StarredGroup> StarredGroups { get; set; }
        public virtual ICollection<UserGroupsMembership> UserGroupsMemberships { get; set; }
        public virtual ICollection<GroupRoleLink> GroupRoleLinks { get; set; }
    }
}
