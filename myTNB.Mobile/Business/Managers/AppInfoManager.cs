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

        private AppInfoManager()
        {
        }

        private object usrInf = new { };
        private object deviceInf = new { };
        private string language = string.Empty;

        public void SetUserInfo(object userInfo)
        {
            if (userInfo != null)
            {
                usrInf = userInfo;
            }
        }

        public void SetDeviceInfo(object deviceInfo)
        {
            if (deviceInfo != null)
            {
                deviceInf = deviceInfo;
            }
        }

        public void SetAppInfo(object userInfo, object deviceInfo, LanguageManager.Language lang = LanguageManager.Language.EN)
        {
            SetUserInfo(userInfo);
            SetDeviceInfo(deviceInfo);
            language = lang.ToString();
        }

        public object GetUserInfo()
        {
            return usrInf;
        }

        public object GetDeviceInfo()
        {
            return deviceInf;
        }

    }
}