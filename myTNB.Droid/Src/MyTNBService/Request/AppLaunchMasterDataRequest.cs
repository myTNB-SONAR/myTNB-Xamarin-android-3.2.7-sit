
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class AppLaunchMasterDataRequest : BaseRequest
    {
        public DeviceInfo deviceInf;
        public AppLaunchMasterDataRequest()
        {
            deviceInf = new DeviceInfo();
        }

        public class DeviceInfo
        {
            public string DeviceId, AppVersion, OsVersion, OsType, DeviceDesc;
            public DeviceInfo()
            {
                DeviceId = UserSessions.GetDeviceId();
                AppVersion = DeviceIdUtils.GetAppVersionName();
                OsType = Constants.DEVICE_PLATFORM;
                OsVersion = DeviceIdUtils.GetAndroidVersion();
                DeviceDesc = LanguageUtil.GetAppLanguage().ToUpper();
            }
        }

    }
}
