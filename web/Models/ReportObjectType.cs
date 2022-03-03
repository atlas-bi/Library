using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class ReportObjectType
    {
        public ReportObjectType()
        {
            ReportObjects = new HashSet<ReportObject>();
        }

        public int ReportObjectTypeId { get; set; }
        public string Name { get; set; }
        public string DefaultEpicMasterFile { get; set; }
        public DateTime? LastLoadDate { get; set; }
        public string ShortName { get; set; }
        public string Visible { get; set; }

        public virtual ICollection<ReportObject> ReportObjects { get; set; }
    }
}
