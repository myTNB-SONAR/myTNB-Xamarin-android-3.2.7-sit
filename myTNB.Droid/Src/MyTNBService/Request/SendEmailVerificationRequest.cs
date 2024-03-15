using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
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
