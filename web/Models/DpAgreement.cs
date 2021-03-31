using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class DpAgreement
    {
        public DpAgreement()
        {
            DpAgreementUsers = new HashSet<DpAgreementUser>();
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
        public virtual ICollection<DpAgreementUser> DpAgreementUsers { get; set; }
    }
}
