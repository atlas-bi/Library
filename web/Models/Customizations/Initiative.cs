using Atlas_Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    public interface Initiative__Metadata { }

    [ModelMetadataType(typeof(Initiative__Metadata))]
    public partial class Initiative
    {
        [NotMapped]
        public virtual string LastUpdatedDateDisplayString
        // don't display the time portion if > 24 hrs ago
        {
            get { return ModelHelpers.RelativeDate(LastUpdateDate); }
        }
    }
}
