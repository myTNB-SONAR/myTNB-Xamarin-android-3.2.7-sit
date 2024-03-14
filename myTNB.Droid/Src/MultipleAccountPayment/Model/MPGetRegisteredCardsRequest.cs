using Refit;

namespace myTNB.Android.Src.MultipleAccountPayment.Model
{
    public class MPGetRegisteredCardsRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("email")]
        public string email { get; set; }

        public MPGetRegisteredCardsRequest(string apiKeyId, string email)
        {
            this.apiKeyID = apiKeyId;
            this.email = email;
        }

    }
}
