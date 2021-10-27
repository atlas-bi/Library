using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class StaredCollection
    {
        public int StarId { get; set; }
        public int? Rank { get; set; }
        public int? Collectionid { get; set; }
        public int? Ownerid { get; set; }

        public virtual DpDataProject Collection { get; set; }
        public virtual User Owner { get; set; }
    }
}
