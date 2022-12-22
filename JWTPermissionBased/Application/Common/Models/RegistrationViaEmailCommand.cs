namespace JWTPermissionBased.Application.Common.Models;

public class RegistrationViaEmailCommand
{
    public string? Email { get; set; }
    public string Password { get; set; }
}