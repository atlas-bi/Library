using Atlas_Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    public interface MaintenanceLog__Metadata { }

    [ModelMetadataType(typeof(MaintenanceLog__Metadata))]
    public partial class MaintenanceLog
    {
        [NotMapped]
        public virtual string MaintenanceDateDisplayString
        // don't display the time portion if > 24 hrs ago
        {
            get { return ModelHelpers.RelativeDate(MaintenanceDate); }
        }
    }
}
