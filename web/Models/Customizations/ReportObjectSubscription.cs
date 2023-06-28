using Atlas_Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    public interface ReportObjectSubscriptions__Metadata { }

    [ModelMetadataType(typeof(ReportObjectSubscriptions__Metadata))]
    public partial class ReportObjectSubscription
    {
        [NotMapped]
        public virtual string LastRunDisplayString
        // don't display the time portion if > 24 hrs ago
        {
            get { return ModelHelpers.RelativeDate(LastRunTime); }
        }
    }
}
