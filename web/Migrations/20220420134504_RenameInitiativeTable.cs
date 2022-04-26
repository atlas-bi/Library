using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class RenameInitiativeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "DP_DataInitiative",
                schema: "app",
                newName: "Initiative",
                newSchema: "app");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Initiative",
                schema: "app",
                newName: "DP_DataInitiative",
                newSchema: "app");
        }
    }
}
