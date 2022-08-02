namespace Atlas_Web.Models
{
    public partial class StrategicImportance
    {
        public StrategicImportance()
        {
            Initiatives = new HashSet<Initiative>();
            Collections = new HashSet<Collection>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Initiative> Initiatives { get; set; }
        public virtual ICollection<Collection> Collections { get; set; }
    }
}
