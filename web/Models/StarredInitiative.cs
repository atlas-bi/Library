using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class StarredInitiative
    {
        public int StarId { get; set; }
        public int? Rank { get; set; }
        public int? Initiativeid { get; set; }
        public int? Ownerid { get; set; }
        public int? Folderid { get; set; }

        public virtual UserFavoriteFolder Folder { get; set; }
        public virtual DpDataInitiative Initiative { get; set; }
        public virtual User Owner { get; set; }
    }
}
