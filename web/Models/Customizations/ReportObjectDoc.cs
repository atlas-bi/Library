using Atlas_Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    public interface ReportObjectDoc__Metadata { }

    [ModelMetadataType(typeof(ReportObjectDoc__Metadata))]
    public partial class ReportObjectDoc
    {
        [NotMapped]
        public virtual string LastUpdatedDateTimeDisplayString
        // don't display the time portion if > 24 hrs ago
        {
            get { return ModelHelpers.RelativeDate(LastUpdateDateTime); }
        }
    }
}
