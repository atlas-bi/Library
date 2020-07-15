using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class ReportObjectImagesDoc
    {
        public int ImageId { get; set; }
        public int ReportObjectId { get; set; }
        public int ImageOrdinal { get; set; }
        public byte[] ImageData { get; set; }

        public virtual ReportObject ReportObject { get; set; }
    }
}
