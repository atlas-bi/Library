﻿namespace Atlas_Web.Models
{
    public partial class StarredGroup
    {
        public int StarId { get; set; }
        public int? Rank { get; set; }
        public int Groupid { get; set; }
        public int Ownerid { get; set; }
        public int? Folderid { get; set; }

        public virtual UserFavoriteFolder Folder { get; set; }
        public virtual UserGroup Group { get; set; }
        public virtual User Owner { get; set; }
    }
}
