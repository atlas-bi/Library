using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class StarredUser
    {
        public int StarId { get; set; }
        public int? Rank { get; set; }
        public int? Userid { get; set; }
        public int? Ownerid { get; set; }
        public int? Folderid { get; set; }

        public virtual UserFavoriteFolder Folder { get; set; }
        public virtual User Owner { get; set; }
        public virtual User User { get; set; }
    }
}
