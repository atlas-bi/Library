using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class ReportManageEngineTickets
    {
        public int ManageEngineTicketsId { get; set; }
        public int? TicketNumber { get; set; }
        public string Description { get; set; }
        public int? ReportObjectId { get; set; }
        public string TicketUrl { get; set; }

        public virtual ReportObject ReportObject { get; set; }
    }
}
