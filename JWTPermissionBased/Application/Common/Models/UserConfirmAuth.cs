namespace JWTPermissionBased.Application.Common.Models;

public class UserConfirmAuth
{
    public Guid Id { get; set; }
    public string? PhoneNumber  { get; set; }
    public string? Email  { get; set; }
    public string? Password  { get; set; }
    public int Code  { get; set; }
    public DateTime DateCreate { get; set; }
}