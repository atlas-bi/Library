using System;
using System.Collections.Generic;

#nullable disable

namespace Atlas_Web.Models
{
    public partial class SearchTable
    {
        public int Id { get; set; }
        public int? ItemId { get; set; }
        public int? TypeId { get; set; }
        public string ItemType { get; set; }
        public int? ItemRank { get; set; }
        public string SearchFieldDescription { get; set; }
        public string SearchField { get; set; }
    }
}
