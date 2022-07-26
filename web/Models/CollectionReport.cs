namespace Atlas_Web.Models
{
    public partial class CollectionReport
    {
        public int LinkId { get; set; }
        public int ReportId { get; set; }
        public int CollectionId { get; set; }
        public int? Rank { get; set; }

        public virtual Collection DataProject { get; set; }
        public virtual ReportObject Report { get; set; }
    }
}
