using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class RemovedUnusedCol_Analytics_CollectionAnnotations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Annotation",
                schema: "app",
                table: "CollectionTerm");

            migrationBuilder.DropColumn(
                name: "Annotation",
                schema: "app",
                table: "CollectionReport");

            migrationBuilder.DropColumn(
                name: "Username",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "appCodeName",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "appName",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "appVersion",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "cookieEnabled",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "host",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "oscpu",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "platform",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "sessionTime",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "title",
                schema: "app",
                table: "Analytics");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Annotation",
                schema: "app",
                table: "CollectionTerm",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Annotation",
                schema: "app",
                table: "CollectionReport",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                schema: "app",
                table: "Analytics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "appCodeName",
                schema: "app",
                table: "Analytics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "appName",
                schema: "app",
                table: "Analytics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "appVersion",
                schema: "app",
                table: "Analytics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cookieEnabled",
                schema: "app",
                table: "Analytics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "host",
                schema: "app",
                table: "Analytics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "oscpu",
                schema: "app",
                table: "Analytics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "platform",
                schema: "app",
                table: "Analytics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sessionTime",
                schema: "app",
                table: "Analytics",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title",
                schema: "app",
                table: "Analytics",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
