using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
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
