using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class RenamedToCollectionTerm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "DP_TermAnnotation",
                schema: "app",
                newName: "CollectionTerm",
                newSchema: "app");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "CollectionTerm",
                schema: "app",
                newName: "DP_TermAnnotation",
                newSchema: "app");
        }
    }
}
