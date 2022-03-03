using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class ReportObjectAttachment
    {
        public int ReportObjectAttachmentId { get; set; }
        public int ReportObjectId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public DateTime? LastLoadDate { get; set; }

        public virtual ReportObject ReportObject { get; set; }
    }
}
