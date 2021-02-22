using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class ReportObjectType
    {
        public ReportObjectType()
        {
            ReportObject = new HashSet<ReportObject>();
        }

        public int ReportObjectTypeId { get; set; }
        public string Name { get; set; }
        public string DefaultEpicMasterFile { get; set; }
        public DateTime? LastLoadDate { get; set; }

        public virtual ICollection<ReportObject> ReportObject { get; set; }
    }
}
