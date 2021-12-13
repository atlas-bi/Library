using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class ReportObjectQuery
    {
        public int ReportObjectQueryId { get; set; }
        public int? ReportObjectId { get; set; }
        public string Query { get; set; }
        public DateTime? LastLoadDate { get; set; }
        public string SourceServer { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }

        public virtual ReportObject ReportObject { get; set; }
    }
}
