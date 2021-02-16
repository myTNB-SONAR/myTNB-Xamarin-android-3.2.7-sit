using System;
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
        private string UserId = string.Empty;
        private string UserName = string.Empty;
        private string Lang = string.Empty;

        internal Language Language { private set; get; } = LanguageManager.Language.EN;

        /// <summary>
        /// Sets User Info to be pased for Service Calls
        /// </summary>
        /// <param name="roleID">0 for All 16 for Consumers</param>
        /// <param name="userID">SSPUID</param>
        /// <param name="userName">Email or EID</param>
        /// <param name="language">Language Selected in the App</param>
        public void SetUserInfo(string roleID
            , string userID
            , string userName
            , Language language = Language.EN)
        {
            this.Language = language;
            RoleId = roleID ?? string.Empty;
            UserId = userID ?? string.Empty;
            UserName = userName ?? string.Empty;
            Lang = this.Language.ToString();
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
    }
}