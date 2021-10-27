using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class StaredUser
    {
        public int StarId { get; set; }
        public int? Rank { get; set; }
        public int? Userid { get; set; }
        public int? Ownerid { get; set; }

        public virtual User Owner { get; set; }
        public virtual User User { get; set; }
    }
}
