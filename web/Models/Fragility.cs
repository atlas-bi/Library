namespace Atlas_Web.Models
{
    public partial class Fragility
    {
        public Fragility()
        {
            ReportObjectDocs = new HashSet<ReportObjectDoc>();
        }

        public int FragilityId { get; set; }
        public string FragilityName { get; set; }

        public virtual ICollection<ReportObjectDoc> ReportObjectDocs { get; set; }
    }
}
