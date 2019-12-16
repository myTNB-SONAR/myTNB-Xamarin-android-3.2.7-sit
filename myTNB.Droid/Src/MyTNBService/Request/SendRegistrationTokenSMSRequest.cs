using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class SendRegistrationTokenSMSRequest : BaseRequest
    {
        public DeviceInfoRequest deviceInf;
        public string mobileNo;
        public SendRegistrationTokenSMSRequest(string mobileNo)
        {
            this.mobileNo = mobileNo;
            deviceInf = new DeviceInfoRequest();
        }
    }
}
