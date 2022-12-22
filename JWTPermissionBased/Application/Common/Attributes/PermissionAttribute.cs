using JWTPermissionBased.Application.Common.Filters;
using Microsoft.AspNetCore.Mvc;

namespace JWTPermissionBased.Application.Common.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class PermissionAttribute : TypeFilterAttribute
{
    public PermissionAttribute(string permissionAction) : base(typeof(PermissionFilter))
    {
        Arguments = new object[] { permissionAction };
    }
}