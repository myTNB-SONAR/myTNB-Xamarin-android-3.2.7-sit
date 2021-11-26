
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class AppLaunchMasterDataRequestAWS : BaseRequest
    {
        public DeviceInfoRequest deviceInf;
        public AppLaunchMasterDataRequestAWS(DeviceInfoRequest deviceInfo)
        {
            deviceInf = deviceInfo;
        }
    }
}
