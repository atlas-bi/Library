using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class UserFavoriteFolder
    {
        public UserFavoriteFolder()
        {
            StarredReports = new HashSet<StarredReport>();
            UserFavorites = new HashSet<UserFavorite>();
        }

        public int UserFavoriteFolderId { get; set; }
        public string FolderName { get; set; }
        public int? UserId { get; set; }
        public int? FolderRank { get; set; }

        public virtual ICollection<StarredReport> StarredReports { get; set; }
        public virtual ICollection<UserFavorite> UserFavorites { get; set; }
    }
}
