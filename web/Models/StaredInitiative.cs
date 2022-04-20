using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class StaredInitiative
    {
        public int StarId { get; set; }
        public int? Rank { get; set; }
        public int? Initiativeid { get; set; }
        public int? Ownerid { get; set; }

        public virtual Initiative Initiative { get; set; }
        public virtual User Owner { get; set; }
    }
}
