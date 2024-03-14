using myTNB.Android.Src.AddAccount.Models;

namespace myTNB.Android.Src.AddAccount.Requests
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