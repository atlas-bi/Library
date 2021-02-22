using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class ReportGroupsMemberships
    {
        public int MembershipId { get; set; }
        public int GroupId { get; set; }
        public int ReportId { get; set; }
        public DateTime? LastLoadDate { get; set; }

        public virtual UserGroups Group { get; set; }
        public virtual ReportObject Report { get; set; }
    }
}
