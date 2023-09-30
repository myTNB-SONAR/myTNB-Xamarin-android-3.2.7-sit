using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using myTNB.Mobile;
using myTNB.Mobile.AWS;
using myTNB.Mobile.Extensions;
using static myTNB.LanguageManager;

namespace myTNB
{
    public sealed class AppInfoManager
    {
        private static readonly Lazy<AppInfoManager> lazy
            = new Lazy<AppInfoManager>(() => new AppInfoManager());

        public static AppInfoManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private AppInfoManager() { }
        private object PlatformUserInfo;
        private string RoleId = string.Empty;
        internal string UserId = string.Empty;
        internal string UserName = string.Empty;
        internal string Lang = string.Empty;
        internal string FCMToken = string.Empty;
        internal string OSVersion = string.Empty;

        internal Language Language { private set; get; } = LanguageManager.Language.EN;
        internal List<ContractAccountModel> ContractAccountList = new List<ContractAccountModel>();

        /// <summary>
        /// Sets User Info to be pased for Service Calls
        /// </summary>
        /// <param name="roleID">0 for All 16 for Consumers</param>
        /// <param name="userID">SSPUID</param>
        /// <param name="userName">Email or EID</param>
        /// <param name="deviceToken">UUDID</param>
        /// <param name="appVersion">ie: 2.3.3</param>
        /// <param name="fontSize">App Font size N or L</param>
        /// <param name="language">Language Selected in the App</param>
        public void SetUserInfo(string roleID
            , string userID
            , string userName
            , string deviceToken
            , string appVersion
            , string osType
            , string osVersion
            , string fcmToken
            , string fontSize = "N"
            , Language language = Language.EN)
        {
            //App Info
            this.Language = language;
            RoleId = roleID ?? string.Empty;
            UserId = userID ?? string.Empty;
            UserName = userName ?? string.Empty;
            Lang = this.Language.ToString();
            FCMToken = fcmToken;
            OSVersion = osVersion;

            //View Info
            ViewInfoHeader = new ViewInfoHeader
            {
                DeviceToken = deviceToken,
                AppVersion = appVersion.IsValid()
                    ? appVersion.Replace("v", string.Empty)
                    : string.Empty,
                RoleId = roleID,
                Lang = language.ToString(),
                FontSize = fontSize.ToUpper() == "L" ? "L" : "N",
                OSType = osType
            };
        }

        internal ViewInfoHeader ViewInfoHeader { set; get; }

        internal string ViewInfo
        {
            get
            {
                try
                {
                    ViewInfoHeader.AppVersion = Regex.Replace(ViewInfoHeader.AppVersion, @"\(.*?\)", "");
                    ViewInfoHeader.AppVersion = Regex.Replace(ViewInfoHeader.AppVersion, @"[^0-9.,]+", "");
                    return Newtonsoft.Json.JsonConvert.SerializeObject(ViewInfoHeader);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[DEBUG] ViewInfo Error: " + e.Message);
                }
                return string.Empty;
            }
        }

        public void SetPlatformUserInfo(object userInfo)
        {
            PlatformUserInfo = userInfo;
        }

        public void SetLanguage(Language language = Language.EN)
        {
            this.Language = language;
            Lang = this.Language.ToString();
        }

        public string GetLanguage()
        {
            return Lang;
        }

        public void Clear()
        {
            RoleId = string.Empty;
            UserId = string.Empty;
            UserName = string.Empty;
            PlatformUserInfo = null;
            FCMToken = string.Empty;
            OSVersion = string.Empty;

            ClearAccountList();
        }

        public string GetUserInfo()
        {
            object userInfo = new
            {
                RoleId,
                UserId,
                UserName,
                Lang
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(userInfo);
        }

        public object GetPlatformUserInfo()
        {
            return PlatformUserInfo;
        }

        public string AccessToken { set; get; }

        internal void SetAccountList(List<ContractAccountModel> caList)
        {
            try
            {
                if (caList != null
                    && caList.Count > 0)
                {
                    ContractAccountList = caList;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] SetAccount: " + e.Message);
            }
        }

        internal void ClearAccountList()
        {
            ContractAccountList.Clear();
        }
    }
}