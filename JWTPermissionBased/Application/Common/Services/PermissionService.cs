using JWTPermissionBased.Application.Common.Interfaces;
using JWTPermissionBased.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JWTPermissionBased.Application.Common.Services;

public class PermissionService: IPermissionService
{
    private readonly ICurrentUserService _currentUser;
    private readonly ApplicationContext _context;

    public PermissionService(ICurrentUserService currentUser, ApplicationContext context)
    {
        _currentUser = currentUser;
        _context = context;
    }

    public async Task ValidatePermission(string perm)
    {
        if (_currentUser.RoleId == Guid.Empty) throw new Exception("Forbidden");

        var checkPermission = await _context.RolePermissions.FirstOrDefaultAsync(x =>
            x.RoleId == _currentUser.RoleId && x.Permission.Name == perm) ?? throw new Exception("Forbidden");
    }
}