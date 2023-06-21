using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // first migration is fake. database already existed.
            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.CreateTable(
                name: "DP_Contact",
                schema: "app",
                columns: table => new
                {
                    ContactID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(55)", maxLength: 55, nullable: true),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_Conta__5C6625BB13B948C2", x => x.ContactID);
                });

            migrationBuilder.CreateTable(
                name: "EstimatedRunFrequency",
                schema: "app",
                columns: table => new
                {
                    EstimatedRunFrequencyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstimatedRunFrequencyName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimatedRunFrequency", x => x.EstimatedRunFrequencyID);
                });

            migrationBuilder.CreateTable(
                name: "FinancialImpact",
                schema: "app",
                columns: table => new
                {
                    FinancialImpactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialImpact", x => x.FinancialImpactId);
                });

            migrationBuilder.CreateTable(
                name: "Fragility",
                schema: "app",
                columns: table => new
                {
                    FragilityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FragilityName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fragility", x => x.FragilityID);
                });

            migrationBuilder.CreateTable(
                name: "FragilityTag",
                schema: "app",
                columns: table => new
                {
                    FragilityTagID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FragilityTagName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FragilityTag", x => x.FragilityTagID);
                });

            migrationBuilder.CreateTable(
                name: "GlobalSiteSettings",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalSiteSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mail_MessageType",
                schema: "app",
                columns: table => new
                {
                    MessageTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Mail_Mes__9BA1E2BAEE19569E", x => x.MessageTypeId);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceLogStatus",
                schema: "app",
                columns: table => new
                {
                    MaintenanceLogStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenanceLogStatusName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceLogStatus", x => x.MaintenanceLogStatusID);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceSchedule",
                schema: "app",
                columns: table => new
                {
                    MaintenanceScheduleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenanceScheduleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceSchedule", x => x.MaintenanceScheduleID);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationalValue",
                schema: "app",
                columns: table => new
                {
                    OrganizationalValueID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationalValueName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationalValue", x => x.OrganizationalValueID);
                });

            migrationBuilder.CreateTable(
                name: "ReportCertificationTags",
                columns: table => new
                {
                    Cert_ID = table.Column<int>(type: "int", nullable: false),
                    CertName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportCertificationTags", x => x.Cert_ID);
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectTags",
                columns: table => new
                {
                    TagID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EpicTagID = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    TagName = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportObjectTags", x => x.TagID);
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectType",
                columns: table => new
                {
                    ReportObjectTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultEpicMasterFile = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    LastLoadDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Visible = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportObjectType", x => x.ReportObjectTypeID);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                schema: "app",
                columns: table => new
                {
                    RolePermissionsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RolePerm__18B281E0AD8AB9C1", x => x.RolePermissionsId);
                });

            migrationBuilder.CreateTable(
                name: "Search_BasicSearchData",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: true),
                    ItemType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ItemRank = table.Column<int>(type: "int", nullable: true),
                    SearchFieldDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SearchField = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hidden = table.Column<int>(type: "int", nullable: true),
                    VisibleType = table.Column<int>(type: "int", nullable: true),
                    Orphaned = table.Column<int>(type: "int", nullable: true),
                    CertificationTag = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Search_BasicSearchData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Search_BasicSearchData_Small",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: true),
                    ItemType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ItemRank = table.Column<int>(type: "int", nullable: true),
                    SearchFieldDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SearchField = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hidden = table.Column<int>(type: "int", nullable: true),
                    VisibleType = table.Column<int>(type: "int", nullable: true),
                    Orphaned = table.Column<int>(type: "int", nullable: true),
                    CertificationTag = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Search_BasicSearchData_Small", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Search_ReportObjectSearchData",
                schema: "app",
                columns: table => new
                {
                    pk = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<int>(type: "int", nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EpicMasterFile = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    DefaultVisibilityYN = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    OrphanedReportObjectYN = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    ReportObjectTypeID = table.Column<int>(type: "int", nullable: true),
                    AuthorUserId = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByUserID = table.Column<int>(type: "int", nullable: true),
                    EpicReportTemplateId = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    SourceServer = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SourceDB = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SourceTable = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Documented = table.Column<int>(type: "int", nullable: false),
                    DocOwnerId = table.Column<int>(type: "int", nullable: true),
                    DocRequesterId = table.Column<int>(type: "int", nullable: true),
                    DocOrgValueId = table.Column<int>(type: "int", nullable: true),
                    DocRunFreqId = table.Column<int>(type: "int", nullable: true),
                    DocFragId = table.Column<int>(type: "int", nullable: true),
                    DocExecVis = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    DocMainSchedId = table.Column<int>(type: "int", nullable: true),
                    DocLastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    DocCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    DocCreatedBy = table.Column<int>(type: "int", nullable: true),
                    DocUpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DocHypeEnabled = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    DocDoNotPurge = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    DocHidden = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    certificationTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoYearRuns = table.Column<int>(type: "int", nullable: true),
                    OneYearRuns = table.Column<int>(type: "int", nullable: true),
                    SixMonthsRuns = table.Column<int>(type: "int", nullable: true),
                    OneMonthRuns = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Search_ReportObjectSearchData_PK", x => x.pk);
                });

            migrationBuilder.CreateTable(
                name: "SearchTable",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: true),
                    ItemType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ItemRank = table.Column<int>(type: "int", nullable: true),
                    SearchFieldDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SearchField = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StrategicImportance",
                schema: "app",
                columns: table => new
                {
                    StrategicImportanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategicImportance", x => x.StrategicImportanceId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Base = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EpicId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastLoadDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "datetime", nullable: true),
                    Fullname_calc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Firstname_calc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "UserFavoriteFolders",
                schema: "app",
                columns: table => new
                {
                    UserFavoriteFolderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FolderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    FolderRank = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteFolders", x => x.UserFavoriteFolderId);
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupSource = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastLoadDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EpicId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "app",
                columns: table => new
                {
                    UserRolesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserRole__41D8EF2F71AEFA0B", x => x.UserRolesId);
                });

            migrationBuilder.CreateTable(
                name: "Analytics",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    appCodeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    appName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    appVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cookieEnabled = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    oscpu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    platform = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    userAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    host = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hostname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    href = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    protocol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    search = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pathname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    screenHeight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    screenWidth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    origin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    loadTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    accessDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    referrer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Zoom = table.Column<double>(type: "float", nullable: true),
                    Epic = table.Column<int>(type: "int", nullable: true),
                    active = table.Column<int>(type: "int", nullable: true),
                    pageId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pageTime = table.Column<int>(type: "int", nullable: true),
                    sessionTime = table.Column<int>(type: "int", nullable: true),
                    updateTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Analytics_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "AnalyticsTrace",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Logger = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    logDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    logId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    handled = table.Column<int>(type: "int", nullable: true),
                    updateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    userAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    referer = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyticsTrace", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Analytics_Trace_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "DP_DataInitiative",
                schema: "app",
                columns: table => new
                {
                    DataInitiativeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperationOwnerID = table.Column<int>(type: "int", nullable: true),
                    ExecutiveOwnerID = table.Column<int>(type: "int", nullable: true),
                    FinancialImpact = table.Column<int>(type: "int", nullable: true),
                    StrategicImportance = table.Column<int>(type: "int", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastUpdateUser = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_DataI__1EFC948C3A83A845", x => x.DataInitiativeID);
                    table.ForeignKey(
                        name: "FK_DP_DataInitiative_FinancialImpact",
                        column: x => x.FinancialImpact,
                        principalSchema: "app",
                        principalTable: "FinancialImpact",
                        principalColumn: "FinancialImpactId");
                    table.ForeignKey(
                        name: "FK_DP_DataInitiative_StrategicImportance",
                        column: x => x.StrategicImportance,
                        principalSchema: "app",
                        principalTable: "StrategicImportance",
                        principalColumn: "StrategicImportanceId");
                    table.ForeignKey(
                        name: "FK_DP_DataInitiative_User",
                        column: x => x.ExecutiveOwnerID,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_DP_DataInitiative_User1",
                        column: x => x.OperationOwnerID,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_DP_DataInitiative_WebAppUsers",
                        column: x => x.LastUpdateUser,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "DP_MilestoneFrequency",
                schema: "app",
                columns: table => new
                {
                    MilestoneTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdateUser = table.Column<int>(type: "int", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
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
                name: "Mail_Drafts",
                schema: "app",
                columns: table => new
                {
                    DraftId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    MessageTypeId = table.Column<int>(type: "int", nullable: true),
                    FromUserId = table.Column<int>(type: "int", nullable: true),
                    MessagePlainText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Recipients = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplyToMessageId = table.Column<int>(type: "int", nullable: true),
                    ReplyToConvId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mail_Drafts", x => x.DraftId);
                    table.ForeignKey(
                        name: "FK_Mail_Drafts_User",
                        column: x => x.FromUserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Mail_Folders",
                schema: "app",
                columns: table => new
                {
                    FolderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentFolderId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rank = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Mail_Fol__ACD7107FA5BAA87B", x => x.FolderId);
                    table.ForeignKey(
                        name: "FK_Mail_Folders_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Mail_Messages",
                schema: "app",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    MessageTypeId = table.Column<int>(type: "int", nullable: true),
                    FromUserId = table.Column<int>(type: "int", nullable: true),
                    MessagePlainText = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Mail_Mes__C87C0C9CF29221A9", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Mail_Messages_Mail_MessageType",
                        column: x => x.MessageTypeId,
                        principalSchema: "app",
                        principalTable: "Mail_MessageType",
                        principalColumn: "MessageTypeId");
                    table.ForeignKey(
                        name: "FK_Mail_Messages_User",
                        column: x => x.FromUserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Mail_Recipients_Deleted",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(type: "int", nullable: true),
                    ToUserId = table.Column<int>(type: "int", nullable: true),
                    ReadDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    AlertDisplayed = table.Column<int>(type: "int", nullable: true),
                    ToGroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mail_Recipients_Deleted", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mail_Recipients_User1",
                        column: x => x.ToUserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceLog",
                schema: "app",
                columns: table => new
                {
                    MaintenanceLogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintainerID = table.Column<int>(type: "int", nullable: true),
                    MaintenanceDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaintenanceLogStatusID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceLog", x => x.MaintenanceLogID);
                    table.ForeignKey(
                        name: "FK__Maintenan__Maint__251C81ED",
                        column: x => x.MaintenanceLogStatusID,
                        principalSchema: "app",
                        principalTable: "MaintenanceLogStatus",
                        principalColumn: "MaintenanceLogStatusID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Maintenan__Maint__65F62111",
                        column: x => x.MaintainerID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportObject",
                columns: table => new
                {
                    ReportObjectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectBizKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceServer = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SourceDB = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SourceTable = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetailedDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportObjectTypeID = table.Column<int>(type: "int", nullable: true),
                    AuthorUserID = table.Column<int>(type: "int", nullable: true),
                    LastModifiedByUserID = table.Column<int>(type: "int", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ReportObjectURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EpicMasterFile = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    EpicRecordID = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    ReportServerCatalogID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DefaultVisibilityYN = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    OrphanedReportObjectYN = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true, defaultValueSql: "('N')"),
                    EpicReportTemplateId = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    ReportServerPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastLoadDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RepositoryDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EpicReleased = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    CertificationTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Availability = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificationTagID = table.Column<int>(type: "int", nullable: true),
                    Runs = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportObject", x => x.ReportObjectID);
                    table.ForeignKey(
                        name: "FK__ReportObj__Autho__35682A19",
                        column: x => x.AuthorUserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__ReportObj__LastM__365C4E52",
                        column: x => x.LastModifiedByUserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__ReportObj__Repor__3750728B",
                        column: x => x.ReportObjectTypeID,
                        principalTable: "ReportObjectType",
                        principalColumn: "ReportObjectTypeID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectRunTime",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RunUserId = table.Column<int>(type: "int", nullable: true),
                    Runs = table.Column<int>(type: "int", nullable: true),
                    RunTime = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    RunWeek = table.Column<DateTime>(type: "datetime", nullable: true),
                    RunWeekString = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportObjectRunTime", x => x.Id);
                    table.ForeignKey(
                        name: "fk_userruntime",
                        column: x => x.RunUserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "SharedItems",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SharedFromUserId = table.Column<int>(type: "int", nullable: true),
                    SharedToUserId = table.Column<int>(type: "int", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShareDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedItems_User",
                        column: x => x.SharedFromUserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_SharedItems_User1",
                        column: x => x.SharedToUserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Term",
                schema: "app",
                columns: table => new
                {
                    TermId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    TechnicalDefinition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedYN = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    ApprovalDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    ApprovedByUserId = table.Column<int>(type: "int", nullable: true),
                    HasExternalStandardYN = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    ExternalStandardUrl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ValidFromDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    ValidToDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true),
                    LastUpdatedDateTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Term", x => x.TermId);
                    table.ForeignKey(
                        name: "FK_Term_WebAppUsers",
                        column: x => x.UpdatedByUserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_Term_WebAppUsers1",
                        column: x => x.ApprovedByUserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "User_NameData",
                schema: "app",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Fullname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "UserPreferences",
                schema: "app",
                columns: table => new
                {
                    UserPreferenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemValue = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.UserPreferenceId);
                    table.ForeignKey(
                        name: "FK_UserPreferences_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "StarredSearches",
                schema: "app",
                columns: table => new
                {
                    star_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rank = table.Column<int>(type: "int", nullable: true),
                    search = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ownerid = table.Column<int>(type: "int", nullable: true),
                    folderid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StarredS__88222DCE6D6FEE1D", x => x.star_id);
                    table.ForeignKey(
                        name: "FK_StarredSearches_User",
                        column: x => x.ownerid,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_StarredSearches_UserFavoriteFolders",
                        column: x => x.folderid,
                        principalSchema: "app",
                        principalTable: "UserFavoriteFolders",
                        principalColumn: "UserFavoriteFolderId");
                });

            migrationBuilder.CreateTable(
                name: "StarredUsers",
                schema: "app",
                columns: table => new
                {
                    star_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rank = table.Column<int>(type: "int", nullable: true),
                    userid = table.Column<int>(type: "int", nullable: true),
                    ownerid = table.Column<int>(type: "int", nullable: true),
                    folderid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StarredU__88222DCE446292A3", x => x.star_id);
                    table.ForeignKey(
                        name: "FK_StarredUsers_User",
                        column: x => x.userid,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_StarredUsers_User_owner",
                        column: x => x.ownerid,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_StarredUsers_UserFavoriteFolders",
                        column: x => x.folderid,
                        principalSchema: "app",
                        principalTable: "UserFavoriteFolders",
                        principalColumn: "UserFavoriteFolderId");
                });

            migrationBuilder.CreateTable(
                name: "UserFavorites",
                schema: "app",
                columns: table => new
                {
                    UserFavoritesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemRank = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FolderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserFavo__1D23EA13087A927D", x => x.UserFavoritesId);
                    table.ForeignKey(
                        name: "FK_UserFavorites_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_UserFavorites_UserFavoriteFolders",
                        column: x => x.FolderId,
                        principalSchema: "app",
                        principalTable: "UserFavoriteFolders",
                        principalColumn: "UserFavoriteFolderId");
                });

            migrationBuilder.CreateTable(
                name: "StarredGroups",
                schema: "app",
                columns: table => new
                {
                    star_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rank = table.Column<int>(type: "int", nullable: true),
                    groupid = table.Column<int>(type: "int", nullable: true),
                    ownerid = table.Column<int>(type: "int", nullable: true),
                    folderid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StarredG__88222DCE76B4AC11", x => x.star_id);
                    table.ForeignKey(
                        name: "FK_StarredGroups_User",
                        column: x => x.ownerid,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_StarredGroups_UserFavoriteFolders",
                        column: x => x.folderid,
                        principalSchema: "app",
                        principalTable: "UserFavoriteFolders",
                        principalColumn: "UserFavoriteFolderId");
                    table.ForeignKey(
                        name: "FK_StarredGroups_UserGroups",
                        column: x => x.groupid,
                        principalTable: "UserGroups",
                        principalColumn: "GroupId");
                });

            migrationBuilder.CreateTable(
                name: "UserGroupsMembership",
                columns: table => new
                {
                    MembershipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    LastLoadDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupsMembership", x => x.MembershipId);
                    table.ForeignKey(
                        name: "FK_UserGroupsMembership_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_UserGroupsMembership_UserGroups",
                        column: x => x.GroupId,
                        principalTable: "UserGroups",
                        principalColumn: "GroupId");
                });

            migrationBuilder.CreateTable(
                name: "RolePermissionLinks",
                schema: "app",
                columns: table => new
                {
                    RolePermissionLinksId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    RolePermissionsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserPerm__40D89C0194D8BDD6", x => x.RolePermissionLinksId);
                    table.ForeignKey(
                        name: "FK_RolePermissionLinks_RolePermissions",
                        column: x => x.RolePermissionsId,
                        principalSchema: "app",
                        principalTable: "RolePermissions",
                        principalColumn: "RolePermissionsId");
                    table.ForeignKey(
                        name: "FK_RolePermissionLinks_UserRoles",
                        column: x => x.RoleId,
                        principalSchema: "app",
                        principalTable: "UserRoles",
                        principalColumn: "UserRolesId");
                });

            migrationBuilder.CreateTable(
                name: "UserRoleLinks",
                schema: "app",
                columns: table => new
                {
                    UserRoleLinksId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    UserRolesId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserRole__41D8EF2F662663D0", x => x.UserRoleLinksId);
                    table.ForeignKey(
                        name: "FK_UserRoleLinks_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_UserRoleLinks_UserRoles",
                        column: x => x.UserRolesId,
                        principalSchema: "app",
                        principalTable: "UserRoles",
                        principalColumn: "UserRolesId");
                });

            migrationBuilder.CreateTable(
                name: "DP_Contact_Links",
                schema: "app",
                columns: table => new
                {
                    LinkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InitiativeId = table.Column<int>(type: "int", nullable: true),
                    ContactId = table.Column<int>(type: "int", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "DP_DataProject",
                schema: "app",
                columns: table => new
                {
                    DataProjectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataInitiativeID = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperationOwnerID = table.Column<int>(type: "int", nullable: true),
                    ExecutiveOwnerID = table.Column<int>(type: "int", nullable: true),
                    AnalyticsOwnerID = table.Column<int>(type: "int", nullable: true),
                    DataManagerID = table.Column<int>(type: "int", nullable: true),
                    FinancialImpact = table.Column<int>(type: "int", nullable: true),
                    StrategicImportance = table.Column<int>(type: "int", nullable: true),
                    ExternalDocumentationURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastUpdateUser = table.Column<int>(type: "int", nullable: true),
                    Hidden = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_DataP__E8D09D08794EBFAD", x => x.DataProjectID);
                    table.ForeignKey(
                        name: "FK_DP_DataProject_DP_DataInitiative",
                        column: x => x.DataInitiativeID,
                        principalSchema: "app",
                        principalTable: "DP_DataInitiative",
                        principalColumn: "DataInitiativeID");
                    table.ForeignKey(
                        name: "FK_DP_DataProject_FinancialImpact",
                        column: x => x.FinancialImpact,
                        principalSchema: "app",
                        principalTable: "FinancialImpact",
                        principalColumn: "FinancialImpactId");
                    table.ForeignKey(
                        name: "FK_DP_DataProject_StrategicImportance",
                        column: x => x.StrategicImportance,
                        principalSchema: "app",
                        principalTable: "StrategicImportance",
                        principalColumn: "StrategicImportanceId");
                    table.ForeignKey(
                        name: "FK_DP_DataProject_User",
                        column: x => x.ExecutiveOwnerID,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_DP_DataProject_User1",
                        column: x => x.OperationOwnerID,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_DP_DataProject_User2",
                        column: x => x.DataManagerID,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_DP_DataProject_WebAppUsers",
                        column: x => x.LastUpdateUser,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_DP_DataProject_WebAppUsers1",
                        column: x => x.AnalyticsOwnerID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "StarredInitiatives",
                schema: "app",
                columns: table => new
                {
                    star_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rank = table.Column<int>(type: "int", nullable: true),
                    initiativeid = table.Column<int>(type: "int", nullable: true),
                    ownerid = table.Column<int>(type: "int", nullable: true),
                    folderid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StarredI__88222DCEFCC5A8E5", x => x.star_id);
                    table.ForeignKey(
                        name: "FK_StarredInitiatives_DP_DataInitiative",
                        column: x => x.initiativeid,
                        principalSchema: "app",
                        principalTable: "DP_DataInitiative",
                        principalColumn: "DataInitiativeID");
                    table.ForeignKey(
                        name: "FK_StarredInitiatives_User",
                        column: x => x.ownerid,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_StarredInitiatives_UserFavoriteFolders",
                        column: x => x.folderid,
                        principalSchema: "app",
                        principalTable: "UserFavoriteFolders",
                        principalColumn: "UserFavoriteFolderId");
                });

            migrationBuilder.CreateTable(
                name: "DP_MilestoneTemplates",
                schema: "app",
                columns: table => new
                {
                    MilestoneTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MilestoneTypeId = table.Column<int>(type: "int", nullable: true),
                    LastUpdateUser = table.Column<int>(type: "int", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Interval = table.Column<int>(type: "int", nullable: true)
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
                name: "Mail_Conversations",
                schema: "app",
                columns: table => new
                {
                    ConversationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Mail_Con__C050D8770DFC66D7", x => x.ConversationId);
                    table.ForeignKey(
                        name: "FK_Mail_Conversations_Mail_Messages",
                        column: x => x.MessageId,
                        principalSchema: "app",
                        principalTable: "Mail_Messages",
                        principalColumn: "MessageId");
                });

            migrationBuilder.CreateTable(
                name: "Mail_FolderMessages",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FolderId = table.Column<int>(type: "int", nullable: true),
                    MessageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mail_FolderMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mail_FolderMessages_Mail_Folders",
                        column: x => x.FolderId,
                        principalSchema: "app",
                        principalTable: "Mail_Folders",
                        principalColumn: "FolderId");
                    table.ForeignKey(
                        name: "FK_Mail_FolderMessages_Mail_Messages",
                        column: x => x.MessageId,
                        principalSchema: "app",
                        principalTable: "Mail_Messages",
                        principalColumn: "MessageId");
                });

            migrationBuilder.CreateTable(
                name: "Mail_Recipients",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(type: "int", nullable: true),
                    ToUserId = table.Column<int>(type: "int", nullable: true),
                    ReadDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    AlertDisplayed = table.Column<int>(type: "int", nullable: true),
                    ToGroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mail_Recipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mail_Recipients_Mail_Messages",
                        column: x => x.MessageId,
                        principalSchema: "app",
                        principalTable: "Mail_Messages",
                        principalColumn: "MessageId");
                    table.ForeignKey(
                        name: "FK_Mail_Recipients_User",
                        column: x => x.ToUserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_Mail_Recipients_UserLDAPGroups",
                        column: x => x.ToGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "GroupId");
                });

            migrationBuilder.CreateTable(
                name: "ReportGroupsMemberships",
                columns: table => new
                {
                    MembershipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    LastLoadDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportGr__92A786790B03128D", x => x.MembershipId);
                    table.ForeignKey(
                        name: "FK_ReportGroupsMemberships_ReportObject",
                        column: x => x.ReportId,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                    table.ForeignKey(
                        name: "FK_ReportGroupsMemberships_UserGroups",
                        column: x => x.GroupId,
                        principalTable: "UserGroups",
                        principalColumn: "GroupId");
                });

            migrationBuilder.CreateTable(
                name: "ReportManageEngineTickets",
                schema: "app",
                columns: table => new
                {
                    ManageEngineTicketsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketNumber = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportObjectId = table.Column<int>(type: "int", nullable: true),
                    TicketUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportMa__97EB8BADB02592C9", x => x.ManageEngineTicketsId);
                    table.ForeignKey(
                        name: "FK_ReportManageEngineTickets_ReportObject",
                        column: x => x.ReportObjectId,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                });

            migrationBuilder.CreateTable(
                name: "ReportObject_doc",
                schema: "app",
                columns: table => new
                {
                    ReportObjectID = table.Column<int>(type: "int", nullable: false),
                    OperationalOwnerUserID = table.Column<int>(type: "int", nullable: true),
                    Requester = table.Column<int>(type: "int", nullable: true),
                    GitLabProjectURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GitLabTreeURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GitLabBlobURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeveloperDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyAssumptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationalValueID = table.Column<int>(type: "int", nullable: true),
                    EstimatedRunFrequencyID = table.Column<int>(type: "int", nullable: true),
                    FragilityID = table.Column<int>(type: "int", nullable: true),
                    ExecutiveVisibilityYN = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    MaintenanceScheduleID = table.Column<int>(type: "int", nullable: true),
                    LastUpdateDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    EnabledForHyperspace = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    DoNotPurge = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    Hidden = table.Column<string>(type: "nchar(1)", fixedLength: true, maxLength: 1, nullable: true),
                    DeveloperNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportOb__B7A74135D2A44EFC", x => x.ReportObjectID);
                    table.ForeignKey(
                        name: "FK__ReportObj__Estim__477199F1",
                        column: x => x.EstimatedRunFrequencyID,
                        principalSchema: "app",
                        principalTable: "EstimatedRunFrequency",
                        principalColumn: "EstimatedRunFrequencyID");
                    table.ForeignKey(
                        name: "FK__ReportObj__Fragi__4865BE2A",
                        column: x => x.FragilityID,
                        principalSchema: "app",
                        principalTable: "Fragility",
                        principalColumn: "FragilityID");
                    table.ForeignKey(
                        name: "FK__ReportObj__Maint__4959E263",
                        column: x => x.MaintenanceScheduleID,
                        principalSchema: "app",
                        principalTable: "MaintenanceSchedule",
                        principalColumn: "MaintenanceScheduleID");
                    table.ForeignKey(
                        name: "FK__ReportObj__Opera__4B422AD5",
                        column: x => x.OperationalOwnerUserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__ReportObj__Organ__4C364F0E",
                        column: x => x.OrganizationalValueID,
                        principalSchema: "app",
                        principalTable: "OrganizationalValue",
                        principalColumn: "OrganizationalValueID");
                    table.ForeignKey(
                        name: "FK__ReportObj__Repor__3938BAFD",
                        column: x => x.ReportObjectID,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                    table.ForeignKey(
                        name: "FK__ReportObj__Reque__4E1E9780",
                        column: x => x.Requester,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_ReportObject_doc_User",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectAttachments",
                columns: table => new
                {
                    ReportObjectAttachmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastLoadDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportObjectAttachments", x => x.ReportObjectAttachmentId);
                    table.ForeignKey(
                        name: "FK_ReportObjectAttachments_ReportObject",
                        column: x => x.ReportObjectId,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                });

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
                name: "ReportObjectHierarchy",
                columns: table => new
                {
                    ParentReportObjectID = table.Column<int>(type: "int", nullable: false),
                    ChildReportObjectID = table.Column<int>(type: "int", nullable: false),
                    Line = table.Column<int>(type: "int", nullable: true),
                    LastLoadDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportOb__913B66516CC9406D", x => new { x.ParentReportObjectID, x.ChildReportObjectID });
                    table.ForeignKey(
                        name: "FK__ReportObj__Child__3C1527A8",
                        column: x => x.ChildReportObjectID,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                    table.ForeignKey(
                        name: "FK__ReportObj__Paren__3D094BE1",
                        column: x => x.ParentReportObjectID,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectImages_doc",
                schema: "app",
                columns: table => new
                {
                    ImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectID = table.Column<int>(type: "int", nullable: false),
                    ImageOrdinal = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))"),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ImageSource = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportOb__7516F4ECD36AFC26", x => x.ImageID);
                    table.ForeignKey(
                        name: "FK__ReportObj__Repor__3A2CDF36",
                        column: x => x.ReportObjectID,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectParameters",
                columns: table => new
                {
                    ReportObjectParameterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectID = table.Column<int>(type: "int", nullable: true),
                    ParameterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParameterValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportObjectParameters", x => x.ReportObjectParameterId);
                    table.ForeignKey(
                        name: "FK_ReportObjectParameters1_ReportObject",
                        column: x => x.ReportObjectID,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectQuery",
                columns: table => new
                {
                    ReportObjectQueryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectId = table.Column<int>(type: "int", nullable: true),
                    Query = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastLoadDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    SourceServer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportObjectQuery", x => x.ReportObjectQueryId);
                    table.ForeignKey(
                        name: "FK_ReportObjectQuery_ReportObject",
                        column: x => x.ReportObjectId,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectReportRunTime",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectId = table.Column<int>(type: "int", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: true),
                    Runs = table.Column<int>(type: "int", nullable: true),
                    RunWeek = table.Column<DateTime>(type: "datetime", nullable: true),
                    RunWeekString = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportObjectReportRunTime", x => x.Id);
                    table.ForeignKey(
                        name: "fk_reportruntime",
                        column: x => x.ReportObjectId,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectRunData",
                columns: table => new
                {
                    ReportObjectID = table.Column<int>(type: "int", nullable: false),
                    RunID = table.Column<long>(type: "bigint", nullable: false),
                    RunUserID = table.Column<int>(type: "int", nullable: true),
                    RunStartTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    RunDurationSeconds = table.Column<int>(type: "int", nullable: true),
                    RunStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastLoadDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportObjectRunData", x => new { x.ReportObjectID, x.RunID });
                    table.ForeignKey(
                        name: "FK__ReportObj__Repor__7B2284FD",
                        column: x => x.ReportObjectID,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                    table.ForeignKey(
                        name: "FK__ReportObj__RunUs__7C16A936",
                        column: x => x.RunUserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectSubscriptions",
                columns: table => new
                {
                    ReportObjectSubscriptionsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    SubscriptionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InactiveFlags = table.Column<int>(type: "int", nullable: true),
                    EmailList = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastRunTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    SubscriptionTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastLoadDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportOb__1AA55D23FE572619", x => x.ReportObjectSubscriptionsId);
                    table.ForeignKey(
                        name: "FK_ReportObjectSubscriptions_ReportObject",
                        column: x => x.ReportObjectId,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                    table.ForeignKey(
                        name: "FK_ReportObjectSubscriptions_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectTagMemberships",
                columns: table => new
                {
                    TagMembershipID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectID = table.Column<int>(type: "int", nullable: false),
                    TagID = table.Column<int>(type: "int", nullable: false),
                    Line = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportObjectTagMemberships", x => x.TagMembershipID);
                    table.ForeignKey(
                        name: "FK_ReportObjectTagMemberships_ReportObject",
                        column: x => x.ReportObjectID,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                    table.ForeignKey(
                        name: "FK_ReportObjectTagMemberships_ReportObjectTags",
                        column: x => x.TagID,
                        principalTable: "ReportObjectTags",
                        principalColumn: "TagID");
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectTopRuns",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RunUserId = table.Column<int>(type: "int", nullable: true),
                    Runs = table.Column<int>(type: "int", nullable: true),
                    RunTime = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    LastRun = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportObjectTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportObjectTopRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportObjectTopRuns_ReportObject",
                        column: x => x.ReportObjectId,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                    table.ForeignKey(
                        name: "fk_user",
                        column: x => x.RunUserId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectWeightedRunRank",
                schema: "app",
                columns: table => new
                {
                    reportobjectid = table.Column<int>(type: "int", nullable: false),
                    weighted_run_rank = table.Column<decimal>(type: "numeric(12,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_ReportObjectWeightedRunRank_ReportObject",
                        column: x => x.reportobjectid,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                });

            migrationBuilder.CreateTable(
                name: "StarredReports",
                schema: "app",
                columns: table => new
                {
                    star_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rank = table.Column<int>(type: "int", nullable: true),
                    reportid = table.Column<int>(type: "int", nullable: true),
                    ownerid = table.Column<int>(type: "int", nullable: true),
                    folderid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StarredR__88222DCE157D560E", x => x.star_id);
                    table.ForeignKey(
                        name: "FK_StarredReports_ReportObject",
                        column: x => x.reportid,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                    table.ForeignKey(
                        name: "FK_StarredReports_User",
                        column: x => x.ownerid,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_StarredReports_UserFavoriteFolders",
                        column: x => x.folderid,
                        principalSchema: "app",
                        principalTable: "UserFavoriteFolders",
                        principalColumn: "UserFavoriteFolderId");
                });

            migrationBuilder.CreateTable(
                name: "StarredTerms",
                schema: "app",
                columns: table => new
                {
                    star_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rank = table.Column<int>(type: "int", nullable: true),
                    termid = table.Column<int>(type: "int", nullable: true),
                    ownerid = table.Column<int>(type: "int", nullable: true),
                    folderid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StarredT__88222DCE11EE382D", x => x.star_id);
                    table.ForeignKey(
                        name: "FK_StarredTerms_Term",
                        column: x => x.termid,
                        principalSchema: "app",
                        principalTable: "Term",
                        principalColumn: "TermId");
                    table.ForeignKey(
                        name: "FK_StarredTerms_User",
                        column: x => x.ownerid,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_StarredTerms_UserFavoriteFolders",
                        column: x => x.folderid,
                        principalSchema: "app",
                        principalTable: "UserFavoriteFolders",
                        principalColumn: "UserFavoriteFolderId");
                });

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
                name: "DP_Agreement",
                schema: "app",
                columns: table => new
                {
                    AgreementID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeetingDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastUpdateUser = table.Column<int>(type: "int", nullable: true),
                    DataProjectId = table.Column<int>(type: "int", nullable: true),
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
                    Rank = table.Column<int>(type: "int", nullable: false),
                    AttachmentData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    AttachmentType = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    AttachmentName = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    AttachmentSize = table.Column<int>(type: "int", nullable: true)
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
                    DataProjectId = table.Column<int>(type: "int", nullable: true),
                    TaskDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    MilestoneChecklistId = table.Column<int>(type: "int", nullable: true),
                    ChecklistStatus = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    CompletionDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CompletionUser = table.Column<int>(type: "int", nullable: true)
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
                    CompletionDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CompletionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime", nullable: true)
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
                name: "DP_ReportAnnotation",
                schema: "app",
                columns: table => new
                {
                    ReportAnnotationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Annotation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportId = table.Column<int>(type: "int", nullable: true),
                    DataProjectId = table.Column<int>(type: "int", nullable: true),
                    Rank = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_Repor__84AFA7F30D34E922", x => x.ReportAnnotationID);
                    table.ForeignKey(
                        name: "FK_DP_ReportAnnotation_DP_DataProject",
                        column: x => x.DataProjectId,
                        principalSchema: "app",
                        principalTable: "DP_DataProject",
                        principalColumn: "DataProjectID");
                    table.ForeignKey(
                        name: "FK_DP_ReportAnnotation_ReportObject",
                        column: x => x.ReportId,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID");
                });

            migrationBuilder.CreateTable(
                name: "DP_TermAnnotation",
                schema: "app",
                columns: table => new
                {
                    TermAnnotationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Annotation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TermId = table.Column<int>(type: "int", nullable: true),
                    DataProjectId = table.Column<int>(type: "int", nullable: true),
                    Rank = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DP_TermA__1BB492E32D415E15", x => x.TermAnnotationID);
                    table.ForeignKey(
                        name: "FK_DP_TermAnnotation_DP_DataProject",
                        column: x => x.DataProjectId,
                        principalSchema: "app",
                        principalTable: "DP_DataProject",
                        principalColumn: "DataProjectID");
                    table.ForeignKey(
                        name: "FK_DP_TermAnnotation_Term",
                        column: x => x.TermId,
                        principalSchema: "app",
                        principalTable: "Term",
                        principalColumn: "TermId");
                });

            migrationBuilder.CreateTable(
                name: "StarredCollections",
                schema: "app",
                columns: table => new
                {
                    star_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rank = table.Column<int>(type: "int", nullable: true),
                    collectionid = table.Column<int>(type: "int", nullable: true),
                    ownerid = table.Column<int>(type: "int", nullable: true),
                    folderid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StarredC__88222DCEA294BB15", x => x.star_id);
                    table.ForeignKey(
                        name: "FK_StarredCollections_DP_DataProject",
                        column: x => x.collectionid,
                        principalSchema: "app",
                        principalTable: "DP_DataProject",
                        principalColumn: "DataProjectID");
                    table.ForeignKey(
                        name: "FK_StarredCollections_User",
                        column: x => x.ownerid,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK_StarredCollections_UserFavoriteFolders",
                        column: x => x.folderid,
                        principalSchema: "app",
                        principalTable: "UserFavoriteFolders",
                        principalColumn: "UserFavoriteFolderId");
                });

            migrationBuilder.CreateTable(
                name: "DP_MilestoneTasks",
                schema: "app",
                columns: table => new
                {
                    MilestoneTaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MilestoneTemplateId = table.Column<int>(type: "int", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastUpdateUser = table.Column<int>(type: "int", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DataProjectId = table.Column<int>(type: "int", nullable: true)
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
                name: "ReportObjectDocFragilityTags",
                schema: "app",
                columns: table => new
                {
                    LinkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectID = table.Column<int>(type: "int", nullable: false),
                    FragilityTagID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportOb__2D122135B03BB8CE", x => x.LinkId);
                    table.ForeignKey(
                        name: "FK__ReportObj__Fragi__71EFB72C",
                        column: x => x.FragilityTagID,
                        principalSchema: "app",
                        principalTable: "FragilityTag",
                        principalColumn: "FragilityTagID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__ReportObj__Repor__72E3DB65",
                        column: x => x.ReportObjectID,
                        principalSchema: "app",
                        principalTable: "ReportObject_doc",
                        principalColumn: "ReportObjectID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectDocMaintenanceLogs",
                schema: "app",
                columns: table => new
                {
                    LinkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectID = table.Column<int>(type: "int", nullable: false),
                    MaintenanceLogID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportOb__2D1221350C7530C5", x => x.LinkId);
                    table.ForeignKey(
                        name: "FK__ReportObj__Maint__6E1F2648",
                        column: x => x.MaintenanceLogID,
                        principalSchema: "app",
                        principalTable: "MaintenanceLog",
                        principalColumn: "MaintenanceLogID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__ReportObj__Repor__6F134A81",
                        column: x => x.ReportObjectID,
                        principalSchema: "app",
                        principalTable: "ReportObject_doc",
                        principalColumn: "ReportObjectID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportObjectDocTerms",
                schema: "app",
                columns: table => new
                {
                    LinkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportObjectID = table.Column<int>(type: "int", nullable: false),
                    TermId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportOb__2D122135AFCD5E79", x => x.LinkId);
                    table.ForeignKey(
                        name: "FK__ReportObj__Repor__6A4E9564",
                        column: x => x.ReportObjectID,
                        principalSchema: "app",
                        principalTable: "ReportObject_doc",
                        principalColumn: "ReportObjectID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__ReportObj__TermI__6B42B99D",
                        column: x => x.TermId,
                        principalSchema: "app",
                        principalTable: "Term",
                        principalColumn: "TermId",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "DP_AgreementUsers",
                schema: "app",
                columns: table => new
                {
                    AgreementUsersID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgreementID = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastUpdateUser = table.Column<int>(type: "int", nullable: true)
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
                name: "accessdatetime",
                schema: "app",
                table: "Analytics",
                column: "accessDateTime");

            migrationBuilder.CreateIndex(
                name: "userid",
                schema: "app",
                table: "Analytics",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsTrace_UserId",
                schema: "app",
                table: "AnalyticsTrace",
                column: "UserId");

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

            migrationBuilder.CreateIndex(
                name: "executiveownerid",
                schema: "app",
                table: "DP_DataInitiative",
                column: "ExecutiveOwnerID");

            migrationBuilder.CreateIndex(
                name: "financialimpact",
                schema: "app",
                table: "DP_DataInitiative",
                column: "FinancialImpact");

            migrationBuilder.CreateIndex(
                name: "initiativeid",
                schema: "app",
                table: "DP_DataInitiative",
                column: "DataInitiativeID");

            migrationBuilder.CreateIndex(
                name: "lastupdatedate",
                schema: "app",
                table: "DP_DataInitiative",
                column: "LastUpdateDate");

            migrationBuilder.CreateIndex(
                name: "lastupdateuser",
                schema: "app",
                table: "DP_DataInitiative",
                column: "LastUpdateUser");

            migrationBuilder.CreateIndex(
                name: "operationownderid",
                schema: "app",
                table: "DP_DataInitiative",
                column: "OperationOwnerID");

            migrationBuilder.CreateIndex(
                name: "strategicimportance",
                schema: "app",
                table: "DP_DataInitiative",
                column: "StrategicImportance");

            migrationBuilder.CreateIndex(
                name: "analyticsownerid",
                schema: "app",
                table: "DP_DataProject",
                column: "AnalyticsOwnerID");

            migrationBuilder.CreateIndex(
                name: "collectionid",
                schema: "app",
                table: "DP_DataProject",
                column: "DataProjectID");

            migrationBuilder.CreateIndex(
                name: "datamanagerid",
                schema: "app",
                table: "DP_DataProject",
                column: "DataManagerID");

            migrationBuilder.CreateIndex(
                name: "executiveownerid1",
                schema: "app",
                table: "DP_DataProject",
                column: "ExecutiveOwnerID");

            migrationBuilder.CreateIndex(
                name: "financialimpact1",
                schema: "app",
                table: "DP_DataProject",
                column: "FinancialImpact");

            migrationBuilder.CreateIndex(
                name: "initiativeid1",
                schema: "app",
                table: "DP_DataProject",
                column: "DataInitiativeID");

            migrationBuilder.CreateIndex(
                name: "lastupdatedate1",
                schema: "app",
                table: "DP_DataProject",
                column: "LastUpdateDate");

            migrationBuilder.CreateIndex(
                name: "lastupdateuser1",
                schema: "app",
                table: "DP_DataProject",
                column: "LastUpdateUser");

            migrationBuilder.CreateIndex(
                name: "operationownerid",
                schema: "app",
                table: "DP_DataProject",
                column: "OperationOwnerID");

            migrationBuilder.CreateIndex(
                name: "strategicimportance1",
                schema: "app",
                table: "DP_DataProject",
                column: "StrategicImportance");

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

            migrationBuilder.CreateIndex(
                name: "IX_DP_ReportAnnotation_DataProjectId",
                schema: "app",
                table: "DP_ReportAnnotation",
                column: "DataProjectId");

            migrationBuilder.CreateIndex(
                name: "reportid+dataprojectid",
                schema: "app",
                table: "DP_ReportAnnotation",
                columns: new[] { "ReportId", "DataProjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_DP_TermAnnotation_DataProjectId",
                schema: "app",
                table: "DP_TermAnnotation",
                column: "DataProjectId");

            migrationBuilder.CreateIndex(
                name: "termid+dataprojectid",
                schema: "app",
                table: "DP_TermAnnotation",
                columns: new[] { "TermId", "DataProjectId" });

            migrationBuilder.CreateIndex(
                name: "estimatedrunfrequencyid",
                schema: "app",
                table: "EstimatedRunFrequency",
                column: "EstimatedRunFrequencyID");

            migrationBuilder.CreateIndex(
                name: "financialimpactid",
                schema: "app",
                table: "FinancialImpact",
                column: "FinancialImpactId");

            migrationBuilder.CreateIndex(
                name: "fragilityid",
                schema: "app",
                table: "Fragility",
                column: "FragilityID");

            migrationBuilder.CreateIndex(
                name: "fragilitytagid",
                schema: "app",
                table: "FragilityTag",
                column: "FragilityTagID");

            migrationBuilder.CreateIndex(
                name: "IX_Mail_Conversations_MessageId",
                schema: "app",
                table: "Mail_Conversations",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Mail_Drafts_FromUserId",
                schema: "app",
                table: "Mail_Drafts",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Mail_FolderMessages_FolderId",
                schema: "app",
                table: "Mail_FolderMessages",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Mail_FolderMessages_MessageId",
                schema: "app",
                table: "Mail_FolderMessages",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Mail_Folders_UserId",
                schema: "app",
                table: "Mail_Folders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Mail_Messages_FromUserId",
                schema: "app",
                table: "Mail_Messages",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Mail_Messages_MessageTypeId",
                schema: "app",
                table: "Mail_Messages",
                column: "MessageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Mail_Recipients_MessageId",
                schema: "app",
                table: "Mail_Recipients",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Mail_Recipients_ToGroupId",
                schema: "app",
                table: "Mail_Recipients",
                column: "ToGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Mail_Recipients_ToUserId",
                schema: "app",
                table: "Mail_Recipients",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Mail_Recipients_Deleted_ToUserId",
                schema: "app",
                table: "Mail_Recipients_Deleted",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "logid",
                schema: "app",
                table: "MaintenanceLog",
                column: "MaintenanceLogID");

            migrationBuilder.CreateIndex(
                name: "maintainerid",
                schema: "app",
                table: "MaintenanceLog",
                column: "MaintainerID");

            migrationBuilder.CreateIndex(
                name: "maintenancedate",
                schema: "app",
                table: "MaintenanceLog",
                column: "MaintenanceDate");

            migrationBuilder.CreateIndex(
                name: "maintenancelogstatusid",
                schema: "app",
                table: "MaintenanceLog",
                column: "MaintenanceLogStatusID");

            migrationBuilder.CreateIndex(
                name: "maintenancelogstatusid1",
                schema: "app",
                table: "MaintenanceLogStatus",
                column: "MaintenanceLogStatusID");

            migrationBuilder.CreateIndex(
                name: "maintenancescheduleid",
                schema: "app",
                table: "MaintenanceSchedule",
                column: "MaintenanceScheduleID");

            migrationBuilder.CreateIndex(
                name: "organizationalvalueid",
                schema: "app",
                table: "OrganizationalValue",
                column: "OrganizationalValueID");

            migrationBuilder.CreateIndex(
                name: "certid",
                table: "ReportCertificationTags",
                column: "Cert_ID");

            migrationBuilder.CreateIndex(
                name: "groupid+reportid",
                table: "ReportGroupsMemberships",
                columns: new[] { "GroupId", "ReportId" });

            migrationBuilder.CreateIndex(
                name: "reportid",
                table: "ReportGroupsMemberships",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "reportobjectid",
                schema: "app",
                table: "ReportManageEngineTickets",
                column: "ReportObjectId");

            migrationBuilder.CreateIndex(
                name: "authorid",
                table: "ReportObject",
                column: "AuthorUserID");

            migrationBuilder.CreateIndex(
                name: "certid1",
                table: "ReportObject",
                column: "CertificationTagID");

            migrationBuilder.CreateIndex(
                name: "defaultvisiblity",
                table: "ReportObject",
                column: "DefaultVisibilityYN");

            migrationBuilder.CreateIndex(
                name: "epicmasterfile",
                table: "ReportObject",
                column: "EpicMasterFile");

            migrationBuilder.CreateIndex(
                name: "epicmasterfile + reportid",
                table: "ReportObject",
                column: "EpicMasterFile");

            migrationBuilder.CreateIndex(
                name: "modifiedby",
                table: "ReportObject",
                column: "LastModifiedByUserID");

            migrationBuilder.CreateIndex(
                name: "reportid1",
                table: "ReportObject",
                column: "ReportObjectID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "typeid",
                table: "ReportObject",
                column: "ReportObjectTypeID");

            migrationBuilder.CreateIndex(
                name: "visibility + orphan + type",
                table: "ReportObject",
                columns: new[] { "DefaultVisibilityYN", "OrphanedReportObjectYN", "ReportObjectTypeID" });

            migrationBuilder.CreateIndex(
                name: "createdby",
                schema: "app",
                table: "ReportObject_doc",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "estimatedrunfreqid",
                schema: "app",
                table: "ReportObject_doc",
                column: "EstimatedRunFrequencyID");

            migrationBuilder.CreateIndex(
                name: "fragilityid1",
                schema: "app",
                table: "ReportObject_doc",
                column: "FragilityID");

            migrationBuilder.CreateIndex(
                name: "maintenancescheduleid1",
                schema: "app",
                table: "ReportObject_doc",
                column: "MaintenanceScheduleID");

            migrationBuilder.CreateIndex(
                name: "operationalownerid",
                schema: "app",
                table: "ReportObject_doc",
                column: "OperationalOwnerUserID");

            migrationBuilder.CreateIndex(
                name: "organizationalvalueid1",
                schema: "app",
                table: "ReportObject_doc",
                column: "OrganizationalValueID");

            migrationBuilder.CreateIndex(
                name: "reportid3",
                schema: "app",
                table: "ReportObject_doc",
                column: "ReportObjectID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "requester",
                schema: "app",
                table: "ReportObject_doc",
                column: "Requester");

            migrationBuilder.CreateIndex(
                name: "updatedby",
                schema: "app",
                table: "ReportObject_doc",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "reportid2",
                table: "ReportObjectAttachments",
                column: "ReportObjectId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectDocFragilityTags_FragilityTagID",
                schema: "app",
                table: "ReportObjectDocFragilityTags",
                column: "FragilityTagID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectDocFragilityTags_ReportObjectID",
                schema: "app",
                table: "ReportObjectDocFragilityTags",
                column: "ReportObjectID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectDocMaintenanceLogs_MaintenanceLogID",
                schema: "app",
                table: "ReportObjectDocMaintenanceLogs",
                column: "MaintenanceLogID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectDocMaintenanceLogs_ReportObjectID",
                schema: "app",
                table: "ReportObjectDocMaintenanceLogs",
                column: "ReportObjectID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectDocTerms_ReportObjectID",
                schema: "app",
                table: "ReportObjectDocTerms",
                column: "ReportObjectID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectDocTerms_TermId",
                schema: "app",
                table: "ReportObjectDocTerms",
                column: "TermId");

            migrationBuilder.CreateIndex(
                name: "childid",
                table: "ReportObjectHierarchy",
                column: "ChildReportObjectID");

            migrationBuilder.CreateIndex(
                name: "parent+child",
                table: "ReportObjectHierarchy",
                columns: new[] { "ParentReportObjectID", "ChildReportObjectID" });

            migrationBuilder.CreateIndex(
                name: "parentid",
                table: "ReportObjectHierarchy",
                column: "ParentReportObjectID");

            migrationBuilder.CreateIndex(
                name: "reportid4",
                schema: "app",
                table: "ReportObjectImages_doc",
                column: "ReportObjectID");

            migrationBuilder.CreateIndex(
                name: "reportobjectid1",
                table: "ReportObjectParameters",
                column: "ReportObjectID");

            migrationBuilder.CreateIndex(
                name: "NonClusteredIndex-20220324-104152",
                table: "ReportObjectQuery",
                column: "ReportObjectId");

            migrationBuilder.CreateIndex(
                name: "queryid",
                table: "ReportObjectQuery",
                column: "ReportObjectQueryId");

            migrationBuilder.CreateIndex(
                name: "reportobjectid2",
                table: "ReportObjectQuery",
                column: "ReportObjectId");

            migrationBuilder.CreateIndex(
                name: "reportid5",
                schema: "app",
                table: "ReportObjectReportRunTime",
                column: "ReportObjectId");

            migrationBuilder.CreateIndex(
                name: "reportid6",
                table: "ReportObjectRunData",
                column: "ReportObjectID");

            migrationBuilder.CreateIndex(
                name: "runuser + report",
                table: "ReportObjectRunData",
                column: "RunUserID");

            migrationBuilder.CreateIndex(
                name: "userid1",
                schema: "app",
                table: "ReportObjectRunTime",
                column: "RunUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectSubscriptions_UserId",
                table: "ReportObjectSubscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "reportid + userid",
                table: "ReportObjectSubscriptions",
                columns: new[] { "ReportObjectId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectTagMemberships_TagID",
                table: "ReportObjectTagMemberships",
                column: "TagID");

            migrationBuilder.CreateIndex(
                name: "tagid+reportid",
                table: "ReportObjectTagMemberships",
                columns: new[] { "ReportObjectID", "TagID" });

            migrationBuilder.CreateIndex(
                name: "tagid",
                table: "ReportObjectTags",
                column: "TagID");

            migrationBuilder.CreateIndex(
                name: "report+user+type",
                schema: "app",
                table: "ReportObjectTopRuns",
                column: "ReportObjectId");

            migrationBuilder.CreateIndex(
                name: "reportid + columns",
                schema: "app",
                table: "ReportObjectTopRuns",
                column: "ReportObjectId");

            migrationBuilder.CreateIndex(
                name: "userid + columns",
                schema: "app",
                table: "ReportObjectTopRuns",
                column: "RunUserId");

            migrationBuilder.CreateIndex(
                name: "typeid1",
                table: "ReportObjectType",
                column: "ReportObjectTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportObjectWeightedRunRank_reportobjectid",
                schema: "app",
                table: "ReportObjectWeightedRunRank",
                column: "reportobjectid");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionLinks_RoleId",
                schema: "app",
                table: "RolePermissionLinks",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionLinks_RolePermissionsId",
                schema: "app",
                table: "RolePermissionLinks",
                column: "RolePermissionsId");

            migrationBuilder.CreateIndex(
                name: "from + to + date",
                schema: "app",
                table: "SharedItems",
                columns: new[] { "SharedFromUserId", "SharedToUserId", "ShareDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SharedItems_SharedToUserId",
                schema: "app",
                table: "SharedItems",
                column: "SharedToUserId");

            migrationBuilder.CreateIndex(
                name: "collectionid + ownerid",
                schema: "app",
                table: "StarredCollections",
                columns: new[] { "collectionid", "ownerid" });

            migrationBuilder.CreateIndex(
                name: "IX_StarredCollections_folderid",
                schema: "app",
                table: "StarredCollections",
                column: "folderid");

            migrationBuilder.CreateIndex(
                name: "IX_StarredCollections_ownerid",
                schema: "app",
                table: "StarredCollections",
                column: "ownerid");

            migrationBuilder.CreateIndex(
                name: "starid",
                schema: "app",
                table: "StarredCollections",
                column: "star_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "folderid",
                schema: "app",
                table: "StarredGroups",
                column: "folderid");

            migrationBuilder.CreateIndex(
                name: "groupid + ownerid",
                schema: "app",
                table: "StarredGroups",
                columns: new[] { "groupid", "ownerid" });

            migrationBuilder.CreateIndex(
                name: "IX_StarredGroups_ownerid",
                schema: "app",
                table: "StarredGroups",
                column: "ownerid");

            migrationBuilder.CreateIndex(
                name: "starid1",
                schema: "app",
                table: "StarredGroups",
                column: "star_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "folderid1",
                schema: "app",
                table: "StarredInitiatives",
                column: "folderid");

            migrationBuilder.CreateIndex(
                name: "initiativeid + ownerid",
                schema: "app",
                table: "StarredInitiatives",
                columns: new[] { "initiativeid", "ownerid" });

            migrationBuilder.CreateIndex(
                name: "IX_StarredInitiatives_ownerid",
                schema: "app",
                table: "StarredInitiatives",
                column: "ownerid");

            migrationBuilder.CreateIndex(
                name: "starid2",
                schema: "app",
                table: "StarredInitiatives",
                column: "star_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "folderid2",
                schema: "app",
                table: "StarredReports",
                column: "folderid");

            migrationBuilder.CreateIndex(
                name: "IX_StarredReports_ownerid",
                schema: "app",
                table: "StarredReports",
                column: "ownerid");

            migrationBuilder.CreateIndex(
                name: "reportid + ownerid",
                schema: "app",
                table: "StarredReports",
                columns: new[] { "reportid", "ownerid" });

            migrationBuilder.CreateIndex(
                name: "starid3",
                schema: "app",
                table: "StarredReports",
                column: "star_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StarredSearches_folderid",
                schema: "app",
                table: "StarredSearches",
                column: "folderid");

            migrationBuilder.CreateIndex(
                name: "ownerid",
                schema: "app",
                table: "StarredSearches",
                column: "ownerid");

            migrationBuilder.CreateIndex(
                name: "starid4",
                schema: "app",
                table: "StarredSearches",
                column: "star_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "folderid3",
                schema: "app",
                table: "StarredTerms",
                column: "folderid");

            migrationBuilder.CreateIndex(
                name: "IX_StarredTerms_ownerid",
                schema: "app",
                table: "StarredTerms",
                column: "ownerid");

            migrationBuilder.CreateIndex(
                name: "starid5",
                schema: "app",
                table: "StarredTerms",
                column: "star_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "termid + ownerid",
                schema: "app",
                table: "StarredTerms",
                columns: new[] { "termid", "ownerid" });

            migrationBuilder.CreateIndex(
                name: "folderid4",
                schema: "app",
                table: "StarredUsers",
                column: "folderid");

            migrationBuilder.CreateIndex(
                name: "IX_StarredUsers_ownerid",
                schema: "app",
                table: "StarredUsers",
                column: "ownerid");

            migrationBuilder.CreateIndex(
                name: "ownerid + userid",
                schema: "app",
                table: "StarredUsers",
                columns: new[] { "userid", "ownerid" });

            migrationBuilder.CreateIndex(
                name: "starid6",
                schema: "app",
                table: "StarredUsers",
                column: "star_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "strategicimportanceid",
                schema: "app",
                table: "StrategicImportance",
                column: "StrategicImportanceId");

            migrationBuilder.CreateIndex(
                name: "approved",
                schema: "app",
                table: "Term",
                column: "ApprovedYN");

            migrationBuilder.CreateIndex(
                name: "approvedby",
                schema: "app",
                table: "Term",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "termid",
                schema: "app",
                table: "Term",
                column: "TermId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "updatedby1",
                schema: "app",
                table: "Term",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "validfrom",
                schema: "app",
                table: "Term",
                column: "ValidFromDateTime");

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

            migrationBuilder.CreateIndex(
                name: "userid2",
                table: "User",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "userid3",
                schema: "app",
                table: "UserFavoriteFolders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_FolderId",
                schema: "app",
                table: "UserFavorites",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_UserId",
                schema: "app",
                table: "UserFavorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "groupid",
                table: "UserGroups",
                column: "GroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "groupdid + userid",
                table: "UserGroupsMembership",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "userid+groupid",
                table: "UserGroupsMembership",
                columns: new[] { "UserId", "GroupId" });

            migrationBuilder.CreateIndex(
                name: "itemvalue + itemid + userid",
                schema: "app",
                table: "UserPreferences",
                columns: new[] { "ItemValue", "ItemId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId",
                schema: "app",
                table: "UserPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleLinks_UserRolesId",
                schema: "app",
                table: "UserRoleLinks",
                column: "UserRolesId");

            migrationBuilder.CreateIndex(
                name: "userid+roleid",
                schema: "app",
                table: "UserRoleLinks",
                columns: new[] { "UserId", "UserRolesId" });

            migrationBuilder.CreateIndex(
                name: "roleid",
                schema: "app",
                table: "UserRoles",
                column: "UserRolesId");

            // seed insert
            // Report Types
            migrationBuilder.Sql(
            @"
                insert into [dbo].[ReportObjectType] ([Name], [DefaultEpicMasterFile], [ShortName], [Visible]) values
                ('Application Report','HRX', null, null),
                ('Application Report Template','HGR', 'RW Template', null),
                ('Epic-Crystal Report','HRX', 'Crystal Report', 'Y'),
                ('Epic-Crystal Template','HGR', 'Crystal Template', null),
                ('Epic-WebI Report','HRX', 'RW Report', null),
                ('Epic-WebI Template','HGR', 'RW Template', null),
                ('External Crystal Template','HGR', 'RW Template', null),
                ('External WebI Template','HGR', 'RW Template', null),
                ('Other HGR Template','HGR', 'RW Template', null),
                ('Other HRX Report','HRX', 'RW Report', null),
                ('Other Radar Dashboard','IDM', 'Radar Dashboard', null),
                ('Personalization Radar Dashboard','IDM', 'Radar Dashboard', null),
                ('Personalization Radar Dashboard Component','IDB', 'Radar Component', null),
                ('Radar Dashboard Resource','IDK', 'Radar Resource', null),
                ('Radar Metric','IDN', null, null),
                ('Redirector Radar Dashboard','IDM', 'Radar Dashboard', null),
                ('Reporting Workbench Report','HRX', 'RW Report', 'Y'),
                ('Reporting Workbench Template','HGR', 'RW Template', null),
                ('SlicerDicer Filter','FDS', null, null),
                ('Source Radar Dashboard','IDM', null, 'Y'),
                ('Source Radar Dashboard Component','IDB', null, 'Y'),
                ('SSRS Datasource',null, null, null),
                ('SSRS File',null, null, 'Y'),
                ('SSRS Folder',null, null, null),
                ('SSRS KPI',null, null, null),
                ('SSRS Linked Report',null, null, null),
                ('SSRS Mobile Report (folder)',null, 'SSRS Folder', null),
                ('SSRS Report',null, null, 'Y'),
                ('SSRS Shared Dataset',null, null, null),
                ('User Created Radar Dashboard','IDM', 'Radar Dashboard', null),
                ('SSRS Report Part',null, null, null),
                ('Other Radar Dashboard Component','IDB', 'Radar Component', null),
                ('User Created Radar Dashboard Component','IDB', 'Radar Component', null),
                ('Datalink Template','HGR', null, null),
                ('Datalink Report','HRX', null, null),
                ('Tableau Workbook',null, null, 'Y'),
                ('SlicerDicer Model','FDM', null, 'Y'),
                ('Tableau Dashboard',null, null, null),
                ('Reporting Workbench Extension','LPP', 'RW Extension', null),
                ('Reporting Workbench Column','PAF', 'RW Column', null),
                ('SlicerDicer Session','HRX', null, 'Y'),
                ('SSAS Cubes',null, 'SASS Cube', 'Y')
            ");

            // user roles
            migrationBuilder.Sql(
            @"
                insert into app.UserRoles (Name, Description) values
                ('Administrator','Administrators have the highest priveleges and are not prevented from taking any available actions.'),
                ('Report Writer','Report Writers can create and edit ReportObject documentation and terms, but cannot approve Terms, edit approved Terms, or delete things they do not own.'),
                ('Term Administrator','Term Admins can create, edit, approve and delete Terms.'),
                ('Term Builder','Term Builders can create and edit Term documentation, but cannot approve them, edit them after approval, or link them to ReportObjects.'),
                ('User','Users do not have any special permissions. They can navigate the site, comment, and view approved Terms and all non-hidden ReportObjects.')
            ");

            // create security points
            migrationBuilder.Sql(
            @"
                insert into app.RolePermissions ([Name], Description) values
                ('Edit User Permissions', 'NULL'),
                ('Approve Terms', 'NULL'),
                ('Create New Terms', 'NULL'),
                ('Delete Approved Terms', 'NULL'),
                ('Delete Unapproved Terms', 'NULL'),
                ('Edit Approved ReportObjects Documentation', 'NULL'),
                ('Edit Approved Terms', 'NULL'),
                ('Edit Report Documentation', 'NULL'),
                ('Edit Unapproved Terms', 'NULL'),
                ('Link/Unlink Terms and ReportObjects', 'NULL'),
                ('View Hidden ReportObjects', 'NULL'),
                ('View Unapproved Terms', 'NULL'),
                ('Edit Role Permissions', 'NULL'),
                ('Search', 'NULL'),
                ('Create Initiative', 'NULL'),
                ('Delete Initiative', 'NULL'),
                ('Edit Initiative', 'NULL'),
                ('Create Milestone Template', 'NULL'),
                ('Delete Milestone Template', 'NULL'),
                ('Delete Comments', 'NULL'),
                ('Create Project', 'NULL'),
                ('Delete Project', 'NULL'),
                ('Edit Project', 'NULL'),
                ('Complete Milestone Task Checklist Item', 'NULL'),
                ('Complete Milestone Task', 'NULL'),
                ('Create Contacts', 'NULL'),
                ('Delete Contacts', 'NULL'),
                ('Create Parameters', 'NULL'),
                ('Delete Parameters', 'NULL'),
                ('Edit Report Purge Option', 'NULL'),
                ('View Site Analytics', 'NULL'),
                ('View Other User', 'NULL'),
                ('Open In Editor', 'NULL'),
                ('View Report Profiles', 'NULL'),
                ('Search For Other User', 'NULL'),
                ('Edit Other Users', 'NULL'),
                ('Uncomplete Milestone Task Checklist Item', 'NULL'),
                ('Edit Report Hidden Option', 'NULL'),
                ('Can Change Roles', 'NULL'),
                ('Manage Global Site Settings', 'NULL')
            ");

            // add some user roles
            migrationBuilder.Sql(
            @"
                insert into app.RolePermissionLinks (RoleId, RolePermissionsId) values
                (3,7),
                (3,10),
                (3,12),
                (3,13),
                (3,14),
                (3,16),
                (3,17),
                (4,6),
                (4,7),
                (4,9),
                (4,11),
                (4,13),
                (4,17),
                (5,17),
                (5,19),
                (2,19),
                (3,19),
                (4,19),
                (5,39),
                (2,37),
                (3,37),
                (4,37),
                (5,37),
                (2,39),
                (3,39),
                (4,39),
                (2,40),
                (3,40),
                (4,40),
                (5,40)
            ");

            // add org value
            migrationBuilder.Sql(
            @"
                insert into app.OrganizationalValue (OrganizationalValueName) values ('Critical'),
                ('High'),
                ('Medium-High'),
                ('Medium')
            ");

            // add maint schedule
            migrationBuilder.Sql(
            @"
                insert into app.MaintenanceSchedule (MaintenanceScheduleName) values
                ('Quarterly'),
                ('Twice a Year'),
                ('Yearly'),
                ('Every Two Years'),
                ('Audit Only')
            ");

            // add log status
            migrationBuilder.Sql(
            @"
                insert into app.MaintenanceLogStatus (MaintenanceLogStatusName) VALUES
                ('Approved - No Changes'),
                ('Approved - With Changes'),
                ('Recommend Retire')
            ");

            // add fragility
            migrationBuilder.Sql(
            @"
                insert into app.Fragility (FragilityName) VALUES
                ('High'),
                ('Medium'),
                ('Low'),
                ('Very Low')
            ");

            // add fragility tags
            migrationBuilder.Sql(
            @"
                insert into app.FragilityTag (FragilityTagName) VALUES
                ('Facility Build'),
                ('Procedure Code (CPT)'),
                ('Procedure Code (ICD)'),
                ('Procedure Code (Proc ID)'),
                ('Diagnosis Code'),
                ('Regulatory Changes'),
                ('Payor Plan Name'),
                ('Agency Build'),
                ('Service Area'),
                ('SER Build'),
                ('Workqueues'),
                ('Billing Indicators'),
                ('Batches'),
                ('M-Code'),
                ('Patient Class'),
                ('Bad Debt'),
                ('New Application'),
                ('Rules Engine'),
                ('Registry'),
                ('SQL'),
                ('Free Text'),
                ('ClarityREF Dependencies'),
                ('External Application'),
                ('Flowsheets'),
                ('Primary Care Provider'),
                ('Order Sets'),
                ('Workflow Dependent')
            ");

            // financial impact
            migrationBuilder.Sql(
            @"
                insert into app.FinancialImpact ([Name]) values
                ('Medium'),
                ('High'),
                ('Critical')
            ");

            // est run freq
            migrationBuilder.Sql(
            @"
                insert into app.EstimatedRunFrequency (EstimatedRunFrequencyName) VALUES
                ('Multiple Times Per Day'),
                ('Daily'),
                ('Weekly'),
                ('Monthly'),
                ('Yearly')
            ");

            //Strategic importance
            migrationBuilder.Sql(
            @"
                insert into app.StrategicImportance ([Name]) VALUES
                ('Medium'),
                ('High'),
                ('Critical')
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // first migration is fake, database already existed
            migrationBuilder.DropTable(
                name: "Analytics",
                schema: "app");

            migrationBuilder.DropTable(
                name: "AnalyticsTrace",
                schema: "app");

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
                name: "DP_ReportAnnotation",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_TermAnnotation",
                schema: "app");

            migrationBuilder.DropTable(
                name: "GlobalSiteSettings",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Mail_Conversations",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Mail_Drafts",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Mail_FolderMessages",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Mail_Recipients",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Mail_Recipients_Deleted",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportCertificationTags");

            migrationBuilder.DropTable(
                name: "ReportGroupsMemberships");

            migrationBuilder.DropTable(
                name: "ReportManageEngineTickets",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectAttachments");

            migrationBuilder.DropTable(
                name: "ReportObjectConversationMessage_doc",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectDocFragilityTags",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectDocMaintenanceLogs",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectDocTerms",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectHierarchy");

            migrationBuilder.DropTable(
                name: "ReportObjectImages_doc",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectParameters");

            migrationBuilder.DropTable(
                name: "ReportObjectQuery");

            migrationBuilder.DropTable(
                name: "ReportObjectReportRunTime",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectRunData");

            migrationBuilder.DropTable(
                name: "ReportObjectRunTime",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectSubscriptions");

            migrationBuilder.DropTable(
                name: "ReportObjectTagMemberships");

            migrationBuilder.DropTable(
                name: "ReportObjectTopRuns",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectWeightedRunRank",
                schema: "app");

            migrationBuilder.DropTable(
                name: "RolePermissionLinks",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Search_BasicSearchData",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Search_BasicSearchData_Small",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Search_ReportObjectSearchData",
                schema: "app");

            migrationBuilder.DropTable(
                name: "SearchTable",
                schema: "app");

            migrationBuilder.DropTable(
                name: "SharedItems",
                schema: "app");

            migrationBuilder.DropTable(
                name: "StarredCollections",
                schema: "app");

            migrationBuilder.DropTable(
                name: "StarredGroups",
                schema: "app");

            migrationBuilder.DropTable(
                name: "StarredInitiatives",
                schema: "app");

            migrationBuilder.DropTable(
                name: "StarredReports",
                schema: "app");

            migrationBuilder.DropTable(
                name: "StarredSearches",
                schema: "app");

            migrationBuilder.DropTable(
                name: "StarredTerms",
                schema: "app");

            migrationBuilder.DropTable(
                name: "StarredUsers",
                schema: "app");

            migrationBuilder.DropTable(
                name: "TermConversationMessage",
                schema: "app");

            migrationBuilder.DropTable(
                name: "User_NameData",
                schema: "app");

            migrationBuilder.DropTable(
                name: "UserFavorites",
                schema: "app");

            migrationBuilder.DropTable(
                name: "UserGroupsMembership");

            migrationBuilder.DropTable(
                name: "UserPreferences",
                schema: "app");

            migrationBuilder.DropTable(
                name: "UserRoleLinks",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_Agreement",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_Contact",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Dp_DataProjectConversation",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_MilestoneTasks",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Mail_Folders",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Mail_Messages",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectConversation_doc",
                schema: "app");

            migrationBuilder.DropTable(
                name: "FragilityTag",
                schema: "app");

            migrationBuilder.DropTable(
                name: "MaintenanceLog",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObject_doc",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectTags");

            migrationBuilder.DropTable(
                name: "RolePermissions",
                schema: "app");

            migrationBuilder.DropTable(
                name: "TermConversation",
                schema: "app");

            migrationBuilder.DropTable(
                name: "UserFavoriteFolders",
                schema: "app");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_DataProject",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_MilestoneTemplates",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Mail_MessageType",
                schema: "app");

            migrationBuilder.DropTable(
                name: "MaintenanceLogStatus",
                schema: "app");

            migrationBuilder.DropTable(
                name: "EstimatedRunFrequency",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Fragility",
                schema: "app");

            migrationBuilder.DropTable(
                name: "MaintenanceSchedule",
                schema: "app");

            migrationBuilder.DropTable(
                name: "OrganizationalValue",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObject");

            migrationBuilder.DropTable(
                name: "Term",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_DataInitiative",
                schema: "app");

            migrationBuilder.DropTable(
                name: "DP_MilestoneFrequency",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ReportObjectType");

            migrationBuilder.DropTable(
                name: "FinancialImpact",
                schema: "app");

            migrationBuilder.DropTable(
                name: "StrategicImportance",
                schema: "app");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
