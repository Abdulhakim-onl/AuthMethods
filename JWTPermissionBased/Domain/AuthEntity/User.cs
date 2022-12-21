namespace JWTPermissionBased.Domain.AuthEntity
{
    public class User : BaseEntity
    {
        public string? UserName { get; set; }
        public string? Name { get; set; }
        public string? SurName { get; set; }
        public string? FatherName { get; set; }

        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }

        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        public string Password { get; set; }

        public int AcccessFailedCount { get; set; }
        public bool Lockouted { get; set; }
        public DateTime LockoutEnd { get; set; }

        public Guid RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
