﻿using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using myTNB.Mobile;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.CompoundView;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Activity;
using myTNB.AndroidApp.Src.MyTNBService.Model;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using static myTNB.AndroidApp.Src.CompoundView.ExpandableTextViewComponent;

namespace myTNB.AndroidApp.Src.ApplicationStatus.ApplicationStatusDetailPayment.MVP
{
    [Activity(Label = "Bill Details", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class ApplicationStatusDetailPaymentActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.accountPayAmountLabel)]
        TextView accountPayAmountLabel;

        [BindView(Resource.Id.accountPayAmountCurrency)]
        TextView accountPayAmountCurrency;

        [BindView(Resource.Id.accountPayAmountValue)]
        TextView accountPayAmountValue;

        [BindView(Resource.Id.otherChargesExpandableView)]
        ExpandableTextViewComponent otherChargesExpandableView;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

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

        private const string PAGE_ID = "ApplicationStatus";
        ISharedPreferences mPref;

        GetApplicationStatusDisplay applicationDetailDisplay;

        [OnClick(Resource.Id.btnPayBill)]
        void OnPayBill(object sender, EventArgs eventArgs)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] OnPayBill");
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent intent = new Intent(this, typeof(PaymentActivity));
                intent.PutExtra("ISAPPLICATIONPAYMENT", true);
                intent.PutExtra("APPLICATIONPAYMENTDETAIL", JsonConvert.SerializeObject(applicationDetailDisplay.applicationPaymentDetail));
                intent.PutExtra("TOTAL", applicationDetailDisplay.PaymentDisplay.TotalPayableAmountDisplay);
                intent.PutExtra("ApplicationType", applicationDetailDisplay.ApplicationTypeCode);
                intent.PutExtra("SearchTerm", string.IsNullOrEmpty(applicationDetailDisplay.SavedApplicationID)
                    || string.IsNullOrWhiteSpace(applicationDetailDisplay.SavedApplicationID)
                        ? applicationDetailDisplay.ApplicationDetail?.ApplicationId ?? string.Empty
                        : applicationDetailDisplay.SavedApplicationID);
                intent.PutExtra("ApplicationSystem", applicationDetailDisplay.System);
                intent.PutExtra("StatusId", applicationDetailDisplay?.ApplicationStatusDetail?.StatusId.ToString() ?? string.Empty);
                intent.PutExtra("StatusCode", applicationDetailDisplay?.ApplicationStatusDetail?.StatusCode ?? string.Empty);
                intent.PutExtra("ApplicationDetailDisplay", JsonConvert.SerializeObject(applicationDetailDisplay) ?? string.Empty);
                StartActivity(intent);
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
            return Resource.Layout.ApplicationStatusDetailPaymentDetailLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);
            SetToolBarTitle(Utility.GetLocalizedLabel("ApplicationStatusPaymentDetails", "title"));
            btnPayBill.Text = Utility.GetLocalizedLabel("ApplicationStatusPaymentDetails", "payNow");
            TextViewUtils.SetMuseoSans300Typeface(accountPayAmountValue, refreshBillingDetailMessage);
            TextViewUtils.SetMuseoSans500Typeface(accountPayAmountLabel, accountPayAmountCurrency
              , btnPayBill, btnBillingDetailefresh);

            mPref = PreferenceManager.GetDefaultSharedPreferences(this);
            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                applicationDetailDisplay = JsonConvert.DeserializeObject<GetApplicationStatusDisplay>(extras.GetString("applicationDetailDisplay"));
            }
            PopulateCharges();
            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
            TextViewUtils.SetTextSize12(accountPayAmountCurrency, refreshBillingDetailMessage);
            TextViewUtils.SetTextSize14(accountPayAmountLabel);
            TextViewUtils.SetTextSize16(btnPayBill, btnBillingDetailefresh);
            TextViewUtils.SetTextSize24(accountPayAmountValue);

            if (!applicationDetailDisplay.IsPaymentEnabled)
            {
                btnPayBill.Enabled = false;
                btnPayBill.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.white));
                btnPayBill.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
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
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        private void PopulateCharges()
        {
            if (applicationDetailDisplay != null & applicationDetailDisplay.PaymentDetailsList != null)
            {
                List<ChargeModel> chargeList = new List<ChargeModel>();

                foreach (var charge in applicationDetailDisplay.PaymentDetailsList)
                {
                    ChargeModel chargeModel = new ChargeModel();
                    chargeModel.Title = charge.Title;
                    chargeModel.AmountDisplay = charge.Value;
                    chargeList.Add(chargeModel);
                }
                accountPayAmountValue.Text = applicationDetailDisplay.PaymentDisplay.TotalPayableAmountDisplay;
                if (applicationDetailDisplay.CTAType == DetailCTAType.Pay)
                {
                    accountPayAmountLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusPaymentDetails", "needToPay");
                    accountPayAmountCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.charcoalGrey)));
                    accountPayAmountValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.charcoalGrey)));
                    bottomLayout.Visibility = Android.Views.ViewStates.Visible;
                }
                else if (applicationDetailDisplay.CTAType == DetailCTAType.PayInProgress)
                {
                    accountPayAmountLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusPaymentDetails", "paymentInProgress");
                    accountPayAmountCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.lightOrange)));
                    accountPayAmountValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.lightOrange)));
                    bottomLayout.Visibility = Android.Views.ViewStates.Gone;
                }
                otherChargesExpandableView.SetExpandableType(ExpandableTextViewType.APPLICATION_STATUS);
                otherChargesExpandableView.SetApplicationChargesLabel(GetLabelByLanguage("applicationCharges"));
                otherChargesExpandableView.SetApplicationOtherCharges(
                    Utility.GetLocalizedLabel("ApplicationStatusPaymentDetails", "oneTimeCharges")
                    , "RM " + applicationDetailDisplay.PaymentDisplay.OneTimeChargesAmountDisplay, chargeList);
                otherChargesExpandableView.RequestLayout();
            }
        }
    }
}