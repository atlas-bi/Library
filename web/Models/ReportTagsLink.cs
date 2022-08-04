// this table will be app, but joined to dbo report object as we will allow ETL to update it
namespace Atlas_Web.Models
{
    public partial class ReportTagLink
    {
        public int ReportTagLinkId { get; set; }
        public int ReportId { get; set; }
        public int TagId { get; set; }
        public string ShowInHeader { get; set; }
        public virtual Tag Tag { get; set; }
        public virtual ReportObject Report { get; set; }
    }
}
