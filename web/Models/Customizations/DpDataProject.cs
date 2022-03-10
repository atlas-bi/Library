using Atlas_Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    public class DpDataProject__Metadata { }

    [ModelMetadataType(typeof(DpDataProject__Metadata))]
    public partial class DpDataProject
    {
        [NotMapped]
        public virtual string LastUpdatedDateDisplayString
        // don't display the time portion if > 24 hrs ago
        {
            get { return ModelHelpers.RelativeDate(LastUpdateDate); }
        }
    }
}
