using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public sealed class SSOManager
    {
        private static readonly Lazy<SSOManager> lazy =
            new Lazy<SSOManager>(() => new SSOManager());

        public static SSOManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        public SSOManager() { }

        /// <summary>
        /// This returns the signature generated from App Parameters
        /// </summary>
        /// <param name="name">myTNB Account Name</param>
        /// <param name="accessToken">Generated Access Token</param>
        /// <param name="deviceToken">App's device id or UDID</param>
        /// <param name="appVersion">3 Digit version. ie: 2.3.3</param>
        /// <param name="roleID">Value will always be 16 for loggedin user</param>
        /// <param name="language">EN or BM</param>
        /// <param name="fontSize">N or L</param>
        /// <param name="originURL">Origin URL of a feature configured in mobile constants</param>
        /// <param name="redirectURL">Redirect URL of a feature configured in mobile constants</param>
        /// <param name="CANumber">Electricity Account Number</param>
        /// <returns></returns>
        public string GetSignature(string name
            , string accessToken
            , string deviceToken
            , string appVersion
            , int roleID
            , string language
            , string fontSize
            , string originURL
            , string redirectURL
            , string CANumber)
        {
            try
            {
                SSOModel ssoModel = new SSOModel
                {
                    Name = name,
                    AccessToken = accessToken,
                    DeviceToken = deviceToken,
                    AppVersion = appVersion,
                    RoleId = roleID,
                    Lang = language,
                    FontSize = fontSize,
                    OriginUrl = originURL,
                    RedirectUrl = redirectURL,
                    CaNo = CANumber
                };

                string signature = AES256_Encrypt(MobileConstants.SaltKey
                    , MobileConstants.PassPhrase
                    , JsonConvert.SerializeObject(ssoModel));
                Debug.WriteLine("[DEBUG] SSO Signature: " + signature);
                return signature;
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] GetSignature: " + e.Message);
                return string.Empty;
            }
        }

        private string AES256_Encrypt(string saltKey
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

        private string AES256_Decrypt(string saltKey
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
    }
}