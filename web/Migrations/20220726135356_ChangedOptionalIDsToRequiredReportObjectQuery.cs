using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class ChangedOptionalIDsToRequiredReportObjectQuery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete from dbo.reportobjectquery where reportobjectid is null");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportObjectQuery_ReportObject",
                schema: "dbo",
                table: "ReportObjectQuery");

            migrationBuilder.RenameColumn(
                name: "ExternalDocumentationURL",
                schema: "app",
                table: "Collection",
                newName: "ExternalDocumentationUrl");

            migrationBuilder.RenameColumn(
                name: "handled",
                schema: "app",
                table: "AnalyticsTrace",
                newName: "Handled");

            migrationBuilder.AlterColumn<int>(
                name: "ReportObjectId",
                schema: "dbo",
                table: "ReportObjectQuery",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportObjectQuery_ReportObject",
                schema: "dbo",
                table: "ReportObjectQuery",
                column: "ReportObjectId",
                principalSchema: "dbo",
                principalTable: "ReportObject",
                principalColumn: "ReportObjectID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportObjectQuery_ReportObject",
                schema: "dbo",
                table: "ReportObjectQuery");

            migrationBuilder.RenameColumn(
                name: "ExternalDocumentationUrl",
                schema: "app",
                table: "Collection",
                newName: "ExternalDocumentationURL");

            migrationBuilder.RenameColumn(
                name: "Handled",
                schema: "app",
                table: "AnalyticsTrace",
                newName: "handled");

            migrationBuilder.AlterColumn<int>(
                name: "ReportObjectId",
                schema: "dbo",
                table: "ReportObjectQuery",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportObjectQuery_ReportObject",
                schema: "dbo",
                table: "ReportObjectQuery",
                column: "ReportObjectId",
                principalSchema: "dbo",
                principalTable: "ReportObject",
                principalColumn: "ReportObjectID");
        }
    }
}
