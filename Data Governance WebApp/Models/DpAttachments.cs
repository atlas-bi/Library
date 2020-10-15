using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class DpAttachments
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
