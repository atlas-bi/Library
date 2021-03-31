using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class ReportGroupsMembership
    {
        public int MembershipId { get; set; }
        public int GroupId { get; set; }
        public int ReportId { get; set; }
        public DateTime? LastLoadDate { get; set; }

        public virtual UserGroup Group { get; set; }
        public virtual ReportObject Report { get; set; }
    }
}
