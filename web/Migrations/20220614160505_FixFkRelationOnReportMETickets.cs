using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class FixFkRelationOnReportMETickets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportManageEngineTickets_ReportObject",
                schema: "app",
                table: "ReportManageEngineTickets");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportManageEngineTickets_ReportObject",
                schema: "app",
                table: "ReportManageEngineTickets",
                column: "ReportObjectId",
                principalSchema: "app",
                principalTable: "ReportObject_doc",
                principalColumn: "ReportObjectID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportManageEngineTickets_ReportObject",
                schema: "app",
                table: "ReportManageEngineTickets");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportManageEngineTickets_ReportObject",
                schema: "app",
                table: "ReportManageEngineTickets",
                column: "ReportObjectId",
                principalTable: "ReportObject",
                principalColumn: "ReportObjectID");
        }
    }
}
