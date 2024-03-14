using System;
namespace myTNB.Android.Src.MyTNBService.Request
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
