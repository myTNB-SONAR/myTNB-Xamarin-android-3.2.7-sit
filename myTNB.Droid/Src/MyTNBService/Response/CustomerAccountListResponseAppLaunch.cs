﻿using System.Collections.Generic;
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
            //public string AccountNumber = "220526520500";

            [JsonProperty(PropertyName = "userAccountId")]
            public string UserAccountID { get; set; }
            //public string UserAccountID = null;

            [JsonProperty(PropertyName = "accDesc")]
            //public string AccDesc { get; set; }
            public string AccDesc = "sync test";

            [JsonProperty(PropertyName = "icNum")]
            public string IcNum { get; set; }
            //public string IcNum = "DC031000515740";

            [JsonProperty(PropertyName = "amCurrentChg")]
            public string AmCurrentChg { get; set; }
            //public string AmCurrentChg = "0.0";

            [JsonProperty(PropertyName = "isRegistered")]
            public bool IsRegistered { get; set; }
            //public bool IsRegistered = true;

            [JsonProperty(PropertyName = "isPaid")]
            public bool IsPaid { get; set; }
            //public bool IsPaid = false;

            [JsonProperty(PropertyName = "isOwned")]
            public bool IsOwned { get; set; }
            //public bool IsOwned = true;

            [JsonProperty(PropertyName = "isError")]
            public bool IsError { get; set; }
            //public string IsError = "false";

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }
            //public string Message = "";

            [JsonProperty(PropertyName = "accountTypeId")]
            public string AccountTypeId { get; set; }
            //public string AccountTypeId = "";

            [JsonProperty(PropertyName = "accountStAddress")]
            public string AccountStAddress { get; set; }
            //public string AccountStAddress = "686 E,JALAN BAHAGIA,TAMAN BAHAGIA, BUKIT BARU 75150 MELAKA MEL";

            [JsonProperty(PropertyName = "ownerName")]
            public string OwnerName { get; set; }
            //public string OwnerName = "SAEDAH BT JERAGAN";

            [JsonProperty(PropertyName = "accountCategoryId")]
            public string AccountCategoryId { get; set; }
            //public string AccountCategoryId = "1";

            [JsonProperty(PropertyName = "smartMeterCode")]
            public string SmartMeterCode { get; set; }
            //public string SmartMeterCode = null;

            [JsonProperty(PropertyName = "isTaggedSMR")]
            public string IsTaggedSMR { get; set; }
            //public string IsTaggedSMR = "";

            [JsonProperty(PropertyName = "BudgetAmount")]
            public string BudgetAmount { get; set; }
            //public string BudgetAmount = "0";

            [JsonProperty(PropertyName = "InstallationType")]
            public string InstallationType { get; set; }
            //public string InstallationType = "01";

            [JsonProperty(PropertyName = "IsApplyEBilling")]
            public bool IsApplyEBilling { get; set; } = false;

            [JsonProperty(PropertyName = "IsHaveAccess")]
            public bool IsHaveAccess { get; set; } = false;

            [JsonProperty(PropertyName = "BusinessArea")]
            public string BusinessArea { get; set; }

            [JsonProperty(PropertyName = "RateCategory")]
            public string RateCategory { get; set; }
        }
    }
}