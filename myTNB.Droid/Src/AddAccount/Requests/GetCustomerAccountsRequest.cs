using myTNB_Android.Src.AddAccount.Models;

namespace myTNB_Android.Src.AddAccount.Requests
{
    public class GetCustomerAccountsRequest : AccountsResquest
    {
        public GetCustomerAccountsRequest(string apiKeyID, string userID) : base(apiKeyID, userID)
        {
            base.apiKeyID = apiKeyID;
            base.userID = userID;
        }
    }
}