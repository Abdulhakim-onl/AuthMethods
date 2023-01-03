namespace JWTPermissionBased.Application.Common.Models
{
    public class GoogleAuthenticatorModel
    {
        public string Account { get; internal set; }

        public string AccountSecretKey { get; internal set; }

        public string ManualEntryKey { get; internal set; }

        public string QrCodeSetupImageUrl { get; internal set; }
    }
}
