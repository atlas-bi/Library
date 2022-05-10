using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class AddIndexUserAnalytics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "user_access_load",
                schema: "app",
                table: "Analytics",
                columns: new[] { "UserId", "accessDateTime" })
                .Annotation("SqlServer:Include", new[] { "loadTime" });

            migrationBuilder.CreateIndex(
                name: "user_access_load_page_session",
                schema: "app",
                table: "Analytics",
                columns: new[] { "UserId", "accessDateTime" })
                .Annotation("SqlServer:Include", new[] { "loadTime", "pageId", "sessionId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "user_access_load",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "user_access_load_page_session",
                schema: "app",
                table: "Analytics");
        }
    }
}
