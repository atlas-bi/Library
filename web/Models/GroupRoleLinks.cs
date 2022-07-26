namespace Atlas_Web.Models
{
    public partial class GroupRoleLink
    {
        public int GroupRoleLinksId { get; set; }
        public int GroupId { get; set; }
        public int UserRolesId { get; set; }

        public virtual UserGroup Group { get; set; }
        public virtual UserRole UserRoles { get; set; }
    }
}
