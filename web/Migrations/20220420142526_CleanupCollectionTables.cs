using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class CleanupCollectionTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DP_ReportAnnotation_DP_DataProject",
                schema: "app",
                table: "DP_ReportAnnotation");

            migrationBuilder.DropForeignKey(
                name: "FK_DP_TermAnnotation_DP_DataProject",
                schema: "app",
                table: "DP_TermAnnotation");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredCollections_DP_DataProject",
                schema: "app",
                table: "StarredCollections");

            migrationBuilder.DropTable(
                name: "Dp_DataProjectConversationMessage",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_MilestoneChecklist",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_MilestoneChecklistCompleted",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_MilestoneTasksCompleted",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Dp_DataProjectConversation",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_MilestoneTasks",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "DP_DataProject",
                schema: "app",
                newName: "Collection",
                newSchema: "app");

            migrationBuilder.DropTable(
                name: "DP_MilestoneTemplates",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_MilestoneFrequency",
                schema: "app");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DP_ReportAnnotation_DP_DataProject",
                schema: "app",
                table: "DP_ReportAnnotation");

            migrationBuilder.DropForeignKey(
                name: "FK_DP_TermAnnotation_DP_DataProject",
                schema: "app",
                table: "DP_TermAnnotation");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredCollections_DP_DataProject",
                schema: "app",
                table: "StarredCollections");


            migrationBuilder.RenameTable(
                name: "Collection",
                schema: "app",
                newName: "DP_DataProject",
                newSchema: "app");


            migrationBuilder.CreateTable(
                name: "DP_MilestoneFrequency",
                schema: "app",
                columns: table => new
                {
                    MilestoneTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateUser = table.Column<int>(type: "int", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_Miles__B88C49912ECA633F", x => x.MilestoneTypeId);
                    table.ForeignKey(
                        name: "FK_DP_MilestoneTypes_WebAppUsers",
                        column: x => x.LastUpdateUser,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Dp_DataProjectConversation",
                schema: "app",
                columns: table => new
                {
                    DataProjectConversationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Dp_DataP__A555F1EB255B0952", x => x.DataProjectConversationId);
                    table.ForeignKey(
                        name: "FK_Dp_DataProjectConversation_DP_DataProject",
                        column: x => x.DataProjectId,
                        principalSchema: "app",
                        principalTable: "DP_DataProject",
                        principalColumn: "DataProjectID");
                });

            migrationBuilder.CreateTable(
                name: "DP_MilestoneChecklistCompleted",
                schema: "app",
                columns: table => new
                {
                    MilestoneChecklistCompletedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompletionUser = table.Column<int>(type: "int", nullable: true),
                    DataProjectId = table.Column<int>(type: "int", nullable: true),
                    ChecklistStatus = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    CompletionDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    MilestoneChecklistId = table.Column<int>(type: "int", nullable: true),
                    TaskDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    TaskId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_Miles__E081AA701711E585", x => x.MilestoneChecklistCompletedId);
                    table.ForeignKey(
                        name: "FK_DP_MilestoneChecklistCompleted_DP_DataProject",
                        column: x => x.DataProjectId,
                        principalSchema: "app",
                        principalTable: "DP_DataProject",
                        principalColumn: "DataProjectID");
                    table.ForeignKey(
                        name: "FK_DP_MilestoneChecklistCompleted_User",
                        column: x => x.CompletionUser,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "DP_MilestoneTasksCompleted",
                schema: "app",
                columns: table => new
                {
                    MilestoneTaskCompletedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataProjectId = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CompletionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_Miles__3226EEDD7DAFACC3", x => x.MilestoneTaskCompletedId);
                    table.ForeignKey(
                        name: "FK_DP_MilestoneTasksCompleted_DP_DataProject",
                        column: x => x.DataProjectId,
                        principalSchema: "app",
                        principalTable: "DP_DataProject",
                        principalColumn: "DataProjectID");
                });

            migrationBuilder.CreateTable(
                name: "DP_MilestoneTemplates",
                schema: "app",
                columns: table => new
                {
                    MilestoneTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateUser = table.Column<int>(type: "int", nullable: true),
                    MilestoneTypeId = table.Column<int>(type: "int", nullable: true),
                    Interval = table.Column<int>(type: "int", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_Miles__6A72A86C4B768C43", x => x.MilestoneTemplateId);
                    table.ForeignKey(
                        name: "FK_DP_MilestoneTemplates_DP_MilestoneTypes",
                        column: x => x.MilestoneTypeId,
                        principalSchema: "app",
                        principalTable: "DP_MilestoneFrequency",
                        principalColumn: "MilestoneTypeId");
                    table.ForeignKey(
                        name: "FK_DP_MilestoneTemplates_WebAppUsers",
                        column: x => x.LastUpdateUser,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Dp_DataProjectConversationMessage",
                schema: "app",
                columns: table => new
                {
                    DataProjectConversationMessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataProjectConversationId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    MessageText = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PostDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Dp_DataP__06C6EA493F53AEBA", x => x.DataProjectConversationMessageId);
                    table.ForeignKey(
                        name: "FK_Dp_DataProjectConversationMessage_Dp_DataProjectConversation",
                        column: x => x.DataProjectConversationId,
                        principalSchema: "app",
                        principalTable: "Dp_DataProjectConversation",
                        principalColumn: "DataProjectConversationId");
                    table.ForeignKey(
                        name: "FK_Dp_DataProjectConversationMessage_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "DP_MilestoneTasks",
                schema: "app",
                columns: table => new
                {
                    MilestoneTaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataProjectId = table.Column<int>(type: "int", nullable: true),
                    LastUpdateUser = table.Column<int>(type: "int", nullable: true),
                    MilestoneTemplateId = table.Column<int>(type: "int", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_Miles__64647109FB6B4EDB", x => x.MilestoneTaskId);
                    table.ForeignKey(
                        name: "FK_DP_MilestoneTasks_DP_DataProject",
                        column: x => x.DataProjectId,
                        principalSchema: "app",
                        principalTable: "DP_DataProject",
                        principalColumn: "DataProjectID");
                    table.ForeignKey(
                        name: "FK_DP_MilestoneTasks_DP_MilestoneTemplates",
                        column: x => x.MilestoneTemplateId,
                        principalSchema: "app",
                        principalTable: "DP_MilestoneTemplates",
                        principalColumn: "MilestoneTemplateId");
                    table.ForeignKey(
                        name: "FK_DP_MilestoneTasks_LastUpdateUser",
                        column: x => x.LastUpdateUser,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_DP_MilestoneTasks_User",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "DP_MilestoneChecklist",
                schema: "app",
                columns: table => new
                {
                    MilestoneChecklistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MilestoneTaskId = table.Column<int>(type: "int", nullable: true),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_Miles__53ECAE4A5F858065", x => x.MilestoneChecklistId);
                    table.ForeignKey(
                        name: "FK_DP_MilestoneChecklist_DP_MilestoneTasks",
                        column: x => x.MilestoneTaskId,
                        principalSchema: "app",
                        principalTable: "DP_MilestoneTasks",
                        principalColumn: "MilestoneTaskId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dp_DataProjectConversation_DataProjectId",
                schema: "app",
                table: "Dp_DataProjectConversation",
                column: "DataProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Dp_DataProjectConversationMessage_DataProjectConversationId",
                schema: "app",
                table: "Dp_DataProjectConversationMessage",
                column: "DataProjectConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Dp_DataProjectConversationMessage_UserId",
                schema: "app",
                table: "Dp_DataProjectConversationMessage",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DP_MilestoneChecklist_MilestoneTaskId",
                schema: "app",
                table: "DP_MilestoneChecklist",
                column: "MilestoneTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_DP_MilestoneChecklistCompleted_CompletionUser",
                schema: "app",
                table: "DP_MilestoneChecklistCompleted",
                column: "CompletionUser");

            migrationBuilder.CreateIndex(
                name: "IX_DP_MilestoneChecklistCompleted_DataProjectId",
                schema: "app",
                table: "DP_MilestoneChecklistCompleted",
                column: "DataProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DP_MilestoneFrequency_LastUpdateUser",
                schema: "app",
                table: "DP_MilestoneFrequency",
                column: "LastUpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_DP_MilestoneTasks_DataProjectId",
                schema: "app",
                table: "DP_MilestoneTasks",
                column: "DataProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DP_MilestoneTasks_LastUpdateUser",
                schema: "app",
                table: "DP_MilestoneTasks",
                column: "LastUpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_DP_MilestoneTasks_MilestoneTemplateId",
                schema: "app",
                table: "DP_MilestoneTasks",
                column: "MilestoneTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_DP_MilestoneTasks_OwnerId",
                schema: "app",
                table: "DP_MilestoneTasks",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_DP_MilestoneTasksCompleted_DataProjectId",
                schema: "app",
                table: "DP_MilestoneTasksCompleted",
                column: "DataProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DP_MilestoneTemplates_LastUpdateUser",
                schema: "app",
                table: "DP_MilestoneTemplates",
                column: "LastUpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_DP_MilestoneTemplates_MilestoneTypeId",
                schema: "app",
                table: "DP_MilestoneTemplates",
                column: "MilestoneTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DP_ReportAnnotation_DP_DataProject",
                schema: "app",
                table: "DP_ReportAnnotation",
                column: "DataProjectId",
                principalSchema: "app",
                principalTable: "DP_DataProject",
                principalColumn: "DataProjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_DP_TermAnnotation_DP_DataProject",
                schema: "app",
                table: "DP_TermAnnotation",
                column: "DataProjectId",
                principalSchema: "app",
                principalTable: "DP_DataProject",
                principalColumn: "DataProjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_StarredCollections_DP_DataProject",
                schema: "app",
                table: "StarredCollections",
                column: "collectionid",
                principalSchema: "app",
                principalTable: "DP_DataProject",
                principalColumn: "DataProjectID");
        }
    }
}
