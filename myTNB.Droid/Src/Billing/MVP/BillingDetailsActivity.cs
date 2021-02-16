﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
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
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.ViewBill.Activity;
using Newtonsoft.Json;

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

        [BindView(Resource.Id.infoLabelContainerDetailEPP)]
        LinearLayout infoLabelContainerDetailEPP;

        [BindView(Resource.Id.infoLabelDetailEPP)]
        TextView infoLabelDetailEPP;

        SimpleDateFormat dateParser = new SimpleDateFormat("yyyyMMdd", LocaleUtils.GetDefaultLocale());
        SimpleDateFormat dateFormatter = new SimpleDateFormat("dd MMM yyyy", LocaleUtils.GetCurrentLocale());

        AccountChargeModel selectedAccountChargeModel;
        AccountData selectedAccountData;
        BillingDetailsContract.IPresenter billingDetailsPresenter;
        private bool fromSelectAccountPage;
        private const string PAGE_ID = "BillDetails";
        ISharedPreferences mPref;

        private bool isPendingPayment = false;
        private bool isCheckPendingPaymentNeeded = false;

        [OnClick(Resource.Id.btnViewBill)]
        void OnViewBill(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                try
                {
                    this.SetIsClicked(true);
                    ShowBillPDF();
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
            SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_DashboardLarge : Resource.Style.Theme_Dashboard);
            TextViewUtils.SetMuseoSans300Typeface(accountAddress, accountPayAmountDate, refreshBillingDetailMessage);
            TextViewUtils.SetMuseoSans500Typeface(accountName, myBillDetailsLabel, accountChargeLabel, accountChargeValue,
                accountBillThisMonthLabel, accountBillThisMonthValue, accountPayAmountLabel, accountPayAmountCurrency,
                accountMinChargeLabel, btnPayBill, btnViewBill, btnBillingDetailefresh);

            accountAddress.TextSize = TextViewUtils.GetFontSize(12f);
            accountPayAmountDate.TextSize = TextViewUtils.GetFontSize(14f);
            accountPayAmountValue.TextSize = TextViewUtils.GetFontSize(TextViewUtils.IsLargeFonts ? 14 : 24f);
            if (TextViewUtils.IsLargeFonts)
            {
                TextViewUtils.SetMuseoSans500Typeface(accountPayAmountValue);
            }
            else
            {
                TextViewUtils.SetMuseoSans300Typeface(accountPayAmountValue);
            }
            refreshBillingDetailMessage.TextSize = TextViewUtils.GetFontSize(12f);

            accountName.TextSize = TextViewUtils.GetFontSize(14f);
            myBillDetailsLabel.TextSize = TextViewUtils.GetFontSize(16f);
            accountChargeLabel.TextSize = TextViewUtils.GetFontSize(14f);
            accountChargeValue.TextSize = TextViewUtils.GetFontSize(14f);

            accountBillThisMonthLabel.TextSize = TextViewUtils.GetFontSize(14f);
            accountBillThisMonthValue.TextSize = TextViewUtils.GetFontSize(14f);
            accountPayAmountLabel.TextSize = TextViewUtils.GetFontSize(14f);
            accountPayAmountCurrency.TextSize = TextViewUtils.GetFontSize(TextViewUtils.IsLargeFonts ? 14 : 12f);
            accountMinChargeLabel.TextSize = TextViewUtils.GetFontSize(12f);
            btnPayBill.TextSize = TextViewUtils.GetFontSize(16f);
            btnViewBill.TextSize = TextViewUtils.GetFontSize(16f);
            btnBillingDetailefresh.TextSize = TextViewUtils.GetFontSize(16f);

            infoLabelDetailEPP.TextSize = TextViewUtils.GetFontSize(11f);

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
            if (extras.ContainsKey("PEEK_BILL_DETAILS"))
            {
                fromSelectAccountPage = extras.GetBoolean("PEEK_BILL_DETAILS");
            }
            else
            {
                fromSelectAccountPage = false;
            }
            if (extras.ContainsKey("PENDING_PAYMENT"))
            {
                isCheckPendingPaymentNeeded = false;
                isPendingPayment = extras.GetBoolean("PENDING_PAYMENT");
            }
            else
            {
                isCheckPendingPaymentNeeded = true;
                isPendingPayment = false;
            }
            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

            accountName.Text = selectedAccountData.AccountNickName;
            accountAddress.Text = selectedAccountData.AddStreet;
            if (selectedAccountChargeModel != null && !isCheckPendingPaymentNeeded)
            {
                topLayout.Visibility = ViewStates.Visible;
                PopulateCharges();
                EnablePayBillButtons();

                if (extras.ContainsKey("IS_VIEW_BILL_DISABLE"))
                {
                    bool isViewBillDisable = extras.GetBoolean("IS_VIEW_BILL_DISABLE");

                    if (isViewBillDisable)
                    {
                        EnableDisableViewBillButtons(false);
                    }
                    else
                    {
                        EnableDisableViewBillButtons(true);
                    }
                }
                else
                {
                    EnableDisableViewBillButtons(true);
                }
            }
            else
            {
                topLayout.Visibility = ViewStates.Invisible;
                this.billingDetailsPresenter.ShowBillDetails(selectedAccountData, isCheckPendingPaymentNeeded);
            }
        }

        private void EnableEppTooltip(bool isTooltipShown)
        {
            if (isTooltipShown == true)
            {
                infoLabelDetailEPP.Text = Utility.GetLocalizedCommonLabel("eppToolTipTitle");
                infoLabelContainerDetailEPP.Visibility = ViewStates.Visible;
            }
            else
            {
                infoLabelContainerDetailEPP.Visibility = ViewStates.Gone;
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

        public void EnableDisableViewBillButtons(bool flag)
        {
            if (flag)
            {
                btnViewBill.Enabled = true;
                btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.freshGreen));
                btnViewBill.Background = ContextCompat.GetDrawable(this, Resource.Drawable.light_green_outline_button_background);
            }
            else
            {
                btnViewBill.Enabled = false;
                btnViewBill.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.silverChalice));
                btnViewBill.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_outline);
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

            EnableEppTooltip(selectedAccountChargeModel.ShowEppToolTip);

            if (selectedAccountChargeModel.MandatoryCharges.TotalAmount > 0f && selectedAccountChargeModel.ShowEppToolTip == false)
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

            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            if (selectedAccountChargeModel.OutstandingCharges < 0f)
            {
                accountChargeValue.Text = "- RM " + (Math.Abs(selectedAccountChargeModel.OutstandingCharges) * -1).ToString("#,##0.00", currCult);
            }
            else
            {
                accountChargeValue.Text = "RM " + (Math.Abs(selectedAccountChargeModel.OutstandingCharges)).ToString("#,##0.00", currCult);
            }

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

            if (selectedAccountChargeModel.CurrentCharges < 0f)
            {
                accountBillThisMonthValue.Text = "- RM " + (selectedAccountChargeModel.CurrentCharges * -1).ToString("#,##0.00", currCult);
            }
            else
            {
                accountBillThisMonthValue.Text = "RM " + selectedAccountChargeModel.CurrentCharges.ToString("#,##0.00", currCult);  //ori code
            }


            accountPayAmountValue.Text = selectedAccountChargeModel.AmountDue.ToString("#,##0.00", currCult);
            if (selectedAccountChargeModel.IsNeedPay)
            {
                accountPayAmountLabel.Visibility = ViewStates.Visible;
                accountPayAmountLabel.Text = GetLabelByLanguage("needToPay");
                accountPayAmountDate.Visibility = ViewStates.Visible;
                accountPayAmountDate.Text = GetLabelByLanguage("by") + " " + dateFormatter.Format(dateParser.Parse(selectedAccountChargeModel.DueDate));

                accountPayAmountCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.tunaGrey)));
                accountPayAmountValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.tunaGrey)));
            }
            else if (selectedAccountChargeModel.IsPaidExtra)
            {
                accountPayAmountLabel.Visibility = ViewStates.Visible;
                accountPayAmountDate.Visibility = ViewStates.Gone;

                accountPayAmountLabel.Text = GetLabelByLanguage("paidExtra");
                accountPayAmountValue.Text = (Math.Abs(selectedAccountChargeModel.AmountDue)).ToString("#,##0.00", currCult);
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

            if (isPendingPayment)
            {
                accountPayAmountLabel.Visibility = ViewStates.Visible;
                accountPayAmountDate.Visibility = ViewStates.Gone;

                accountPayAmountLabel.Text = Utility.GetLocalizedCommonLabel("paymentPendingMsg");
                accountPayAmountCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.lightOrange)));
                accountPayAmountValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.lightOrange)));
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
                if (modelList != null && modelList.Count > 0)
                {
                    UnderstandBillToolTipAdapter adapter = new UnderstandBillToolTipAdapter(modelList);
                    MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.LISTVIEW_WITH_INDICATOR_AND_HEADER)
                        .SetAdapter(adapter)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "gotIt"))
                        .SetCTAaction(() => { this.SetIsClicked(false); })
                        .Build()
                        .Show();
                }
                else
                {
                    this.SetIsClicked(false);
                }
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

        [OnClick(Resource.Id.infoLabelContainerDetailEPP)]
        void OnTapEPPTooltip(object sender, EventArgs eventArgs)
        {
            ShowEPPDetailsTooltip();
        }

        [OnClick(Resource.Id.btnBillingDetailefresh)]
        void OnTapBillingDetailRefresh(object sender, EventArgs eventArgs)
        {
            topLayout.Visibility = ViewStates.Invisible;
            this.billingDetailsPresenter.ShowBillDetails(selectedAccountData, isCheckPendingPaymentNeeded);
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


        public void ShowEPPDetailsTooltip()
        {
            List<EPPTooltipResponse> modelList = MyTNBAppToolTipData.GetEppToolTipData();

            if (modelList != null && modelList.Count > 0)
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    MyTNBAppToolTipBuilder eppTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER_TWO_BUTTON)
                       .SetHeaderImageBitmap(modelList[0].ImageBitmap)
                       .SetTitle(modelList[0].PopUpTitle)
                       .SetMessage(modelList[0].PopUpBody)
                       .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                       .SetCTAaction(() => { this.SetIsClicked(false); })

                       .SetSecondaryCTALabel(Utility.GetLocalizedCommonLabel("viewBill"))
                       .SetSecondaryCTAaction(() => ShowBillPDF())
                       .Build();

                    eppTooltip.Show();
                }
            }
        }

        public void ShowBillPDF()
        {
            Intent viewBill = new Intent(this, typeof(ViewBillActivity));
            viewBill.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(selectedAccountData));
            viewBill.PutExtra(Constants.CODE_KEY, Constants.SELECT_ACCOUNT_PDF_REQUEST_CODE);
            StartActivity(viewBill);
            this.SetIsClicked(false);
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

        public void OnUpdatePendingPayment(bool mIsPendingPayament)
        {
            isPendingPayment = mIsPendingPayament;
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

                i = offsetViewBounds.Top + (int)DPUtils.ConvertDPToPx(14f);

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }

        public void ShowViewBillError(string title, string message)
        {
            string errorTitle = string.IsNullOrEmpty(title) ? Utility.GetLocalizedErrorLabel("defaultErrorTitle") : title;
            string errorMessage = string.IsNullOrEmpty(message) ? Utility.GetLocalizedErrorLabel("defaultErrorMessage") : message;
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(errorTitle)
                .SetMessage(errorMessage)
                .SetCTALabel(Utility.GetLocalizedCommonLabel("ok"))
                .Build().Show();
            this.SetIsClicked(false);
        }

    }
}
