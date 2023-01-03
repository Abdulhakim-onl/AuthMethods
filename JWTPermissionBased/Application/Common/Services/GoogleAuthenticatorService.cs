using JWTPermissionBased.Application.Common.Interfaces;
using JWTPermissionBased.Application.Common.Models;
using System.Security.Cryptography;
using System.Text;

namespace JWTPermissionBased.Application.Common.Services;

public class GoogleAuthenticatorService : IGoogleAuthenticatorService
{
    public GoogleAuthenticatorModel GenerateSetupCode(string accountTitleNoSpaces, string accountSecretKey, int qrCodeWidth, int qrCodeHeight)
    {
        if (accountTitleNoSpaces == null) { throw new NullReferenceException("Account Title is null"); }

        accountTitleNoSpaces = accountTitleNoSpaces.Replace(" ", "");

        var sC = new GoogleAuthenticatorModel();
        sC.Account = accountTitleNoSpaces;
        sC.AccountSecretKey = accountSecretKey;

        var encodedSecretKey = Base32Encode(Encoding.UTF8.GetBytes(accountSecretKey));
        sC.ManualEntryKey = encodedSecretKey;

        var provisionUrl = $"otpauth://totp/{accountTitleNoSpaces}?secret={encodedSecretKey}";

        var url = $"https://chart.googleapis.com/chart?cht=qr&chs={qrCodeWidth}x{qrCodeHeight}&chl={provisionUrl}";

        sC.QrCodeSetupImageUrl = url;

        return sC;
    }

    public bool ValidateTwoFactorPIN(string accountSecretKey, string twoFactorCodeFromClient)
    {
        long iterationCounter = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds / 30;
        var current = GenerateHashedCode(Encoding.UTF8.GetBytes(accountSecretKey), iterationCounter);
        return current == twoFactorCodeFromClient;
    }

    internal string GenerateHashedCode(byte[] key, long iterationNumber, int digits = 6)
    {
        byte[] counter = BitConverter.GetBytes(iterationNumber);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(counter);
        }

        HMACSHA1 hmac = getHMACSha1Algorithm(key);

        byte[] hash = hmac.ComputeHash(counter);

        int offset = hash[hash.Length - 1] & 0xf;

        // Convert the 4 bytes into an integer, ignoring the sign.
        int binary =
            ((hash[offset] & 0x7f) << 24)
            | (hash[offset + 1] << 16)
            | (hash[offset + 2] << 8)
            | (hash[offset + 3]);

        int password = binary % (int)Math.Pow(10, digits);
        return password.ToString(new string('0', digits));
    }

    private string Base32Encode(byte[] data)
    {
        int inByteSize = 8;
        int outByteSize = 5;
        char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

        int index = 0, digit = 0;
        int current_byte, next_byte;
        StringBuilder result = new StringBuilder((data.Length + 7) * inByteSize / outByteSize);

        for (int i = 0; i < data.Length; i++)
        {
            current_byte = (data[i] >= 0) ? data[i] : (data[i] + 256);

            if (index > (inByteSize - outByteSize))
            {
                if ((i + 1) < data.Length)
                    next_byte = (data[i + 1] >= 0) ? data[i + 1] : (data[i + 1] + 256);
                else
                    next_byte = 0;

                digit = current_byte & (0xFF >> index);
                index = (index + outByteSize) % inByteSize;
                digit <<= index;
                digit |= next_byte >> (inByteSize - index);
                i++;
            }
            else
            {
                digit = (current_byte >> (inByteSize - (index + outByteSize))) & 0x1F;
                index = (index + outByteSize) % inByteSize;
                if (index == 0)
                    i++;
            }
            result.Append(alphabet[digit]);
        }
    
        return result.ToString();
    }

    private HMACSHA1 getHMACSha1Algorithm(byte[] key)
    {
        HMACSHA1 hmac;

        try
        {
            hmac = new HMACSHA1(key, true);
        }
        catch (InvalidOperationException ioe)
        {
            try
            {
                hmac = new HMACSHA1(key, false);
            }
            catch (InvalidOperationException ioe2)
            {
                throw ioe2;
            }
        }

        return hmac;
    }
}
