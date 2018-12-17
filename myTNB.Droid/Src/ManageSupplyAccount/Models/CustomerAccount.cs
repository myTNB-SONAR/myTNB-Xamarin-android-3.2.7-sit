﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.ManageSupplyAccount.Models
{
    public class CustomerAccount
    {
        [JsonProperty("accNum")]
        public string AccNum { get; set; }

        [JsonProperty("accDesc")]
        public string AccDesc { get; set; }

        [JsonProperty("userAccountID")]
        public string UserAccountId { get; set; }

        [JsonProperty("icNum")]
        public string ICNum { get; set; }

        [JsonProperty("amCurrentChg")]
        public string AmtCurrentChg { get; set; }

        [JsonProperty("isRegistered")]
        public bool IsRegistered { get; set; }

        [JsonProperty("isPaid")]
        public bool IsPaid { get; set; }

        [JsonProperty("isOwned")]
        public bool IsOwned { get; set; }

        [JsonProperty("isSelected")]
        public bool IsSelected { get; set; }

        public static CustomerAccount Copy(CustomerBillingAccount customerBillingAccount)
        {
            return new CustomerAccount()
            {
                AccNum = customerBillingAccount.AccNum,
                AccDesc = customerBillingAccount.AccDesc,
                UserAccountId = customerBillingAccount.UserAccountId,
                ICNum = customerBillingAccount.ICNum,
                AmtCurrentChg = customerBillingAccount.AmtCurrentChg,
                IsRegistered = customerBillingAccount.IsRegistered,
                IsPaid = customerBillingAccount.IsPaid,
                IsOwned = customerBillingAccount.isOwned,
                IsSelected = customerBillingAccount.IsSelected
            };
        }
    }
}