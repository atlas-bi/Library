using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class GlobalSiteSetting
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
    }
}
