namespace Atlas_Web.Models
{
    public partial class CollectionReport
    {
        public int ReportAnnotationId { get; set; }
        public int? ReportId { get; set; }
        public int? DataProjectId { get; set; }
        public int? Rank { get; set; }

        public virtual Collection DataProject { get; set; }
        public virtual ReportObject Report { get; set; }
    }
}
