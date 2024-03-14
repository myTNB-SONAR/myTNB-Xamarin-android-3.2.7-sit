using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.MyTNBService.Request
{
    public class DeviceInfoRequest
    {
        public string DeviceId, AppVersion, OsVersion, OsType, DeviceDesc, VersionCode;
        public DeviceInfoRequest()
        {
            DeviceId = UserSessions.GetDeviceId();
            AppVersion = DeviceIdUtils.GetAppVersionName();
#if SIT
            AppVersion += string.Format("({0})", DeviceIdUtils.GetAppVersionCode());
#endif
            OsType = Constants.DEVICE_PLATFORM;
            OsVersion = DeviceIdUtils.GetAndroidVersion();
            DeviceDesc = LanguageUtil.GetAppLanguage().ToUpper();
            VersionCode = string.Empty;
        }
    }
}