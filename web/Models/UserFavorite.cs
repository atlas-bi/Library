using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class UserFavorite
    {
        public int UserFavoritesId { get; set; }
        public string ItemType { get; set; }
        public int? ItemRank { get; set; }
        public int? ItemId { get; set; }
        public int? UserId { get; set; }
        public string ItemName { get; set; }
        public int? FolderId { get; set; }

        public virtual UserFavoriteFolder Folder { get; set; }
        public virtual User User { get; set; }
    }
}
