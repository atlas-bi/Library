using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class RemovedTermConvTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TermConversationMessage",
                schema: "app");

            migrationBuilder.DropTable(
                name: "TermConversation",
                schema: "app");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TermConversation",
                schema: "app",
                columns: table => new
                {
                    TermConversationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TermId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermConversation", x => x.TermConversationId);
                    table.ForeignKey(
                        name: "FK__TermConve__TermI__7C4F7684",
                        column: x => x.TermId,
                        principalSchema: "app",
                        principalTable: "Term",
                        principalColumn: "TermId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TermConversationMessage",
                schema: "app",
                columns: table => new
                {
                    TermConversationMessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TermConversationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    MessageText = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    PostDateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermConversationMessage", x => x.TermConversationMessageID);
                    table.ForeignKey(
                        name: "FK_TermConversationMessage_TermConversation",
                        column: x => x.TermConversationId,
                        principalSchema: "app",
                        principalTable: "TermConversation",
                        principalColumn: "TermConversationId");
                    table.ForeignKey(
                        name: "FK_TermConversationMessage_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TermConversation_TermId",
                schema: "app",
                table: "TermConversation",
                column: "TermId");

            migrationBuilder.CreateIndex(
                name: "IX_TermConversationMessage_TermConversationId",
                schema: "app",
                table: "TermConversationMessage",
                column: "TermConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_TermConversationMessage_UserId",
                schema: "app",
                table: "TermConversationMessage",
                column: "UserId");
        }
    }
}
