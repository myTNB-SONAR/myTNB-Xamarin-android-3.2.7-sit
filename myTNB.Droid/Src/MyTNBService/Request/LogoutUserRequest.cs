using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class LogoutUserRequest : BaseRequest
    {
        public DeviceInfoRequest deviceInf;
        public LogoutUserRequest()
        {
            deviceInf = new DeviceInfoRequest();
        }
    }
}
