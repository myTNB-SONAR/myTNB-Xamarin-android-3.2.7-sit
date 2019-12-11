using System;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class DeviceInfoRequest
    {
		public string DeviceId, AppVersion, OsVersion, OsType, DeviceDesc;
		public DeviceInfoRequest()
		{
			DeviceId = UserSessions.GetDeviceId();
			AppVersion = DeviceIdUtils.GetAppVersionName();
			OsType = Constants.DEVICE_PLATFORM;
			OsVersion = DeviceIdUtils.GetAndroidVersion();
			DeviceDesc = LanguageUtil.GetAppLanguage().ToUpper();
		}
	}
}
