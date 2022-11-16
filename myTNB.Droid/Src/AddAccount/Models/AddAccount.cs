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

        [JsonProperty(PropertyName = "isError")]
        [AliasAs("isError")]
        public bool IsError { get; set; }

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

        [JsonProperty(PropertyName = "BudgetAmount")]
        [AliasAs("BudgetAmount")]
        public string BudgetAmount { get; set; }

        [JsonProperty(PropertyName = "InstallationType")]
        [AliasAs("InstallationType")]
        public string  InstallationType { get; set; }

        [JsonProperty(PropertyName ="CreatedDate")]
        [AliasAs("CreatedDate")]
        public string CreatedDate { set; get; }

        [JsonProperty(PropertyName = "IsApplyEBilling")]
        [AliasAs("IsApplyEBilling")]
        public bool IsApplyEBilling { get; set; }

        [JsonProperty(PropertyName = "IsHaveAccess")]
        [AliasAs("IsHaveAccess")]
        public bool IsHaveAccess { get; set; }

        [JsonProperty(PropertyName = "BusinessArea")]
        [AliasAs("BusinessArea")]
        public string BusinessArea { get; set; }

        [JsonProperty(PropertyName = "IsInManageAccessList")]
        [AliasAs("IsInManageAccessList")]
        public bool IsInManageAccessList { get; set; }

        [JsonProperty(PropertyName = "RateCategory")]
        [AliasAs("RateCategory")]
        public string RateCategory { get; set; }

        [JsonProperty(PropertyName = "AccountHasOwner")]
        [AliasAs("AccountHasOwner")]
        public bool AccountHasOwner { get; set; }
        
        [JsonProperty(PropertyName = "AMSIDCategory")]
        [AliasAs("AMSIDCategory")]
        public string AMSIDCategory { get; set; }
    }
}