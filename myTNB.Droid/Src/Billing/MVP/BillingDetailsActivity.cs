
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Text;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.Adapter;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using myTNB_Android.Src.ViewBill.Activity;
using myTNB_Android.Src.ViewReceipt.Activity;
using Newtonsoft.Json;
using static myTNB_Android.Src.MyTNBService.Model.AccountBillPayHistoryModel;

namespace myTNB_Android.Src.Billing.MVP
{
    [Activity(Label = "Bill Details", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class BillingDetailsActivity : BaseToolbarAppCompatActivity, BillingDetailsContract.IView
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

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.btnViewBill)]
        Button btnViewBill;

        SimpleDateFormat dateParser = new SimpleDateFormat("yyyyMMdd");
        SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM yyyy");

        SimpleDateFormat billPdfDateParser = new SimpleDateFormat("yyyyMMdd");
        SimpleDateFormat billPdfDateFormatter = new SimpleDateFormat("dd/MM/yyyy");

        AccountChargeModel selectedAccountChargeModel;
        BillingHistoryData billingHistoryData;
        AccountData selectedAccountData;
        BillingDetailsContract.IPresenter billingDetailsPresenter;
        private LoadingOverlay loadingOverlay;

        [OnClick(Resource.Id.btnViewBill)]
        void OnViewBill(object sender, EventArgs eventArgs)
        {
            billingDetailsPresenter.GetBillHistory(selectedAccountData);
            try
            {
                FirebaseAnalyticsUtils.LogClickEvent(this, "View Bill Button Click");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        [OnClick(Resource.Id.btnPayBill)]
        void OnPayBill(object sender, EventArgs eventArgs)
        {
            Intent payment_activity = new Intent(this, typeof(SelectAccountsActivity));
            payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccountData));
            StartActivity(payment_activity);
            try
            {
                FirebaseAnalyticsUtils.LogClickEvent(this, "Pay Bill Button Click");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

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
            billingDetailsPresenter = new BillingDetailsPresenter(this);
            Bundle extras = Intent.Extras;
            if (extras.ContainsKey("SELECTED_ACCOUNT"))
            {
                selectedAccountData = JsonConvert.DeserializeObject<AccountData>(extras.GetString("SELECTED_ACCOUNT"));
            }
            if (extras.ContainsKey("SELECTED_BILL_DETAILS"))
            {
                selectedAccountChargeModel = JsonConvert.DeserializeObject<AccountChargeModel>(extras.GetString("SELECTED_BILL_DETAILS"));
            }
            if (extras.ContainsKey("LATEST_BILL_HISTORY"))
            {
                billingHistoryData = JsonConvert.DeserializeObject<BillingHistoryData>(extras.GetString("LATEST_BILL_HISTORY"));
            }
            SetStatusBarBackground(Resource.Drawable.dashboard_fluid_background);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

            accountName.Text = selectedAccountData.AccountNickName;
            accountAddress.Text = selectedAccountData.AddStreet;
            PopulateCharges();
        }

        private void EnableShowBillButtons(bool isEnable)
        {
            btnViewBill.Enabled = isEnable;
            if (isEnable)
            {
                btnViewBill.SetTextColor(new Color(ContextCompat.GetColor(this, Resource.Color.freshGreen)));
                btnViewBill.Background = ContextCompat.GetDrawable(this, Resource.Drawable.light_button_background);
            }
            else
            {
                btnViewBill.SetTextColor(new Color(ContextCompat.GetColor(this, Resource.Color.silverChalice)));
                btnViewBill.Background = ContextCompat.GetDrawable(this, Resource.Drawable.light_button_background_disabled);
            }
        }

        private void PopulateCharges()
        {
            if (selectedAccountChargeModel.MandatoryCharges.TotalAmount > 0f)
            {
                otherChargesExpandableView.Visibility = ViewStates.Visible;
                accountMinChargeLabelContainer.Visibility = ViewStates.Visible;
                otherChargesExpandableView.SetOtherCharges(selectedAccountChargeModel.MandatoryCharges.TotalAmount, selectedAccountChargeModel.MandatoryCharges.ChargeModelList);
                otherChargesExpandableView.RequestLayout();
            }
            else
            {
                otherChargesExpandableView.Visibility = ViewStates.Gone;
                accountMinChargeLabelContainer.Visibility = ViewStates.Gone;
            }

            accountChargeValue.Text = "RM " + (Math.Abs(selectedAccountChargeModel.OutstandingCharges)).ToString("#,##0.00");
            if (selectedAccountChargeModel.OutstandingCharges < 0f)
            {
                accountChargeLabel.Text = "I've paid extra";
                accountChargeValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.freshGreen)));
            }
            else
            {
                accountChargeLabel.Text = "My outstanding charges";
                accountChargeValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.tunaGrey)));
            }
            accountBillThisMonthValue.Text = "RM " + selectedAccountChargeModel.CurrentCharges.ToString("#,##0.00");
            accountPayAmountValue.Text = selectedAccountChargeModel.AmountDue.ToString("#,##0.00");
            if (selectedAccountChargeModel.IsNeedPay)
            {
                accountPayAmountLabel.Visibility = ViewStates.Visible;
                accountPayAmountLabel.Text = "I need to pay";
                accountPayAmountDate.Visibility = ViewStates.Visible;
                accountPayAmountDate.Text = "by " + dateFormatter.Format(dateParser.Parse(selectedAccountChargeModel.DueDate));

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

        public void ShowBillPDF(BillHistoryV5 selectedBill)
        {
            if (selectedBill != null && selectedBill.NrBill != null)
            {
                selectedBill.NrBill = null;
            }

            Intent viewBill = new Intent(this, typeof(ViewBillActivity));
            viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccountData));
            viewBill.PutExtra(Constants.SELECTED_BILL, JsonConvert.SerializeObject(selectedBill));
            StartActivity(viewBill);
        }

        public void ShowProgressDialog()
        {
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mLoadBillSnackBar;
        public void ShowBillErrorSnackBar()
        {
            try
            {
                if (mLoadBillSnackBar != null && mLoadBillSnackBar.IsShown)
                {
                    mLoadBillSnackBar.Dismiss();
                }

                mLoadBillSnackBar = Snackbar.Make(rootView, GetString(Resource.String.dashboard_chart_cancelled_exception_error), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.dashboard_chartview_data_not_available_no_internet_btn_close), delegate
                {
                    mLoadBillSnackBar.Dismiss();
                }
                );
                mLoadBillSnackBar.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
