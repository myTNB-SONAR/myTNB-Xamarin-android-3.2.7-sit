using System;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.AddAccount.Models
{
    public class AddAccount
    {
        [JsonProperty(PropertyName = "accNum")]
        [AliasAs("accNum")]
        public string accountNumber { get; set; }

        [JsonProperty(PropertyName = "accountTypeId")]
        [AliasAs("accountTypeId")]
        public string accountTypeId { get; set; }

        [JsonProperty(PropertyName = "accountStAddress")]
        [AliasAs("accountStAddress")]
        public string accountStAddress { get; set; }

        [JsonProperty(PropertyName = "icNum")]
        [AliasAs("icNum")]
        public string icNum { get; set; }

        [JsonProperty(PropertyName = "accountNickName")]
        [AliasAs("accountNickName")]
        public string accountNickName { get; set; }

        [JsonProperty(PropertyName = "isOwned")]
        [AliasAs("isOwned")]
        public bool isOwned { get; set; }

        [JsonProperty(PropertyName = "accountCategoryId")]
        [AliasAs("accountCategoryId")]
        public string accountCategoryId { get; set; }

        [JsonProperty(PropertyName = "accountOwnerName")]
        [AliasAs("accountOwnerName")]
        public string accountOwnerName { get; set; }

        [JsonProperty(PropertyName = "smartMeterCode")]
        [AliasAs("smartMeterCode")]
        public string smartMeterCode { get; set; }

        [JsonProperty(PropertyName = "isTaggedSMR")]
        [AliasAs("isTaggedSMR")]
        public string IsTaggedSMR { get; set; }

        [JsonProperty(PropertyName = "OwnerEmail")]
        [AliasAs("OwnerEmail")]
        public string emailOwner { get; set; }

        [JsonProperty(PropertyName = "OwnerMobileNum")]
        [AliasAs("OwnerMobileNum")]
        public string mobileNoOwner { get; set; }

        [JsonProperty(PropertyName = "unitNo")]
        [AliasAs("unitNo")]
        public string unitNo { get; set; }

        [JsonProperty(PropertyName = "building")]
        [AliasAs("building")]
        public string building { get; set; }

        [JsonProperty(PropertyName = "houseNo")]
        [AliasAs("houseNo")]
        public string houseNo { get; set; }

        [JsonProperty(PropertyName = "street")]
        [AliasAs("street")]
        public string street { get; set; }

        [JsonProperty(PropertyName = "area")]
        [AliasAs("area")]
        public string area { get; set; }

        [JsonProperty(PropertyName = "city")]
        [AliasAs("city")]
        public string city { get; set; }

        [JsonProperty(PropertyName = "postCode")]
        [AliasAs("postCode")]
        public string postCode { get; set; }

        [JsonProperty(PropertyName = "state")]
        [AliasAs("state")]
        public string state { get; set; }

        [JsonProperty(PropertyName = "BudgetAmount")]
        [AliasAs("BudgetAmount")]
        public string BudgetAmount { get; set; }

        [JsonProperty(PropertyName = "InstallationType")]
        [AliasAs("InstallationType")]
        public string  InstallationType { get; set; }

        [JsonProperty("CreatedDate")]
        [AliasAs("CreatedDate")]
        public DateTime? CreatedDate { set; get; }
    }
}