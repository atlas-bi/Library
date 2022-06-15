using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class RemovedRedundantBridgeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReportObjectId",
                schema: "app",
                table: "MaintenanceLog",
                type: "int",
                nullable: true);

            // move data into replacement table
            migrationBuilder.Sql("update l set l.ReportObjectID = d.ReportObjectID from app.ReportObjectDocMaintenanceLogs d inner join app.MaintenanceLog l on d.MaintenanceLogID = l.MaintenanceLogID");

            migrationBuilder.DropTable(
                name: "ReportObjectDocMaintenanceLogs",
                schema: "app");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceLog_ReportObjectId",
                schema: "app",
                table: "MaintenanceLog",
                column: "ReportObjectId");

            migrationBuilder.AddForeignKey(
                name: "FK__ReportObj__MaintLog",
                schema: "app",
                table: "MaintenanceLog",
                column: "ReportObjectId",
                principalSchema: "app",
                principalTable: "ReportObject_doc",
                principalColumn: "ReportObjectID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__ReportObj__MaintLog",
                schema: "app",
                table: "MaintenanceLog");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceLog_ReportObjectId",
                schema: "app",
                table: "MaintenanceLog");

            migrationBuilder.CreateTable(
                name: "ReportObjectDocMaintenanceLogs",
                schema: "app",
                columns: table => new
                {
                    LinkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenanceLogID = table.Column<int>(type: "int", nullable: false),
                    ReportObjectID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportOb__2D1221350C7530C5", x => x.LinkId);
                    table.ForeignKey(
                        name: "FK__ReportObj__Maint__6E1F2648",
                        column: x => x.MaintenanceLogID,
                        principalSchema: "app",
                        principalTable: "MaintenanceLog",
                        principalColumn: "MaintenanceLogID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__ReportObj__Repor__6F134A81",
                        column: x => x.ReportObjectID,
                        principalSchema: "app",
                        principalTable: "ReportObject_doc",
                        principalColumn: "ReportObjectID",
                        onDelete: ReferentialAction.Cascade);
                });

            // move data back into redundant table
            migrationBuilder.Sql("insert into app.ReportObjectDocMaintenanceLogs (reportobjectid, MaintenanceLogID) select reportobjectid, maintenancelogid from app.MaintenanceLog");

            migrationBuilder.DropColumn(
                name: "ReportObjectId",
                schema: "app",
                table: "MaintenanceLog");

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectDocMaintenanceLogs_MaintenanceLogID",
                schema: "app",
                table: "ReportObjectDocMaintenanceLogs",
                column: "MaintenanceLogID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectDocMaintenanceLogs_ReportObjectID",
                schema: "app",
                table: "ReportObjectDocMaintenanceLogs",
                column: "ReportObjectID");
        }
    }
}
