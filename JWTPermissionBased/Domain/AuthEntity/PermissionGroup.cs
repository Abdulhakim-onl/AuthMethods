namespace JWTPermissionBased.Domain.AuthEntity
{
    public class PermissionGroup : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public List<Permission> Permissions { get; set; }
    }
}
