using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class SendRegistrationTokenSMSRequest : BaseRequest
    {
        public DeviceInfoRequest deviceInf;
        public SendRegistrationTokenSMSRequest()
        {
            deviceInf = new DeviceInfoRequest();
        }
    }
}
