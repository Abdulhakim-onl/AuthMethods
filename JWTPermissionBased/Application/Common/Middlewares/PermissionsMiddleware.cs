using JWTPermissionBased.Application.Common.Interfaces;
using JWTPermissionBased.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JWTPermissionBased.Application.Common.Middlewares
{
    public class PermissionsMiddleware
    {
        //private readonly ApplicationContext _context;
        //private readonly IAuthService _authService;
        //private readonly ICurrentUserService _currentUser;
        private readonly RequestDelegate _next;

        public PermissionsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService, ApplicationContext dbcontext, ICurrentUserService currentUser)
        {
            if (currentUser.RoleId != Guid.Empty)
            {
                currentUser.Permissions = await dbcontext.RolePermissions
                    .Where(x => x.RoleId == currentUser.RoleId)
                    .Select(x => x.Permission.Name)
                    .ToListAsync();
            }
            await _next(context);
        }
    }
}
