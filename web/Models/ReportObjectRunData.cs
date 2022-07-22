namespace Atlas_Web.Models
{
    public partial class ReportObjectRunData
    {
        public ReportObjectRunData()
        {
            ReportObjectRunDataBridges = new HashSet<ReportObjectRunDataBridge>();
        }

        public int RunId { get; set; }
        public string RunDataId { get; set; }
        public int? RunUserId { get; set; }
        public DateTime RunStartTime { get; set; }
        public int? RunDurationSeconds { get; set; }
        public string RunStatus { get; set; }
        public DateTime LastLoadDate { get; set; }
        public DateTime RunStartTime_Hour { get; set; }
        public DateTime RunStartTime_Day { get; set; }
        public DateTime RunStartTime_Month { get; set; }
        public DateTime RunStartTime_Year { get; set; }

        public virtual ICollection<ReportObjectRunDataBridge> ReportObjectRunDataBridges { get; set; }
        public virtual User RunUser { get; set; }
    }
}
