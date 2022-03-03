using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class DpAttachment
    {
        public int AttachmentId { get; set; }
        public int DataProjectId { get; set; }
        public int Rank { get; set; }
        public byte[] AttachmentData { get; set; }
        public string AttachmentType { get; set; }
        public string AttachmentName { get; set; }
        public int? AttachmentSize { get; set; }

        public virtual DpDataProject DataProject { get; set; }
    }
}
