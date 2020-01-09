
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
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
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.NewAppTutorial.MVP;
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
    public class BillingDetailsActivity : BaseActivityCustom, BillingDetailsContract.IView
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

        [BindView(Resource.Id.btnPayBill)]
        Button btnPayBill;

        [BindView(Resource.Id.bottomLayout)]
        LinearLayout bottomLayout;

        [BindView(Resource.Id.topLayout)]
        LinearLayout topLayout;

        [BindView(Resource.Id.detailLayout)]
        LinearLayout detailLayout;

        [BindView(Resource.Id.refreshLayout)]
        LinearLayout refreshLayout;

        [BindView(Resource.Id.refreshBillingDetailImg)]
        ImageView refreshBillingDetailImg;

        [BindView(Resource.Id.refreshBillingDetailMessage)]
        TextView refreshBillingDetailMessage;

        [BindView(Resource.Id.btnBillingDetailefresh)]
        Button btnBillingDetailefresh;


        SimpleDateFormat dateParser = new SimpleDateFormat("yyyyMMdd");
        SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM yyyy");

        SimpleDateFormat billPdfDateParser = new SimpleDateFormat("yyyyMMdd");
        SimpleDateFormat billPdfDateFormatter = new SimpleDateFormat("dd/MM/yyyy");

        AccountChargeModel selectedAccountChargeModel;
        BillingHistoryData billingHistoryData;
        AccountData selectedAccountData;
        BillingDetailsContract.IPresenter billingDetailsPresenter;
        private LoadingOverlay loadingOverlay;
		private bool fromSelectAccountPage;
        private const string PAGE_ID = "BillDetails";
        ISharedPreferences mPref;
        private bool isTutorialShown = false;

        [OnClick(Resource.Id.btnViewBill)]
        void OnViewBill(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                billingDetailsPresenter.GetBillHistory(selectedAccountData);
                try
                {
                    FirebaseAnalyticsUtils.LogClickEvent(this, "View Bill Buttom Clicked");
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
            }
        }

        [OnClick(Resource.Id.btnPayBill)]
        void OnPayBill(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                if (fromSelectAccountPage)
                {
                    Finish();
                }
                else
                {
                    this.SetIsClicked(true);
                    Intent payment_activity = new Intent(this, typeof(SelectAccountsActivity));
                    payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccountData));
                    payment_activity.PutExtra(Constants.FROM_BILL_DETAILS_PAGE, true);
                    StartActivity(payment_activity);
                }

                try
                {
                    FirebaseAnalyticsUtils.LogClickEvent(this, "Billing Payment Buttom Clicked");
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
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
            TextViewUtils.SetMuseoSans300Typeface(accountAddress, accountPayAmountDate, accountPayAmountValue, refreshBillingDetailMessage);
            TextViewUtils.SetMuseoSans500Typeface(accountName, myBillDetailsLabel, accountChargeLabel, accountChargeValue,
                accountBillThisMonthLabel, accountBillThisMonthValue, accountPayAmountLabel, accountPayAmountCurrency,
                accountMinChargeLabel, btnPayBill, btnViewBill, btnBillingDetailefresh);
            billingDetailsPresenter = new BillingDetailsPresenter(this);
            myBillDetailsLabel.Text = GetLabelByLanguage("billDetails");
            accountBillThisMonthLabel.Text = GetLabelByLanguage("billThisMonth");
            accountMinChargeLabel.Text = GetLabelByLanguage("minimumChargeDescription");
            btnViewBill.Text = GetLabelCommonByLanguage("viewBill");
            btnPayBill.Text = GetLabelByLanguage("pay");
            mPref = PreferenceManager.GetDefaultSharedPreferences(this);
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
			if (extras.ContainsKey("PEEK_BILL_DETAILS"))
			{
                fromSelectAccountPage = extras.GetBoolean("PEEK_BILL_DETAILS");
			}
			else
			{
				fromSelectAccountPage = false;
			}
			SetStatusBarBackground(Resource.Drawable.dashboard_fluid_background);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

            accountName.Text = selectedAccountData.AccountNickName;
            accountAddress.Text = selectedAccountData.AddStreet;
            if (selectedAccountChargeModel != null)
            {
                topLayout.Visibility = ViewStates.Visible;
                PopulateCharges();
                EnablePayBillButtons();
            }
            else
            {
                topLayout.Visibility = ViewStates.Invisible;
                this.billingDetailsPresenter.ShowBillDetails(selectedAccountData);
            }
        }

        private void EnablePayBillButtons()
        {
            bool isPaymentButtonEnable = Utility.IsEnablePayment();
            btnPayBill.Enabled = isPaymentButtonEnable;
            if (isPaymentButtonEnable)
            {
                btnPayBill.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
            }
            else
            {
                btnPayBill.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
            }
        }

        public void ShowBillDetails(List<AccountChargeModel> accountChargeModelList)
        {
            try
            {
                topLayout.Visibility = ViewStates.Visible;
                detailLayout.Visibility = ViewStates.Visible;
                refreshLayout.Visibility = ViewStates.Gone;
                selectedAccountChargeModel = accountChargeModelList[0];
                PopulateCharges();
                EnablePayBillButtons();
                Handler h = new Handler();
                Action myAction = () =>
                {
                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                    if (!UserSessions.HasItemizedBillingDetailTutorialShown(this.mPref))
                    {
                        OnShowItemizedBillingTutorialDialog();
                    }
                };
                h.PostDelayed(myAction, 50);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowBillDetailsError(bool isRefresh, string btnText, string contentText)
        {
            try
            {
                topLayout.Visibility = ViewStates.Visible;
                detailLayout.Visibility = ViewStates.Gone;
                refreshLayout.Visibility = ViewStates.Visible;
                if (isRefresh)
                {
                    refreshBillingDetailImg.SetImageResource(Resource.Drawable.refresh_1);
                    if (!string.IsNullOrEmpty(contentText))
                    {
                        refreshBillingDetailMessage.TextFormatted = GetFormattedText(contentText);
                    }
                    else
                    {
                        refreshBillingDetailMessage.TextFormatted = GetFormattedText(GetLabelCommonByLanguage("refreshDescription"));
                    }
                    if (!string.IsNullOrEmpty(btnText))
                    {
                        btnBillingDetailefresh.Text = btnText;
                    }
                    else
                    {
                        btnBillingDetailefresh.Text = Utility.GetLocalizedCommonLabel("refreshNow");
                    }
                    btnBillingDetailefresh.Visibility = ViewStates.Visible;
                }
                else
                {
                    refreshBillingDetailImg.SetImageResource(Resource.Drawable.maintenance_new);
                    if (!string.IsNullOrEmpty(contentText))
                    {
                        refreshBillingDetailMessage.TextFormatted = GetFormattedText(contentText);
                    }
                    else
                    {
                        refreshBillingDetailMessage.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("Error", "plannedDownTimeMessage"));
                    }
                    btnBillingDetailefresh.Visibility = ViewStates.Gone;
                }

                EnablePayBillButtons();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void PopulateCharges()
        {
            if (selectedAccountChargeModel.MandatoryCharges.TotalAmount > 0f)
            {
                otherChargesExpandableView.Visibility = ViewStates.Visible;
                accountMinChargeLabelContainer.Visibility = ViewStates.Visible;
                otherChargesExpandableView.SetApplicationChargesLabel(GetLabelByLanguage("applicationCharges"));
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
                accountChargeLabel.Text = GetLabelByLanguage("paidExtra");
                accountChargeValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.freshGreen)));
            }
            else
            {
                accountChargeLabel.Text = GetLabelByLanguage("outstandingCharges");// "My outstanding charges";
                accountChargeValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.tunaGrey)));
            }
            accountBillThisMonthValue.Text = "RM " + selectedAccountChargeModel.CurrentCharges.ToString("#,##0.00");
            accountPayAmountValue.Text = selectedAccountChargeModel.AmountDue.ToString("#,##0.00");
            if (selectedAccountChargeModel.IsNeedPay)
            {
                accountPayAmountLabel.Visibility = ViewStates.Visible;
                accountPayAmountLabel.Text = GetLabelByLanguage("needToPay");
                accountPayAmountDate.Visibility = ViewStates.Visible;
                accountPayAmountDate.Text = GetLabelByLanguage("by") + " " + dateFormatter.Format(dateParser.Parse(selectedAccountChargeModel.DueDate));

                accountPayAmountCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.tunaGrey)));
                accountPayAmountValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.tunaGrey)));
            }
            else if(selectedAccountChargeModel.IsPaidExtra)
            {
                accountPayAmountLabel.Visibility = ViewStates.Visible;
                accountPayAmountDate.Visibility = ViewStates.Gone;

                accountPayAmountLabel.Text = GetLabelByLanguage("paidExtra");
                accountPayAmountValue.Text = (Math.Abs(selectedAccountChargeModel.AmountDue)).ToString("#,##0.00");
                accountPayAmountCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.freshGreen)));
                accountPayAmountValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.freshGreen)));
            }
            else if (selectedAccountChargeModel.IsCleared)
            {
                accountPayAmountLabel.Visibility = ViewStates.Visible;
                accountPayAmountDate.Visibility = ViewStates.Gone;

                accountPayAmountLabel.Text = GetLabelByLanguage("clearedBills");
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
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                List<UnderstandTooltipModel> modelList = MyTNBAppToolTipData.GetUnderstandBillTooltipData(this);
                UnderstandBillToolTipAdapter adapter = new UnderstandBillToolTipAdapter(modelList);
                MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.LISTVIEW_WITH_INDICATOR_AND_HEADER)
                    .SetAdapter(adapter)
                    .SetContext(this)
                    .SetCTALabel(Utility.GetLocalizedLabel("Common", "gotIt"))
                    .SetCTAaction(()=> { this.SetIsClicked(false);})
                    .Build()
                    .Show();
            }
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

        [OnClick(Resource.Id.btnBillingDetailefresh)]
        void OnTapBillingDetailRefresh(object sender, EventArgs eventArgs)
        {
            topLayout.Visibility = ViewStates.Invisible;
            this.billingDetailsPresenter.ShowBillDetails(selectedAccountData);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (selectedAccountChargeModel != null)
            {
                Handler h = new Handler();
                Action myAction = () =>
                {
                    NewAppTutorialUtils.ForceCloseNewAppTutorial();
                    if (!UserSessions.HasItemizedBillingDetailTutorialShown(this.mPref))
                    {
                        OnShowItemizedBillingTutorialDialog();
                    }
                };
                h.PostDelayed(myAction, 50);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public void ShowAccountHasMinCharge()
        {
            BillMandatoryChargesTooltipModel mandatoryTooltipModel = MyTNBAppToolTipData.GetInstance().GetMandatoryChargesTooltipData();
            if (mandatoryTooltipModel != null)
            {
                MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(mandatoryTooltipModel.Title)
                .SetMessage(mandatoryTooltipModel.Description)
                .SetCTALabel(mandatoryTooltipModel.CTA)
                .Build().Show();
            }
        }

        public void ShowBillPDF(string selectedBillJson)
        {
            //if (selectedBill != null && selectedBill.NrBill != null)
            //{
            //    selectedBill.NrBill = null;
            //}

            Intent viewBill = new Intent(this, typeof(ViewBillActivity));
            viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccountData));
            viewBill.PutExtra(Constants.SELECTED_BILL, selectedBillJson);
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

                mLoadBillSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
                {
                    mLoadBillSnackBar.Dismiss();
                }
                );
                View v = mLoadBillSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mLoadBillSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public void OnShowItemizedBillingTutorialDialog()
        {
            Handler h = new Handler();
            Action myAction = () =>
            {
                NewAppTutorialUtils.OnShowNewAppTutorial(this, null, mPref, this.billingDetailsPresenter.OnGeneraNewAppTutorialList());
            };
            h.PostDelayed(myAction, 100);
        }

        public int GetViewBillButtonHeight()
        {
            int height = btnViewBill.Height;
            return height;
        }

        public int GetViewBillButtonWidth()
        {
            int width = btnViewBill.Width;
            return width;
        }

        public int GetTopHeight()
        {
            int i = 0;

            try
            {
                Rect offsetViewBounds = new Rect();
                //returns the visible bounds
                bottomLayout.GetDrawingRect(offsetViewBounds);
                // calculates the relative coordinates to the parent

                rootView.OffsetDescendantRectToMyCoords(bottomLayout, offsetViewBounds);

                i = offsetViewBounds.Top + (int) DPUtils.ConvertDPToPx(6f);

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }
    }
}
