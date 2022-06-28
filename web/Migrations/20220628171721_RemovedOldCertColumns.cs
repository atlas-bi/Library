using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class RemovedOldCertColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "certid",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "type_report_cert",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "visibility_report_cert_masterfile",
                table: "ReportObject");

            migrationBuilder.DropColumn(
                name: "CertificationTag",
                table: "ReportObject");

            migrationBuilder.DropColumn(
                name: "CertificationTagID",
                table: "ReportObject");

            migrationBuilder.CreateIndex(
                name: "type_report",
                table: "ReportObject",
                columns: new[] { "ReportObjectTypeID", "EpicMasterFile" })
                .Annotation("SqlServer:Include", new[] { "ReportObjectID" });

            migrationBuilder.CreateIndex(
                name: "visibility_report_masterfile",
                table: "ReportObject",
                column: "DefaultVisibilityYN")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "EpicMasterFile" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "type_report",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "visibility_report_masterfile",
                table: "ReportObject");

            migrationBuilder.AddColumn<string>(
                name: "CertificationTag",
                table: "ReportObject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CertificationTagID",
                table: "ReportObject",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "certid",
                table: "ReportObject",
                column: "CertificationTagID");

            migrationBuilder.CreateIndex(
                name: "type_report_cert",
                table: "ReportObject",
                columns: new[] { "ReportObjectTypeID", "EpicMasterFile" })
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "CertificationTag" });

            migrationBuilder.CreateIndex(
                name: "visibility_report_cert_masterfile",
                table: "ReportObject",
                column: "DefaultVisibilityYN")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "CertificationTag", "EpicMasterFile" });
        }
    }
}
