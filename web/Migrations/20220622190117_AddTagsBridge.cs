using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class AddTagsBridge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportCertificationTags");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    ShowInHeader = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "ReportTagLinks",
                columns: table => new
                {
                    ReportTagLinkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false),
                    ShowInHeader = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTagLinks", x => x.ReportTagLinkId);
                    table.ForeignKey(
                        name: "FK_ReportTagLinks_ReportObject_ReportId",
                        column: x => x.ReportId,
                        principalTable: "ReportObject",
                        principalColumn: "ReportObjectID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportTagLinks_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportTagLinks_TagId",
                table: "ReportTagLinks",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "report_tag",
                table: "ReportTagLinks",
                columns: new[] { "ReportId", "TagId" });

            migrationBuilder.CreateIndex(
                name: "tagid1",
                table: "Tags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "tagname",
                table: "Tags",
                column: "Name");

            migrationBuilder.Sql("insert into Tags (Name,Description,Priority) values ('Analytics Certified','This report has received the highest level of scrutiny from the Analytics Team and the owning end user. Certified reports are the most reliable and accurate in the system.',1)");
            migrationBuilder.Sql("insert into Tags (Name,Description,Priority) values ('Analytics Reviewed','This report has gone through the standard Analytics code review and validation process. Reviewed reports can be trusted for monitoring most operational processes.',2)");
            migrationBuilder.Sql("insert into Tags (Name,Description,Priority) values ('Epic Released','This report has not been validated by the Analytics team for accuracy and reliability. Use with caution.',3)");
            migrationBuilder.Sql("insert into Tags (Name,Description,Priority) values ('Legacy','This report has not been validated by the Analytics team for accuracy and reliability. Use with caution.',4)");
            migrationBuilder.Sql("insert into Tags (Name,Description,Priority) values ('High Risk','Data has been validated but content produced via self-service is the responsibility of the end user.',5)");
            migrationBuilder.Sql("insert into Tags (Name,Description,Priority) values ('Self-Service','Data has been validated but content produced via self-service is the responsibility of the end user.',6)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportTagLinks");

            migrationBuilder.DropTable(
                name: "Tags");

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

            migrationBuilder.CreateIndex(
                name: "certid",
                table: "ReportCertificationTags",
                column: "Cert_ID");
        }
    }
}
