using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Database.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Models
{
    public class AccountData
    {
        [JsonProperty("AccountNum")]
        public string AccountNum { get; set; }

        [JsonProperty("AccountName")]
        public string AccountName { get; set; }

        [JsonProperty("AccountNickName")]
        public string AccountNickName { get; set; }

        [JsonProperty("AccICNo")]
        public string AccICNo { get; set; }

        [JsonProperty("AccICNoNew")]
        public string AccICNoNew { get; set; }

        [JsonProperty("AccComno")]
        public string AccComno { get; set; }

        [JsonProperty("AmtDeposit")]
        public double AmtDeposit { get; set; }

        [JsonProperty("AmtCurrentChg")]
        public double AmtCurrentChg { get; set; }

        [JsonProperty("AmtOutstandingChg")]
        public double AmtOutstandingChg { get; set; }

        [JsonProperty("AmtPayableChg")]
        public double AmtPayableChg { get; set; }

        [JsonProperty("AmtLastPay")]
        public double AmtLastPay { get; set; }

        [JsonProperty("DateBill")]
        public string DateBill { get; set; }

        [JsonProperty("DatePaymentDue")]
        public string DatePaymentDue { get; set; }

        [JsonProperty("DateLastPay")]
        public string DateLastPay { get; set; }

        [JsonProperty("SttSupply")]
        public string SttSupply { get; set; }

        [JsonProperty("AddStreet")]
        public string AddStreet { get; set; }

        [JsonProperty("AddArea")]
        public string AddArea { get; set; }

        [JsonProperty("AddTown")]
        public string AddTown { get; set; }

        [JsonProperty("AddState")]
        public string AddState { get; set; }

        [JsonProperty("StnName")]
        public string StnName { get; set; }

        [JsonProperty("StnAddStreet")]
        public string StnAddStreet { get; set; }

        [JsonProperty("StnAddArea")]
        public string StnAddArea { get; set; }

        [JsonProperty("StnAddTown")]
        public string StnAddTown { get; set; }

        [JsonProperty("StnAddState")]
        public string StnAddState { get; set; }

        [JsonProperty("AmtCustBal")]
        public double AmtCustBal { get; set; }

        [JsonProperty("ItemizedBillings")]
        public List<ItemizedBillingDetails> ItemizedBilling { get; set; }   

        [JsonProperty("OpenChargesTotal")]
        public double OpenChargesTotal { get; set; }   

        [JsonProperty("WhatIsThisLink")]
        public string WhatIsThisLink { get; set; }

        [JsonProperty("WhatIsThisTitle")]
        public string WhatIsThisTitle { get; set; }

        [JsonProperty("WhatIsThisMessage")]
        public string WhatIsThisMessage { get; set; }

        [JsonProperty("WhatIsThisButtonText")]
        public string WhatIsThisButtonText { get; set; } 

        [JsonProperty("IsSelected")]
        public bool IsSelected { get; set; }

        [JsonProperty("IsOwner")]
        public bool IsOwner { get; set; }
        
        [JsonProperty("AccountCategoryId")]
        public string AccountCategoryId { get; set; }

        [JsonProperty("smartMeterCode")]
        public string SmartMeterCode { get; set; }

        [JsonProperty("accountTypeId")]
        public string AccountTypeId { get; set; }

        [JsonProperty(PropertyName = "IsHaveAccess")]
        public bool IsHaveAccess { get; set; }

        [JsonProperty(PropertyName = "IsApplyEBilling")]
        public bool IsApplyEBilling { get; set; }

        [JsonProperty(PropertyName = "IsInManageAccessList")]
        public bool IsInManageAccessList { get; set; }

        [JsonProperty(PropertyName = "CreatedBy")]
        public string CreatedBy { get; set; }

        internal static AccountData Copy(AccountDetails accountDetails, bool isSelected)
        {
            return new AccountData()
            {
                AccountNum = accountDetails.AccountNum,
                AccountName = accountDetails.AccountName,
                AccICNo = accountDetails.AccICNo,
                AccICNoNew = accountDetails.AccICNoNew,
                AccComno = accountDetails.AccComno,
                AmtDeposit = accountDetails.AmtDeposit,
                AmtCurrentChg = accountDetails.AmtCurrentChg,
                AmtOutstandingChg = accountDetails.AmtOutstandingChg,
                AmtPayableChg = accountDetails.AmtPayableChg,
                AmtLastPay = accountDetails.AmtLastPay,
                DateBill = accountDetails.DateBill,
                DatePaymentDue = accountDetails.DatePaymentDue,
                DateLastPay = accountDetails.DateLastPay,
                SttSupply = accountDetails.SttSupply,
                AddStreet = accountDetails.AddStreet,
                AddArea = accountDetails.AddArea,
                AddTown = accountDetails.AddTown,
                AddState = accountDetails.AddState,
                StnName = accountDetails.StnName,
                StnAddStreet = accountDetails.StnAddStreet,
                StnAddArea = accountDetails.StnAddArea,
                StnAddTown = accountDetails.StnAddTown,
                StnAddState = accountDetails.StnAddState,
                AmtCustBal = accountDetails.AmtCustBal,
                ItemizedBilling = accountDetails.ItemizedBilling,
                OpenChargesTotal = accountDetails.OpenChargesTotal,
                WhatIsThisLink = accountDetails.WhatIsThisLink,
                WhatIsThisTitle = accountDetails.WhatIsThisTitle,
                WhatIsThisMessage = accountDetails.WhatIsThisMessage,
                WhatIsThisButtonText = accountDetails.WhatIsThisButtonText,
                AccountCategoryId = "",
                AccountTypeId = "",
                SmartMeterCode = accountDetails.SmartMeterCode,
                IsSelected = isSelected

            };
        }

        internal static AccountData Copy(AccountDetails accountDetails, CustomerBillingAccount customerBilling, bool isSelected)
        {
            return new AccountData()
            {
                AccountNum = accountDetails.AccountNum,
                AccountName = customerBilling.OwnerName,
                AccountNickName = customerBilling.AccDesc,
                AccICNo = accountDetails.AccICNo,
                AccICNoNew = accountDetails.AccICNoNew,
                AccComno = accountDetails.AccComno,
                AmtDeposit = accountDetails.AmtDeposit,
                AmtCurrentChg = accountDetails.AmtCurrentChg,
                AmtOutstandingChg = accountDetails.AmtOutstandingChg,
                AmtPayableChg = accountDetails.AmtPayableChg,
                AmtLastPay = accountDetails.AmtLastPay,
                DateBill = accountDetails.DateBill,
                DatePaymentDue = accountDetails.DatePaymentDue,
                DateLastPay = accountDetails.DateLastPay,
                SttSupply = accountDetails.SttSupply,
                AddStreet = customerBilling.AccountStAddress,
                AddArea = accountDetails.AddArea,
                AddTown = accountDetails.AddTown,
                AddState = accountDetails.AddState,
                StnName = accountDetails.StnName,
                StnAddStreet = accountDetails.StnAddStreet,
                StnAddArea = accountDetails.StnAddArea,
                StnAddTown = accountDetails.StnAddTown,
                StnAddState = accountDetails.StnAddState,
                AmtCustBal = accountDetails.AmtCustBal,
                ItemizedBilling = accountDetails.ItemizedBilling,
                OpenChargesTotal = accountDetails.OpenChargesTotal,
                WhatIsThisLink = accountDetails.WhatIsThisLink,
                WhatIsThisTitle = accountDetails.WhatIsThisTitle,
                WhatIsThisMessage = accountDetails.WhatIsThisMessage,
                WhatIsThisButtonText = accountDetails.WhatIsThisButtonText,
                AccountCategoryId = customerBilling.AccountCategoryId,
                AccountTypeId = customerBilling.AccountTypeId,
                IsOwner = customerBilling.isOwned,
                SmartMeterCode = customerBilling.SmartMeterCode,
                IsSelected = isSelected,
                IsInManageAccessList = customerBilling.IsInManageAccessList,
                CreatedBy = customerBilling.CreatedBy

            };
        }

        internal static AccountData Copy(CustomerBillingAccount accountDetails, bool isSelected)
        {
            return new AccountData()
            {
                AccountNum = accountDetails.AccNum,
                AccountName = accountDetails.OwnerName,
                AccountNickName = accountDetails.AccDesc,
                AccICNo = "",
                AccICNoNew = "",
                AccComno = "",
                AmtDeposit = 0,
                AmtCurrentChg = 0,
                AmtOutstandingChg = 0,
                AmtPayableChg = 0,
                AmtLastPay = 0,
                DateBill = "",
                DatePaymentDue = "",
                DateLastPay = "",
                SttSupply = "",
                AddStreet = accountDetails.AccountStAddress,
                AddArea = "",
                AddTown = "",
                AddState = "",
                StnName = "",
                StnAddStreet = "",
                StnAddArea = "",
                StnAddTown = "",
                StnAddState = "",
                AmtCustBal = 0,
                ItemizedBilling = null,
                OpenChargesTotal = 0,
                WhatIsThisLink = "",
                WhatIsThisTitle = "",
                WhatIsThisMessage = "",
                WhatIsThisButtonText = "",
                IsOwner = accountDetails.isOwned,
                AccountCategoryId = accountDetails.AccountCategoryId,
                AccountTypeId = accountDetails.AccountTypeId,
                SmartMeterCode = accountDetails.SmartMeterCode,
                IsSelected = isSelected,
                IsHaveAccess = accountDetails.IsHaveAccess,
                IsApplyEBilling = accountDetails.IsApplyEBilling,
                IsInManageAccessList = accountDetails.IsInManageAccessList,
                CreatedBy = accountDetails.CreatedBy


            };
        }

    }
}