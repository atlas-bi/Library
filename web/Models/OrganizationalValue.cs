namespace Atlas_Web.Models
{
    public partial class OrganizationalValue
    {
        public OrganizationalValue()
        {
            ReportObjectDocs = new HashSet<ReportObjectDoc>();
        }

        public int OrganizationalValueId { get; set; }
        public string OrganizationalValueName { get; set; }

        public virtual ICollection<ReportObjectDoc> ReportObjectDocs { get; set; }
    }
}
