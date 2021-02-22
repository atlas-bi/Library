using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class ReportObjectSubscriptions
    {
        public int ReportObjectSubscriptionsId { get; set; }
        public int? ReportObjectId { get; set; }
        public int? UserId { get; set; }
        public string SubscriptionId { get; set; }
        public int? InactiveFlags { get; set; }
        public string EmailList { get; set; }
        public string Description { get; set; }
        public string LastStatus { get; set; }
        public DateTime? LastRunTime { get; set; }
        public string SubscriptionTo { get; set; }
        public DateTime? LastLoadDate { get; set; }

        public virtual ReportObject ReportObject { get; set; }
        public virtual User User { get; set; }
    }
}
