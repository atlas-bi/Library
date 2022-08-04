namespace Atlas_Web.Models
{
    public partial class FragilityTag
    {
        public FragilityTag()
        {
            ReportObjectDocFragilityTags = new HashSet<ReportObjectDocFragilityTag>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ReportObjectDocFragilityTag> ReportObjectDocFragilityTags { get; set; }
    }
}
