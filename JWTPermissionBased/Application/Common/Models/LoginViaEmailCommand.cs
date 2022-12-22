namespace JWTPermissionBased.Application.Common.Models;

public class LoginViaEmailCommand
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}