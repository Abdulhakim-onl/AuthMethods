namespace JWTPermissionBased.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? RoleId { get; }
    List<string> Permissions { get; set; }

}