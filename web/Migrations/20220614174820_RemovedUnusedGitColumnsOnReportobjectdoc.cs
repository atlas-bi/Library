using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class RemovedUnusedGitColumnsOnReportobjectdoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GitLabBlobURL",
                schema: "app",
                table: "ReportObject_doc");

            migrationBuilder.DropColumn(
                name: "GitLabTreeURL",
                schema: "app",
                table: "ReportObject_doc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GitLabBlobURL",
                schema: "app",
                table: "ReportObject_doc",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitLabTreeURL",
                schema: "app",
                table: "ReportObject_doc",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
