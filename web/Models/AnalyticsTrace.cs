using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class AnalyticsTrace
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? Level { get; set; }
        public string Message { get; set; }
        public string Logger { get; set; }
        public DateTime? LogDateTime { get; set; }
        public string LogId { get; set; }
        public int? Handled { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UserAgent { get; set; }
        public string Referer { get; set; }

        public virtual User User { get; set; }
    }
}
