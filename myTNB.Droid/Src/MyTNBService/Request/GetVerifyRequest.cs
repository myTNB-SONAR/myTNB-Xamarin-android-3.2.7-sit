using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class GetVerifyRequest : BaseRequest
    {
        public string usremail;

        public GetVerifyRequest(string usremail)
        {
            this.usremail = usremail;
        }
    }
}
