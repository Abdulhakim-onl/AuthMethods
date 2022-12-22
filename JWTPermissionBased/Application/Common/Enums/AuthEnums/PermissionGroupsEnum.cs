namespace JWTPermissionBased.Application.Common.Enums.AuthEnums;

public class PermissionGroupsEnum : EnumBase<Guid, string, PermissionEnum>
{
    public static PermissionGroupsEnum Weather { get; } =
        new PermissionGroupsEnum(Guid.Parse("4243d00b-6ad2-47dc-80b7-5b338741d184"), "Погода");

    protected PermissionGroupsEnum(Guid key, string description) : base(key, description)
    {
    }
}