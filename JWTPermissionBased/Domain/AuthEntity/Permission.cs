namespace JWTPermissionBased.Domain.AuthEntity
{
    public class Permission : BaseEntity
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public Guid PermissionGroupsID { get; set; }
        public PermissionGroup PermissionGroup { get; set; }

        public List<RolePermission> RolePermissionsList { get; set; }
    }
}
