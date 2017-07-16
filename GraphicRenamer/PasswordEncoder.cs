using System;
using System.Text;
using System.Security.Cryptography;

namespace GraphicRenamer
{
    public class PasswordEncoder
    {
        private PasswordEncoder() { }

        private const string AesIV = @"&%jqiIurtmslLE58";
        private const string AesKey = @"3uJi<9!$kM0lkxme";

        public static string Encrypt(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            { return ""; }

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 128;
            aes.IV = Encoding.UTF8.GetBytes(AesIV);
            aes.Key = Encoding.UTF8.GetBytes(AesKey);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] src = Encoding.Unicode.GetBytes(text);

            using (ICryptoTransform encrypt = aes.CreateEncryptor())
            {
                byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);
                return Convert.ToBase64String(dest);
            }
        }

        public static string Decrypt(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            { return ""; }

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 128;
            aes.IV = Encoding.UTF8.GetBytes(AesIV);
            aes.Key = Encoding.UTF8.GetBytes(AesKey);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] src = System.Convert.FromBase64String(text);
            using (ICryptoTransform decrypt = aes.CreateDecryptor())
            {
                byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                return Encoding.Unicode.GetString(dest);
            }
        }
    }
}