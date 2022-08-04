using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class AddETLEditPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            @"
            insert into app.RolePermissions (Name) values ('Manage ETL');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.Sql(
            @"
            delete from app.RolePermissions where name ='Manage ETL';
            ");
        }
    }
}
