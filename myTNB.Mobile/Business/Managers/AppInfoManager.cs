using System;
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
            , LanguageManager.Language language = LanguageManager.Language.EN)
        {
            UserInfo = new
            {
                RoleId = roleID ?? string.Empty,
                UserId = userID ?? string.Empty,
                UserName = userName ?? string.Empty,
                Lang = language.ToString()
            };
        }

        public string GetUserInfo()
        {
            //TODO: Update Default Value
            return Newtonsoft.Json.JsonConvert.SerializeObject(UserInfo != null ? UserInfo :
                new
                {
                    RoleId = "16",
                    UserId = "243A701C-761A-415D-BAC0-DD69490513B1",
                    UserName = "tester1.tnb@gmail.com",
                    Lang = LanguageManager.Language.EN.ToString()
                });
        }
    }
}