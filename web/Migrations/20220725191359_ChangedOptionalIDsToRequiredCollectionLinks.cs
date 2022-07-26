using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class ChangedOptionalIDsToRequiredCollectionLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete from app.CollectionTerm
            where termid is null");

            migrationBuilder.Sql(@"delete from app.CollectionReport
            where reportid is null");

            migrationBuilder.DropForeignKey(
                name: "FK_DP_ReportAnnotation_ReportObject",
                schema: "app",
                table: "CollectionReport");

            // fix index name
            migrationBuilder.RenameIndex(
                name: "IX_DP_TermAnnotation_DataProjectId",
                newName: "IX_CollectionTerm_DataProjectId",
                table: "CollectionTerm",
                schema: "app"
            );

            // fix index name
            migrationBuilder.RenameIndex(
                name: "IX_DP_ReportAnnotation_DataProjectId",
                newName: "IX_CollectionReport_DataProjectId",
                table: "CollectionReport",
                schema: "app"
            );


            migrationBuilder.DropForeignKey(
                name: "FK_DP_TermAnnotation_Term",
                schema: "app",
                table: "CollectionTerm");

            migrationBuilder.AlterColumn<int>(
                name: "TermId",
                schema: "app",
                table: "CollectionTerm",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DataProjectId",
                schema: "app",
                table: "CollectionTerm",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReportId",
                schema: "app",
                table: "CollectionReport",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DataProjectId",
                schema: "app",
                table: "CollectionReport",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DP_ReportAnnotation_DP_DataProject",
                schema: "app",
                table: "CollectionReport",
                column: "DataProjectId",
                principalSchema: "app",
                principalTable: "Collection",
                principalColumn: "DataProjectID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DP_ReportAnnotation_ReportObject",
                schema: "app",
                table: "CollectionReport",
                column: "ReportId",
                principalTable: "ReportObject",
                principalColumn: "ReportObjectID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DP_TermAnnotation_DP_DataProject",
                schema: "app",
                table: "CollectionTerm",
                column: "DataProjectId",
                principalSchema: "app",
                principalTable: "Collection",
                principalColumn: "DataProjectID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DP_TermAnnotation_Term",
                schema: "app",
                table: "CollectionTerm",
                column: "TermId",
                principalSchema: "app",
                principalTable: "Term",
                principalColumn: "TermId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DP_ReportAnnotation_DP_DataProject",
                schema: "app",
                table: "CollectionReport");

            migrationBuilder.DropForeignKey(
                name: "FK_DP_ReportAnnotation_ReportObject",
                schema: "app",
                table: "CollectionReport");

            migrationBuilder.DropForeignKey(
                name: "FK_DP_TermAnnotation_DP_DataProject",
                schema: "app",
                table: "CollectionTerm");

            migrationBuilder.DropForeignKey(
                name: "FK_DP_TermAnnotation_Term",
                schema: "app",
                table: "CollectionTerm");

            migrationBuilder.AlterColumn<int>(
                name: "TermId",
                schema: "app",
                table: "CollectionTerm",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DataProjectId",
                schema: "app",
                table: "CollectionTerm",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ReportId",
                schema: "app",
                table: "CollectionReport",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DataProjectId",
                schema: "app",
                table: "CollectionReport",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_DP_ReportAnnotation_DP_DataProject",
                schema: "app",
                table: "CollectionReport",
                column: "DataProjectId",
                principalSchema: "app",
                principalTable: "Collection",
                principalColumn: "DataProjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_DP_ReportAnnotation_ReportObject",
                schema: "app",
                table: "CollectionReport",
                column: "ReportId",
                principalTable: "ReportObject",
                principalColumn: "ReportObjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_DP_TermAnnotation_DP_DataProject",
                schema: "app",
                table: "CollectionTerm",
                column: "DataProjectId",
                principalSchema: "app",
                principalTable: "Collection",
                principalColumn: "DataProjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_DP_TermAnnotation_Term",
                schema: "app",
                table: "CollectionTerm",
                column: "TermId",
                principalSchema: "app",
                principalTable: "Term",
                principalColumn: "TermId");
        }
    }
}
