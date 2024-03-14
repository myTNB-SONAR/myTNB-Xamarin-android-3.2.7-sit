using Newtonsoft.Json;
using Refit;

namespace myTNB.Android.Src.AddAccount.Models
{
    public class BCRMAccount
    {
        [JsonProperty(PropertyName = "accNum")]
        [AliasAs("accNum")]
        public string accNum { get; set; }

        [JsonProperty(PropertyName = "accountTypeId")]
        [AliasAs("accountTypeId")]
        public string accountTypeId { get; set; }

        [JsonProperty(PropertyName = "accountStAddress")]
        [AliasAs("accountStAddress")]
        public string accountStAddress { get; set; }

        [JsonProperty(PropertyName = "icNum")]
        [AliasAs("icNum")]
        public string icNum { get; set; }

        [JsonProperty(PropertyName = "isOwned")]
        [AliasAs("isOwned")]
        public bool isOwned { get; set; }

        [JsonProperty(PropertyName = "accountCategoryId")]
        [AliasAs("accountCategoryId")]
        public string accountCategoryId { get; set; }
    }
}