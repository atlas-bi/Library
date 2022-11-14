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
            if ((report.OrphanedReportObjectYn ?? "N") == "Y")
            {
                return null;
            }

            if (
                report.EpicRecordId.HasValue
                && !string.IsNullOrEmpty(report.EpicMasterFile)
                && context.IsHyperspace()
            )
            {
                Url =
                    "EpicAct:AR_RECORD_VIEWER,runparams:"
                    + report.EpicMasterFile
                    + "|"
                    + report.EpicRecordId;
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
            else if (Epic)
            {
                if (report.EpicMasterFile == "HGR" && report.EpicRecordId != null)
                {
                    if (AGL)
                    {
                        // not yet developed
                        Url = null;
                    }
                    else
                    {
                        Url =
                            "EpicAct:AC_NEW_REPORT_ADMIN,INFONAME:HGRRECORDID,INFOVALUE:"
                            + report.EpicRecordId;
                    }
                }
                else if (report.EpicMasterFile == "IDM" && report.EpicRecordId != null)
                {
                    if (AGL)
                    {
                        // not yet developed
                        Url = null;
                    }
                    else
                    {
                        Url =
                            "EpicAct:WM_DASHBOARD_EDITOR,INFONAME:IDMRECORDID,INFOVALUE:"
                            + report.EpicRecordId;
                    }
                }
                else if (report.EpicMasterFile == "IDB" && report.EpicRecordId != null)
                {
                    if (AGL)
                    {
                        Url = "WM_ITM_COMPONENT_EDITOR";
                    }
                    else
                    {
                        Url =
                            "EpicAct:WM_COMPONENT_EDITOR,INFONAME:IDBRECORDID,INFOVALUE:"
                            + report.EpicRecordId;
                    }
                }
                else if (
                    report.EpicMasterFile == "HRX"
                    && report.EpicRecordId != null
                    && report.EpicReportTemplateId != null
                )
                {
                    if (AGL)
                    {
                        Url = "AC_REPORT_SETTINGS_WEB";
                    }
                    else
                    {
                        Url =
                            "EpicAct:IP_REPORT_SETTING_POPUP,runparams:"
                            + report.EpicReportTemplateId
                            + "|"
                            + report.EpicRecordId;
                    }
                }
                else if (report.EpicMasterFile == "IDN" && report.EpicRecordId != null)
                {
                    if (AGL)
                    {
                        Url = "WM_ITM_METRIC_EDITOR";
                    }
                    else
                    {
                        Url =
                            "EpicAct:WM_METRIC_EDITOR,INFONAME:IDNRECORDID,INFOVALUE:"
                            + report.EpicRecordId;
                    }
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
            string NewUrl = null;
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
                        && Epic
                        && ReportType != "Source Radar Dashboard Component"
                    )
                )
                && ReportType != "Epic-Crystal Report"
                && ReportType != "Crystal Report"
            )
            {
                if (EpicMasterFile == "HRX" && ReportType != "SlicerDicer Session"
                // hrx authorization is done in a resource
                // authorization service before hitting this code.
                )
                {
                    if (AGL)
                    {
                        NewUrl = "AC_RW_STATUS";
                    }
                    else
                    {
                        NewUrl =
                            "EpicAct:AC_RW_STATUS,RUNPARAMS:"
                            + EpicReportTemplateId
                            + "|"
                            + EpicRecordId;
                    }
                }
                else if (EpicMasterFile == "HGR")
                {
                    if (AGL)
                    {
                        NewUrl = "AC_REPORT_SETTINGS_WEB";
                    }
                    else
                    {
                        NewUrl = "EpicAct:AC_RW_STATUS,RUNPARAMS:" + EpicReportTemplateId;
                    }
                }
                else if (EpicMasterFile == "IDM")
                {
                    if (AGL)
                    {
                        NewUrl = "WM_DASHBOARD_LAUNCHER";
                    }
                    else
                    {
                        NewUrl = "EpicAct:WM_DASHBOARD_LAUNCHER,runparams:" + EpicRecordId;
                    }
                }
                else if ((ReportType == "SSRS Report" || ReportType == "SSRS File") && Epic)
                {
                    if (AGL)
                    {
                        NewUrl = "CST_WEB_BROWSER";
                    }
                    else
                    {
                        if (EnabledForHyperspace == "Y")
                        {
                            NewUrl =
                                "EpicAct:AC_RW_WEB_BROWSER,LaunchOptions:2,runparams:"
                                + Url
                                + "&EPIC=1|FormCaption="
                                + ReportName
                                + "|ActivityName="
                                + ReportName;
                        }
                        else
                        {
                            NewUrl =
                                "EpicAct:AC_RW_WEB_BROWSER,LaunchOptions:2,runparams:"
                                + Url
                                + "|FormCaption="
                                + ReportName
                                + "|ActivityName="
                                + ReportName;
                        }
                    }
                }
                else if (EpicMasterFile == "IDN")
                {
                    if (AGL)
                    {
                        NewUrl = "WM_ITM_METRIC_EDITOR";
                    }
                    else
                    {
                        NewUrl =
                            "EpicAct:WM_METRIC_EDITOR,INFONAME:IDNRECORDID,INFOVALUE:"
                            + EpicRecordId;
                    }
                }
                else if (EpicMasterFile == "FDM" && EpicRecordId != null)
                {
                    if (AGL)
                    {
                        NewUrl = "BI_ITM_SLICERDICER";
                    }
                    else
                    {
                        NewUrl =
                            "EpicACT:BI_SLICERDICER,LaunchOptions:16,RunParams:StartingDataModelId="
                            + EpicRecordId;
                    }
                }
                else if (ReportType == "SlicerDicer Session" && EpicRecordId != null)
                {
                    if (AGL)
                    {
                        NewUrl = "BI_ITM_SLICERDICER";
                    }
                    else
                    {
                        NewUrl =
                            "EpicACT:BI_SLICERDICER,RunParams:StartingPopulationId=" + EpicRecordId;
                    }
                }
                else
                {
                    NewUrl = Url;
                }
            }

            return NewUrl;
        }
    }
}
