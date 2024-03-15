using myTNB.AndroidApp.Src.Base.Models;

namespace myTNB.AndroidApp.Src.AppLaunch.Requests
{
    public class GetAccountTypesRequest : BaseRequest
    {
        public GetAccountTypesRequest(string apiKeyID) : base(apiKeyID)
        {
            base.apiKeyID = apiKeyID;
        }
    }
}