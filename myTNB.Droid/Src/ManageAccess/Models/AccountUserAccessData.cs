using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Database.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB_Android.Src.ManageAccess.Models
{
    public class AccountUserAccessData
    {
        [JsonProperty("accNum")]
        public string AccNum { get; set; }

        [JsonProperty("accDesc")]
        public string AccDesc { get; set; }

        [JsonProperty("userAccountID")]
        public string UserAccountId { get; set; }

        [JsonProperty("IsApplyEBilling")]
        public bool IsApplyEBilling { get; set; }

        [JsonProperty("IsHaveAccess")]
        public bool IsHaveAccess { get; set; }

        [JsonProperty("IsOwnedAccount")]
        public bool IsOwnedAccount { get; set; }

        [JsonProperty("IsPreRegister")]
        public bool IsPreRegister { get; set; }

        [JsonProperty("email")]
        public string email { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("isSelected")]
        public bool? isSelected { get; set; }

        [JsonProperty("userId")]
        public string userId { get; set; }

        internal static AccountUserAccessData Copy(AccountDetails accountDetails, bool isSelected)
        {
            return new AccountUserAccessData()
            {
                /*AccountNum = accountDetails.AccountNum,
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
                SmartMeterCode = accountDetails.SmartMeterCode,
                IsSelected = isSelected*/

            };
        }
    }
}