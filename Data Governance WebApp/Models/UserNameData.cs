using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class UserNameData
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public virtual User User { get; set; }
    }
}
