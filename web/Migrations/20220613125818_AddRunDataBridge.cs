using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class AddRunDataBridge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__ReportObj__Repor__7B2284FD",
                table: "ReportObjectRunData");

            migrationBuilder.DropTable(
                name: "ReportObjectReportRunTime",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectRunTime",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectTopRuns",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectWeightedRunRank",
                schema: "app");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportObjectRunData",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "IX_ReportObjectRunData_ReportObjectID_RunStartTime",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "reportid_starttime_status",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "reportid_starttime_user",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "reportid_status_run_user",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "reportid6",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstart_duration",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstart_report_duration",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstart_report_status",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstart_report_user",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstart_status",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstart_user",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstarttime",
                table: "ReportObjectRunData");

            migrationBuilder.DropColumn(
                name: "ReportObjectID",
                table: "ReportObjectRunData");

            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "ReportObjectRunData",
                newName: "ReportObjectRunData",
                newSchema: "dbo");

            migrationBuilder.DropColumn(
                name: "RunID",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.AddColumn<int>(
                name: "RunId",
                schema: "dbo",
                table: "ReportObjectRunData",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.RenameIndex(
                name: "runuser",
                schema: "dbo",
                table: "ReportObjectRunData",
                newName: "IX_ReportObjectRunData_RunUserID");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RunStartTime",
                schema: "dbo",
                table: "ReportObjectRunData",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1753, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLoadDate",
                schema: "dbo",
                table: "ReportObjectRunData",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1753, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);



            migrationBuilder.AddColumn<string>(
                name: "RunDataId",
                schema: "dbo",
                table: "ReportObjectRunData",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RunStartTime_Day",
                schema: "dbo",
                table: "ReportObjectRunData",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1753, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RunStartTime_Hour",
                schema: "dbo",
                table: "ReportObjectRunData",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1753, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RunStartTime_Month",
                schema: "dbo",
                table: "ReportObjectRunData",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1753, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RunStartTime_Year",
                schema: "dbo",
                table: "ReportObjectRunData",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1753, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ReportObjectRunData_RunDataId",
                schema: "dbo",
                table: "ReportObjectRunData",
                column: "RunDataId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportObjectRunData",
                schema: "dbo",
                table: "ReportObjectRunData",
                column: "RunId");

            migrationBuilder.CreateTable(
                name: "ReportObjectRunDataBridge",
                schema: "dbo",
                columns: table => new
                {
                    BridgeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectId = table.Column<int>(type: "int", nullable: false),
                    RunId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Runs = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportOb__2D122135AFCD5E790", x => x.BridgeId);
                    table.ForeignKey(
                        name: "FK__ReportObj__Repor__7B2284FD",
                        column: x => x.ReportObjectId,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                    table.ForeignKey(
                        name: "FK__ReportObj__Run__6A4E9564",
                        column: x => x.RunId,
                        principalSchema: "dbo",
                        principalTable: "ReportObjectRunData",
                        principalColumn: "RunDataId");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectRunDataBridge_RunId",
                schema: "dbo",
                table: "ReportObjectRunDataBridge",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "reportid_runid_runs",
                schema: "dbo",
                table: "ReportObjectRunDataBridge",
                column: "ReportObjectId")
                .Annotation("SqlServer:Include", new[] { "RunId", "Runs" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportObjectRunDataBridge",
                schema: "dbo");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ReportObjectRunData_RunDataId",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportObjectRunData",
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

            migrationBuilder.DropColumn(
                name: "RunDataId",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropColumn(
                name: "RunStartTime_Day",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropColumn(
                name: "RunStartTime_Hour",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropColumn(
                name: "RunStartTime_Month",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.DropColumn(
                name: "RunStartTime_Year",
                schema: "dbo",
                table: "ReportObjectRunData");

            migrationBuilder.RenameTable(
                name: "ReportObjectRunData",
                schema: "dbo",
                newName: "ReportObjectRunData");

            migrationBuilder.DropColumn(
                name: "RunId",
                table: "ReportObjectRunData");

            migrationBuilder.RenameIndex(
                name: "IX_ReportObjectRunData_RunUserID",
                table: "ReportObjectRunData",
                newName: "runuser");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RunStartTime",
                table: "ReportObjectRunData",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLoadDate",
                table: "ReportObjectRunData",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<long>(
                name: "RunID",
                table: "ReportObjectRunData",
                type: "bigint",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "ReportObjectID",
                table: "ReportObjectRunData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportObjectRunData",
                table: "ReportObjectRunData",
                columns: new[] { "ReportObjectID", "RunID" });

            migrationBuilder.CreateTable(
                name: "ReportObjectReportRunTime",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectId = table.Column<int>(type: "int", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: true),
                    RunWeek = table.Column<DateTime>(type: "datetime", nullable: true),
                    RunWeekString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Runs = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportObjectReportRunTime", x => x.Id);
                    table.ForeignKey(
                        name: "fk_reportruntime",
                        column: x => x.ReportObjectId,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectRunTime",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RunUserId = table.Column<int>(type: "int", nullable: true),
                    RunTime = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    RunWeek = table.Column<DateTime>(type: "datetime", nullable: true),
                    RunWeekString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Runs = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportObjectRunTime", x => x.Id);
                    table.ForeignKey(
                        name: "fk_userruntime",
                        column: x => x.RunUserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectTopRuns",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectId = table.Column<int>(type: "int", nullable: true),
                    RunUserId = table.Column<int>(type: "int", nullable: true),
                    LastRun = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportObjectTypeId = table.Column<int>(type: "int", nullable: true),
                    RunTime = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Runs = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportObjectTopRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportObjectTopRuns_ReportObject",
                        column: x => x.ReportObjectId,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                    table.ForeignKey(
                        name: "fk_user",
                        column: x => x.RunUserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectWeightedRunRank",
                schema: "app",
                columns: table => new
                {
                    reportobjectid = table.Column<int>(type: "int", nullable: false),
                    weighted_run_rank = table.Column<decimal>(type: "numeric(12,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_ReportObjectWeightedRunRank_ReportObject",
                        column: x => x.reportobjectid,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectRunData_ReportObjectID_RunStartTime",
                table: "ReportObjectRunData",
                columns: new[] { "ReportObjectID", "RunStartTime" });

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
                name: "reportid_status_run_user",
                table: "ReportObjectRunData",
                columns: new[] { "ReportObjectID", "RunStatus" })
                .Annotation("SqlServer:Include", new[] { "RunUserID", "RunStartTime", "RunDurationSeconds" });

            migrationBuilder.CreateIndex(
                name: "reportid6",
                table: "ReportObjectRunData",
                column: "ReportObjectID");

            migrationBuilder.CreateIndex(
                name: "runstart_duration",
                table: "ReportObjectRunData",
                column: "RunStartTime")
                .Annotation("SqlServer:Include", new[] { "RunDurationSeconds" });

            migrationBuilder.CreateIndex(
                name: "runstart_report_duration",
                table: "ReportObjectRunData",
                column: "RunStartTime")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "RunDurationSeconds" });

            migrationBuilder.CreateIndex(
                name: "runstart_report_status",
                table: "ReportObjectRunData",
                column: "RunStartTime")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "RunStatus" });

            migrationBuilder.CreateIndex(
                name: "runstart_report_user",
                table: "ReportObjectRunData",
                column: "RunStartTime")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "RunUserID" });

            migrationBuilder.CreateIndex(
                name: "runstart_status",
                table: "ReportObjectRunData",
                column: "RunStartTime")
                .Annotation("SqlServer:Include", new[] { "RunStatus" });

            migrationBuilder.CreateIndex(
                name: "runstart_user",
                table: "ReportObjectRunData",
                column: "RunStartTime")
                .Annotation("SqlServer:Include", new[] { "RunUserID" });

            migrationBuilder.CreateIndex(
                name: "runstarttime",
                table: "ReportObjectRunData",
                column: "RunStartTime");

            migrationBuilder.CreateIndex(
                name: "reportid5",
                schema: "app",
                table: "ReportObjectReportRunTime",
                column: "ReportObjectId");

            migrationBuilder.CreateIndex(
                name: "userid1",
                schema: "app",
                table: "ReportObjectRunTime",
                column: "RunUserId");

            migrationBuilder.CreateIndex(
                name: "report+user+type",
                schema: "app",
                table: "ReportObjectTopRuns",
                column: "ReportObjectId");

            migrationBuilder.CreateIndex(
                name: "reportid + columns",
                schema: "app",
                table: "ReportObjectTopRuns",
                column: "ReportObjectId");

            migrationBuilder.CreateIndex(
                name: "userid + columns",
                schema: "app",
                table: "ReportObjectTopRuns",
                column: "RunUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectWeightedRunRank_reportobjectid",
                schema: "app",
                table: "ReportObjectWeightedRunRank",
                column: "reportobjectid");

            migrationBuilder.AddForeignKey(
                name: "FK__ReportObj__Repor__7B2284FD",
                table: "ReportObjectRunData",
                column: "ReportObjectID",
                principalTable: "ReportObject",
                principalColumn: "ReportObjectID");
        }
    }
}
