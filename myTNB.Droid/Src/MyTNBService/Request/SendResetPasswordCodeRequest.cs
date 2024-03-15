using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class SendResetPasswordCodeRequest : BaseRequest
    {
        public DeviceInfoRequest deviceInf;
        public SendResetPasswordCodeRequest()
        {
            deviceInf = new DeviceInfoRequest();
        }
    }
}
