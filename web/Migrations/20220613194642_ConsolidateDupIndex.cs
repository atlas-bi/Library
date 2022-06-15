using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class ConsolidateDupIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "defaultvisiblity",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "epicmasterfile",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "epicmasterfile + reportid",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "masterfile_report_type",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "masterfile_report_visiblity",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "sourcedb_report",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "sourceserver_report",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "type_report_masterfile",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "typeid",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "visibility_report_cert",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "visibility_report_masterfile",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "accessdatetime_screen",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "accessdatetime_session",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "accessdatetime_sessionId",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "accessdatetime_useragent",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "accessdatetime_userid",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "user_access_load",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "user_access_load_page_session",
                schema: "app",
                table: "Analytics");

            migrationBuilder.CreateIndex(
                name: "masterfile_report_visiblity_type",
                table: "ReportObject",
                column: "EpicMasterFile")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "DefaultVisibilityYN", "ReportObjectTypeID" });

            migrationBuilder.CreateIndex(
                name: "type_report_masterfile",
                table: "ReportObject",
                column: "ReportObjectTypeID")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "EpicMasterFile", "Name", "DisplayTitle" });

            migrationBuilder.CreateIndex(
                name: "visibility_report_cert_masterfile",
                table: "ReportObject",
                column: "DefaultVisibilityYN")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "CertificationTag", "EpicMasterFile" });

            migrationBuilder.CreateIndex(
                name: "accessdatetime_session_width_agent",
                schema: "app",
                table: "Analytics",
                column: "accessDateTime")
                .Annotation("SqlServer:Include", new[] { "pageId", "sessionId", "screenWidth", "userAgent" });

            migrationBuilder.CreateIndex(
                name: "user_access_load_page_session_path",
                schema: "app",
                table: "Analytics",
                columns: new[] { "UserId", "accessDateTime" })
                .Annotation("SqlServer:Include", new[] { "loadTime", "pageId", "sessionId", "pathname" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "masterfile_report_visiblity_type",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "type_report_masterfile",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "visibility_report_cert_masterfile",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "accessdatetime_session_width_agent",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "user_access_load_page_session_path",
                schema: "app",
                table: "Analytics");

            migrationBuilder.CreateIndex(
                name: "defaultvisiblity",
                table: "ReportObject",
                column: "DefaultVisibilityYN");

            migrationBuilder.CreateIndex(
                name: "epicmasterfile",
                table: "ReportObject",
                column: "EpicMasterFile");

            migrationBuilder.CreateIndex(
                name: "epicmasterfile + reportid",
                table: "ReportObject",
                column: "EpicMasterFile");

            migrationBuilder.CreateIndex(
                name: "masterfile_report_type",
                table: "ReportObject",
                column: "EpicMasterFile")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "ReportObjectTypeID" });

            migrationBuilder.CreateIndex(
                name: "masterfile_report_visiblity",
                table: "ReportObject",
                column: "EpicMasterFile")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "DefaultVisibilityYN" });

            migrationBuilder.CreateIndex(
                name: "sourcedb_report",
                table: "ReportObject",
                column: "SourceDB")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID" });

            migrationBuilder.CreateIndex(
                name: "sourceserver_report",
                table: "ReportObject",
                column: "SourceServer")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID" });

            migrationBuilder.CreateIndex(
                name: "type_report_masterfile",
                table: "ReportObject",
                column: "ReportObjectTypeID")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "EpicMasterFile" });

            migrationBuilder.CreateIndex(
                name: "typeid",
                table: "ReportObject",
                column: "ReportObjectTypeID");

            migrationBuilder.CreateIndex(
                name: "visibility_report_cert",
                table: "ReportObject",
                column: "DefaultVisibilityYN")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "CertificationTag" });

            migrationBuilder.CreateIndex(
                name: "visibility_report_masterfile",
                table: "ReportObject",
                column: "DefaultVisibilityYN")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "EpicMasterFile" });

            migrationBuilder.CreateIndex(
                name: "accessdatetime_screen",
                schema: "app",
                table: "Analytics",
                column: "accessDateTime")
                .Annotation("SqlServer:Include", new[] { "screenHeight", "screenWidth" });

            migrationBuilder.CreateIndex(
                name: "accessdatetime_session",
                schema: "app",
                table: "Analytics",
                column: "accessDateTime")
                .Annotation("SqlServer:Include", new[] { "pageId", "sessionId" });

            migrationBuilder.CreateIndex(
                name: "accessdatetime_sessionId",
                schema: "app",
                table: "Analytics",
                column: "accessDateTime")
                .Annotation("SqlServer:Include", new[] { "sessionId" });

            migrationBuilder.CreateIndex(
                name: "accessdatetime_useragent",
                schema: "app",
                table: "Analytics",
                column: "accessDateTime")
                .Annotation("SqlServer:Include", new[] { "userAgent" });

            migrationBuilder.CreateIndex(
                name: "accessdatetime_userid",
                schema: "app",
                table: "Analytics",
                columns: new[] { "accessDateTime", "UserId" })
                .Annotation("SqlServer:Include", new[] { "pathname", "loadTime" });

            migrationBuilder.CreateIndex(
                name: "user_access_load",
                schema: "app",
                table: "Analytics",
                columns: new[] { "UserId", "accessDateTime" })
                .Annotation("SqlServer:Include", new[] { "loadTime" });

            migrationBuilder.CreateIndex(
                name: "user_access_load_page_session",
                schema: "app",
                table: "Analytics",
                columns: new[] { "UserId", "accessDateTime" })
                .Annotation("SqlServer:Include", new[] { "loadTime", "pageId", "sessionId" });
        }
    }
}
