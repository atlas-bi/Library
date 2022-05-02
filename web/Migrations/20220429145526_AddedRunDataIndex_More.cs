using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class AddedRunDataIndex_More : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "reportid_starttime_status",
                table: "ReportObjectRunData",
                columns: new[] { "ReportObjectID", "RunStartTime", "RunStatus" });

            migrationBuilder.CreateIndex(
                name: "reportid_starttime_user",
                table: "ReportObjectRunData",
                columns: new[] { "ReportObjectID", "RunStartTime" })
                .Annotation("SqlServer:Include", new[] { "RunUserID" });

            migrationBuilder.CreateIndex(
                name: "runstarttime",
                table: "ReportObjectRunData",
                column: "RunStartTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "reportid_starttime_status",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "reportid_starttime_user",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstarttime",
                table: "ReportObjectRunData");
        }
    }
}
