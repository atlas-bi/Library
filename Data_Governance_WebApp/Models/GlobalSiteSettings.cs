using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class GlobalSiteSettings
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
    }
}
