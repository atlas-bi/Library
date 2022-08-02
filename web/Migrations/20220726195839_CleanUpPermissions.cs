using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas_Web.Migrations
{
    public partial class CleanUpPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            delete
            from app.RolePermissions
            where name in (
            'Complete Milestone Task',
            'Complete Milestone Task Checklist Item',
            'Create Contacts',
            'Create Milestone Template',
            'Delete Milestone Template',
            'Edit Approved ReportObjects Documentation',
            'Uncomplete Milestone Task Checklist Item' )");

            migrationBuilder.Sql(@"
            delete from app.rolepermissionlinks
			where  rolepermissionsid in
			(select rolepermissionsid from app.RolePermissions
            where name in (
            'Complete Milestone Task',
            'Complete Milestone Task Checklist Item',
            'Create Contacts',
            'Create Milestone Template',
            'Delete Milestone Template',
            'Edit Approved ReportObjects Documentation',
            'Uncomplete Milestone Task Checklist Item'))");

            migrationBuilder.Sql(@"
            update app.RolePermissions set name = 'Delete Collection' where name = 'Delete Project'");
            migrationBuilder.Sql(@"
            update app.RolePermissions set name = 'Create Collection' where name = 'Create Project'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.Sql(@"
            insert into app.RolePermissions (name) values
             ('Complete Milestone Task'),
            ('Complete Milestone Task Checklist Item'),
            ('Create Contacts'),
            ('Create Milestone Template'),
            ('Delete Milestone Template'),
            ('Edit Approved ReportObjects Documentation'),
            ('Uncomplete Milestone Task Checklist Item')
            ");

            migrationBuilder.Sql(@"
            update app.RolePermissions set name = 'Delete Project' where name = 'Delete Collection'");
            migrationBuilder.Sql(@"
            update app.RolePermissions set name = 'Create Project' where name = 'Create Collection'");
        }
    }
}
