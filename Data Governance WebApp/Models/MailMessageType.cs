using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class MailMessageType
    {
        public MailMessageType()
        {
            MailMessages = new HashSet<MailMessages>();
        }

        public int MessageTypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<MailMessages> MailMessages { get; set; }
    }
}
