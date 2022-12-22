namespace JWTPermissionBased.Application.Common.Enums.AuthEnums;

public class RoleEnum: EnumBase< Guid, string,RoleEnum>
{
    public static RoleEnum Client { get; } = 
        new RoleEnum(Guid.Parse("4243d00b-6ad2-47dc-80b7-5b338741d184"), "Client");

    public static RoleEnum SuperAdmin { get; } = 
        new RoleEnum(Guid.Parse("8fd56aaa-bc80-41c7-a9cb-5057329652b9"), "SuperAdmin");

    protected RoleEnum(Guid key, string val) : base(key, val)
    {
    }
}