using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class StarredTerm
    {
        public int StarId { get; set; }
        public int? Rank { get; set; }
        public int? Termid { get; set; }
        public int? Ownerid { get; set; }
        public int? Folderid { get; set; }

        public virtual UserFavoriteFolder Folder { get; set; }
        public virtual User Owner { get; set; }
        public virtual Term Term { get; set; }
    }
}
