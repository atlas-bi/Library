namespace Atlas_Web.Models
{
    public partial class MaintenanceLogStatus
    {
        public MaintenanceLogStatus()
        {
            MaintenanceLogs = new HashSet<MaintenanceLog>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<MaintenanceLog> MaintenanceLogs { get; set; }
    }
}
