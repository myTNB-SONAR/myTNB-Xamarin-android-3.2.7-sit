using System;
using System.Diagnostics;
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
        public string GetDBRSignature(string name
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
                    FontSize = fontSize == "L" ? "L" : "N",
                    OriginUrl = originURL,
                    RedirectUrl = redirectURL,
                    CaNo = CANumber
                };
                Debug.WriteLine("[DEBUG] SSO ssoModel: " + JsonConvert.SerializeObject(ssoModel));
                string signature = SecurityManager.Instance.AES256_Encrypt(AWSConstants.SaltKey
                    , AWSConstants.PassPhrase
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

        public string GetMyHomeSignature(string name
            , string accessToken
            , string deviceToken
            , string appVersion
            , int roleID
            , string language
            , string fontSize
            , string originURL
            , string redirectURL
            , string userID
            , string idNumber
            , int osType
            , string email
            , string appReference
            , int? applicationModuleID
            , string mobileNumber
            , string cancelURL)
        {
            try
            {
                MyHomeModel ssoModel = new MyHomeModel
                {
                    Name = name,
                    AccessToken = accessToken,
                    DeviceToken = deviceToken,
                    AppVersion = appVersion,
                    RoleId = roleID,
                    Lang = language,
                    FontSize = fontSize == "L" ? "L" : "N",
                    OriginUrl = originURL,
                    RedirectUrl = redirectURL,
                    CaNo = string.Empty,
                    UserID = userID,
                    IdType = null,
                    IdNo = idNumber,
                    TransactionType = "MyHome",
                    InitiateTime = DateTime.UtcNow,
                    QRMappingID = null,
                    OSType = osType,
                    Email = email,
                    IsContractorApplied = false,
                    IsNonLogin = false,
                    AppRef = appReference,
                    ApplicationModuleID = applicationModuleID,
                    MobileNo = mobileNumber,
                    CancelUrl = cancelURL
                };
                Debug.WriteLine("[DEBUG] SSO ssoModel: " + JsonConvert.SerializeObject(ssoModel));
                string signature = SecurityManager.Instance.AES256_Encrypt(AWSConstants.SaltKey
                    , AWSConstants.PassPhrase
                    , JsonConvert.SerializeObject(ssoModel));
                Debug.WriteLine("[DEBUG] SSO GetMyHomeSignature: " + signature);
                return signature;
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] GetMyHomeSignature Error: " + e.Message);
                return string.Empty;
            }
        }
    }
}