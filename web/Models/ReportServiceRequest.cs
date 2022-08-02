namespace Atlas_Web.Models
{
    public partial class ReportServiceRequest
    {
        public int ServiceRequestId { get; set; }
        public string TicketNumber { get; set; }
        public string Description { get; set; }
        public int ReportObjectId { get; set; }
        public string TicketUrl { get; set; }

        public virtual ReportObjectDoc ReportObject { get; set; }
    }
}
