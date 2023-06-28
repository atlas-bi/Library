using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using Atlas_Web.Helpers;

namespace Atlas_Web.Models
{
    public interface Analytics__Metadata { }

    [ModelMetadataType(typeof(Analytics__Metadata))]
    public partial class Analytic
    {
        [NotMapped]
        public virtual string AccessDateTimeDisplayString
        // don't display the time portion if > 24 hrs ago
        {
            get { return ModelHelpers.RelativeDate(AccessDateTime); }
        }
    }
}
