using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class AddIndexForProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "runstart_duration",
                table: "ReportObjectRunData",
                column: "RunStartTime")
                .Annotation("SqlServer:Include", new[] { "RunDurationSeconds" });

            migrationBuilder.CreateIndex(
                name: "runstart_report_duration",
                table: "ReportObjectRunData",
                column: "RunStartTime")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "RunDurationSeconds" });

            migrationBuilder.CreateIndex(
                name: "runstart_report_status",
                table: "ReportObjectRunData",
                column: "RunStartTime")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "RunStatus" });

            migrationBuilder.CreateIndex(
                name: "runstart_report_user",
                table: "ReportObjectRunData",
                column: "RunStartTime")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "RunUserID" });

            migrationBuilder.CreateIndex(
                name: "runstart_status",
                table: "ReportObjectRunData",
                column: "RunStartTime")
                .Annotation("SqlServer:Include", new[] { "RunStatus" });

            migrationBuilder.CreateIndex(
                name: "runstart_user",
                table: "ReportObjectRunData",
                column: "RunStartTime")
                .Annotation("SqlServer:Include", new[] { "RunUserID" });

            migrationBuilder.CreateIndex(
                name: "masterfile_report_type",
                table: "ReportObject",
                column: "EpicMasterFile")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "ReportObjectTypeID" });

            migrationBuilder.CreateIndex(
                name: "masterfile_report_visiblity",
                table: "ReportObject",
                column: "EpicMasterFile")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "DefaultVisibilityYN" });

            migrationBuilder.CreateIndex(
                name: "masterfile_sourceserver_type_report",
                table: "ReportObject",
                columns: new[] { "EpicMasterFile", "SourceServer", "ReportObjectTypeID" })
                .Annotation("SqlServer:Include", new[] { "ReportObjectID" });

            migrationBuilder.CreateIndex(
                name: "sourcedb_masterfile_report",
                table: "ReportObject",
                columns: new[] { "SourceDB", "EpicMasterFile" })
                .Annotation("SqlServer:Include", new[] { "ReportObjectID" });

            migrationBuilder.CreateIndex(
                name: "sourcedb_report",
                table: "ReportObject",
                column: "SourceDB")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID" });

            migrationBuilder.CreateIndex(
                name: "sourcedb_report_masterfile",
                table: "ReportObject",
                column: "SourceDB")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "EpicMasterFile" });

            migrationBuilder.CreateIndex(
                name: "sourceserver_masterfile_report",
                table: "ReportObject",
                columns: new[] { "SourceServer", "EpicMasterFile" })
                .Annotation("SqlServer:Include", new[] { "ReportObjectID" });

            migrationBuilder.CreateIndex(
                name: "sourceserver_report",
                table: "ReportObject",
                column: "SourceServer")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID" });

            migrationBuilder.CreateIndex(
                name: "sourceserver_report_masterfile",
                table: "ReportObject",
                column: "SourceServer")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "EpicMasterFile" });

            migrationBuilder.CreateIndex(
                name: "sourceserver_type_report",
                table: "ReportObject",
                columns: new[] { "SourceServer", "ReportObjectTypeID" })
                .Annotation("SqlServer:Include", new[] { "ReportObjectID" });

            migrationBuilder.CreateIndex(
                name: "type_report_cert",
                table: "ReportObject",
                columns: new[] { "ReportObjectTypeID", "EpicMasterFile" })
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "CertificationTag" });

            migrationBuilder.CreateIndex(
                name: "type_report_masterfile",
                table: "ReportObject",
                column: "ReportObjectTypeID")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "EpicMasterFile" });

            migrationBuilder.CreateIndex(
                name: "visibility_report_cert",
                table: "ReportObject",
                column: "DefaultVisibilityYN")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "CertificationTag" });

            migrationBuilder.CreateIndex(
                name: "visibility_report_masterfile",
                table: "ReportObject",
                column: "DefaultVisibilityYN")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "EpicMasterFile" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "runstart_duration",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstart_report_duration",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstart_report_status",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstart_report_user",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstart_status",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "runstart_user",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "masterfile_report_type",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "masterfile_report_visiblity",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "masterfile_sourceserver_type_report",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "sourcedb_masterfile_report",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "sourcedb_report",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "sourcedb_report_masterfile",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "sourceserver_masterfile_report",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "sourceserver_report",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "sourceserver_report_masterfile",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "sourceserver_type_report",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "type_report_cert",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "type_report_masterfile",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "visibility_report_cert",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "visibility_report_masterfile",
                table: "ReportObject");
        }
    }
}
