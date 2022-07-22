namespace Atlas_Web.Models
{
    public partial class UserRoleLink
    {
        public int UserRoleLinksId { get; set; }
        public int? UserId { get; set; }
        public int? UserRolesId { get; set; }

        public virtual User User { get; set; }
        public virtual UserRole UserRoles { get; set; }
    }
}
