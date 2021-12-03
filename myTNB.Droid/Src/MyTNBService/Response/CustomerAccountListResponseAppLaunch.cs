using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class CustomerAccountListResponseAppLaunch
    {

        [JsonProperty(PropertyName = "data")]
        [AliasAs("data")]
        public List<CustomerAccountData> customerAccountData { set; get; }

        public class CustomerAccountData
        {
            //[JsonProperty(PropertyName = "__type")]
            //public string Type { get; set; }

            [JsonProperty(PropertyName = "accNum")]
            public string AccountNumber { get; set; }

            [JsonProperty(PropertyName = "userAccountId")]
            public string UserAccountID { get; set; }

            [JsonProperty(PropertyName = "accDesc")]
            public string AccDesc = "sync test";

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

            [JsonProperty(PropertyName = "IsHaveAccess")]
            public bool IsHaveAccess { get; set; }

            [JsonProperty(PropertyName = "IsApplyEBilling")]
            public bool IsApplyEBilling { get; set; }

            [JsonProperty(PropertyName = "BudgetAmount")]
            public string BudgetAmount { get; set; }

            [JsonProperty(PropertyName = "InstallationType")]
            public string InstallationType { get; set; }

            [JsonProperty("CreatedDate")]
            public string CreatedDate { set; get; }
        }


    }
}
