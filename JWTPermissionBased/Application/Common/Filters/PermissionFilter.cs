using JWTPermissionBased.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JWTPermissionBased.Application.Common.Filters
{
    public class PermissionFilter : IAuthorizationFilter
    {
        private readonly string _permissionAction;
        private readonly ICurrentUserService _currentUser;


        public PermissionFilter(string permissionAction, IAuthService authService, ICurrentUserService currentUser)
        {
            _permissionAction = permissionAction;
            _currentUser = currentUser;
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (_currentUser.RoleId == Guid.Empty) throw new Exception("Forbidden");
            if (!_currentUser.Permissions.Contains(_permissionAction)) throw new Exception("Forbidden");
        }
    }

}
