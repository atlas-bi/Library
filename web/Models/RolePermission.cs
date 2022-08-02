namespace Atlas_Web.Models
{
    public partial class RolePermission
    {
        public RolePermission()
        {
            RolePermissionLinks = new HashSet<RolePermissionLink>();
        }

        public int RolePermissionsId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RolePermissionLink> RolePermissionLinks { get; set; }
    }
}
