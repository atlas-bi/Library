using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class RemovedReportConvTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportObjectConversationMessage_doc",
                schema: "app");

            migrationBuilder.DropTable(
                name: "User_NameData",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectConversation_doc",
                schema: "app");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportObjectConversation_doc",
                schema: "app",
                columns: table => new
                {
                    ConversationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportOb__C050D8972E11C321", x => x.ConversationID);
                    table.ForeignKey(
                        name: "FK__ReportObj__Repor__3B21036F",
                        column: x => x.ReportObjectID,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                });

            migrationBuilder.CreateTable(
                name: "User_NameData",
                schema: "app",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fullname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_Nam__1788CC4CF297F9FC", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_User_NameData_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectConversationMessage_doc",
                schema: "app",
                columns: table => new
                {
                    MessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    MessageText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportOb__C87C037C3C86464B", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK__ReportObj__Conve__4E53A1AA",
                        column: x => x.ConversationID,
                        principalSchema: "app",
                        principalTable: "ReportObjectConversation_doc",
                        principalColumn: "ConversationID");
                    table.ForeignKey(
                        name: "FK_ReportObjectConversationMessage_doc_User",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectConversation_doc_ReportObjectID",
                schema: "app",
                table: "ReportObjectConversation_doc",
                column: "ReportObjectID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectConversationMessage_doc_ConversationID",
                schema: "app",
                table: "ReportObjectConversationMessage_doc",
                column: "ConversationID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectConversationMessage_doc_UserID",
                schema: "app",
                table: "ReportObjectConversationMessage_doc",
                column: "UserID");
        }
    }
}
