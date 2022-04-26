using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class NewAnalyticsIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "accessdatetime_session",
                schema: "app",
                table: "Analytics",
                column: "accessDateTime")
                .Annotation("SqlServer:Include", new[] { "pageId", "sessionId" });

            migrationBuilder.CreateIndex(
                name: "accessdatetime_userid",
                schema: "app",
                table: "Analytics",
                columns: new[] { "accessDateTime", "UserId" })
                .Annotation("SqlServer:Include", new[] { "pathname", "loadTime" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "accessdatetime_session",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "accessdatetime_userid",
                schema: "app",
                table: "Analytics");
        }
    }
}
