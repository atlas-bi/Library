using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class AddedBridgeColumnToShoInheritiedRuns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Inherited",
                schema: "dbo",
                table: "ReportObjectRunDataBridge",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "reportid_inheritied_runid_runs",
                schema: "dbo",
                table: "ReportObjectRunDataBridge",
                columns: new[] { "ReportObjectId", "Inherited" })
                .Annotation("SqlServer:Include", new[] { "RunId", "Runs" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "reportid_inheritied_runid_runs",
                schema: "dbo",
                table: "ReportObjectRunDataBridge");

            migrationBuilder.DropColumn(
                name: "Inherited",
                schema: "dbo",
                table: "ReportObjectRunDataBridge");
        }
    }
}
