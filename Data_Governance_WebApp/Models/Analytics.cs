using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class Analytics
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string AppCodeName { get; set; }
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string CookieEnabled { get; set; }
        public string Language { get; set; }
        public string Oscpu { get; set; }
        public string Platform { get; set; }
        public string UserAgent { get; set; }
        public string Host { get; set; }
        public string Hostname { get; set; }
        public string Href { get; set; }
        public string Protocol { get; set; }
        public string Search { get; set; }
        public string Pathname { get; set; }
        public string Hash { get; set; }
        public string ScreenHeight { get; set; }
        public string ScreenWidth { get; set; }
        public string Origin { get; set; }
        public string Title { get; set; }
        public string LoadTime { get; set; }
        public DateTime? AccessDateTime { get; set; }
        public string Referrer { get; set; }
        public int? UserId { get; set; }
        public double? Zoom { get; set; }
        public int? Epic { get; set; }
        public int? Active { get; set; }
        public string PageId { get; set; }
        public string SessionId { get; set; }
        public int? PageTime { get; set; }
        public int? SessionTime { get; set; }
        public DateTime? UpdateTime { get; set; }

        public virtual User User { get; set; }
    }
}
