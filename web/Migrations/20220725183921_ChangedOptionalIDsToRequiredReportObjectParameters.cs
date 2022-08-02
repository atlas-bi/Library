using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class ChangedOptionalIDsToRequiredReportObjectParameters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            delete from dbo.ReportObjectParameters
            where reportobjectid is null
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportObjectParameters1_ReportObject",
                table: "ReportObjectParameters");

            migrationBuilder.AlterColumn<int>(
                name: "ReportObjectID",
                table: "ReportObjectParameters",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportObjectParameters1_ReportObject",
                table: "ReportObjectParameters",
                column: "ReportObjectID",
                principalTable: "ReportObject",
                principalColumn: "ReportObjectID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportObjectParameters1_ReportObject",
                table: "ReportObjectParameters");

            migrationBuilder.AlterColumn<int>(
                name: "ReportObjectID",
                table: "ReportObjectParameters",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportObjectParameters1_ReportObject",
                table: "ReportObjectParameters",
                column: "ReportObjectID",
                principalTable: "ReportObject",
                principalColumn: "ReportObjectID");
        }
    }
}
