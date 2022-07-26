using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class ChangedOptionalIDsToRequiredMaintenanceLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            delete app.MaintenanceLog
            where maintainerid is null or ReportObjectId is null or MaintenanceLogStatusID is null
            ");

            migrationBuilder.DropForeignKey(
                name: "FK__ReportObj__MaintLog",
                schema: "app",
                table: "MaintenanceLog");

            migrationBuilder.AlterColumn<int>(
                name: "ReportObjectId",
                schema: "app",
                table: "MaintenanceLog",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaintainerID",
                schema: "app",
                table: "MaintenanceLog",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK__ReportObj__MaintLog",
                schema: "app",
                table: "MaintenanceLog",
                column: "ReportObjectId",
                principalSchema: "app",
                principalTable: "ReportObject_doc",
                principalColumn: "ReportObjectID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__ReportObj__MaintLog",
                schema: "app",
                table: "MaintenanceLog");

            migrationBuilder.AlterColumn<int>(
                name: "ReportObjectId",
                schema: "app",
                table: "MaintenanceLog",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MaintainerID",
                schema: "app",
                table: "MaintenanceLog",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK__ReportObj__MaintLog",
                schema: "app",
                table: "MaintenanceLog",
                column: "ReportObjectId",
                principalSchema: "app",
                principalTable: "ReportObject_doc",
                principalColumn: "ReportObjectID");
        }
    }
}
