using Atlas_Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    public class ReportObjectRunData__Metadata { }

    [ModelMetadataType(typeof(ReportObjectRunData__Metadata))]
    public partial class ReportObjectRunDatum
    {
        [NotMapped]
        public virtual string RunStartTimeDisplayString
        // don't display the time portion if > 24 hrs ago
        {
            get { return ModelHelpers.RelativeDate(RunStartTime); }
        }
    }
}
