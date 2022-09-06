using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class RenamedDupeKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__ReportObj__Repor__72E3DB65",
                schema: "app",
                table: "ReportObjectDocFragilityTags");

            migrationBuilder.AddForeignKey(
                name: "FK__ReportObj__Repor__72E3DB67",
                schema: "app",
                table: "ReportObjectDocFragilityTags",
                column: "ReportObjectId",
                principalSchema: "app",
                principalTable: "ReportObject_doc",
                principalColumn: "ReportObjectID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__ReportObj__Repor__72E3DB67",
                schema: "app",
                table: "ReportObjectDocFragilityTags");

            migrationBuilder.AddForeignKey(
                name: "FK__ReportObj__Repor__72E3DB65",
                schema: "app",
                table: "ReportObjectDocFragilityTags",
                column: "ReportObjectId",
                principalSchema: "app",
                principalTable: "ReportObject_doc",
                principalColumn: "ReportObjectID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
