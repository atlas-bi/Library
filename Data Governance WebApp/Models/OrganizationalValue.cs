using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class OrganizationalValue
    {
        public OrganizationalValue()
        {
            ReportObjectDoc = new HashSet<ReportObjectDoc>();
        }

        public int OrganizationalValueId { get; set; }
        public string OrganizationalValueName { get; set; }

        public virtual ICollection<ReportObjectDoc> ReportObjectDoc { get; set; }
    }
}
