using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class DpReportAnnotation
    {
        public int ReportAnnotationId { get; set; }
        public string Annotation { get; set; }
        public int? ReportId { get; set; }
        public int? DataProjectId { get; set; }
        public int? Rank { get; set; }

        public virtual Collection DataProject { get; set; }
        public virtual ReportObject Report { get; set; }
    }
}
