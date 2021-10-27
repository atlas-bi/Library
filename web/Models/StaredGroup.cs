using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class StaredGroup
    {
        public int StarId { get; set; }
        public int? Rank { get; set; }
        public int? Groupid { get; set; }
        public int? Ownerid { get; set; }

        public virtual UserGroup Group { get; set; }
        public virtual User Owner { get; set; }
    }
}
