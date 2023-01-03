using JWTPermissionBased.Application.Common.Models;

namespace JWTPermissionBased.Application.Common.Interfaces
{
    public interface IGoogleAuthenticatorService
    {
        GoogleAuthenticatorModel GenerateSetupCode(string accountTitleNoSpaces, string accountSecretKey, int qrCodeWidth, int qrCodeHeight);
        bool ValidateTwoFactorPIN(string accountSecretKey, string twoFactorCodeFromClient);
    }
}
