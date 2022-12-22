namespace JWTPermissionBased.Application.Common.Interfaces;

public interface IPermissionService
{
    Task ValidatePermission(string perm);
}