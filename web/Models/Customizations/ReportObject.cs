using Atlas_Web.Helpers;
using Atlas_Web.Services;
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
        [DisplayFormat(
            DataFormatString = "{0}",
            ApplyFormatInEditMode = true,
            NullDisplayText = "(N/A)"
        )]
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
            get { return ModelHelpers.RelativeDate(LastModifiedDate); }
        }

        [NotMapped]
        public virtual string LastLoadDateDisplayString
        // don't display the time portion if > 24 hrs ago
        {
            get { return ModelHelpers.RelativeDate(LastLoadDate); }
        }

        [NotMapped]
        public virtual string DisplayName
        {
            get
            {
                if (DisplayTitle == null)
                {
                    return Name;
                }
                return DisplayTitle;
            }
        }
    }

    public static class ReportObjectExtensions
    {
        public static string RecordViewerUrl(this ReportObject report, HttpContext context)
        {
            string Url = null;
            bool AGL = context.IsAgl();
            if ((report.OrphanedReportObjectYn ?? "N") == "Y")
            {
                return null;
            }

            if (
                report.EpicRecordId.HasValue
                && !string.IsNullOrEmpty(report.EpicMasterFile)
                && context.IsAgl()
            )
            {
                Url = "AR_ITM_RECORDVIEWER";
            }
            return Url;
        }

        public static string ManageReportUrl(
            this ReportObject report,
            HttpContext context,
            IConfiguration config
        )
        {
            string Url = null;

            if ((report.OrphanedReportObjectYn ?? "N") == "Y")
            {
                return null;
            }

            if (
                !context.IsHyperspace()
                && (
                    report.ReportObjectType.Name == "SSRS Report"
                    || report.ReportObjectType.Name == "SSRS File"
                    || report.ReportObjectType.Name == "SSRS Report Link"
                )
            )
            {
                Url =
                    "https://"
                    + report.SourceServer
                    + "."
                    + config["AppSettings:org_domain"]
                    + "/Reports/manage/catalogitem/properties"
                    + report.ReportServerPath;
            }

            return Url;
        }

        public static string EditReportUrl(
            this ReportObject report,
            HttpContext context,
            IConfiguration config
        )
        {
            string Url = null;
            if ((report.OrphanedReportObjectYn ?? "N") == "Y")
            {
                return null;
            }
            bool Epic = context.IsHyperspace();
            bool AGL = context.IsAgl();

            if (!string.IsNullOrEmpty(report.ReportServerPath) && !Epic)
            {
                Url =
                    "reportbuilder:Action=Edit&ItemPath="
                    + @Uri.EscapeDataString(report.ReportServerPath)
                    + "&Endpoint=https%3A%2F%2F"
                    + report.SourceServer
                    + "."
                    + config["AppSettings:org_domain"]
                    + "%3A443%2FReportServer";
            }
            else if (AGL)
            {
                if (report.EpicMasterFile == "HGR" && report.EpicRecordId != null)
                {
                    // not yet developed
                    Url = null;
                }
                else if (report.EpicMasterFile == "IDM" && report.EpicRecordId != null)
                {
                    // not yet developed
                    Url = null;
                }
                else if (report.EpicMasterFile == "IDB" && report.EpicRecordId != null)
                {
                    Url = "WM_ITM_COMPONENT_EDITOR";
                }
                else if (
                    report.EpicMasterFile == "HRX"
                    && report.EpicRecordId != null
                    && report.EpicReportTemplateId != null
                )
                {
                    Url = "AC_REPORT_SETTINGS_WEB";
                }
                else if (report.EpicMasterFile == "IDN" && report.EpicRecordId != null)
                {
                    Url = "WM_ITM_METRIC_EDITOR";
                }
            }

            return Url;
        }

        public static string RunReportUrl(
            this ReportObject report,
            HttpContext context,
            IConfiguration config,
            bool authorized
        )
        {
            if (!authorized)
            {
                return null;
            }
            if ((report.OrphanedReportObjectYn ?? "N") == "Y")
            {
                return null;
            }
            string Url = report.ReportObjectUrl;
            string Name = report.Name;
            string ReportType = report.ReportObjectType.Name;
            string EpicReportTemplateId = report.EpicReportTemplateId.ToString();
            string EpicRecordId = report.EpicRecordId.ToString();
            string EpicMasterFile = report.EpicMasterFile;
            string EnabledForHyperspace =
                (report.ReportObjectDoc != null ? report.ReportObjectDoc.EnabledForHyperspace : "N")
                ?? "N";
            string NewUrl = Url;
            if (Name is null)
            {
                return null;
            }
            string ReportName = Name.Replace("|", " ").Replace("=", " ");
            bool Epic = context.IsHyperspace();
            bool AGL = context.IsAgl();

            if (
                (
                    (Url != "" && Url != null)
                    || (
                        ReportType != "SSRS Report"
                        && ReportType != "SSRS File"
                        && ReportType != "Source Radar Dashboard Component"
                    )
                )
                && ReportType != "Epic-Crystal Report"
                && ReportType != "Crystal Report"
                && AGL
            )
            {
                if (EpicMasterFile == "HRX" && ReportType != "SlicerDicer Session"
                // hrx authorization is done in a resource
                // authorization service before hitting this code.
                )
                {
                    NewUrl = "AC_RW_STATUS";
                }
                else if (EpicMasterFile == "HGR")
                {
                    NewUrl = "AC_REPORT_SETTINGS_WEB";
                }
                else if (EpicMasterFile == "IDM")
                {
                    NewUrl = "WM_DASHBOARD_LAUNCHER";
                }
                else if ((ReportType == "SSRS Report" || ReportType == "SSRS File") && Epic)
                {
                    NewUrl = "CST_WEB_BROWSER";
                }
                else if (EpicMasterFile == "IDN")
                {
                    NewUrl = "WM_ITM_METRIC_EDITOR";
                }
                else if (EpicMasterFile == "FDM" && EpicRecordId != null)
                {
                    NewUrl = "BI_ITM_SLICERDICER";
                }
                else if (ReportType == "SlicerDicer Session" && EpicRecordId != null)
                {
                    NewUrl = "BI_ITM_SLICERDICER";
                }
                // after migration fully to hyperdrive this can be removed
                // it is kept so that tableau links are
                // else if (!string.IsNullOrEmpty(Url)) {
                //     NewUrl = "CST_WEB_BROWSER";
                // }
            }

            return NewUrl;
        }
    }
}
