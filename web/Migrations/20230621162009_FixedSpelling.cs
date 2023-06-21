using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    /// <inheritdoc />
    public partial class FixedSpelling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Referer",
                schema: "app",
                table: "AnalyticsError",
                newName: "Referrer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Referrer",
                schema: "app",
                table: "AnalyticsError",
                newName: "Referer");
        }
    }
}
