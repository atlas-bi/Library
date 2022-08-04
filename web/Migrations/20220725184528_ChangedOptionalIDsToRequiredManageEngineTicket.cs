using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class ChangedOptionalIDsToRequiredManageEngineTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            delete from app.ReportManageEngineTickets
            where reportobjectid is null
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportManageEngineTickets_ReportObject",
                schema: "app",
                table: "ReportManageEngineTickets");

            migrationBuilder.AlterColumn<int>(
                name: "ReportObjectId",
                schema: "app",
                table: "ReportManageEngineTickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportManageEngineTickets_ReportObject",
                schema: "app",
                table: "ReportManageEngineTickets",
                column: "ReportObjectId",
                principalSchema: "app",
                principalTable: "ReportObject_doc",
                principalColumn: "ReportObjectID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportManageEngineTickets_ReportObject",
                schema: "app",
                table: "ReportManageEngineTickets");

            migrationBuilder.AlterColumn<int>(
                name: "ReportObjectId",
                schema: "app",
                table: "ReportManageEngineTickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportManageEngineTickets_ReportObject",
                schema: "app",
                table: "ReportManageEngineTickets",
                column: "ReportObjectId",
                principalSchema: "app",
                principalTable: "ReportObject_doc",
                principalColumn: "ReportObjectID");
        }
    }
}
