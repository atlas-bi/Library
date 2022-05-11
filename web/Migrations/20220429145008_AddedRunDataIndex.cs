using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class AddedRunDataIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "runuser + report",
                table: "ReportObjectRunData",
                newName: "runuser");

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectRunData_ReportObjectID_RunStartTime",
                table: "ReportObjectRunData",
                columns: new[] { "ReportObjectID", "RunStartTime" });

            migrationBuilder.CreateIndex(
                name: "reportid_status_run_user",
                table: "ReportObjectRunData",
                columns: new[] { "ReportObjectID", "RunStatus" })
                .Annotation("SqlServer:Include", new[] { "RunUserID", "RunStartTime", "RunDurationSeconds" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportObjectRunData_ReportObjectID_RunStartTime",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "reportid_status_run_user",
                table: "ReportObjectRunData");

            migrationBuilder.RenameIndex(
                name: "runuser",
                table: "ReportObjectRunData",
                newName: "runuser + report");
        }
    }
}
