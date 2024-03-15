using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
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
