using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using myTNB.Mobile.Business;
using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public sealed class APISecurityManager
    {
        private static readonly Lazy<APISecurityManager> lazy =
          new Lazy<APISecurityManager>(() => new APISecurityManager());

        public static APISecurityManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public APISecurityManager()
        {
        }

        public EncryptedRequest GetEncryptedRequest(object request)
        {
            Aes aes = Aes.Create();
            byte[] key = aes.Key;
            byte[] iv = aes.IV;
            string requestString = JsonConvert.SerializeObject(request);
            byte[] dataBytes = Encoding.Default.GetBytes(requestString);
            byte[] encryptedData = AESEncrypt(dataBytes
                , key
                , iv);
            string encryptedDataString = Convert.ToBase64String(encryptedData);

            byte[] encryptedKey = RSAEncrypt(key);
            string keyString = Convert.ToBase64String(encryptedKey);
            string ivString = Convert.ToBase64String(iv);

            EncryptedRequest encryptedRequest = new EncryptedRequest {
                dt = new EncryptedDataRequest
                {
                    ae = encryptedDataString,
                    ak = keyString,
                    av = ivString
                }
            };
            return encryptedRequest;
        }

        private byte[] RSAEncrypt(byte[] data)
        {
            const string path = "myTNB.Mobile.Resources.Keys.Key.txt";
            string publicKey = string.Empty;
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(path);
            using (StreamReader reader = new StreamReader(stream))
            {
                publicKey = reader.ReadToEnd();
            }
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                return rsa.Encrypt(data, true);
            }
        }

        private byte[] AESEncrypt(byte[] dataArray
            , byte[] key
            , byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(dataArray, 0, dataArray.Length);
                        csEncrypt.FlushFinalBlock();
                        return msEncrypt.ToArray();
                    }
                }
            }
        }
    }
}