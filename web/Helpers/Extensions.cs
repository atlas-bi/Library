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
using System;
using Atlas_Web.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Markdig;
using Microsoft.Extensions.Caching.Memory;
using System.Data.SqlClient;

namespace Atlas_Web.Helpers
{
    public class TaskData
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int TaskId { get; set; }
        public string MonthName { get; set; }
        public string Date { get; set; }
    }

    public class UserHelpers
    {
        public static Boolean IsAdmin(
            IMemoryCache cache,
            Atlas_WebContext _context,
            string username
        )
        {
            username = username ?? "default";
            var is_Admin = (
                from r in _context.UserRoles
                where r.UserRoleLinks.Any(x => x.User.Username == username && x.UserRolesId == 1)
                select new { r.Name }
            ).Count();
            if (is_Admin > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<UserPreference> GetPreferences(
            IMemoryCache cache,
            Atlas_WebContext _context,
            string username
        )
        {
            username = username ?? "default";
            return cache.GetOrCreate<List<UserPreference>>(
                "Preferences-" + username,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                    return _context.UserPreferences
                        .Where(x => x.User.Username == username)
                        .ToList();
                }
            );
        }

        public static MyRole GetMyRole(
            IMemoryCache cache,
            Atlas_WebContext _context,
            string username
        )
        {
            username = username ?? "default";
            return (
                from p in _context.UserPreferences
                where p.User.Username == username && p.ItemType == "ActiveRole"
                join u in _context.UserRoles on p.ItemValue equals u.UserRolesId
                select new MyRole { Id = (int)p.ItemValue, Name = u.Name }
            ).FirstOrDefault();
        }

        public static User GetUser(IMemoryCache cache, Atlas_WebContext _context, string username)
        {
            username = username ?? "default";
            return cache.GetOrCreate<User>(
                "User-" + username,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);

                    return _context.Users.Where(u => u.Username.Equals(username)).FirstOrDefault();
                }
            );
        }

        public static List<UserFavorite> GetUserFavorites(
            IMemoryCache cache,
            Atlas_WebContext _context,
            string username
        )
        {
            username = username ?? "default";
            return cache.GetOrCreate<List<UserFavorite>>(
                "Favorites-" + username,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                    return _context.UserFavorites
                        .Where(
                            x =>
                                x.User == (UserHelpers.GetUser(cache, _context, username))
                                && (x.ItemId > 0 || x.ItemName != null)
                        )
                        .ToList();
                }
            );
        }

        public static List<int?> GetUserPermissions(
            IMemoryCache cache,
            Atlas_WebContext _context,
            string username
        )
        {
            username = username ?? "default";
            // master permision cache - so we can clear all when a perm changes.
            var master = cache.GetOrCreate<List<string>>(
                "MasterUserPermissions",
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                    return new List<String> { "UserPermissions-" + username };
                }
            );

            if (master.Count() > 1)
            {
                // add in new item
                master.Add("UserPermissions-" + username);
                cache.Set<List<string>>("MasterUserPermissions", master, TimeSpan.FromHours(2));
            }

            return cache.GetOrCreate<List<int?>>(
                "UserPermissions-" + username,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);

                    // permission table links
                    // user (username) > user role links > user roles > role permissions links > role permissions (activity)

                    // if an admin, return all privaledges.
                    // ps, never check on the name, always the ID... someone could create a duplicate fake "admin" role and steal privaledges
                    var is_Admin = (
                        from r in _context.UserRoles
                        where
                            r.UserRoleLinks.Any(
                                x => x.User.Username == username && x.UserRolesId == 1
                            )
                        select new { r.Name }
                    ).Count();

                    if (is_Admin > 0)
                    {
                        var MyRole = GetMyRole(cache, _context, username);
                        if (MyRole != null)
                        {
                            if (
                                _context.UserRoles.Any(x => x.UserRolesId == MyRole.Id)
                                && MyRole.Id != 1
                            )
                            {
                                List<int?> roles = _context.RolePermissionLinks
                                    .Where(k => k.RoleId == MyRole.Id)
                                    .Select(x => x.RolePermissionsId)
                                    .ToList();
                                roles.Add(44);
                                return roles;
                            }
                        }
                        return _context.RolePermissions
                            .Select(x => (int?)x.RolePermissionsId)
                            .ToList();
                    }

                    // default role is 5 (user)
                    var RoleList = (
                        from r in _context.UserRoles
                        where r.UserRoleLinks.Any(x => x.User.Username == username)
                        select r.UserRolesId
                    ).ToList();

                    if (RoleList.Count() == 0)
                    {
                        RoleList = new List<int> { 5 };
                    }

                    var myPermissions = _context.RolePermissionLinks
                        .Where(k => RoleList.Contains(k.RoleId ?? 5))
                        .Select(x => x.RolePermissionsId)
                        .ToList();

                    return myPermissions;
                }
            );
        }

        public static Boolean CheckHrxPermissions(
            Atlas_WebContext _context,
            int ReportObjectId,
            string username
        )
        {
            username = username ?? "default";

            var report = _context.ReportObjects
                .Where(x => x.ReportObjectId == ReportObjectId)
                .FirstOrDefault();

            //Epic - Crystal Report = 3
            // Reporting Workbench Report = 17

            if (report != null && report.ReportObjectTypeId != 3 && report.ReportObjectTypeId != 17)
            {
                return true;
            }

            // check hrx

            var hrx = (
                from g in _context.ReportGroupsMemberships
                where
                    g.ReportId == ReportObjectId
                    && (
                        from ug in _context.UserGroupsMemberships
                        where ug.User.Username == username
                        select ug.GroupId
                    ).Contains(g.GroupId)
                select g.GroupId
            )
                .ToList()
                .Count();
            if (hrx >= 1)
            {
                return true;
            }

            var hrg = (
                from g in _context.ReportGroupsMemberships
                join h in _context.ReportObjectHierarchies.Where(
                    x => x.ChildReportObjectId == ReportObjectId
                )
                    on g.ReportId equals h.ParentReportObjectId
                where
                    (
                        from ug in _context.UserGroupsMemberships
                        where ug.User.Username == username
                        select ug.GroupId
                    ).Contains(g.GroupId)
                select g.GroupId
            )
                .ToList()
                .Count();
            if (hrg >= 1)
            {
                return true;
            }

            return false;
        }

        public static Boolean CheckUserPermissions(
            IMemoryCache cache,
            Atlas_WebContext _context,
            string username,
            int Access
        )
        {
            username = username ?? "default";
            return GetUserPermissions(cache, _context, username).Any(x => x.Value == Access);
            /*var myPermissions = (from r in _context.UserRoles
                                where (r.RolePermissionLinks.Any(x => x.RolePermissionsId == Access)
                                   && r.UserRoleLinks.Any(x => x.User.Username == username)
                                   ||  r.UserRoleLinks.Any(x => x.User.Username == username && x.UserRolesId == 1))
                                select new {
                                    r.Name
                                }).Count();

            return myPermissions > 0 ? true : false;
            */
        }
    }

    public class HtmlHelpers
    {
        public static string SiteMessage(HttpContext _httpContext, Atlas_WebContext _context)
        {
            string text = "";
            // site wide message
            GlobalSiteSetting msg = _context.GlobalSiteSettings
                .Where(x => x.Name == "msg" && x.Value == null)
                .FirstOrDefault();

            if (msg != null && msg.Description != null && msg.Description != "")
            {
                text += msg.Description;
            }

            if (_httpContext.Request.Query["msg"].ToString() != "")
            {
                // url message
                msg = _context.GlobalSiteSettings
                    .Where(
                        x =>
                            x.Name == "msg"
                            && x.Value == _httpContext.Request.Query["msg"].ToString()
                            && x.Value != null
                    )
                    .FirstOrDefault();

                if (msg != null && msg.Description != null && msg.Description != "")
                {
                    text += msg.Description;
                }
            }
            return text;
        }

        public static string MarkdownToHtml(string text)
        {
            if (text is null)
            {
                return "";
            }
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseEmojiAndSmiley()
                .UseSmartyPants()
                .UseDiagrams()
                .Build();
            return Markdown.ToHtml(text, pipeline);
        }

        public static string MilestoneFrequencyName(DpMilestoneTemplate content, string name)
        {
            if (name is null)
            {
                return "invalid";
            }
            else if (name == "One Time")
            {
                return name;
            }
            else if (content.Interval <= 0)
            {
                return "invalid";
            }

            return "Every " + content.Interval + " " + name;
            ;
        }

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
            string Domain,
            HttpContext Context,
            string EpicMasterFile,
            string EpicRecordId
        )
        {
            string Url = null;
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
            string EpicRecordId
        )
        {
            string Url = null;
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
            string SourceServer
        )
        {
            string Url = null;
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
            string Domain,
            HttpContext Context,
            ReportObject reportObject,
            Atlas_WebContext _context,
            string username
        )
        {
            if (reportObject == null)
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
