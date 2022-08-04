using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class AddGroupPermissionSecurityPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"insert into app.rolepermissions  (Name) values ('Edit Group Permissions')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.Sql(@"delete from app.rolepermissions  where Name = 'Edit Group Permissions'");
        }
    }
}
