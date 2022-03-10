using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class DpContact
    {
        public DpContact()
        {
            DpContactLinks = new HashSet<DpContactLink>();
        }

        public int ContactId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Company { get; set; }

        public virtual ICollection<DpContactLink> DpContactLinks { get; set; }
    }
}
