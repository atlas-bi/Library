using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class StaredSearch
    {
        public int StarId { get; set; }
        public int? Rank { get; set; }
        public string Search { get; set; }
        public int? Ownerid { get; set; }

        public virtual User Owner { get; set; }
    }
}
