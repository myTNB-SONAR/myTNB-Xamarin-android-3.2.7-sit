
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.Adapter;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Billing.MVP
{
    [Activity(Label = "Bill Details", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
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

        [BindView(Resource.Id.otherChargesExpandableView)]
        ExpandableTextViewComponent otherChargesExpandableView;

        [BindView(Resource.Id.accountMinChargeLabelContainer)]
        LinearLayout accountMinChargeLabelContainer;

        AccountChargeModel selectedAccountChargeModel;

        AccountData selectedAccountData;

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

            Bundle extras = Intent.Extras;
            if (extras.ContainsKey("SELECTED_ACCOUNT"))
            {
                selectedAccountData = JsonConvert.DeserializeObject<AccountData>(extras.GetString("SELECTED_ACCOUNT"));
            }
            if (extras.ContainsKey("SELECTED_BILL_DETAILS"))
            {
                selectedAccountChargeModel = JsonConvert.DeserializeObject<AccountChargeModel>(extras.GetString("SELECTED_BILL_DETAILS"));
            }
            SetStatusBarBackground(Resource.Drawable.dashboard_fluid_background);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

            accountName.Text = selectedAccountData.AccountNickName;
            accountAddress.Text = selectedAccountData.AddStreet;
            PopulateCharges();
        }

        private void PopulateCharges()
        {
            if (selectedAccountChargeModel.MandatoryCharges.TotalAmount > 0f)
            {
                otherChargesExpandableView.Visibility = ViewStates.Visible;
                accountMinChargeLabelContainer.Visibility = ViewStates.Visible;
                otherChargesExpandableView.SetOtherCharges(selectedAccountChargeModel.MandatoryCharges.TotalAmount, selectedAccountChargeModel.MandatoryCharges.ChargeModelList);
                otherChargesExpandableView.RequestLayout();
                ShowAccountHasMinCharge();
            }
            else
            {
                otherChargesExpandableView.Visibility = ViewStates.Gone;
                accountMinChargeLabelContainer.Visibility = ViewStates.Gone;
            }

            accountChargeValue.Text = "RM " + selectedAccountChargeModel.OutstandingCharges.ToString("#,##0.00");
            accountBillThisMonthValue.Text = "RM " + selectedAccountChargeModel.CurrentCharges.ToString("#,##0.00");
            accountPayAmountValue.Text = selectedAccountChargeModel.AmountDue.ToString("#,##0.00");
            if (selectedAccountChargeModel.IsNeedPay)
            {
                accountPayAmountLabel.Visibility = ViewStates.Visible;
                accountPayAmountLabel.Text = "I need to pay";
                accountPayAmountDate.Visibility = ViewStates.Visible;
                accountPayAmountDate.Text = "by " + selectedAccountChargeModel.DueDate;

                accountPayAmountCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.tunaGrey)));
                accountPayAmountValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.tunaGrey)));
            }
            else if(selectedAccountChargeModel.IsPaidExtra)
            {
                accountPayAmountLabel.Visibility = ViewStates.Visible;
                accountPayAmountDate.Visibility = ViewStates.Gone;

                accountPayAmountLabel.Text = "I've paid extra";
                accountPayAmountValue.Text = (Math.Abs(selectedAccountChargeModel.AmountDue)).ToString("#,##0.00");
                accountPayAmountCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.freshGreen)));
                accountPayAmountValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.freshGreen)));
            }
            else if (selectedAccountChargeModel.IsCleared)
            {
                accountPayAmountLabel.Visibility = ViewStates.Visible;
                accountPayAmountDate.Visibility = ViewStates.Gone;

                accountPayAmountLabel.Text = "I've cleared all bills";
                accountPayAmountCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.tunaGrey)));
                accountPayAmountValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.tunaGrey)));
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SSMRMeterSubmitMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        private void ShowUnderstandBillTooltip()
        {
            List<UnderstandTooltipModel> modelList = MyTNBAppToolTipData.GetUnderstandBillTooltipData();
            UnderstandBillToolTipAdapter adapter = new UnderstandBillToolTipAdapter(modelList);
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.LISTVIEW_WITH_INDICATOR_AND_HEADER)
                .SetAdapter(adapter)
                .SetContext(this)
                .Build()
                .Show();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_ssmr_meter_reading_more:
                    ShowUnderstandBillTooltip();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        [OnClick(Resource.Id.accountMinChargeLabelContainer)]
        void OnTapMinChargeTooltip(object sender, EventArgs eventArgs)
        {
            ShowAccountHasMinCharge();
        }

        public void ShowAccountHasMinCharge()
        {
            BillMandatoryChargesTooltipModel mandatoryTooltipModel = MyTNBAppToolTipData.GetInstance().GetMandatoryChargesTooltipData();
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(mandatoryTooltipModel.Title)
                .SetMessage(mandatoryTooltipModel.Description)
                .SetCTALabel(mandatoryTooltipModel.CTA)
                .Build().Show();
        }
    }
}
