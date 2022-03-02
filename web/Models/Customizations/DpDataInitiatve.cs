using Atlas_Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    public class DpDataInitiative__Metadata { }

    [ModelMetadataType(typeof(DpDataInitiative__Metadata))]
    public partial class DpDataInitiative
    {
        [NotMapped]
        public virtual string LastUpdatedDateDisplayString
        // don't display the time portion if > 24 hrs ago
        {
            get { return ModelHelpers.RelativeDate(LastUpdateDate); }
        }
    }
}
