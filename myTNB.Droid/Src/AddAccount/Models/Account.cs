using Newtonsoft.Json;
using SQLite;

namespace myTNB_Android.Src.AddAccount.Models
{

    public class Account
    {
        [JsonProperty(PropertyName = "__type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "accNum")]
        public string AccountNumber { get; set; }

        [JsonProperty(PropertyName = "userAccountId")]
        public string UserAccountID { get; set; }

        [JsonProperty(PropertyName = "accDesc")]
        public string AccDesc { get; set; }

        [JsonProperty(PropertyName = "icNum")]
        public string IcNum { get; set; }

        [JsonProperty(PropertyName = "amCurrentChg")]
        public string AmCurrentChg { get; set; }

        [JsonProperty(PropertyName = "isRegistered")]
        public bool IsRegistered { get; set; }

        [JsonProperty(PropertyName = "isPaid")]
        public bool IsPaid { get; set; }

        [JsonProperty(PropertyName = "isOwned")]
        public bool IsOwned { get; set; }

        [JsonProperty(PropertyName = "isError")]
        public string IsError { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "accountTypeId")]
        public string AccountTypeId { get; set; }

        [JsonProperty(PropertyName = "accountStAddress")]
        public string AccountStAddress { get; set; }

        [JsonProperty(PropertyName = "ownerName")]
        public string OwnerName { get; set; }

        [JsonProperty(PropertyName = "accountCategoryId")]
        public string AccountCategoryId { get; set; }

        [JsonProperty(PropertyName = "smartMeterCode")]
        public string SmartMeterCode { get; set; }

        [JsonProperty(PropertyName = "isTaggedSMR")]
        public string IsTaggedSMR { get; set; }

        [JsonProperty(PropertyName = "AMSIDCategory")]
        public string AMSIDCategory { get; set; }
    }
}