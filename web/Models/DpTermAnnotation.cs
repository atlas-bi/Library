using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class DpTermAnnotation
    {
        public int TermAnnotationId { get; set; }
        public string Annotation { get; set; }
        public int? TermId { get; set; }
        public int? DataProjectId { get; set; }
        public int? Rank { get; set; }

        public virtual DpDataProject DataProject { get; set; }
        public virtual Term Term { get; set; }
    }
}
