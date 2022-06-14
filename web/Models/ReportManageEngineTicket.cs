using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class ReportManageEngineTicket
    {
        public int ManageEngineTicketsId { get; set; }
        public string TicketNumber { get; set; }
        public string Description { get; set; }
        public int? ReportObjectId { get; set; }
        public string TicketUrl { get; set; }

        public virtual ReportObjectDoc ReportObject { get; set; }
    }
}
