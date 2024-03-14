
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.MyTNBService.Request
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
