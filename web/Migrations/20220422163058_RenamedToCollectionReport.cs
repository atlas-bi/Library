using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class RenamedToCollectionReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "DP_ReportAnnotation",
                schema: "app",
                newName: "CollectionReport",
                newSchema: "app");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "CollectionReport",
                schema: "app",
                newName: "DP_ReportAnnotation",
                newSchema: "app");
        }
    }
}
