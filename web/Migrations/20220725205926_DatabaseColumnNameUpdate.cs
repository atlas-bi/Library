using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class DatabaseColumnNameUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportServiceRequests",
                schema: "app",
                columns: table => new
                {
                    ServiceRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportObjectId = table.Column<int>(type: "int", nullable: false),
                    TicketUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportMa__ServiceRequestId", x => x.ServiceRequestId);
                    table.ForeignKey(
                        name: "FK_ReportServiceRequests_ReportObject",
                        column: x => x.ReportObjectId,
                        principalSchema: "app",
                        principalTable: "ReportObject_doc",
                        principalColumn: "ReportObjectID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"
            insert into app.ReportServiceRequests  (ticketnumber,description,reportobjectid,ticketurl)
            select ticketnumber,description,reportobjectid,ticketurl from app.ReportManageEngineTickets
            ");

            migrationBuilder.DropTable(
                name: "ReportManageEngineTickets",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "accessdatetime_session_width_agent",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "user_access_load_page_session_path",
                schema: "app",
                table: "Analytics");

            migrationBuilder.RenameTable(
                name: "UserGroupsMembership",
                newName: "UserGroupsMembership",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "UserGroups",
                newName: "UserGroups",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "User",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "Tags",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ReportTagLinks",
                newName: "ReportTagLinks",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ReportObjectType",
                newName: "ReportObjectType",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ReportObjectTags",
                newName: "ReportObjectTags",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ReportObjectTagMemberships",
                newName: "ReportObjectTagMemberships",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ReportObjectSubscriptions",
                newName: "ReportObjectSubscriptions",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ReportObjectQuery",
                newName: "ReportObjectQuery",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ReportObjectParameters",
                newName: "ReportObjectParameters",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ReportObjectHierarchy",
                newName: "ReportObjectHierarchy",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ReportObjectAttachments",
                newName: "ReportObjectAttachments",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ReportObject",
                newName: "ReportObject",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ReportGroupsMemberships",
                newName: "ReportGroupsMemberships",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "StrategicImportanceId",
                schema: "app",
                table: "StrategicImportance",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "strategicimportanceid",
                schema: "app",
                table: "StrategicImportance",
                newName: "IX_StrategicImportance_Id");

            migrationBuilder.RenameColumn(
                name: "userid",
                schema: "app",
                table: "StarredUsers",
                newName: "Userid");

            migrationBuilder.RenameColumn(
                name: "rank",
                schema: "app",
                table: "StarredUsers",
                newName: "Rank");

            migrationBuilder.RenameColumn(
                name: "ownerid",
                schema: "app",
                table: "StarredUsers",
                newName: "Ownerid");

            migrationBuilder.RenameColumn(
                name: "folderid",
                schema: "app",
                table: "StarredUsers",
                newName: "Folderid");

            migrationBuilder.RenameColumn(
                name: "star_id",
                schema: "app",
                table: "StarredUsers",
                newName: "StarId");

            migrationBuilder.RenameIndex(
                name: "IX_StarredUsers_ownerid",
                schema: "app",
                table: "StarredUsers",
                newName: "IX_StarredUsers_Ownerid");

            migrationBuilder.RenameColumn(
                name: "termid",
                schema: "app",
                table: "StarredTerms",
                newName: "Termid");

            migrationBuilder.RenameColumn(
                name: "rank",
                schema: "app",
                table: "StarredTerms",
                newName: "Rank");

            migrationBuilder.RenameColumn(
                name: "ownerid",
                schema: "app",
                table: "StarredTerms",
                newName: "Ownerid");

            migrationBuilder.RenameColumn(
                name: "folderid",
                schema: "app",
                table: "StarredTerms",
                newName: "Folderid");

            migrationBuilder.RenameColumn(
                name: "star_id",
                schema: "app",
                table: "StarredTerms",
                newName: "StarId");

            migrationBuilder.RenameIndex(
                name: "IX_StarredTerms_ownerid",
                schema: "app",
                table: "StarredTerms",
                newName: "IX_StarredTerms_Ownerid");

            migrationBuilder.RenameColumn(
                name: "search",
                schema: "app",
                table: "StarredSearches",
                newName: "Search");

            migrationBuilder.RenameColumn(
                name: "rank",
                schema: "app",
                table: "StarredSearches",
                newName: "Rank");

            migrationBuilder.RenameColumn(
                name: "ownerid",
                schema: "app",
                table: "StarredSearches",
                newName: "Ownerid");

            migrationBuilder.RenameColumn(
                name: "folderid",
                schema: "app",
                table: "StarredSearches",
                newName: "Folderid");

            migrationBuilder.RenameColumn(
                name: "star_id",
                schema: "app",
                table: "StarredSearches",
                newName: "StarId");

            migrationBuilder.RenameIndex(
                name: "IX_StarredSearches_folderid",
                schema: "app",
                table: "StarredSearches",
                newName: "IX_StarredSearches_Folderid");

            migrationBuilder.RenameColumn(
                name: "reportid",
                schema: "app",
                table: "StarredReports",
                newName: "Reportid");

            migrationBuilder.RenameColumn(
                name: "rank",
                schema: "app",
                table: "StarredReports",
                newName: "Rank");

            migrationBuilder.RenameColumn(
                name: "ownerid",
                schema: "app",
                table: "StarredReports",
                newName: "Ownerid");

            migrationBuilder.RenameColumn(
                name: "folderid",
                schema: "app",
                table: "StarredReports",
                newName: "Folderid");

            migrationBuilder.RenameColumn(
                name: "star_id",
                schema: "app",
                table: "StarredReports",
                newName: "StarId");

            migrationBuilder.RenameIndex(
                name: "IX_StarredReports_ownerid",
                schema: "app",
                table: "StarredReports",
                newName: "IX_StarredReports_Ownerid");

            migrationBuilder.RenameColumn(
                name: "rank",
                schema: "app",
                table: "StarredInitiatives",
                newName: "Rank");

            migrationBuilder.RenameColumn(
                name: "ownerid",
                schema: "app",
                table: "StarredInitiatives",
                newName: "Ownerid");

            migrationBuilder.RenameColumn(
                name: "initiativeid",
                schema: "app",
                table: "StarredInitiatives",
                newName: "Initiativeid");

            migrationBuilder.RenameColumn(
                name: "folderid",
                schema: "app",
                table: "StarredInitiatives",
                newName: "Folderid");

            migrationBuilder.RenameColumn(
                name: "star_id",
                schema: "app",
                table: "StarredInitiatives",
                newName: "StarId");

            migrationBuilder.RenameIndex(
                name: "IX_StarredInitiatives_ownerid",
                schema: "app",
                table: "StarredInitiatives",
                newName: "IX_StarredInitiatives_Ownerid");

            migrationBuilder.RenameColumn(
                name: "rank",
                schema: "app",
                table: "StarredGroups",
                newName: "Rank");

            migrationBuilder.RenameColumn(
                name: "ownerid",
                schema: "app",
                table: "StarredGroups",
                newName: "Ownerid");

            migrationBuilder.RenameColumn(
                name: "groupid",
                schema: "app",
                table: "StarredGroups",
                newName: "Groupid");

            migrationBuilder.RenameColumn(
                name: "folderid",
                schema: "app",
                table: "StarredGroups",
                newName: "Folderid");

            migrationBuilder.RenameColumn(
                name: "star_id",
                schema: "app",
                table: "StarredGroups",
                newName: "StarId");

            migrationBuilder.RenameIndex(
                name: "IX_StarredGroups_ownerid",
                schema: "app",
                table: "StarredGroups",
                newName: "IX_StarredGroups_Ownerid");

            migrationBuilder.RenameColumn(
                name: "rank",
                schema: "app",
                table: "StarredCollections",
                newName: "Rank");

            migrationBuilder.RenameColumn(
                name: "ownerid",
                schema: "app",
                table: "StarredCollections",
                newName: "Ownerid");

            migrationBuilder.RenameColumn(
                name: "folderid",
                schema: "app",
                table: "StarredCollections",
                newName: "Folderid");

            migrationBuilder.RenameColumn(
                name: "collectionid",
                schema: "app",
                table: "StarredCollections",
                newName: "Collectionid");

            migrationBuilder.RenameColumn(
                name: "star_id",
                schema: "app",
                table: "StarredCollections",
                newName: "StarId");

            migrationBuilder.RenameIndex(
                name: "IX_StarredCollections_ownerid",
                schema: "app",
                table: "StarredCollections",
                newName: "IX_StarredCollections_Ownerid");

            migrationBuilder.RenameIndex(
                name: "IX_StarredCollections_folderid",
                schema: "app",
                table: "StarredCollections",
                newName: "IX_StarredCollections_Folderid");

            migrationBuilder.RenameColumn(
                name: "ReportObjectID",
                schema: "app",
                table: "ReportObjectImages_doc",
                newName: "ReportObjectId");

            migrationBuilder.RenameColumn(
                name: "ImageID",
                schema: "app",
                table: "ReportObjectImages_doc",
                newName: "ImageId");

            migrationBuilder.RenameColumn(
                name: "ReportObjectID",
                schema: "app",
                table: "ReportObjectDocTerms",
                newName: "ReportObjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportObjectDocTerms_ReportObjectID",
                schema: "app",
                table: "ReportObjectDocTerms",
                newName: "IX_ReportObjectDocTerms_ReportObjectId");

            migrationBuilder.RenameColumn(
                name: "ReportObjectID",
                schema: "app",
                table: "ReportObjectDocFragilityTags",
                newName: "ReportObjectId");

            migrationBuilder.RenameColumn(
                name: "FragilityTagID",
                schema: "app",
                table: "ReportObjectDocFragilityTags",
                newName: "FragilityTagId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportObjectDocFragilityTags_ReportObjectID",
                schema: "app",
                table: "ReportObjectDocFragilityTags",
                newName: "IX_ReportObjectDocFragilityTags_ReportObjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportObjectDocFragilityTags_FragilityTagID",
                schema: "app",
                table: "ReportObjectDocFragilityTags",
                newName: "IX_ReportObjectDocFragilityTags_FragilityTagId");

            migrationBuilder.RenameIndex(
                name: "organizationalvalueid1",
                schema: "app",
                table: "ReportObject_doc",
                newName: "organizationalvalueid");

            migrationBuilder.RenameIndex(
                name: "maintenancescheduleid1",
                schema: "app",
                table: "ReportObject_doc",
                newName: "maintenancescheduleid");

            migrationBuilder.RenameIndex(
                name: "fragilityid1",
                schema: "app",
                table: "ReportObject_doc",
                newName: "fragilityid");

            migrationBuilder.RenameColumn(
                name: "OrganizationalValueName",
                schema: "app",
                table: "OrganizationalValue",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "OrganizationalValueID",
                schema: "app",
                table: "OrganizationalValue",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "organizationalvalueid",
                schema: "app",
                table: "OrganizationalValue",
                newName: "IX_OrganizationalValue_Id");

            migrationBuilder.RenameColumn(
                name: "MaintenanceScheduleName",
                schema: "app",
                table: "MaintenanceSchedule",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "MaintenanceScheduleID",
                schema: "app",
                table: "MaintenanceSchedule",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "maintenancescheduleid",
                schema: "app",
                table: "MaintenanceSchedule",
                newName: "IX_MaintenanceSchedule_Id");

            migrationBuilder.RenameColumn(
                name: "MaintenanceLogStatusName",
                schema: "app",
                table: "MaintenanceLogStatus",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "MaintenanceLogStatusID",
                schema: "app",
                table: "MaintenanceLogStatus",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "maintenancelogstatusid1",
                schema: "app",
                table: "MaintenanceLogStatus",
                newName: "IX_MaintenanceLogStatus_Id");

            migrationBuilder.RenameColumn(
                name: "MaintenanceLogStatusID",
                schema: "app",
                table: "MaintenanceLog",
                newName: "MaintenanceLogStatusId");

            migrationBuilder.RenameColumn(
                name: "MaintainerID",
                schema: "app",
                table: "MaintenanceLog",
                newName: "MaintainerId");

            migrationBuilder.RenameColumn(
                name: "MaintenanceLogID",
                schema: "app",
                table: "MaintenanceLog",
                newName: "MaintenanceLogId");

            migrationBuilder.RenameColumn(
                name: "ReportObjectId",
                schema: "app",
                table: "MaintenanceLog",
                newName: "ReportId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceLog_ReportObjectId",
                schema: "app",
                table: "MaintenanceLog",
                newName: "IX_MaintenanceLog_ReportId");

            migrationBuilder.RenameColumn(
                name: "OperationOwnerID",
                schema: "app",
                table: "Initiative",
                newName: "OperationOwnerId");

            migrationBuilder.RenameColumn(
                name: "ExecutiveOwnerID",
                schema: "app",
                table: "Initiative",
                newName: "ExecutiveOwnerId");

            migrationBuilder.RenameColumn(
                name: "DataInitiativeID",
                schema: "app",
                table: "Initiative",
                newName: "InitiativeId");

            migrationBuilder.RenameColumn(
                name: "FragilityTagName",
                schema: "app",
                table: "FragilityTag",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "FragilityTagID",
                schema: "app",
                table: "FragilityTag",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "fragilitytagid",
                schema: "app",
                table: "FragilityTag",
                newName: "IX_FragilityTag_Id");

            migrationBuilder.RenameColumn(
                name: "FragilityName",
                schema: "app",
                table: "Fragility",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "FragilityID",
                schema: "app",
                table: "Fragility",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "fragilityid",
                schema: "app",
                table: "Fragility",
                newName: "IX_Fragility_Id");

            migrationBuilder.RenameColumn(
                name: "FinancialImpactId",
                schema: "app",
                table: "FinancialImpact",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "financialimpactid",
                schema: "app",
                table: "FinancialImpact",
                newName: "IX_FinancialImpact_Id");

            migrationBuilder.RenameColumn(
                name: "EstimatedRunFrequencyName",
                schema: "app",
                table: "EstimatedRunFrequency",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "EstimatedRunFrequencyID",
                schema: "app",
                table: "EstimatedRunFrequency",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "estimatedrunfrequencyid",
                schema: "app",
                table: "EstimatedRunFrequency",
                newName: "IX_EstimatedRunFrequency_Id");

            migrationBuilder.RenameColumn(
                name: "DataProjectId",
                schema: "app",
                table: "CollectionTerm",
                newName: "CollectionId");

            migrationBuilder.RenameColumn(
                name: "TermAnnotationID",
                schema: "app",
                table: "CollectionTerm",
                newName: "LinkId");

            migrationBuilder.RenameIndex(
                name: "termid+dataprojectid",
                schema: "app",
                table: "CollectionTerm",
                newName: "termid+collectionid");

            migrationBuilder.RenameIndex(
                name: "IX_CollectionTerm_DataProjectId",
                schema: "app",
                table: "CollectionTerm",
                newName: "IX_CollectionTerm_CollectionId");

            migrationBuilder.RenameColumn(
                name: "DataProjectId",
                schema: "app",
                table: "CollectionReport",
                newName: "CollectionId");

            migrationBuilder.RenameColumn(
                name: "ReportAnnotationID",
                schema: "app",
                table: "CollectionReport",
                newName: "LinkId");

            migrationBuilder.RenameIndex(
                name: "reportid+dataprojectid",
                schema: "app",
                table: "CollectionReport",
                newName: "reportid+collectionid");

            migrationBuilder.RenameIndex(
                name: "IX_CollectionReport_DataProjectId",
                schema: "app",
                table: "CollectionReport",
                newName: "IX_CollectionReport_CollectionId");

            migrationBuilder.RenameColumn(
                name: "OperationOwnerID",
                schema: "app",
                table: "Collection",
                newName: "OperationOwnerId");

            migrationBuilder.RenameColumn(
                name: "ExecutiveOwnerID",
                schema: "app",
                table: "Collection",
                newName: "ExecutiveOwnerId");

            migrationBuilder.RenameColumn(
                name: "DataManagerID",
                schema: "app",
                table: "Collection",
                newName: "DataManagerId");

            migrationBuilder.RenameColumn(
                name: "AnalyticsOwnerID",
                schema: "app",
                table: "Collection",
                newName: "AnalyticsOwnerId");

            migrationBuilder.RenameColumn(
                name: "DataInitiativeID",
                schema: "app",
                table: "Collection",
                newName: "InitiativeId");

            migrationBuilder.RenameColumn(
                name: "DataProjectID",
                schema: "app",
                table: "Collection",
                newName: "CollectionId");

            migrationBuilder.RenameColumn(
                name: "userAgent",
                schema: "app",
                table: "AnalyticsTrace",
                newName: "UserAgent");

            migrationBuilder.RenameColumn(
                name: "updateTime",
                schema: "app",
                table: "AnalyticsTrace",
                newName: "UpdateTime");

            migrationBuilder.RenameColumn(
                name: "referer",
                schema: "app",
                table: "AnalyticsTrace",
                newName: "Referer");

            migrationBuilder.RenameColumn(
                name: "logId",
                schema: "app",
                table: "AnalyticsTrace",
                newName: "LogId");

            migrationBuilder.RenameColumn(
                name: "logDateTime",
                schema: "app",
                table: "AnalyticsTrace",
                newName: "LogDateTime");

            migrationBuilder.RenameColumn(
                name: "userAgent",
                schema: "app",
                table: "AnalyticsError",
                newName: "UserAgent");

            migrationBuilder.RenameColumn(
                name: "updateTime",
                schema: "app",
                table: "AnalyticsError",
                newName: "UpdateTime");

            migrationBuilder.RenameColumn(
                name: "referer",
                schema: "app",
                table: "AnalyticsError",
                newName: "Referer");

            migrationBuilder.RenameColumn(
                name: "logDateTime",
                schema: "app",
                table: "AnalyticsError",
                newName: "LogDateTime");

            migrationBuilder.RenameColumn(
                name: "handled",
                schema: "app",
                table: "AnalyticsError",
                newName: "Handled");

            migrationBuilder.RenameColumn(
                name: "userAgent",
                schema: "app",
                table: "Analytics",
                newName: "UserAgent");

            migrationBuilder.RenameColumn(
                name: "updateTime",
                schema: "app",
                table: "Analytics",
                newName: "UpdateTime");

            migrationBuilder.RenameColumn(
                name: "sessionId",
                schema: "app",
                table: "Analytics",
                newName: "SessionId");

            migrationBuilder.RenameColumn(
                name: "search",
                schema: "app",
                table: "Analytics",
                newName: "Search");

            migrationBuilder.RenameColumn(
                name: "screenWidth",
                schema: "app",
                table: "Analytics",
                newName: "ScreenWidth");

            migrationBuilder.RenameColumn(
                name: "screenHeight",
                schema: "app",
                table: "Analytics",
                newName: "ScreenHeight");

            migrationBuilder.RenameColumn(
                name: "referrer",
                schema: "app",
                table: "Analytics",
                newName: "Referrer");

            migrationBuilder.RenameColumn(
                name: "protocol",
                schema: "app",
                table: "Analytics",
                newName: "Protocol");

            migrationBuilder.RenameColumn(
                name: "pathname",
                schema: "app",
                table: "Analytics",
                newName: "Pathname");

            migrationBuilder.RenameColumn(
                name: "pageTime",
                schema: "app",
                table: "Analytics",
                newName: "PageTime");

            migrationBuilder.RenameColumn(
                name: "pageId",
                schema: "app",
                table: "Analytics",
                newName: "PageId");

            migrationBuilder.RenameColumn(
                name: "origin",
                schema: "app",
                table: "Analytics",
                newName: "Origin");

            migrationBuilder.RenameColumn(
                name: "loadTime",
                schema: "app",
                table: "Analytics",
                newName: "LoadTime");

            migrationBuilder.RenameColumn(
                name: "language",
                schema: "app",
                table: "Analytics",
                newName: "Language");

            migrationBuilder.RenameColumn(
                name: "href",
                schema: "app",
                table: "Analytics",
                newName: "Href");

            migrationBuilder.RenameColumn(
                name: "hostname",
                schema: "app",
                table: "Analytics",
                newName: "Hostname");

            migrationBuilder.RenameColumn(
                name: "hash",
                schema: "app",
                table: "Analytics",
                newName: "Hash");

            migrationBuilder.RenameColumn(
                name: "active",
                schema: "app",
                table: "Analytics",
                newName: "Active");

            migrationBuilder.RenameColumn(
                name: "accessDateTime",
                schema: "app",
                table: "Analytics",
                newName: "AccessDateTime");

            migrationBuilder.RenameIndex(
                name: "reportobjectid2",
                schema: "dbo",
                table: "ReportObjectQuery",
                newName: "reportobjectid1");

            migrationBuilder.RenameColumn(
                name: "ReportObjectID",
                schema: "dbo",
                table: "ReportObjectParameters",
                newName: "ReportObjectId");

            migrationBuilder.RenameIndex(
                name: "reportobjectid1",
                schema: "dbo",
                table: "ReportObjectParameters",
                newName: "reportobjectid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdateDate",
                schema: "app",
                table: "Initiative",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);



            migrationBuilder.CreateIndex(
                name: "accessdatetime_session_width_agent",
                schema: "app",
                table: "Analytics",
                column: "AccessDateTime")
                .Annotation("SqlServer:Include", new[] { "PageId", "SessionId", "ScreenWidth", "UserAgent" });

            migrationBuilder.CreateIndex(
                name: "user_access_load_page_session_path",
                schema: "app",
                table: "Analytics",
                columns: new[] { "UserId", "AccessDateTime" })
                .Annotation("SqlServer:Include", new[] { "LoadTime", "PageId", "SessionId", "Pathname" });

            migrationBuilder.CreateIndex(
                name: "reportobjectid2",
                schema: "app",
                table: "ReportServiceRequests",
                column: "ReportObjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportServiceRequests",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "accessdatetime_session_width_agent",
                schema: "app",
                table: "Analytics");

            migrationBuilder.DropIndex(
                name: "user_access_load_page_session_path",
                schema: "app",
                table: "Analytics");

            migrationBuilder.RenameTable(
                name: "UserGroupsMembership",
                schema: "dbo",
                newName: "UserGroupsMembership");

            migrationBuilder.RenameTable(
                name: "UserGroups",
                schema: "dbo",
                newName: "UserGroups");

            migrationBuilder.RenameTable(
                name: "User",
                schema: "dbo",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "Tags",
                schema: "dbo",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "ReportTagLinks",
                schema: "dbo",
                newName: "ReportTagLinks");

            migrationBuilder.RenameTable(
                name: "ReportObjectType",
                schema: "dbo",
                newName: "ReportObjectType");

            migrationBuilder.RenameTable(
                name: "ReportObjectTags",
                schema: "dbo",
                newName: "ReportObjectTags");

            migrationBuilder.RenameTable(
                name: "ReportObjectTagMemberships",
                schema: "dbo",
                newName: "ReportObjectTagMemberships");

            migrationBuilder.RenameTable(
                name: "ReportObjectSubscriptions",
                schema: "dbo",
                newName: "ReportObjectSubscriptions");

            migrationBuilder.RenameTable(
                name: "ReportObjectQuery",
                schema: "dbo",
                newName: "ReportObjectQuery");

            migrationBuilder.RenameTable(
                name: "ReportObjectParameters",
                schema: "dbo",
                newName: "ReportObjectParameters");

            migrationBuilder.RenameTable(
                name: "ReportObjectHierarchy",
                schema: "dbo",
                newName: "ReportObjectHierarchy");

            migrationBuilder.RenameTable(
                name: "ReportObjectAttachments",
                schema: "dbo",
                newName: "ReportObjectAttachments");

            migrationBuilder.RenameTable(
                name: "ReportObject",
                schema: "dbo",
                newName: "ReportObject");

            migrationBuilder.RenameTable(
                name: "ReportGroupsMemberships",
                schema: "dbo",
                newName: "ReportGroupsMemberships");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "app",
                table: "StrategicImportance",
                newName: "StrategicImportanceId");

            migrationBuilder.RenameIndex(
                name: "IX_StrategicImportance_Id",
                schema: "app",
                table: "StrategicImportance",
                newName: "strategicimportanceid");

            migrationBuilder.RenameColumn(
                name: "Userid",
                schema: "app",
                table: "StarredUsers",
                newName: "userid");

            migrationBuilder.RenameColumn(
                name: "Rank",
                schema: "app",
                table: "StarredUsers",
                newName: "rank");

            migrationBuilder.RenameColumn(
                name: "Ownerid",
                schema: "app",
                table: "StarredUsers",
                newName: "ownerid");

            migrationBuilder.RenameColumn(
                name: "Folderid",
                schema: "app",
                table: "StarredUsers",
                newName: "folderid");

            migrationBuilder.RenameColumn(
                name: "StarId",
                schema: "app",
                table: "StarredUsers",
                newName: "star_id");

            migrationBuilder.RenameIndex(
                name: "IX_StarredUsers_Ownerid",
                schema: "app",
                table: "StarredUsers",
                newName: "IX_StarredUsers_ownerid");

            migrationBuilder.RenameColumn(
                name: "Termid",
                schema: "app",
                table: "StarredTerms",
                newName: "termid");

            migrationBuilder.RenameColumn(
                name: "Rank",
                schema: "app",
                table: "StarredTerms",
                newName: "rank");

            migrationBuilder.RenameColumn(
                name: "Ownerid",
                schema: "app",
                table: "StarredTerms",
                newName: "ownerid");

            migrationBuilder.RenameColumn(
                name: "Folderid",
                schema: "app",
                table: "StarredTerms",
                newName: "folderid");

            migrationBuilder.RenameColumn(
                name: "StarId",
                schema: "app",
                table: "StarredTerms",
                newName: "star_id");

            migrationBuilder.RenameIndex(
                name: "IX_StarredTerms_Ownerid",
                schema: "app",
                table: "StarredTerms",
                newName: "IX_StarredTerms_ownerid");

            migrationBuilder.RenameColumn(
                name: "Search",
                schema: "app",
                table: "StarredSearches",
                newName: "search");

            migrationBuilder.RenameColumn(
                name: "Rank",
                schema: "app",
                table: "StarredSearches",
                newName: "rank");

            migrationBuilder.RenameColumn(
                name: "Ownerid",
                schema: "app",
                table: "StarredSearches",
                newName: "ownerid");

            migrationBuilder.RenameColumn(
                name: "Folderid",
                schema: "app",
                table: "StarredSearches",
                newName: "folderid");

            migrationBuilder.RenameColumn(
                name: "StarId",
                schema: "app",
                table: "StarredSearches",
                newName: "star_id");

            migrationBuilder.RenameIndex(
                name: "IX_StarredSearches_Folderid",
                schema: "app",
                table: "StarredSearches",
                newName: "IX_StarredSearches_folderid");

            migrationBuilder.RenameColumn(
                name: "Reportid",
                schema: "app",
                table: "StarredReports",
                newName: "reportid");

            migrationBuilder.RenameColumn(
                name: "Rank",
                schema: "app",
                table: "StarredReports",
                newName: "rank");

            migrationBuilder.RenameColumn(
                name: "Ownerid",
                schema: "app",
                table: "StarredReports",
                newName: "ownerid");

            migrationBuilder.RenameColumn(
                name: "Folderid",
                schema: "app",
                table: "StarredReports",
                newName: "folderid");

            migrationBuilder.RenameColumn(
                name: "StarId",
                schema: "app",
                table: "StarredReports",
                newName: "star_id");

            migrationBuilder.RenameIndex(
                name: "IX_StarredReports_Ownerid",
                schema: "app",
                table: "StarredReports",
                newName: "IX_StarredReports_ownerid");

            migrationBuilder.RenameColumn(
                name: "Rank",
                schema: "app",
                table: "StarredInitiatives",
                newName: "rank");

            migrationBuilder.RenameColumn(
                name: "Ownerid",
                schema: "app",
                table: "StarredInitiatives",
                newName: "ownerid");

            migrationBuilder.RenameColumn(
                name: "Initiativeid",
                schema: "app",
                table: "StarredInitiatives",
                newName: "initiativeid");

            migrationBuilder.RenameColumn(
                name: "Folderid",
                schema: "app",
                table: "StarredInitiatives",
                newName: "folderid");

            migrationBuilder.RenameColumn(
                name: "StarId",
                schema: "app",
                table: "StarredInitiatives",
                newName: "star_id");

            migrationBuilder.RenameIndex(
                name: "IX_StarredInitiatives_Ownerid",
                schema: "app",
                table: "StarredInitiatives",
                newName: "IX_StarredInitiatives_ownerid");

            migrationBuilder.RenameColumn(
                name: "Rank",
                schema: "app",
                table: "StarredGroups",
                newName: "rank");

            migrationBuilder.RenameColumn(
                name: "Ownerid",
                schema: "app",
                table: "StarredGroups",
                newName: "ownerid");

            migrationBuilder.RenameColumn(
                name: "Groupid",
                schema: "app",
                table: "StarredGroups",
                newName: "groupid");

            migrationBuilder.RenameColumn(
                name: "Folderid",
                schema: "app",
                table: "StarredGroups",
                newName: "folderid");

            migrationBuilder.RenameColumn(
                name: "StarId",
                schema: "app",
                table: "StarredGroups",
                newName: "star_id");

            migrationBuilder.RenameIndex(
                name: "IX_StarredGroups_Ownerid",
                schema: "app",
                table: "StarredGroups",
                newName: "IX_StarredGroups_ownerid");

            migrationBuilder.RenameColumn(
                name: "Rank",
                schema: "app",
                table: "StarredCollections",
                newName: "rank");

            migrationBuilder.RenameColumn(
                name: "Ownerid",
                schema: "app",
                table: "StarredCollections",
                newName: "ownerid");

            migrationBuilder.RenameColumn(
                name: "Folderid",
                schema: "app",
                table: "StarredCollections",
                newName: "folderid");

            migrationBuilder.RenameColumn(
                name: "Collectionid",
                schema: "app",
                table: "StarredCollections",
                newName: "collectionid");

            migrationBuilder.RenameColumn(
                name: "StarId",
                schema: "app",
                table: "StarredCollections",
                newName: "star_id");

            migrationBuilder.RenameIndex(
                name: "IX_StarredCollections_Ownerid",
                schema: "app",
                table: "StarredCollections",
                newName: "IX_StarredCollections_ownerid");

            migrationBuilder.RenameIndex(
                name: "IX_StarredCollections_Folderid",
                schema: "app",
                table: "StarredCollections",
                newName: "IX_StarredCollections_folderid");

            migrationBuilder.RenameColumn(
                name: "ReportObjectId",
                schema: "app",
                table: "ReportObjectImages_doc",
                newName: "ReportObjectID");

            migrationBuilder.RenameColumn(
                name: "ImageId",
                schema: "app",
                table: "ReportObjectImages_doc",
                newName: "ImageID");

            migrationBuilder.RenameColumn(
                name: "ReportObjectId",
                schema: "app",
                table: "ReportObjectDocTerms",
                newName: "ReportObjectID");

            migrationBuilder.RenameIndex(
                name: "IX_ReportObjectDocTerms_ReportObjectId",
                schema: "app",
                table: "ReportObjectDocTerms",
                newName: "IX_ReportObjectDocTerms_ReportObjectID");

            migrationBuilder.RenameColumn(
                name: "ReportObjectId",
                schema: "app",
                table: "ReportObjectDocFragilityTags",
                newName: "ReportObjectID");

            migrationBuilder.RenameColumn(
                name: "FragilityTagId",
                schema: "app",
                table: "ReportObjectDocFragilityTags",
                newName: "FragilityTagID");

            migrationBuilder.RenameIndex(
                name: "IX_ReportObjectDocFragilityTags_ReportObjectId",
                schema: "app",
                table: "ReportObjectDocFragilityTags",
                newName: "IX_ReportObjectDocFragilityTags_ReportObjectID");

            migrationBuilder.RenameIndex(
                name: "IX_ReportObjectDocFragilityTags_FragilityTagId",
                schema: "app",
                table: "ReportObjectDocFragilityTags",
                newName: "IX_ReportObjectDocFragilityTags_FragilityTagID");

            migrationBuilder.RenameIndex(
                name: "organizationalvalueid",
                schema: "app",
                table: "ReportObject_doc",
                newName: "organizationalvalueid1");

            migrationBuilder.RenameIndex(
                name: "maintenancescheduleid",
                schema: "app",
                table: "ReportObject_doc",
                newName: "maintenancescheduleid1");

            migrationBuilder.RenameIndex(
                name: "fragilityid",
                schema: "app",
                table: "ReportObject_doc",
                newName: "fragilityid1");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "app",
                table: "OrganizationalValue",
                newName: "OrganizationalValueName");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "app",
                table: "OrganizationalValue",
                newName: "OrganizationalValueID");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationalValue_Id",
                schema: "app",
                table: "OrganizationalValue",
                newName: "organizationalvalueid");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "app",
                table: "MaintenanceSchedule",
                newName: "MaintenanceScheduleName");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "app",
                table: "MaintenanceSchedule",
                newName: "MaintenanceScheduleID");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceSchedule_Id",
                schema: "app",
                table: "MaintenanceSchedule",
                newName: "maintenancescheduleid");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "app",
                table: "MaintenanceLogStatus",
                newName: "MaintenanceLogStatusName");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "app",
                table: "MaintenanceLogStatus",
                newName: "MaintenanceLogStatusID");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceLogStatus_Id",
                schema: "app",
                table: "MaintenanceLogStatus",
                newName: "maintenancelogstatusid1");

            migrationBuilder.RenameColumn(
                name: "MaintenanceLogStatusId",
                schema: "app",
                table: "MaintenanceLog",
                newName: "MaintenanceLogStatusID");

            migrationBuilder.RenameColumn(
                name: "MaintainerId",
                schema: "app",
                table: "MaintenanceLog",
                newName: "MaintainerID");

            migrationBuilder.RenameColumn(
                name: "MaintenanceLogId",
                schema: "app",
                table: "MaintenanceLog",
                newName: "MaintenanceLogID");

            migrationBuilder.RenameColumn(
                name: "ReportId",
                schema: "app",
                table: "MaintenanceLog",
                newName: "ReportObjectId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceLog_ReportId",
                schema: "app",
                table: "MaintenanceLog",
                newName: "IX_MaintenanceLog_ReportObjectId");

            migrationBuilder.RenameColumn(
                name: "OperationOwnerId",
                schema: "app",
                table: "Initiative",
                newName: "OperationOwnerID");

            migrationBuilder.RenameColumn(
                name: "ExecutiveOwnerId",
                schema: "app",
                table: "Initiative",
                newName: "ExecutiveOwnerID");

            migrationBuilder.RenameColumn(
                name: "InitiativeId",
                schema: "app",
                table: "Initiative",
                newName: "DataInitiativeID");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "app",
                table: "FragilityTag",
                newName: "FragilityTagName");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "app",
                table: "FragilityTag",
                newName: "FragilityTagID");

            migrationBuilder.RenameIndex(
                name: "IX_FragilityTag_Id",
                schema: "app",
                table: "FragilityTag",
                newName: "fragilitytagid");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "app",
                table: "Fragility",
                newName: "FragilityName");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "app",
                table: "Fragility",
                newName: "FragilityID");

            migrationBuilder.RenameIndex(
                name: "IX_Fragility_Id",
                schema: "app",
                table: "Fragility",
                newName: "fragilityid");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "app",
                table: "FinancialImpact",
                newName: "FinancialImpactId");

            migrationBuilder.RenameIndex(
                name: "IX_FinancialImpact_Id",
                schema: "app",
                table: "FinancialImpact",
                newName: "financialimpactid");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "app",
                table: "EstimatedRunFrequency",
                newName: "EstimatedRunFrequencyName");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "app",
                table: "EstimatedRunFrequency",
                newName: "EstimatedRunFrequencyID");

            migrationBuilder.RenameIndex(
                name: "IX_EstimatedRunFrequency_Id",
                schema: "app",
                table: "EstimatedRunFrequency",
                newName: "estimatedrunfrequencyid");

            migrationBuilder.RenameColumn(
                name: "CollectionId",
                schema: "app",
                table: "CollectionTerm",
                newName: "DataProjectId");

            migrationBuilder.RenameColumn(
                name: "LinkId",
                schema: "app",
                table: "CollectionTerm",
                newName: "TermAnnotationID");

            migrationBuilder.RenameIndex(
                name: "termid+collectionid",
                schema: "app",
                table: "CollectionTerm",
                newName: "termid+dataprojectid");

            migrationBuilder.RenameIndex(
                name: "IX_CollectionTerm_CollectionId",
                schema: "app",
                table: "CollectionTerm",
                newName: "IX_CollectionTerm_DataProjectId");

            migrationBuilder.RenameColumn(
                name: "CollectionId",
                schema: "app",
                table: "CollectionReport",
                newName: "DataProjectId");

            migrationBuilder.RenameColumn(
                name: "LinkId",
                schema: "app",
                table: "CollectionReport",
                newName: "ReportAnnotationID");

            migrationBuilder.RenameIndex(
                name: "reportid+collectionid",
                schema: "app",
                table: "CollectionReport",
                newName: "reportid+dataprojectid");

            migrationBuilder.RenameIndex(
                name: "IX_CollectionReport_CollectionId",
                schema: "app",
                table: "CollectionReport",
                newName: "IX_CollectionReport_DataProjectId");

            migrationBuilder.RenameColumn(
                name: "OperationOwnerId",
                schema: "app",
                table: "Collection",
                newName: "OperationOwnerID");

            migrationBuilder.RenameColumn(
                name: "ExecutiveOwnerId",
                schema: "app",
                table: "Collection",
                newName: "ExecutiveOwnerID");

            migrationBuilder.RenameColumn(
                name: "DataManagerId",
                schema: "app",
                table: "Collection",
                newName: "DataManagerID");

            migrationBuilder.RenameColumn(
                name: "AnalyticsOwnerId",
                schema: "app",
                table: "Collection",
                newName: "AnalyticsOwnerID");

            migrationBuilder.RenameColumn(
                name: "InitiativeId",
                schema: "app",
                table: "Collection",
                newName: "DataInitiativeID");

            migrationBuilder.RenameColumn(
                name: "CollectionId",
                schema: "app",
                table: "Collection",
                newName: "DataProjectID");

            migrationBuilder.RenameColumn(
                name: "UserAgent",
                schema: "app",
                table: "AnalyticsTrace",
                newName: "userAgent");

            migrationBuilder.RenameColumn(
                name: "UpdateTime",
                schema: "app",
                table: "AnalyticsTrace",
                newName: "updateTime");

            migrationBuilder.RenameColumn(
                name: "Referer",
                schema: "app",
                table: "AnalyticsTrace",
                newName: "referer");

            migrationBuilder.RenameColumn(
                name: "LogId",
                schema: "app",
                table: "AnalyticsTrace",
                newName: "logId");

            migrationBuilder.RenameColumn(
                name: "LogDateTime",
                schema: "app",
                table: "AnalyticsTrace",
                newName: "logDateTime");

            migrationBuilder.RenameColumn(
                name: "UserAgent",
                schema: "app",
                table: "AnalyticsError",
                newName: "userAgent");

            migrationBuilder.RenameColumn(
                name: "UpdateTime",
                schema: "app",
                table: "AnalyticsError",
                newName: "updateTime");

            migrationBuilder.RenameColumn(
                name: "Referer",
                schema: "app",
                table: "AnalyticsError",
                newName: "referer");

            migrationBuilder.RenameColumn(
                name: "LogDateTime",
                schema: "app",
                table: "AnalyticsError",
                newName: "logDateTime");

            migrationBuilder.RenameColumn(
                name: "Handled",
                schema: "app",
                table: "AnalyticsError",
                newName: "handled");

            migrationBuilder.RenameColumn(
                name: "UserAgent",
                schema: "app",
                table: "Analytics",
                newName: "userAgent");

            migrationBuilder.RenameColumn(
                name: "UpdateTime",
                schema: "app",
                table: "Analytics",
                newName: "updateTime");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                schema: "app",
                table: "Analytics",
                newName: "sessionId");

            migrationBuilder.RenameColumn(
                name: "Search",
                schema: "app",
                table: "Analytics",
                newName: "search");

            migrationBuilder.RenameColumn(
                name: "ScreenWidth",
                schema: "app",
                table: "Analytics",
                newName: "screenWidth");

            migrationBuilder.RenameColumn(
                name: "ScreenHeight",
                schema: "app",
                table: "Analytics",
                newName: "screenHeight");

            migrationBuilder.RenameColumn(
                name: "Referrer",
                schema: "app",
                table: "Analytics",
                newName: "referrer");

            migrationBuilder.RenameColumn(
                name: "Protocol",
                schema: "app",
                table: "Analytics",
                newName: "protocol");

            migrationBuilder.RenameColumn(
                name: "Pathname",
                schema: "app",
                table: "Analytics",
                newName: "pathname");

            migrationBuilder.RenameColumn(
                name: "PageTime",
                schema: "app",
                table: "Analytics",
                newName: "pageTime");

            migrationBuilder.RenameColumn(
                name: "PageId",
                schema: "app",
                table: "Analytics",
                newName: "pageId");

            migrationBuilder.RenameColumn(
                name: "Origin",
                schema: "app",
                table: "Analytics",
                newName: "origin");

            migrationBuilder.RenameColumn(
                name: "LoadTime",
                schema: "app",
                table: "Analytics",
                newName: "loadTime");

            migrationBuilder.RenameColumn(
                name: "Language",
                schema: "app",
                table: "Analytics",
                newName: "language");

            migrationBuilder.RenameColumn(
                name: "Href",
                schema: "app",
                table: "Analytics",
                newName: "href");

            migrationBuilder.RenameColumn(
                name: "Hostname",
                schema: "app",
                table: "Analytics",
                newName: "hostname");

            migrationBuilder.RenameColumn(
                name: "Hash",
                schema: "app",
                table: "Analytics",
                newName: "hash");

            migrationBuilder.RenameColumn(
                name: "Active",
                schema: "app",
                table: "Analytics",
                newName: "active");

            migrationBuilder.RenameColumn(
                name: "AccessDateTime",
                schema: "app",
                table: "Analytics",
                newName: "accessDateTime");

            migrationBuilder.RenameIndex(
                name: "reportobjectid1",
                table: "ReportObjectQuery",
                newName: "reportobjectid2");

            migrationBuilder.RenameColumn(
                name: "ReportObjectId",
                table: "ReportObjectParameters",
                newName: "ReportObjectID");

            migrationBuilder.RenameIndex(
                name: "reportobjectid",
                table: "ReportObjectParameters",
                newName: "reportobjectid1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdateDate",
                schema: "app",
                table: "Initiative",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ReportManageEngineTickets",
                schema: "app",
                columns: table => new
                {
                    ManageEngineTicketsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportMa__97EB8BADB02592C9", x => x.ManageEngineTicketsId);
                    table.ForeignKey(
                        name: "FK_ReportManageEngineTickets_ReportObject",
                        column: x => x.ReportObjectId,
                        principalSchema: "app",
                        principalTable: "ReportObject_doc",
                        principalColumn: "ReportObjectID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "accessdatetime_session_width_agent",
                schema: "app",
                table: "Analytics",
                column: "accessDateTime")
                .Annotation("SqlServer:Include", new[] { "pageId", "sessionId", "screenWidth", "userAgent" });

            migrationBuilder.CreateIndex(
                name: "user_access_load_page_session_path",
                schema: "app",
                table: "Analytics",
                columns: new[] { "UserId", "accessDateTime" })
                .Annotation("SqlServer:Include", new[] { "loadTime", "pageId", "sessionId", "pathname" });

            migrationBuilder.CreateIndex(
                name: "reportobjectid",
                schema: "app",
                table: "ReportManageEngineTickets",
                column: "ReportObjectId");
        }
    }
}
