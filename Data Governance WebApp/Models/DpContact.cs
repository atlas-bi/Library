using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class DpContact
    {
        public DpContact()
        {
            DpContactLinks = new HashSet<DpContactLinks>();
        }

        public int ContactId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Company { get; set; }

        public virtual ICollection<DpContactLinks> DpContactLinks { get; set; }
    }
}
