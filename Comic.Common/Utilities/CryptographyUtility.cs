using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Comic.Common.Utilities
{
    public class CryptographyUtility
    {
        public static string Create(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                var hash = Convert.ToBase64String(hashBytes);

                return hash;
            }
        }

        public static string Create(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: value,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }

        public static bool Validate(string value, string salt, string hash) => Create(value, salt) == hash;

        public static string GenerateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public static string RsaEncrypt(string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString("<RSAKeyValue><Modulus>1kFkM0YFGNdEHKee6En3jseNRcMHBK29xMQ+FFz3lJswb4EcWRKv00kmg5ApP8cnGn62J5xCcYCjMA0BVOpEodZmmpXAPmwGmaB5K7vVyNhL8owNwgtmtejrLqLfAD2Yo2JH80TxJZ4osOyEd2gXw/4eqZUyunJYqkW8nYdSsc0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
            var encryptString = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(content), false));
            return encryptString;
        }

        public static string RsaDecrypt(string encryptedContent)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString("<RSAKeyValue><Modulus>1kFkM0YFGNdEHKee6En3jseNRcMHBK29xMQ+FFz3lJswb4EcWRKv00kmg5ApP8cnGn62J5xCcYCjMA0BVOpEodZmmpXAPmwGmaB5K7vVyNhL8owNwgtmtejrLqLfAD2Yo2JH80TxJZ4osOyEd2gXw/4eqZUyunJYqkW8nYdSsc0=</Modulus><Exponent>AQAB</Exponent><P>4tSy0FY2C4KBzhJL4gTlL71dxMYig4MUgo2hnkfUE8oEXOIFbZAEvvYWlngTsqqO2O1C/9/maYRc+BxX+6M5Aw==</P><Q>8c61X8i6PPVdHx4/VmOHlVVGxvpuWa2r0+OFHXR+m393PKXsWAGrCEYjzd5OqjFZcpLGPX+YLO4YXF3ZgFgo7w==</Q><DP>A6pSNWGfP/Jc7JqJFV7k4S+bK0nUVvGwTCfDu84HVEaIaYPReKLCIN/TH51THCj7Y+/5jC7vBzd3VR8IIO3TRw==</DP><DQ>XJG2q6mLaoTAFoac1rSZlhikVM5QanJrAl3qeuE7CspcWlJmzTYuRKedU1WUpeDojk0UKTzsg97OYqXm+xRE7w==</DQ><InverseQ>E+2xzCjxwPSZ0Gt4e3FGpL/XPVdMeGHPZQLysK4OwGEwhxxewiUPT2uhEQ5btnK2WZ1rxqCVde6n+fUNAcyy7Q==</InverseQ><D>fsBSue6bh1WaiuUzIjLYDJTLY9D77u2mDakXHM7e/QBOSDj2f0+JyxJEgaKgdIWqAYei1Y4g1KsIX53iAs1Q6eQBCWiMZACvKI510dfOs6D/zsBo6kZvT6dnIBPCNTCybVPbG0eyPrF2VK09rd9mexORukQayBfQedOcuvR+o9U=</D></RSAKeyValue>");
            var decryptString = Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(encryptedContent), false));
            return decryptString;
        }

        public static string HMACSHA1Encrypt(string content, string encryptKey)
        {
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.UTF8.GetBytes(encryptKey);
            byte[] dataBuffer = Encoding.UTF8.GetBytes(content);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
            return Convert.ToBase64String(hashBytes).Replace('+', '-').Replace('/', '_');
        }
    }
}
