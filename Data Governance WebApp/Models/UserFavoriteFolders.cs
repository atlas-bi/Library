using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class UserFavoriteFolders
    {
        public UserFavoriteFolders()
        {
            UserFavorites = new HashSet<UserFavorites>();
        }

        public int UserFavoriteFolderId { get; set; }
        public string FolderName { get; set; }
        public int? UserId { get; set; }
        public int? FolderRank { get; set; }

        public virtual ICollection<UserFavorites> UserFavorites { get; set; }
    }
}
