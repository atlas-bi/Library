using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class DpAgreementUsers
    {
        public int AgreementUsersId { get; set; }
        public int? AgreementId { get; set; }
        public int? UserId { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? LastUpdateUser { get; set; }

        public virtual DpAgreement Agreement { get; set; }
        public virtual User LastUpdateUserNavigation { get; set; }
        public virtual User User { get; set; }
    }
}
