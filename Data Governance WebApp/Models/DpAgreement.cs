using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class DpAgreement
    {
        public DpAgreement()
        {
            DpAgreementUsers = new HashSet<DpAgreementUsers>();
        }

        public int AgreementId { get; set; }
        public string Description { get; set; }
        public DateTime? MeetingDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? LastUpdateUser { get; set; }
        public int? DataProjectId { get; set; }
        public int? Rank { get; set; }

        public virtual DpDataProject DataProject { get; set; }
        public virtual User LastUpdateUserNavigation { get; set; }
        public virtual ICollection<DpAgreementUsers> DpAgreementUsers { get; set; }
    }
}
