using Refit;

namespace myTNB_Android.Src.AddAccount.Requests
{
    public class AccountDetailsRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("CANum")]
        public string CANum { get; set; }

    }
}