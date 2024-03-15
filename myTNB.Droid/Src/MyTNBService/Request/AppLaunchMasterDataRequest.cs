
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class AppLaunchMasterDataRequest : BaseRequest
    {
        public DeviceInfoRequest deviceInf;
        public AppLaunchMasterDataRequest()
        {
            deviceInf = new DeviceInfoRequest();
        }
    }
}
