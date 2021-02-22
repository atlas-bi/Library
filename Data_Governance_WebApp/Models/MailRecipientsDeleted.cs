using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class MailRecipientsDeleted
    {
        public int Id { get; set; }
        public int? MessageId { get; set; }
        public int? ToUserId { get; set; }
        public DateTime? ReadDate { get; set; }
        public int? AlertDisplayed { get; set; }
        public int? ToGroupId { get; set; }

        public virtual User ToUser { get; set; }
    }
}
