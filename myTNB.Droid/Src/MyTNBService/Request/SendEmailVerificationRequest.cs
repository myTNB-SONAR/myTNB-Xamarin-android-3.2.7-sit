using System;
namespace myTNB.Android.Src.MyTNBService.Request
{
    public class SendEmailVerificationRequest : BaseRequest
    {
        public DeviceInfoRequest deviceInf;
        public string Email;
        public SendEmailVerificationRequest(string email)
        {
            deviceInf = new DeviceInfoRequest();
            this.Email = email;
        }
    }
}
