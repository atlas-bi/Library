using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class UserSetting
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }

        public virtual User User { get; set; }
    }
}
