
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.MyTNBService.Request
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
