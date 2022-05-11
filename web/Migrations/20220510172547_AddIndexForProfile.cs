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
                name: "IX_ReportObjectRunData_RunStartTime",
                table: "ReportObjectRunData",
                column: "RunStartTime")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "RunUserID" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportObject_DefaultVisibilityYN",
                table: "ReportObject",
                column: "DefaultVisibilityYN")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "EpicMasterFile" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportObject_EpicMasterFile",
                table: "ReportObject",
                column: "EpicMasterFile")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "ReportObjectTypeID" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportObject_EpicMasterFile_SourceServer_ReportObjectTypeID",
                table: "ReportObject",
                columns: new[] { "EpicMasterFile", "SourceServer", "ReportObjectTypeID" })
                .Annotation("SqlServer:Include", new[] { "ReportObjectID" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportObject_ReportObjectTypeID",
                table: "ReportObject",
                column: "ReportObjectTypeID")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "EpicMasterFile" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportObject_ReportObjectTypeID_EpicMasterFile",
                table: "ReportObject",
                columns: new[] { "ReportObjectTypeID", "EpicMasterFile" })
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "CertificationTag" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportObject_SourceDB",
                table: "ReportObject",
                column: "SourceDB")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "EpicMasterFile" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportObject_SourceDB_EpicMasterFile",
                table: "ReportObject",
                columns: new[] { "SourceDB", "EpicMasterFile" })
                .Annotation("SqlServer:Include", new[] { "ReportObjectID" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportObject_SourceServer",
                table: "ReportObject",
                column: "SourceServer")
                .Annotation("SqlServer:Include", new[] { "ReportObjectID", "EpicMasterFile" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportObject_SourceServer_EpicMasterFile",
                table: "ReportObject",
                columns: new[] { "SourceServer", "EpicMasterFile" })
                .Annotation("SqlServer:Include", new[] { "ReportObjectID" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportObject_SourceServer_ReportObjectTypeID",
                table: "ReportObject",
                columns: new[] { "SourceServer", "ReportObjectTypeID" })
                .Annotation("SqlServer:Include", new[] { "ReportObjectID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReportObjectRunData_RunStartTime",
                table: "ReportObjectRunData");

            migrationBuilder.DropIndex(
                name: "IX_ReportObject_DefaultVisibilityYN",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "IX_ReportObject_EpicMasterFile",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "IX_ReportObject_EpicMasterFile_SourceServer_ReportObjectTypeID",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "IX_ReportObject_ReportObjectTypeID",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "IX_ReportObject_ReportObjectTypeID_EpicMasterFile",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "IX_ReportObject_SourceDB",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "IX_ReportObject_SourceDB_EpicMasterFile",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "IX_ReportObject_SourceServer",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "IX_ReportObject_SourceServer_EpicMasterFile",
                table: "ReportObject");

            migrationBuilder.DropIndex(
                name: "IX_ReportObject_SourceServer_ReportObjectTypeID",
                table: "ReportObject");
        }
    }
}
