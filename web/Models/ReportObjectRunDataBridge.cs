namespace Atlas_Web.Models
{
    public partial class ReportObjectRunDataBridge
    {
        public int BridgeId { get; set; }
        public int ReportObjectId { get; set; }
        public string RunId { get; set; }
        public int Runs { get; set; }
        public int Inherited { get; set; }

        public virtual ReportObject ReportObject { get; set; }
        public virtual ReportObjectRunData RunData { get; set; }
    }
}
