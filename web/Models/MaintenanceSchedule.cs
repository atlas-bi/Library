namespace Atlas_Web.Models
{
    public partial class MaintenanceSchedule
    {
        public MaintenanceSchedule()
        {
            ReportObjectDocs = new HashSet<ReportObjectDoc>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ReportObjectDoc> ReportObjectDocs { get; set; }
    }
}
