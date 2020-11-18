using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class SearchBasicSearchDataSmall
    {
        public int Id { get; set; }
        public int? ItemId { get; set; }
        public int? TypeId { get; set; }
        public string ItemType { get; set; }
        public int? ItemRank { get; set; }
        public string SearchFieldDescription { get; set; }
        public string SearchField { get; set; }
        public int? Hidden { get; set; }
        public int? VisibleType { get; set; }
        public int? Orphaned { get; set; }
    }
}
