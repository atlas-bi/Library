using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class UserFavoriteFolder
    {
        public UserFavoriteFolder()
        {
            UserFavorites = new HashSet<UserFavorite>();
        }

        public int UserFavoriteFolderId { get; set; }
        public string FolderName { get; set; }
        public int? UserId { get; set; }
        public int? FolderRank { get; set; }

        public virtual ICollection<UserFavorite> UserFavorites { get; set; }
    }
}
