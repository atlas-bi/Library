using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Atlas_Web.Models
{
    public class UserFavorites__Metadata { }

    [ModelMetadataType(typeof(UserFavorites__Metadata))]
    public partial class UserFavorite
    {
        [NotMapped]
        public string ItemType_Proper
        {
            get
            {
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                return textInfo.ToTitleCase(ItemType.ToLower());
            }
        }
    }
}
