using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class CustomerAccountListResponseAppLaunch
    {

        public List<CustomerAccountData> GetData()
        {
            return customerAccountData;
        }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "ErrorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "DisplayMessage")]
        public string DisplayMessage { get; set; }

        [JsonProperty(PropertyName = "DisplayType")]
        public string DisplayType { get; set; }

        [JsonProperty(PropertyName = "DisplayTitle")]
        public string DisplayTitle { get; set; }

        [JsonProperty(PropertyName = "RefreshTitle")]
        public string RefreshTitle { get; set; }

        [JsonProperty(PropertyName = "RefreshMessage")]
        public string RefreshMessage { get; set; }

        [JsonProperty(PropertyName = "RefreshBtnText")]
        public string RefreshBtnText { get; set; }

        

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
            public bool IsError { get; set; }

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

            [JsonProperty(PropertyName = "AMSIDCategory")]
            public string AMSIDCategory { get; set; }

            [JsonProperty(PropertyName = "CreatedDate")]
            public string CreatedDate { set; get; }

            [JsonProperty(PropertyName = "BusinessArea")]
            public string BusinessArea { get; set; }

            [JsonProperty(PropertyName = "RateCategory")]
            public string RateCategory { get; set; }

            [JsonProperty(PropertyName = "IsInManageAccessList")]
            public bool IsInManageAccessList { get; set; }

            [JsonProperty(PropertyName = "CreatedBy")]
            public string CreatedBy { set; get; }

            [JsonProperty(PropertyName = "AccountHasOwner")]
            public bool AccountHasOwner { get; set; }
        }


    }
}



