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
        private object UserInfo;
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
            UserInfo = new
            {
                RoleId = roleID ?? string.Empty,
                UserId = userID ?? string.Empty,
                UserName = userName ?? string.Empty,
                Lang = this.Language.ToString()
            };
        }

        public void SetLanguage(Language language = Language.EN)
        {
            this.Language = language;
        }

        public void Clear()
        {
            UserInfo = null;
        }

        public string GetUserInfo()
        {
            //return Newtonsoft.Json.JsonConvert.SerializeObject(UserInfo);

            return Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                RoleId = "16",
                UserId = "2EA0AF57-0B9F-4A41-81F3-BBEF6AAD680C",
                UserName = "kingkong@gmail.com"
            });

            //TODO: Update Default Value
            /*return Newtonsoft.Json.JsonConvert.SerializeObject(UserInfo != null ? UserInfo :
                new
                {
                    RoleId = "16",
                    UserId = "243A701C-761A-415D-BAC0-DD69490513B1",
                    UserName = "tester1.tnb@gmail.com",
                    Lang = Language.EN.ToString()
                });*/
        }
    }
}