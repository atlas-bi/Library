using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class ReportCertificationTag
    {
        public int CertId { get; set; }
        public string CertName { get; set; }
        public string Description { get; set; }
        public int? Priority { get; set; }
    }
}
