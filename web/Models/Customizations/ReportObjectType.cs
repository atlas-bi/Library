using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Models
{
    public class ReportObjectType__Metadata
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportObjectTypeId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Name { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string DefaultEpicMasterFile { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public virtual ICollection<ReportObject> ReportObject { get; set; }
    }

    [ModelMetadataType(typeof(ReportObjectType__Metadata))]
    public partial class ReportObjectType { }
}
