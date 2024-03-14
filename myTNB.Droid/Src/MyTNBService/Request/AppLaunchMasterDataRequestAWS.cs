
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.MyTNBService.Request
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
