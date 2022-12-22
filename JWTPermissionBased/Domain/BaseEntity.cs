namespace JWTPermissionBased.Domain
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public bool IsActive { get; set; } = true;

        public DateTime Created { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime LastModified { get; set; }

        public Guid? LastModifiedBy { get; set; }
    }
}
