using Newtonsoft.Json;
using Refit;

namespace myTNB.Android.Src.MultipleAccountPayment.Model
{
    public class MPCreditCard
    {


        [JsonProperty(PropertyName = "Id")]
        [AliasAs("Id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Email")]
        [AliasAs("Email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "CardHashCode")]
        [AliasAs("CardHashCode")]
        public string CardHashCode { get; set; }

        [JsonProperty(PropertyName = "CardType")]
        [AliasAs("CardType")]
        public string CardType { get; set; }

        [JsonProperty(PropertyName = "CreatedDate")]
        [AliasAs("CreatedDate")]
        public string CreatedDate { get; set; }

        [JsonProperty(PropertyName = "LastDigits")]
        [AliasAs("LastDigits")]
        public string LastDigits { get; set; }

    }
}
