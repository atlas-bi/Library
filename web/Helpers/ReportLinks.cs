using System;
using Atlas_Web.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Atlas_Web.Helpers
{
    public static class ReportLinkHelpers
    {
        public static Boolean IsEpic(HttpContext Context)
        {
            if (Context.Request.Cookies["EPIC"] == "1" || Context.Request.Query["EPIC"] == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string RecordViewerLink(
            HttpContext Context,
            string EpicMasterFile,
            string EpicRecordId,
            string Orphan
        )
        {
            string Url = null;
            if ((Orphan ?? "N") == "Y")
            {
                return null;
            }
            bool Epic = IsEpic(Context);

            if (
                EpicRecordId != null
                && EpicRecordId != ""
                && EpicMasterFile != null
                && EpicMasterFile != ""
                && Epic
            )
            {
                Url = "EpicAct:AR_RECORD_VIEWER,runparams:" + EpicMasterFile + "|" + EpicRecordId;
            }
            return Url;
        }

        public static string EditReportFromParams(
            string Domain,
            HttpContext Context,
            string ReportServerPath,
            string SourceServer,
            string EpicMasterFile,
            string EpicReportTemplateId,
            string EpicRecordId,
            string Orphan
        )
        {
            string Url = null;
            if ((Orphan ?? "N") == "Y")
            {
                return null;
            }
            bool Epic = IsEpic(Context);
            if (ReportServerPath != null && ReportServerPath != "" && !Epic)
            {
                Url =
                    "reportbuilder:Action=Edit&ItemPath="
                    + @Uri.EscapeDataString(ReportServerPath)
                    + "&Endpoint=https%3A%2F%2F"
                    + SourceServer
                    + "."
                    + Domain
                    + "%3A443%2FReportServer";
            }
            else if (EpicMasterFile == "HGR" && EpicRecordId != null && Epic)
            {
                Url = "EpicAct:AC_NEW_REPORT_ADMIN,INFONAME:HGRRECORDID,INFOVALUE:" + EpicRecordId;
            }
            else if (EpicMasterFile == "IDM" && EpicRecordId != null && Epic)
            {
                Url = "EpicAct:WM_DASHBOARD_EDITOR,INFONAME:IDMRECORDID,INFOVALUE:" + EpicRecordId;
            }
            else if (EpicMasterFile == "IDB" && EpicRecordId != null && Epic)
            {
                Url = "EpicAct:WM_COMPONENT_EDITOR,INFONAME:IDBRECORDID,INFOVALUE:" + EpicRecordId;
            }
            else if (
                EpicMasterFile == "HRX"
                && EpicRecordId != null
                && Epic
                && EpicReportTemplateId != null
            )
            {
                Url =
                    "EpicAct:IP_REPORT_SETTING_POPUP,runparams:"
                    + EpicReportTemplateId
                    + "|"
                    + EpicRecordId;
            }
            else if (
                EpicMasterFile == "IDN"
                && EpicRecordId != null
                && Epic
                && EpicReportTemplateId != null
            )
            {
                Url = "EpicAct:WM_METRIC_EDITOR,INFONAME:IDNRECORDID,INFOVALUE:" + EpicRecordId;
            }

            return Url;
        }

        public static string ReportManageUrlFromParams(
            string Domain,
            HttpContext Context,
            string ReportType,
            string ReportServerPath,
            string SourceServer,
            string Orphan
        )
        {
            string Url = null;

            if ((Orphan ?? "N") == "Y")
            {
                return null;
            }

            bool Epic = IsEpic(Context);
            if ((ReportType == "SSRS Report" || ReportType == "SSRS File") && !Epic)
            {
                Url =
                    "https://"
                    + SourceServer
                    + "."
                    + Domain
                    + "/Reports/manage/catalogitem/properties"
                    + ReportServerPath;
            }

            return Url;
        }

        public static string ReportUrlFromParams(
            HttpContext Context,
            ReportObject reportObject,
            Atlas_WebContext _context,
            string username
        )
        {
            if (reportObject == null || (reportObject.OrphanedReportObjectYn ?? "N") == "Y")
            {
                return null;
            }
            string Url = reportObject.ReportObjectUrl;
            string Name = reportObject.Name;
            string ReportType =
                _context.ReportObjectTypes
                    .Where(x => x.ReportObjectTypeId == reportObject.ReportObjectTypeId)
                    .First().Name;
            int ReportTypeId = (int)reportObject.ReportObjectTypeId;
            string EpicReportTemplateId = reportObject.EpicReportTemplateId.ToString();
            string EpicRecordId = reportObject.EpicRecordId.ToString();
            string EpicMasterFile = reportObject.EpicMasterFile;
            string EnabledForHyperspace =
                (
                    reportObject.ReportObjectDoc != null
                        ? reportObject.ReportObjectDoc.EnabledForHyperspace
                        : "N"
                ) ?? "N";
            string NewUrl = null;
            if (Name is null)
            {
                return null;
            }
            string ReportName = Name.Replace("|", " ").Replace("=", " ");
            bool Epic = IsEpic(Context);

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
                if (
                    EpicMasterFile == "HRX"
                    && ReportType != "SlicerDicer Session"
                    && (
                        (
                            (ReportTypeId == 3 || ReportTypeId == 17)
                            && UserHelpers.CheckHrxPermissions(
                                _context,
                                reportObject.ReportObjectId,
                                username
                            )
                        ) || (ReportTypeId != 3 && ReportTypeId != 17)
                    )
                )
                {
                    NewUrl =
                        "EpicAct:AC_RW_STATUS,RUNPARAMS:"
                        + EpicReportTemplateId
                        + "|"
                        + EpicRecordId;
                }
                else if (EpicMasterFile == "HGR")
                {
                    NewUrl = "EpicAct:AC_RW_STATUS,RUNPARAMS:" + EpicReportTemplateId;
                }
                else if (EpicMasterFile == "IDM")
                {
                    NewUrl = "EpicAct:WM_DASHBOARD_LAUNCHER,runparams:" + EpicRecordId;
                }
                else if ((ReportType == "SSRS Report" || ReportType == "SSRS File") && Epic)
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
                else if (EpicMasterFile == "IDN")
                {
                    NewUrl =
                        "EpicAct:WM_METRIC_EDITOR,INFONAME:IDNRECORDID,INFOVALUE:" + EpicRecordId;
                }
                else if (EpicMasterFile == "FDM" && EpicRecordId != null)
                {
                    NewUrl =
                        "EpicACT:BI_SLICERDICER,LaunchOptions:16,RunParams:StartingDataModelId="
                        + EpicRecordId;
                }
                else if (ReportType == "SlicerDicer Session" && EpicRecordId != null)
                {
                    NewUrl =
                        "EpicACT:BI_SLICERDICER,RunParams:StartingPopulationId=" + EpicRecordId;
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
