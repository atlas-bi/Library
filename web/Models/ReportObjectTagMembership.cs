namespace Atlas_Web.Models
{
    public partial class ReportObjectTagMembership
    {
        public int TagMembershipId { get; set; }
        public int ReportObjectId { get; set; }
        public int TagId { get; set; }
        public int? Line { get; set; }

        public virtual ReportObject ReportObject { get; set; }
        public virtual ReportObjectTag Tag { get; set; }
    }
}
