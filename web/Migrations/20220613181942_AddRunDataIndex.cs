using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class AddRunDataIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportObjectRunData_RunUserID",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstart_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstartday_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstarthour_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstartmonth_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstartyear_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.CreateIndex(
                name: "runstart_status_duration_user",
                schema: "dbo",
                table: "ReportObjectRunData",
                columns: new[] { "RunStatus", "RunStartTime" })
                .Annotation("SqlServer:Include", new[] { "RunDataId", "RunDurationSeconds", "RunUserID" });

            migrationBuilder.CreateIndex(
                name: "runstart_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData",
                columns: new[] { "RunUserID", "RunStartTime" })
                .Annotation("SqlServer:Include", new[] { "RunDataId", "RunDurationSeconds", "RunStatus" });

            migrationBuilder.CreateIndex(
                name: "runstartday_status_duration_user",
                schema: "dbo",
                table: "ReportObjectRunData",
                columns: new[] { "RunStatus", "RunStartTime_Day" })
                .Annotation("SqlServer:Include", new[] { "RunDataId", "RunDurationSeconds", "RunUserID" });

            migrationBuilder.CreateIndex(
                name: "runstartday_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData",
                columns: new[] { "RunUserID", "RunStartTime_Day" })
                .Annotation("SqlServer:Include", new[] { "RunDataId", "RunDurationSeconds", "RunStatus" });

            migrationBuilder.CreateIndex(
                name: "runstarthour_status_duration_user",
                schema: "dbo",
                table: "ReportObjectRunData",
                columns: new[] { "RunStatus", "RunStartTime_Hour" })
                .Annotation("SqlServer:Include", new[] { "RunDataId", "RunDurationSeconds", "RunUserID" });

            migrationBuilder.CreateIndex(
                name: "runstarthour_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData",
                columns: new[] { "RunUserID", "RunStartTime_Hour" })
                .Annotation("SqlServer:Include", new[] { "RunDataId", "RunDurationSeconds", "RunStatus" });

            migrationBuilder.CreateIndex(
                name: "runstartmonth_status_duration_user",
                schema: "dbo",
                table: "ReportObjectRunData",
                columns: new[] { "RunStatus", "RunStartTime_Month" })
                .Annotation("SqlServer:Include", new[] { "RunDataId", "RunDurationSeconds", "RunUserID" });

            migrationBuilder.CreateIndex(
                name: "runstartmonth_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData",
                columns: new[] { "RunUserID", "RunStartTime_Month" })
                .Annotation("SqlServer:Include", new[] { "RunDataId", "RunDurationSeconds", "RunStatus" });

            migrationBuilder.CreateIndex(
                name: "runstartyear_status_duration_user",
                schema: "dbo",
                table: "ReportObjectRunData",
                columns: new[] { "RunStatus", "RunStartTime_Year" })
                .Annotation("SqlServer:Include", new[] { "RunDataId", "RunDurationSeconds", "RunUserID" });

            migrationBuilder.CreateIndex(
                name: "runstartyear_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData",
                columns: new[] { "RunUserID", "RunStartTime_Year" })
                .Annotation("SqlServer:Include", new[] { "RunDataId", "RunDurationSeconds", "RunStatus" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "runstart_status_duration_user",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstart_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstartday_status_duration_user",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstartday_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstarthour_status_duration_user",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstarthour_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstartmonth_status_duration_user",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstartmonth_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstartyear_status_duration_user",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstartyear_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectRunData_RunUserID",
                schema: "dbo",
                table: "ReportObjectRunData",
                column: "RunUserID");

            migrationBuilder.CreateIndex(
                name: "runstart_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData",
                columns: new[] { "RunDataId", "RunStartTime" })
                .Annotation("SqlServer:Include", new[] { "RunUserID", "RunDurationSeconds", "RunStatus" });

            migrationBuilder.CreateIndex(
                name: "runstartday_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData",
                columns: new[] { "RunDataId", "RunStartTime_Day" })
                .Annotation("SqlServer:Include", new[] { "RunUserID", "RunDurationSeconds", "RunStatus" });

            migrationBuilder.CreateIndex(
                name: "runstarthour_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData",
                columns: new[] { "RunDataId", "RunStartTime_Hour" })
                .Annotation("SqlServer:Include", new[] { "RunUserID", "RunDurationSeconds", "RunStatus" });

            migrationBuilder.CreateIndex(
                name: "runstartmonth_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData",
                columns: new[] { "RunDataId", "RunStartTime_Month" })
                .Annotation("SqlServer:Include", new[] { "RunUserID", "RunDurationSeconds", "RunStatus" });

            migrationBuilder.CreateIndex(
                name: "runstartyear_user_duration_status",
                schema: "dbo",
                table: "ReportObjectRunData",
                columns: new[] { "RunDataId", "RunStartTime_Year" })
                .Annotation("SqlServer:Include", new[] { "RunUserID", "RunDurationSeconds", "RunStatus" });
        }
    }
}
