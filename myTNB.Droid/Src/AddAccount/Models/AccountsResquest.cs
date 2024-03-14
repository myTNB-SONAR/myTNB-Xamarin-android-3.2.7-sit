using Refit;

namespace myTNB.Android.Src.AddAccount.Models
{
    public class AccountsResquest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("userID")]
        public string userID { get; set; }

        public AccountsResquest(string apiKeyID, string userID)
        {
            this.apiKeyID = apiKeyID;
            this.userID = userID;
        }
    }
}