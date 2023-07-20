using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public sealed class SecurityManager
    {
        private static readonly Lazy<SecurityManager> lazy =
          new Lazy<SecurityManager>(() => new SecurityManager());

        public static SecurityManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public SecurityManager()
        {
        }

        internal string AES256_Encrypt(string saltKey
            , string passPhrase
            , string plainText)
        {
            string saltValue = saltKey;
            int passwordIterations = 1000;
            string initVector = "pOWaTbO92LfXbh69";
            int keySize = 32;

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations);
            byte[] keyBytes = password.GetBytes(keySize);

            RijndaelManaged symmetricKey = new RijndaelManaged
            {
                BlockSize = 128,
                KeySize = 256,
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.CBC
            };
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();

            return Convert.ToBase64String(cipherTextBytes);
        }

        public string AES256_Decrypt(string saltKey
            , string passPhrase
            , string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            string saltValue = saltKey;
            int passwordIterations = 1000;
            string initVector = "pOWaTbO92LfXbh69";
            int keySize = 32;

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations);
            byte[] keyBytes = password.GetBytes(keySize);

            RijndaelManaged symmetricKey = new RijndaelManaged
            {
                BlockSize = 128,
                KeySize = 256,
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.CBC
            };
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();

            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).Replace("\0", "");
        }

        public string Encrypt<T>(T responseClass)
        {
            try
            {
                string responseString = JsonConvert.SerializeObject(responseClass);
                string encryptedString = AES256_Encrypt(MobileConstants.SaltKey
                    , MobileConstants.PassPhrase
                    , responseString);
                return encryptedString;
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG][Encrypt]General Exception: " + e.Message);
            }
            return string.Empty;
        }

        public T Decrypt<T>(string encryptedString) where T : new()
        {
            T customClass = new T();
            try
            {
                string decryptedString = AES256_Decrypt(MobileConstants.SaltKey
                    , MobileConstants.PassPhrase
                    , encryptedString);
                if (decryptedString.IsValid())
                {
                    return JsonConvert.DeserializeObject<T>(decryptedString);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG][Decrypt]General Exception: " + e.Message);
            }
            return customClass;
        }
    }
}
