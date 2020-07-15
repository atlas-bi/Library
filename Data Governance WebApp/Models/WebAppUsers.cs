using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class WebAppUsers
    {
        public WebAppUsers()
        {
            DpAgreementUsers = new HashSet<DpAgreementUsers>();
            DpMilestoneFrequency = new HashSet<DpMilestoneFrequency>();
            TermCodeExamplesHistory = new HashSet<TermCodeExamplesHistory>();
            WebAppUserRoles = new HashSet<WebAppUserRoles>();
        }

        public int WebAppUserId { get; set; }
        public string WebAppUserName { get; set; }
        public string Email { get; set; }
        public int? MappedUserId { get; set; }
        public DateTime? LastLoginDateTime { get; set; }

        public virtual User MappedUser { get; set; }
        public virtual ICollection<DpAgreementUsers> DpAgreementUsers { get; set; }
        public virtual ICollection<DpMilestoneFrequency> DpMilestoneFrequency { get; set; }
        public virtual ICollection<TermCodeExamplesHistory> TermCodeExamplesHistory { get; set; }
        public virtual ICollection<WebAppUserRoles> WebAppUserRoles { get; set; }
    }
}
