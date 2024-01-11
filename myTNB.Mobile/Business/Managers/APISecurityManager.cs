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

        public static string AES256_Encrypt(string salt, string rawContent)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(rawContent);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(salt, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var stream = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(stream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                    }
                    rawContent = Convert.ToBase64String(stream.ToArray());
                }
            }
            return rawContent;
        }

        private byte[] RSAEncrypt(byte[] data)
        {
#if DEBUG || MASTER || SIT
            const string path = "myTNB.Mobile.Resources.Keys.SKey.txt";
#else
            const string path = "myTNB.Mobile.Resources.Keys.PKey.txt";
#endif
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