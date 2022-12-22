namespace JWTPermissionBased.Application.Common.Constants;

public class AuthSchemeConstants
{
    public const string LuckyScheme = "Lucky";
    public const string LToken = $"{LuckyScheme} (?<token>.*)";
}