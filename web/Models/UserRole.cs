namespace Atlas_Web.Models
{
    public partial class UserRole
    {
        public UserRole()
        {
            RolePermissionLinks = new HashSet<RolePermissionLink>();
            UserRoleLinks = new HashSet<UserRoleLink>();
            GroupRoleLinks = new HashSet<GroupRoleLink>();
        }

        public int UserRolesId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RolePermissionLink> RolePermissionLinks { get; set; }
        public virtual ICollection<UserRoleLink> UserRoleLinks { get; set; }
        public virtual ICollection<GroupRoleLink> GroupRoleLinks { get; set; }
    }
}
