using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class ResetPasswordWithTokenRequest : BaseRequest
    {
        public string token;

        public ResetPasswordWithTokenRequest(string token)
        {
            this.token = token;
        }
    }
}
