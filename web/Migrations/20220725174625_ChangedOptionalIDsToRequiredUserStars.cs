using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class ChangedOptionalIDsToRequiredUserStars : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            delete from app.StarredUsers
                where userid is null or ownerid is null
            ");

            migrationBuilder.Sql(@"
            delete from app.StarredTerms
            where ownerid is null or termid is null");

            migrationBuilder.Sql(@"
            delete from app.starredsearches
            where ownerid is null");

            migrationBuilder.Sql(@"
            delete from app.StarredReports
            where ownerid is null or reportid is null");

            migrationBuilder.Sql(@"
            delete from app.StarredCollections
            where ownerid is null or collectionid is null");

            migrationBuilder.Sql(@"
            delete from app.StarredGroups
            where ownerid is null or groupid is null");

            migrationBuilder.Sql(@"
            delete from app.StarredInitiatives
            where ownerid is null or initiativeid is null");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredCollections_User",
                schema: "app",
                table: "StarredCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredGroups_User",
                schema: "app",
                table: "StarredGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredGroups_UserGroups",
                schema: "app",
                table: "StarredGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredInitiatives_DP_DataInitiative",
                schema: "app",
                table: "StarredInitiatives");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredInitiatives_User",
                schema: "app",
                table: "StarredInitiatives");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredReports_ReportObject",
                schema: "app",
                table: "StarredReports");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredReports_User",
                schema: "app",
                table: "StarredReports");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredSearches_User",
                schema: "app",
                table: "StarredSearches");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredTerms_Term",
                schema: "app",
                table: "StarredTerms");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredTerms_User",
                schema: "app",
                table: "StarredTerms");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredUsers_User_owner",
                schema: "app",
                table: "StarredUsers");

            migrationBuilder.AlterColumn<int>(
                name: "userid",
                schema: "app",
                table: "StarredUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ownerid",
                schema: "app",
                table: "StarredUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "termid",
                schema: "app",
                table: "StarredTerms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ownerid",
                schema: "app",
                table: "StarredTerms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ownerid",
                schema: "app",
                table: "StarredSearches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "reportid",
                schema: "app",
                table: "StarredReports",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ownerid",
                schema: "app",
                table: "StarredReports",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ownerid",
                schema: "app",
                table: "StarredInitiatives",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "initiativeid",
                schema: "app",
                table: "StarredInitiatives",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ownerid",
                schema: "app",
                table: "StarredGroups",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "groupid",
                schema: "app",
                table: "StarredGroups",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ownerid",
                schema: "app",
                table: "StarredCollections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "collectionid",
                schema: "app",
                table: "StarredCollections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StarredCollections_DP_DataProject",
                schema: "app",
                table: "StarredCollections",
                column: "collectionid",
                principalSchema: "app",
                principalTable: "Collection",
                principalColumn: "DataProjectID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StarredCollections_User",
                schema: "app",
                table: "StarredCollections",
                column: "ownerid",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StarredGroups_User",
                schema: "app",
                table: "StarredGroups",
                column: "ownerid",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StarredGroups_UserGroups",
                schema: "app",
                table: "StarredGroups",
                column: "groupid",
                principalTable: "UserGroups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StarredInitiatives_DP_DataInitiative",
                schema: "app",
                table: "StarredInitiatives",
                column: "initiativeid",
                principalSchema: "app",
                principalTable: "Initiative",
                principalColumn: "DataInitiativeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StarredInitiatives_User",
                schema: "app",
                table: "StarredInitiatives",
                column: "ownerid",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StarredReports_ReportObject",
                schema: "app",
                table: "StarredReports",
                column: "reportid",
                principalTable: "ReportObject",
                principalColumn: "ReportObjectID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StarredReports_User",
                schema: "app",
                table: "StarredReports",
                column: "ownerid",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StarredSearches_User",
                schema: "app",
                table: "StarredSearches",
                column: "ownerid",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StarredTerms_Term",
                schema: "app",
                table: "StarredTerms",
                column: "termid",
                principalSchema: "app",
                principalTable: "Term",
                principalColumn: "TermId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StarredTerms_User",
                schema: "app",
                table: "StarredTerms",
                column: "ownerid",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StarredUsers_User_owner",
                schema: "app",
                table: "StarredUsers",
                column: "ownerid",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StarredCollections_DP_DataProject",
                schema: "app",
                table: "StarredCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredCollections_User",
                schema: "app",
                table: "StarredCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredGroups_User",
                schema: "app",
                table: "StarredGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredGroups_UserGroups",
                schema: "app",
                table: "StarredGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredInitiatives_DP_DataInitiative",
                schema: "app",
                table: "StarredInitiatives");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredInitiatives_User",
                schema: "app",
                table: "StarredInitiatives");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredReports_ReportObject",
                schema: "app",
                table: "StarredReports");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredReports_User",
                schema: "app",
                table: "StarredReports");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredSearches_User",
                schema: "app",
                table: "StarredSearches");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredTerms_Term",
                schema: "app",
                table: "StarredTerms");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredTerms_User",
                schema: "app",
                table: "StarredTerms");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredUsers_User_owner",
                schema: "app",
                table: "StarredUsers");

            migrationBuilder.AlterColumn<int>(
                name: "userid",
                schema: "app",
                table: "StarredUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ownerid",
                schema: "app",
                table: "StarredUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "termid",
                schema: "app",
                table: "StarredTerms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ownerid",
                schema: "app",
                table: "StarredTerms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ownerid",
                schema: "app",
                table: "StarredSearches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "reportid",
                schema: "app",
                table: "StarredReports",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ownerid",
                schema: "app",
                table: "StarredReports",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ownerid",
                schema: "app",
                table: "StarredInitiatives",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "initiativeid",
                schema: "app",
                table: "StarredInitiatives",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ownerid",
                schema: "app",
                table: "StarredGroups",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "groupid",
                schema: "app",
                table: "StarredGroups",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ownerid",
                schema: "app",
                table: "StarredCollections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "collectionid",
                schema: "app",
                table: "StarredCollections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_StarredCollections_DP_DataProject",
                schema: "app",
                table: "StarredCollections",
                column: "collectionid",
                principalSchema: "app",
                principalTable: "Collection",
                principalColumn: "DataProjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_StarredCollections_User",
                schema: "app",
                table: "StarredCollections",
                column: "ownerid",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_StarredGroups_User",
                schema: "app",
                table: "StarredGroups",
                column: "ownerid",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_StarredGroups_UserGroups",
                schema: "app",
                table: "StarredGroups",
                column: "groupid",
                principalTable: "UserGroups",
                principalColumn: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_StarredInitiatives_DP_DataInitiative",
                schema: "app",
                table: "StarredInitiatives",
                column: "initiativeid",
                principalSchema: "app",
                principalTable: "Initiative",
                principalColumn: "DataInitiativeID");

            migrationBuilder.AddForeignKey(
                name: "FK_StarredInitiatives_User",
                schema: "app",
                table: "StarredInitiatives",
                column: "ownerid",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_StarredReports_ReportObject",
                schema: "app",
                table: "StarredReports",
                column: "reportid",
                principalTable: "ReportObject",
                principalColumn: "ReportObjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_StarredReports_User",
                schema: "app",
                table: "StarredReports",
                column: "ownerid",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_StarredSearches_User",
                schema: "app",
                table: "StarredSearches",
                column: "ownerid",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_StarredTerms_Term",
                schema: "app",
                table: "StarredTerms",
                column: "termid",
                principalSchema: "app",
                principalTable: "Term",
                principalColumn: "TermId");

            migrationBuilder.AddForeignKey(
                name: "FK_StarredTerms_User",
                schema: "app",
                table: "StarredTerms",
                column: "ownerid",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_StarredUsers_User_owner",
                schema: "app",
                table: "StarredUsers",
                column: "ownerid",
                principalTable: "User",
                principalColumn: "UserID");
        }
    }
}
