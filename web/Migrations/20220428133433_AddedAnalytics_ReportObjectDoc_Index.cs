using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class AddedAnalytics_ReportObjectDoc_Index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "execvis_reportid",
                schema: "app",
                table: "ReportObject_doc",
                column: "ExecutiveVisibilityYN")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID" });

            migrationBuilder.CreateIndex(
                name: "maintschedule_report_updated",
                schema: "app",
                table: "ReportObject_doc",
                column: "MaintenanceScheduleID")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "LastUpdateDateTime", "UpdatedBy" });

            migrationBuilder.CreateIndex(
                name: "maintschedule_report_updated_created",
                schema: "app",
                table: "ReportObject_doc",
                column: "MaintenanceScheduleID")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "LastUpdateDateTime", "UpdatedBy", "CreatedDateTime" });

            migrationBuilder.CreateIndex(
                name: "accessdatetime_screen",
                schema: "app",
                table: "Analytics",
                column: "accessDateTime")
                .Annotation("SqlServer:Include", new[] { "screenHeight", "screenWidth" });

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "execvis_reportid",
                schema: "app",
                table: "ReportObject_doc");

            migrationBuilder.DropIndex(
                name: "maintschedule_report_updated",
                schema: "app",
                table: "ReportObject_doc");

            migrationBuilder.DropIndex(
                name: "maintschedule_report_updated_created",
                schema: "app",
                table: "ReportObject_doc");

            migrationBuilder.DropIndex(
                name: "accessdatetime_screen",
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
        }
    }
}
