using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class RemoveUnusedSearchTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Search_BasicSearchData",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Search_BasicSearchData_Small",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Search_ReportObjectSearchData",
                schema: "app");

            migrationBuilder.DropTable(
                name: "SearchTable",
                schema: "app");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Search_BasicSearchData",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CertificationTag = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Hidden = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    ItemRank = table.Column<int>(type: "int", nullable: true),
                    ItemType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Orphaned = table.Column<int>(type: "int", nullable: true),
                    SearchField = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SearchFieldDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: true),
                    VisibleType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Search_BasicSearchData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Search_BasicSearchData_Small",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CertificationTag = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Hidden = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    ItemRank = table.Column<int>(type: "int", nullable: true),
                    ItemType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Orphaned = table.Column<int>(type: "int", nullable: true),
                    SearchField = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SearchFieldDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: true),
                    VisibleType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Search_BasicSearchData_Small", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Search_ReportObjectSearchData",
                schema: "app",
                columns: table => new
                {
                    pk = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorUserId = table.Column<int>(type: "int", nullable: true),
                    certificationTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColumnName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultVisibilityYN = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    DocCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    DocCreatedBy = table.Column<int>(type: "int", nullable: true),
                    DocDoNotPurge = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    DocExecVis = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    DocFragId = table.Column<int>(type: "int", nullable: true),
                    DocHidden = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    DocHypeEnabled = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    DocLastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    DocMainSchedId = table.Column<int>(type: "int", nullable: true),
                    DocOrgValueId = table.Column<int>(type: "int", nullable: true),
                    DocOwnerId = table.Column<int>(type: "int", nullable: true),
                    DocRequesterId = table.Column<int>(type: "int", nullable: true),
                    DocRunFreqId = table.Column<int>(type: "int", nullable: true),
                    DocUpdatedBy = table.Column<int>(type: "int", nullable: true),
                    Documented = table.Column<int>(type: "int", nullable: false),
                    EpicMasterFile = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    EpicReportTemplateId = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    Id = table.Column<int>(type: "int", nullable: false),
                    LastModifiedByUserID = table.Column<int>(type: "int", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    OneMonthRuns = table.Column<int>(type: "int", nullable: true),
                    OneYearRuns = table.Column<int>(type: "int", nullable: true),
                    OrphanedReportObjectYN = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    ReportObjectTypeID = table.Column<int>(type: "int", nullable: true),
                    SixMonthsRuns = table.Column<int>(type: "int", nullable: true),
                    SourceDB = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SourceServer = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SourceTable = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TwoYearRuns = table.Column<int>(type: "int", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Search_ReportObjectSearchData_PK", x => x.pk);
                });

            migrationBuilder.CreateTable(
                name: "SearchTable",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    ItemRank = table.Column<int>(type: "int", nullable: true),
                    ItemType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SearchField = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SearchFieldDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchTable", x => x.Id);
                });
        }
    }
}
