namespace Atlas_Web.Models
{
    public partial class ReportObjectParameter
    {
        public int ReportObjectParameterId { get; set; }
        public int? ReportObjectId { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }

        public virtual ReportObject ReportObject { get; set; }
    }
}
