
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.API;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Billing.MVP
{
    [Activity(Label = "Bill Details", MainLauncher = true,ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class BillingDetailsActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.accountName)]
        TextView accountName;

        [BindView(Resource.Id.accountAddress)]
        TextView accountAddress;

        [BindView(Resource.Id.myBillDetailsLabel)]
        TextView myBillDetailsLabel;

        [BindView(Resource.Id.accountChargeLabel)]
        TextView accountChargeLabel;

        [BindView(Resource.Id.accountChargeValue)]
        TextView accountChargeValue;

        [BindView(Resource.Id.accountBillThisMonthLabel)]
        TextView accountBillThisMonthLabel;

        [BindView(Resource.Id.accountBillThisMonthValue)]
        TextView accountBillThisMonthValue;

        [BindView(Resource.Id.accountPayAmountLabel)]
        TextView accountPayAmountLabel;

        [BindView(Resource.Id.accountPayAmountDate)]
        TextView accountPayAmountDate;

        [BindView(Resource.Id.accountPayAmountCurrency)]
        TextView accountPayAmountCurrency;

        [BindView(Resource.Id.accountPayAmountValue)]
        TextView accountPayAmountValue; 

        [BindView(Resource.Id.accountMinChargeLabel)]
        TextView accountMinChargeLabel;

        List<AccountChargeModel> selectedAccountChargesModelList;

        public override int ResourceId()
        {
            return Resource.Layout.BillingDetailsLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            TextViewUtils.SetMuseoSans300Typeface(accountAddress, accountPayAmountDate, accountPayAmountValue);
            TextViewUtils.SetMuseoSans500Typeface(accountName, myBillDetailsLabel, accountChargeLabel, accountChargeValue,
                accountBillThisMonthLabel, accountBillThisMonthValue, accountPayAmountLabel, accountPayAmountCurrency, accountMinChargeLabel);

            //Bundle extras = Intent.Extras;
            //if (extras.ContainsKey("BILL_DETAILS"))
            //{
            //    selectedAccountChargesModelList = JsonConvert.DeserializeObject<List<AccountChargeModel>>(extras.GetString("BILL_DETAILS"));
            //}
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SSMRMeterSubmitMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        [OnClick(Resource.Id.accountMinChargeLabelContainer)]
        void OnTapMinChargeTooltip(object sender, EventArgs eventArgs)
        {
            ShowAccountHasMinCharge();
        }

        public void ShowAccountHasMinCharge()
        {
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle("This account has a minimum amount to be paid.")
                .SetMessage("Your <strong>other charges</strong> like Security Deposit, Processing Fee, Stamp Duty and Meter Cost are required to be cleared first and will be your account’s minimum payment.")
                .SetCTALabel("Got It!")
                .Build()
                .Show();
        }
    }
}
