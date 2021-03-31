/*
    Atlas of Information Management business intelligence library and documentation database.
    Copyright (C) 2020  Riverside Healthcare, Kankakee, IL

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    public class ReportObject__Metadata
    {
        // all ReportObject fields are readonly. Only ReportObject_doc fields are writable by the application.
        // https://stackoverflow.com/questions/6564772/ef-code-first-readonly-column
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportObjectId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ReportObjectBizKey { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SourceServer { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SourceDb { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SourceTable { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Name { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]

        [Display(Name = "System Description")]
        public string Description { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "System Detailed Description")]
        public string DetailedDescription { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? ReportObjectTypeId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? AuthorUserId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? LastModifiedByUserId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Last Modified on")]
        [DisplayFormat(NullDisplayText = "(N/A)")]
        public DateTime? LastModifiedDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ReportObjectUrl { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Epic INI")]
        [DisplayFormat(NullDisplayText = "(N/A)")]
        public string EpicMasterFile { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = "{0}", ApplyFormatInEditMode = true, NullDisplayText = "(N/A)")]
        [Display(Name = "Epic Record ID")]
        public decimal? EpicRecordId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Orphaned?")]
        public string OrphanedReportObjectYn { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Author")]
        [DisplayFormat(NullDisplayText = "(N/A)")]
        public virtual User AuthorUser { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Last Modified by")]
        [DisplayFormat(NullDisplayText = "(N/A)")]
        public virtual User LastModifiedByUser { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Type")]
        public virtual ReportObjectType ReportObjectType { get; set; }

    }

    [ModelMetadataType(typeof(ReportObject__Metadata))]
    public partial class ReportObject
    {
        [NotMapped]
        public virtual string LastUpdatedDateDisplayString
        // don't display the time portion if > 24 hrs ago
        {
            get
            {
                if (LastModifiedDate == null) { return ""; }
                var timeAgo = System.DateTime.Now.Subtract(LastModifiedDate ?? DateTime.Now);
                if (timeAgo.TotalMinutes < 1) { return String.Concat(timeAgo.Seconds.ToString(), " seconds ago"); }
                if (timeAgo.TotalHours < 1) { return String.Concat(timeAgo.Minutes.ToString(), " minutes ago"); }
                else if (timeAgo.TotalHours < 24) { return String.Concat(timeAgo.Hours.ToString(), " hours ago"); }
                else return (LastModifiedDate ?? DateTime.Now).ToShortDateString();
            }
        }
        [NotMapped]
        public virtual string LastLoadDateDisplayString
        // don't display the time portion if > 24 hrs ago
        {
            get
            {
                if (LastLoadDate == null) { return ""; }
                var timeAgo = System.DateTime.Now.Subtract(LastLoadDate ?? DateTime.Now);
                if (timeAgo.TotalMinutes < 1) { return String.Concat(timeAgo.Seconds.ToString(), " seconds ago"); }
                if (timeAgo.TotalHours < 1) { return String.Concat(timeAgo.Minutes.ToString(), " minutes ago"); }
                else if (timeAgo.TotalHours < 24) { return String.Concat(timeAgo.Hours.ToString(), " hours ago"); }
                else return (LastLoadDate ?? DateTime.Now).ToShortDateString();
            }
        }
        [NotMapped]
        public virtual string DisplayName

        {
            get
            {
                if (DisplayTitle == null) { return Name; }
                return DisplayTitle;
            }
        }
    }
}
