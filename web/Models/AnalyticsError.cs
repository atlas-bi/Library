namespace Atlas_Web.Models
{
    public partial class AnalyticsError
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? StatusCode { get; set; }
        public string Message { get; set; }
        public string Trace { get; set; }
        public DateTime? LogDateTime { get; set; }
        public int? Handled { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UserAgent { get; set; }
        public string Referrer { get; set; }

        public virtual User User { get; set; }
    }
}
