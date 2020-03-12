using System;
namespace myTNB_Android.Src.MyTNBService.Request
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
