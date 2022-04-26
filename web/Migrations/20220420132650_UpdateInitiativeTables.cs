using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class UpdateInitiativeTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DP_AgreementUsers",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_Attachments",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_Contact_Links",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_Agreement",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_Contact",
                schema: "app");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DP_Agreement",
                schema: "app",
                columns: table => new
                {
                    AgreementID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataProjectId = table.Column<int>(type: "int", nullable: true),
                    LastUpdateUser = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    MeetingDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Rank = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_Agree__0A309D2318E98A83", x => x.AgreementID);
                    table.ForeignKey(
                        name: "FK_DP_Agreement_DP_DataProject",
                        column: x => x.DataProjectId,
                        principalSchema: "app",
                        principalTable: "DP_DataProject",
                        principalColumn: "DataProjectID");
                    table.ForeignKey(
                        name: "FK_DP_Agreement_WebAppUsers",
                        column: x => x.LastUpdateUser,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "DP_Attachments",
                schema: "app",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataProjectId = table.Column<int>(type: "int", nullable: false),
                    AttachmentData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    AttachmentName = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    AttachmentSize = table.Column<int>(type: "int", nullable: true),
                    AttachmentType = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_Attac__442C64BE2CE337F1", x => x.AttachmentId);
                    table.ForeignKey(
                        name: "FK_DP_Attachments_DP_DataProject",
                        column: x => x.DataProjectId,
                        principalSchema: "app",
                        principalTable: "DP_DataProject",
                        principalColumn: "DataProjectID");
                });

            migrationBuilder.CreateTable(
                name: "DP_Contact",
                schema: "app",
                columns: table => new
                {
                    ContactID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(55)", maxLength: 55, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_Conta__5C6625BB13B948C2", x => x.ContactID);
                });

            migrationBuilder.CreateTable(
                name: "DP_AgreementUsers",
                schema: "app",
                columns: table => new
                {
                    AgreementUsersID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgreementID = table.Column<int>(type: "int", nullable: true),
                    LastUpdateUser = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_Agree__3DA9AA218A1E6C55", x => x.AgreementUsersID);
                    table.ForeignKey(
                        name: "FK_DP_AgreementUsers_DP_Agreement",
                        column: x => x.AgreementID,
                        principalSchema: "app",
                        principalTable: "DP_Agreement",
                        principalColumn: "AgreementID");
                    table.ForeignKey(
                        name: "FK_DP_AgreementUsers_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_DP_AgreementUsers_WebAppUsers",
                        column: x => x.LastUpdateUser,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "DP_Contact_Links",
                schema: "app",
                columns: table => new
                {
                    LinkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    InitiativeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_Conta__2D1221358626B09F", x => x.LinkId);
                    table.ForeignKey(
                        name: "FK_DP_Contact_Links_DP_Contact",
                        column: x => x.ContactId,
                        principalSchema: "app",
                        principalTable: "DP_Contact",
                        principalColumn: "ContactID");
                    table.ForeignKey(
                        name: "FK_DP_Contact_Links_DP_DataInitiative",
                        column: x => x.InitiativeId,
                        principalSchema: "app",
                        principalTable: "DP_DataInitiative",
                        principalColumn: "DataInitiativeID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DP_Agreement_DataProjectId",
                schema: "app",
                table: "DP_Agreement",
                column: "DataProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DP_Agreement_LastUpdateUser",
                schema: "app",
                table: "DP_Agreement",
                column: "LastUpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_DP_AgreementUsers_AgreementID",
                schema: "app",
                table: "DP_AgreementUsers",
                column: "AgreementID");

            migrationBuilder.CreateIndex(
                name: "IX_DP_AgreementUsers_LastUpdateUser",
                schema: "app",
                table: "DP_AgreementUsers",
                column: "LastUpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_DP_AgreementUsers_UserId",
                schema: "app",
                table: "DP_AgreementUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DP_Attachments_DataProjectId",
                schema: "app",
                table: "DP_Attachments",
                column: "DataProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DP_Contact_Links_ContactId",
                schema: "app",
                table: "DP_Contact_Links",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_DP_Contact_Links_InitiativeId",
                schema: "app",
                table: "DP_Contact_Links",
                column: "InitiativeId");
        }
    }
}
