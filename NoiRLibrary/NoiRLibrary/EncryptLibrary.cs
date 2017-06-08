using System;
using System.EnterpriseServices;
using System.Security.Cryptography;
using System.Text;

namespace NoiRLibrary
{
    public class EncryptLibrary : ServicedComponent
    {
        /// <summary>
        /// 고급 암호화 표준 암호화
        /// Advanced Encryption Standard
        /// </summary>
        /// <param name="text">암호화 할 문자열.</param>
        /// <param name="key">암호화 키.</param>
        /// <returns></returns>
        public string AesEncrypt(string text, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] encryptArray = UTF8Encoding.UTF8.GetBytes(text);

            RijndaelManaged rijndael = new RijndaelManaged();

            rijndael.Key = keyArray;
            rijndael.Mode = CipherMode.ECB;
            rijndael.Padding = PaddingMode.PKCS7;

            ICryptoTransform crypto = rijndael.CreateEncryptor();

            byte[] resultArray = crypto.TransformFinalBlock(encryptArray, 0, encryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// 고급 암호화 표준 복호화
        /// Advanced Encryption Standard
        /// </summary>
        /// <param name="text">암호화 할 문자열.</param>
        /// <param name="key">암호화 키.</param>
        /// <returns></returns>
        public string AesDecrypt(string text, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] decryptArray = Convert.FromBase64String(text);

            RijndaelManaged rijndael = new RijndaelManaged();

            rijndael.Key = keyArray;
            rijndael.Mode = CipherMode.ECB;
            rijndael.Padding = PaddingMode.PKCS7;

            ICryptoTransform crypto = rijndael.CreateDecryptor();

            byte[] resultArray = crypto.TransformFinalBlock(decryptArray, 0, decryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
