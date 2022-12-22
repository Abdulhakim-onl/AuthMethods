namespace JWTPermissionBased.Application.Common.Enums.AuthEnums;

public class PermissionEnum: EnumBase<Guid, string,PermissionEnum>
{
    public static PermissionEnum GetWeather { get; } = 
        new PermissionEnum(Guid.Parse("4243d00b-6ad2-47dc-80b7-5b338741d184"), "Просмотр погоды");

    protected PermissionEnum(Guid key, string description) : base(key, description)
    {
    }
}