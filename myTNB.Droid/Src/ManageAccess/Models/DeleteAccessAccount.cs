using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.ManageAccess.Models
{
    public class DeleteAccessAccount
    {
        [JsonProperty(PropertyName = "accNum")]
        [AliasAs("accNum")]
        public string accountNumber { get; set; }
    }
}