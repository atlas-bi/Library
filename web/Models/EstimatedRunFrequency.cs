namespace Atlas_Web.Models
{
    public partial class EstimatedRunFrequency
    {
        public EstimatedRunFrequency()
        {
            ReportObjectDocs = new HashSet<ReportObjectDoc>();
        }

        public int EstimatedRunFrequencyId { get; set; }
        public string EstimatedRunFrequencyName { get; set; }

        public virtual ICollection<ReportObjectDoc> ReportObjectDocs { get; set; }
    }
}
