namespace Atlas_Web.Models
{
    public partial class FinancialImpact
    {
        public FinancialImpact()
        {
            Initiatives = new HashSet<Initiative>();
            Collections = new HashSet<Collection>();
        }

        public int FinancialImpactId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Initiative> Initiatives { get; set; }
        public virtual ICollection<Collection> Collections { get; set; }
    }
}
