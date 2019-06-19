using myTNB_Android.Src.Base.Models;

namespace myTNB_Android.Src.AppLaunch.Requests
{
    public class GetAccountTypesRequest : BaseRequest
    {
        public GetAccountTypesRequest(string apiKeyID) : base(apiKeyID)
        {
            base.apiKeyID = apiKeyID;
        }
    }
}