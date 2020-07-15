using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class UserFavorites
    {
        public int UserFavoritesId { get; set; }
        public string ItemType { get; set; }
        public int? ItemRank { get; set; }
        public int? ItemId { get; set; }
        public int? UserId { get; set; }
        public string ItemName { get; set; }
        public int? FolderId { get; set; }

        public virtual UserFavoriteFolders Folder { get; set; }
        public virtual User User { get; set; }
    }
}
