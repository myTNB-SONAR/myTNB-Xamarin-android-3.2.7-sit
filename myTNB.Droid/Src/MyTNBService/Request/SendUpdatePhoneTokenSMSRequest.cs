using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class SendUpdatePhoneTokenSMSRequest : BaseRequest
    {
        public string mobileNo;

        public SendUpdatePhoneTokenSMSRequest(string mobileNo)
        {
            this.mobileNo = mobileNo;
        }
    }
}
