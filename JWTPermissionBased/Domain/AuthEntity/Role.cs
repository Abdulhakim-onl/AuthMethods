namespace JWTPermissionBased.Domain.AuthEntity
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }

        public List<User>? Users { get; set; }
        public List<RolePermission> RolePermissionsList { get; set; }

    }
}
