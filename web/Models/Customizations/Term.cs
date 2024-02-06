using System.ComponentModel.DataAnnotations.Schema;
using Atlas_Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Models
{
    public class Term__Metadata
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TermHistoryId { get; set; }
    }

    [ModelMetadataType(typeof(Term__Metadata))]
    public partial class Term
    {
        [NotMapped]
        public virtual string LastUpdatedDateTimeDisplayString
        // don't display the time portion if > 24 hrs ago
        {
            get { return ModelHelpers.RelativeDate(LastUpdatedDateTime); }
        }

        [NotMapped]
        public virtual string ApprovalDateTimeDisplayString
        // don't display the time portion if > 24 hrs ago
        {
            get { return ModelHelpers.RelativeDate(ApprovalDateTime); }
        }
    }
}
