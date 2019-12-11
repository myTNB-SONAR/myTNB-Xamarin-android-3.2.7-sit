
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.MyTNBService.Request
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
