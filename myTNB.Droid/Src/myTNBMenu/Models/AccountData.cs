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
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.myTNBMenu.Models
{
    public class AccountData
    {
           //"accNum": "240000876706",
           // "accName": "CHONG MING LUEN",
           // "accICNo": null,
           // "accICNoNew": "830311045007",
           // "accComNo": null,
           // "amDeposit": 0,
           // "amCurrentChg": -746.12,
           // "amOutstandingChg": 0,
           // "amPayableChg": -746.12,
           // "amLastPay": 0,
           // "dateBill": "04/08/2017",
           // "datePymtDue": "03/09/2017",
           // "dateLastPay": "N/A",
           // "sttSupply": "Active",
           // "addStreet": "JLN MACAP UMBOO, KG BARU MACAP, 76100, MACHAP",
           // "addArea": null,
           // "addTown": null,
           // "addState": null,
           // "stnName": "PC Jasin",
           // "stnAddStreet": null,
           // "stnAddArea": null,
           // "stnAddTown": null,
           // "stnAddState": null,
           // "amCustBal": -746.12,
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

        [JsonProperty("IsSelected")]
        public bool IsSelected { get; set; }

        [JsonProperty("IsOwner")]
        public bool IsOwner { get; set; }

        [JsonProperty("AccountCategoryId")]
        public string AccountCategoryId { get; set; }

        [JsonProperty("smartMeterCode")]
        public string SmartMeterCode { get; set; }

        internal static AccountData Copy(AccountDetails accountDetails , bool isSelected)
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
                AccountCategoryId = "",
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
                AccountCategoryId = customerBilling.AccountCategoryId,
                IsOwner = customerBilling.isOwned,
                SmartMeterCode = customerBilling.SmartMeterCode,
                IsSelected = isSelected

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
                IsOwner = accountDetails.isOwned,
                AccountCategoryId = accountDetails.AccountCategoryId,
                SmartMeterCode = accountDetails.SmartMeterCode,
                IsSelected = isSelected

            };
        }


    }
}