using Refit;

namespace myTNB_Android.Src.MakePayment.Models
{
    public class GetRegisteredCardsRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("email")]
        public string email { get; set; }

        public GetRegisteredCardsRequest(string apiKeyId, string email)
        {
            this.apiKeyID = apiKeyId;
            this.email = email;
        }

    }
}
