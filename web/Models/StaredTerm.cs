using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class StaredTerm
    {
        public int StarId { get; set; }
        public int? Rank { get; set; }
        public int? Termid { get; set; }
        public int? Ownerid { get; set; }

        public virtual User Owner { get; set; }
        public virtual Term Term { get; set; }
    }
}
