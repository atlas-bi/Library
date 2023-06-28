using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    /// <inheritdoc />
    public partial class UserProfilePhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePhoto",
                schema: "dbo",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePhoto",
                schema: "dbo",
                table: "User");
        }
    }
}
