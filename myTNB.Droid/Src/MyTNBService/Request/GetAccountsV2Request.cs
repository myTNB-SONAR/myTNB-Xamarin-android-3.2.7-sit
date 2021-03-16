using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class GetAcccountsV2Request : BaseRequest
    {
        public DeviceInfoRequest deviceInf;
        public GetAcccountsV2Request()
        {
            deviceInf = new DeviceInfoRequest();
        }
    }
}
