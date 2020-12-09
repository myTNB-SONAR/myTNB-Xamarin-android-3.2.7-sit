using Android.App;
using Android.Content;
using Android.OS;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Base.Activity;
using myTNB.Mobile;
using Newtonsoft.Json;
using CheeseBind;
using Android.Widget;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.Adapter;
using AndroidX.RecyclerView.Widget;
using System;
using myTNB_Android.Src.Database.Model;
using AndroidX.Core.Content;
using Android.Views;
using myTNB_Android.Src.ApplicationStatus.ApplicationDetailActivityLog.MVP;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetailPayment.MVP;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.ViewReceipt.Activity;
using myTNB_Android.Src.ViewBill.Activity;
using Android.Preferences;
using Android.Graphics;
using myTNB_Android.Src.Login.Activity;
using myTNB.Mobile.SessionCache;
using myTNB_Android.Src.ApplicationStatusRating.Activity;
using myTNB.Mobile.API.Managers.Rating;
using myTNB.Mobile.API.Models.Rating.GetCustomerRatingMaster;
using myTNB;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP
{
    [Activity(Label = "Application Details", Theme = "@style/Theme.AppointmentScheduler")]
    public class ApplicationStatusDetailActivity : BaseActivityCustom, ApplicationStatusDetailContract.IView
    {
        [BindView(Resource.Id.rootview)]
        LinearLayout rootview;

        const string PAGE_ID = "ApplicationStatus";
        ApplicationStatusDetailProgressAdapter adapter;
        ApplicationStatusDetailSubDetailAdapter subAdapter;
        RecyclerView.LayoutManager layoutManager;
        GetApplicationStatusDisplay applicationDetailDisplay;
        GetCustomerRatingMasterResponse customerRatingMasterResponse;

        IMenuItem applicationFilterMenuItem;
        ApplicationStatusDetailPresenter presenter;
        [BindView(Resource.Id.txtApplicationStatusMainTitle)]
        TextView txtApplicationStatusMainTitle;

        [BindView(Resource.Id.applicationStatusMainStatusLayout)]
        LinearLayout applicationStatusMainStatusLayout;


        [BindView(Resource.Id.txtApplicationStatusSubTitle)]
        TextView txtApplicationStatusSubTitle;

        [BindView(Resource.Id.txtApplicationStatusTitle)]
        TextView txtApplicationStatusTitle;

        [BindView(Resource.Id.btnApplicationStatusViewBill)]
        Button btnApplicationStatusViewBill;

        [BindView(Resource.Id.txtApplicationStatusBottomPayableTitle)]
        TextView txtApplicationStatusBottomPayableTitle;

        [BindView(Resource.Id.txtApplicationStatusBottomPayable)]
        TextView txtApplicationStatusBottomPayable;

        [BindView(Resource.Id.txtApplicationStatusUpdated)]
        TextView txtApplicationStatusUpdated;

        [BindView(Resource.Id.applicationStatusStatusListRecyclerView)]
        RecyclerView applicationStatusStatusListRecyclerView;

        [BindView(Resource.Id.applicationStatusAdditionalListRecyclerView)]
        RecyclerView applicationStatusAdditionalListRecyclerView;

        [BindView(Resource.Id.applicationStatusBotomPayableLayout)]
        LinearLayout applicationStatusBotomPayableLayout;

        [BindView(Resource.Id.applicationStatusDetailDoubleButtonLayout)]
        LinearLayout applicationStatusDetailDoubleButtonLayout;

        [BindView(Resource.Id.applicationStatusDetailSingleButtonLayout)]
        LinearLayout applicationStatusDetailSingleButtonLayout;

        [BindView(Resource.Id.ctaSelection)]
        LinearLayout ctaSelection;

        [BindView(Resource.Id.btnViewActivityLogLayout)]
        LinearLayout btnViewActivityLogLayout;

        [BindView(Resource.Id.applicationStatusLine)]
        View applicationStatusLine;

        [BindView(Resource.Id.txtApplicationStatusDetail)]
        TextView txtApplicationStatusDetail;

        [BindView(Resource.Id.txtApplicationStatusDetailNote)]
        TextView txtApplicationStatusDetailNote;

        [BindView(Resource.Id.btnPrimaryCTA)]
        Button btnPrimaryCTA;

        [BindView(Resource.Id.btnViewActivityLog)]
        Button btnViewActivityLog;

        [BindView(Resource.Id.howDoISeeApplicaton)]
        TextView howDoISeeApplicaton;

        [BindView(Resource.Id.btnApplicationStatusPay)]
        Button btnApplicationStatusPay;

        [BindView(Resource.Id.txtApplicationStatusBottomPayableCurrency)]
        TextView txtApplicationStatusBottomPayableCurrency;

        [BindView(Resource.Id.linkedWithLayout)]
        LinearLayout linkedWithLayout;

        [BindView(Resource.Id.txtLinkedWithHeader)]
        TextView txtLinkedWithHeader;

        [BindView(Resource.Id.txtLinkedWithReferencNo)]
        TextView txtLinkedWithReferencNo;

        [BindView(Resource.Id.txtLinkedWithView)]
        TextView txtLinkedWithView;

        [BindView(Resource.Id.paymentFirstReceiptLayout)]
        LinearLayout paymentFirstReceiptLayout;

        [BindView(Resource.Id.paymentSecondReceiptLayout)]
        LinearLayout paymentSecondReceiptLayout;

        [BindView(Resource.Id.paymentTaxInvoiceLayout)]
        LinearLayout paymentTaxInvoiceLayout;

        [BindView(Resource.Id.txtpaymentFirstReceiptDate)]
        TextView txtpaymentFirstReceiptDate;

        [BindView(Resource.Id.txtpaymentFirstReceiptAmount)]
        TextView txtpaymentFirstReceiptAmount;

        [BindView(Resource.Id.txtpaymentFirstReceiptView)]
        TextView txtpaymentFirstReceiptView;

        [BindView(Resource.Id.txtpaymentSecondReceiptDate)]
        TextView txtpaymentSecondReceiptDate;

        [BindView(Resource.Id.txtpaymentSecondReceiptAmount)]
        TextView txtpaymentSecondReceiptAmount;

        [BindView(Resource.Id.txtpaymentSecondReceiptView)]
        TextView txtpaymentSecondReceiptView;

        [BindView(Resource.Id.txtTaxInvoiceTitle)]
        TextView txtTaxInvoiceTitle;

        [BindView(Resource.Id.txtTaxInvoiceAmount)]
        TextView txtTaxInvoiceAmount;

        [BindView(Resource.Id.txtTaxInvoiceView)]
        TextView txtTaxInvoiceView;

        [BindView(Resource.Id.ctaParentLayout)]
        LinearLayout ctaParentLayout;

        [BindView(Resource.Id.bcrmDownContainer)]
        LinearLayout bcrmDownContainer;

        [BindView(Resource.Id.txtBCRMDownMessage)]
        TextView txtBCRMDownMessage;

        [BindView(Resource.Id.txtApplicationRateStar)]
        TextView txtApplicationRateStar;

        [BindView(Resource.Id.imgStar)]
        ImageView imgStar;

        [BindView(Resource.Id.layoutstar)]
        LinearLayout layoutstar;

        private bool IsFromLinkedWith = false;
        private Snackbar mNoInternetSnackbar;

        [OnClick(Resource.Id.btnPrimaryCTA)]
        internal void OnPrimaryCTAClick(object sender, EventArgs e)
        {
            GetCustomerRatingAsync();
        }

        [OnClick(Resource.Id.btnViewActivityLog)]
        internal void OnViewActivityLog(object sender, EventArgs e)
        {
            ViewActivityLog();
        }

        [OnClick(Resource.Id.btnApplicationStatusViewBill)]
        internal void OnPaymentViewDetails(object sender, EventArgs e)
        {
            Intent applicationStatusDetailPaymentIntent = new Intent(this, typeof(ApplicationStatusDetailPaymentActivity));
            applicationStatusDetailPaymentIntent.PutExtra("applicationDetailDisplay", JsonConvert.SerializeObject(applicationDetailDisplay));

            StartActivity(applicationStatusDetailPaymentIntent);
        }

        [OnClick(Resource.Id.btnApplicationStatusPay)]
        internal void OnPay(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] OnPay");
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent payment_activity = new Intent(this, typeof(PaymentActivity));
                payment_activity.PutExtra("ISAPPLICATIONPAYMENT", true);
                payment_activity.PutExtra("APPLICATIONPAYMENTDETAIL", JsonConvert.SerializeObject(applicationDetailDisplay.applicationPaymentDetail));
                payment_activity.PutExtra("TOTAL", applicationDetailDisplay.PaymentDisplay.TotalPayableAmountDisplay);
                StartActivityForResult(payment_activity, PaymentActivity.SELECT_PAYMENT_ACTIVITY_CODE);

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

        [OnClick(Resource.Id.ctaSelection)]
        void OnApplicationDetailTooltipClick(object sender, EventArgs eventArgs)
        {
            try
            {
                string textTitle = string.Empty;
                string textMessage = string.Empty;
                if (applicationDetailDisplay != null && applicationDetailDisplay.IsFullApplicationTooltipDisplayed)
                {
                    if (applicationDetailDisplay.IsKedaiTenagaApplication)
                    {
                        textTitle = Utility.GetLocalizedLabel("ApplicationStatusDetails", "seeFullDetailsTitleKedai");
                        textMessage = Utility.GetLocalizedLabel("ApplicationStatusDetails", "seeFullDetailsMessageKedai");
                    }
                    else
                    {
                        textTitle = Utility.GetLocalizedLabel("ApplicationStatusDetails", "seeFullDetailsTitle");
                        textMessage = Utility.GetLocalizedLabel("ApplicationStatusDetails", "seeFullDetailsMessage");
                    }
                }

                string btnLabel = Utility.GetLocalizedCommonLabel("gotIt");

                if (textTitle != "" && textMessage != "" && btnLabel != "")
                {
                    MyTNBAppToolTipBuilder whatIsThisTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                        .SetTitle(textTitle)
                        .SetMessage(textMessage)
                        .SetCTALabel(btnLabel)
                        .Build();
                    whatIsThisTooltip.Show();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        public async void ShowApplicaitonPopupMessage(Android.App.Activity context, StatusDetail statusDetail)
        {
            MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(statusDetail.Title)
                .SetMessage(statusDetail.Message)
                .SetCTALabel(statusDetail.PrimaryCTATitle)
                .Build();
            whereisMyacc.Show();

        }
        public async void GetCustomerRatingAsync()
        {
            try
            {
                ShowProgressDialog();

                if (applicationDetailDisplay != null)
                {
                    if (applicationDetailDisplay.CTAType == DetailCTAType.Save)
                    {
                        SaveApplication();
                    }
                    else if (applicationDetailDisplay.CTAType == DetailCTAType.Rate)
                    {
                        customerRatingMasterResponse = await RatingManager.Instance.GetCustomerRatingMaster();
                        if (!customerRatingMasterResponse.StatusDetail.IsSuccess)
                        {
                            ShowApplicaitonPopupMessage(this, customerRatingMasterResponse.StatusDetail);
                        }
                        else
                        {
                            Intent rating_activity = new Intent(this, typeof(ApplicationRatingActivity));
                            rating_activity.PutExtra("applicationDetailDisplay", JsonConvert.SerializeObject(applicationDetailDisplay));
                            rating_activity.PutExtra("customerRatingMasterResponse", JsonConvert.SerializeObject(customerRatingMasterResponse));
                            StartActivity(rating_activity);
                        }
                    }
                }
                FirebaseAnalyticsUtils.LogClickEvent(this, "Rate Buttom Clicked");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
                HideProgressDialog();
            }
            HideProgressDialog();
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.DashboardToolbarMenu, menu);
            applicationFilterMenuItem = menu.FindItem(Resource.Id.action_notification);
            applicationFilterMenuItem.SetIcon(ContextCompat.GetDrawable(this, Resource.Drawable.delete_application));
            if (applicationDetailDisplay != null && applicationDetailDisplay.IsDeleteEnable)
            {
                applicationFilterMenuItem.SetVisible(true);
            }
            else
            {
                applicationFilterMenuItem.SetVisible(false);

            }
            return base.OnCreateOptionsMenu(menu);
        }

        private async void ViewActivityLog()
        {
            ShowProgressDialog();
            Intent applicationDetailActivityLogIntent = new Intent(this, typeof(ApplicationDetailActivityLogActivity));
            applicationDetailActivityLogIntent.PutExtra("applicationActivityLogDetail", JsonConvert.SerializeObject(applicationDetailDisplay.ApplicationActivityLogDetail));
            StartActivity(applicationDetailActivityLogIntent);
            HideProgressDialog();
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

        private async void SaveApplication()
        {
            try
            {
                UserEntity loggedUser = UserEntity.GetActive();
                if (loggedUser == null)
                {
                    MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(Utility.GetLocalizedLabel("ApplicationStatusDetails", "loginTitle"))
                        .SetMessage(Utility.GetLocalizedLabel("ApplicationStatusDetails", "loginMessage"))
                        .SetCTALabel(Utility.GetLocalizedLabel("ApplicationStatusDetails", "loginPrimaryCTA"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("ApplicationStatusDetails", "loginSecondaryCTA"))
                        .SetSecondaryCTAaction(() => ShowLogin())
                        .Build();
                    whereisMyacc.Show();
                }
                else
                {
                    if (ConnectionUtils.HasInternetConnection(this))
                    {
                        ShowProgressDialog();
                        PostSaveApplicationResponse postSaveApplicationResponse = await ApplicationStatusManager.Instance.SaveApplication(
                            applicationDetailDisplay.ApplicationDetail.ReferenceNo
                            , applicationDetailDisplay.ApplicationDetail.ApplicationModuleId
                            , applicationDetailDisplay.ApplicationTypeID
                            , applicationDetailDisplay.ApplicationDetail.BackendReferenceNo
                            , applicationDetailDisplay.ApplicationDetail.BackendApplicationType
                            , applicationDetailDisplay.ApplicationDetail.BackendModule
                            , applicationDetailDisplay.ApplicationDetail.StatusCode
                            , applicationDetailDisplay.ApplicationDetail.CreatedDate.Value);
                        HideProgressDialog();
                        if (postSaveApplicationResponse.StatusDetail.IsSuccess)
                        {
                            Toast.MakeText(this, postSaveApplicationResponse.StatusDetail.Message ?? string.Empty, ToastLength.Long).Show();
                            SetResult(Result.Ok, new Intent());
                            Finish();
                        }
                        else
                        {
                            MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                                .SetTitle(postSaveApplicationResponse.StatusDetail.Title)
                                .SetMessage(postSaveApplicationResponse.StatusDetail.Message)
                                .SetCTALabel(postSaveApplicationResponse.StatusDetail.PrimaryCTATitle)
                                .SetSecondaryCTALabel(postSaveApplicationResponse.StatusDetail.SecondaryCTATitle)
                                .SetSecondaryCTAaction(() => ShowStatusLanding())
                                .Build();
                            whereisMyacc.Show();
                        }
                    }
                    else
                    {
                        ShowNoInternetSnackbar();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowNoInternetSnackbar()
        {
            if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
            {
                mNoInternetSnackbar.Dismiss();
            }

            mNoInternetSnackbar = Snackbar.Make(rootview, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {
                mNoInternetSnackbar.Dismiss();
            }
            );
            View v = mNoInternetSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mNoInternetSnackbar.Show();
            this.SetIsClicked(false);
        }

        public void ShowLogin()
        {
            ApplicationStatusSearchDetailCache.Instance.SetData(applicationDetailDisplay);
            StartActivity(typeof(LoginActivity));
        }

        public void ShowStatusLanding()
        {
            ApplicationStatusSearchDetailCache.Instance.Clear();
            SetResult(Result.Ok, new Intent());
            Finish();
        }

        public override int ResourceId()
        {
            return Resource.Layout.ApplicationStatusDetailLayout;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //  TODO: ApplicationStatus Multilingual
            SetToolBarTitle(Utility.GetLocalizedLabel("ApplicationStatusDetails", "title"));
            presenter = new ApplicationStatusDetailPresenter(this);
            applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Gone;
            applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;
            btnPrimaryCTA.Visibility = ViewStates.Visible;
            linkedWithLayout.Visibility = ViewStates.Gone;
            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            applicationStatusStatusListRecyclerView.SetLayoutManager(layoutManager);
            applicationStatusStatusListRecyclerView.SetAdapter(adapter);
            TextViewUtils.SetMuseoSans500Typeface(btnViewActivityLog);

            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);

            applicationStatusAdditionalListRecyclerView.SetLayoutManager(layoutManager);
            applicationStatusAdditionalListRecyclerView.SetAdapter(subAdapter);

            TextViewUtils.SetMuseoSans300Typeface(txtApplicationStatusUpdated, txtApplicationStatusDetail, txtApplicationRateStar);

            txtApplicationStatusUpdated.SetTypeface(txtApplicationStatusUpdated.Typeface, Android.Graphics.TypefaceStyle.Italic);
            // Create your application here
            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey(Constants.APPLICATION_STATUS_DETAIL_TITLE_KEY))
                {
                    SetToolBarTitle(extras.GetString(Constants.APPLICATION_STATUS_DETAIL_TITLE_KEY));
                }
                if (extras.ContainsKey("IsFromLinkedWith"))
                {
                    IsFromLinkedWith = extras.GetBoolean("IsFromLinkedWith");
                }
                if (extras != null)
                {
                    if (extras.ContainsKey("applicationStatusResponse"))
                    {
                        ctaSelection.Visibility = ViewStates.Gone;
                        txtApplicationStatusUpdated.Visibility = ViewStates.Gone;
                        txtApplicationStatusDetail.Visibility = ViewStates.Gone;
                        applicationStatusLine.Visibility = ViewStates.Gone;
                        bcrmDownContainer.Visibility = ViewStates.Gone;
                        applicationStatusDetailSingleButtonLayout.Visibility = ViewStates.Gone;
                        btnViewActivityLogLayout.Visibility = ViewStates.Gone;
                        txtApplicationStatusDetailNote.Visibility = ViewStates.Gone;
                        applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;

                        applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Gone;
                        applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;

                        applicationDetailDisplay = new GetApplicationStatusDisplay();
                        applicationDetailDisplay = JsonConvert.DeserializeObject<GetApplicationStatusDisplay>(extras.GetString("applicationStatusResponse"));

                        if (applicationDetailDisplay != null)
                        {
                            if (applicationDetailDisplay.IsOffLine)
                            {
                                applicationStatusLine.Visibility = ViewStates.Visible;
                                bcrmDownContainer.Visibility = ViewStates.Visible;
                                txtBCRMDownMessage.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "bcrmDownMessage");
                            }
                            else if (applicationDetailDisplay.ApplicationStatusDetail != null
                                && applicationDetailDisplay.ApplicationStatusDetail.StatusTracker != null
                                && applicationDetailDisplay.ApplicationStatusDetail.StatusTracker.Count > 0)
                            {
                                applicationStatusLine.Visibility = ViewStates.Visible;
                                adapter = new ApplicationStatusDetailProgressAdapter(this
                                    , applicationDetailDisplay.ApplicationStatusDetail.StatusTracker
                                    , applicationDetailDisplay.ApplicationStatusDetail.IsPayment);
                                applicationStatusStatusListRecyclerView.SetAdapter(adapter);
                                adapter.NotifyDataSetChanged();
                            }
                            txtApplicationStatusTitle.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "status") + " ";
                            btnViewActivityLog.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "viewActivityLog");
                            howDoISeeApplicaton.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "seeFullDetails");

                            applicationStatusDetailSingleButtonLayout.Visibility = applicationDetailDisplay.IsSavedApplication
                                ? ViewStates.Visible
                                : ViewStates.Gone;

                            if (applicationDetailDisplay.IsLinkedWithDisplayed)
                            {
                                txtLinkedWithHeader.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "linkedWith");
                                txtLinkedWithView.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "linkedWithView");
                                txtLinkedWithReferencNo.Text = applicationDetailDisplay.LinkedWithDisplay.ReferenceNo;

                                TextViewUtils.SetMuseoSans300Typeface(txtLinkedWithHeader, txtLinkedWithReferencNo);
                                TextViewUtils.SetMuseoSans500Typeface(txtLinkedWithView);
                                linkedWithLayout.Visibility = ViewStates.Visible;
                                linkedWithLayout.Clickable = true;
                                linkedWithLayout.Click += OnLinkedWithClick;
                            }

                            EvaluateReceipts();

                            btnViewActivityLogLayout.Visibility = applicationDetailDisplay.IsActivityLogDisplayed
                                 ? ViewStates.Visible
                                 : ViewStates.Gone;

                            txtApplicationStatusMainTitle.Text = applicationDetailDisplay.ApplicationStatusDetail.StatusDescription;
                            txtApplicationStatusSubTitle.Text = applicationDetailDisplay.ApplicationType;
                            if (applicationDetailDisplay.ApplicationDetail != null
                                && applicationDetailDisplay.ApplicationDetail.IsDisplayLastUpdatedDate)
                            {
                                txtApplicationStatusUpdated.Visibility = ViewStates.Visible;
                                txtApplicationStatusUpdated.Text = applicationDetailDisplay.ApplicationDetail.LastUpdatedDateDisplay;
                            }

                            txtApplicationStatusDetail.Text = applicationDetailDisplay.PortalMessage;
                            txtApplicationStatusDetail.Visibility = applicationDetailDisplay.IsPortalMessageDisplayed
                                ? ViewStates.Visible
                                : ViewStates.Gone;


                            txtApplicationStatusDetailNote.Text = applicationDetailDisplay.SaveMessage;
                            txtApplicationStatusDetailNote.Visibility = applicationDetailDisplay.IsSaveMessageDisplayed
                                ? ViewStates.Visible
                                : ViewStates.Gone;

                            if (applicationDetailDisplay.StatusColor.Length > 2)
                            {
                                Android.Graphics.Color color = Android.Graphics.Color.Rgb(
                                    applicationDetailDisplay.StatusColor[0]
                                    , applicationDetailDisplay.StatusColor[1]
                                    , applicationDetailDisplay.StatusColor[2]);

                                txtApplicationStatusMainTitle.SetTextColor(color);
                            }

                            ctaSelection.Visibility = applicationDetailDisplay.IsFullApplicationTooltipDisplayed
                                 ? ViewStates.Visible
                                 : ViewStates.Gone;

                            if (applicationDetailDisplay.AdditionalInfoList != null
                                && applicationDetailDisplay.AdditionalInfoList.Count > 0)
                            {
                                subAdapter = new ApplicationStatusDetailSubDetailAdapter(this, applicationDetailDisplay.AdditionalInfoList);
                                applicationStatusAdditionalListRecyclerView.SetAdapter(subAdapter);
                                subAdapter.NotifyDataSetChanged();
                            }

                            if (applicationDetailDisplay.PaymentDisplay != null
                                && applicationDetailDisplay.PaymentDisplay.TotalPayableAmountDisplay != string.Empty)
                            {
                                txtApplicationStatusBottomPayableCurrency.Text = "RM";
                                txtApplicationStatusBottomPayable.Text = applicationDetailDisplay.PaymentDisplay.TotalPayableAmountDisplay;
                                applicationStatusBotomPayableLayout.Visibility = ViewStates.Visible;
                            }

                            ctaParentLayout.Visibility = ViewStates.Visible;
                            btnApplicationStatusViewBill.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "viewDetails");
                            btnApplicationStatusPay.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "payNow");

                            if (applicationDetailDisplay.CTAType == DetailCTAType.PayOffline)
                            {
                                txtApplicationStatusBottomPayable.Text = "--";
                                txtApplicationStatusBottomPayableTitle.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "needToPay");
                                txtApplicationStatusBottomPayableCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.charcoalGrey)));
                                txtApplicationStatusBottomPayable.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.charcoalGrey)));
                                applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Visible;
                                applicationStatusBotomPayableLayout.Visibility = ViewStates.Visible;
                                btnApplicationStatusPay.Visibility = ViewStates.Visible;
                                btnApplicationStatusPay.Enabled = false;
                                btnApplicationStatusPay.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.white));
                                btnApplicationStatusPay.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
                                btnApplicationStatusViewBill.Enabled = false;
                                btnApplicationStatusViewBill.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.silverChalice));
                                btnApplicationStatusViewBill.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_outline);
                            }
                            else if (applicationDetailDisplay.CTAType == DetailCTAType.Pay)
                            {
                                txtApplicationStatusBottomPayableTitle.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "needToPay");
                                txtApplicationStatusBottomPayableCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.charcoalGrey)));
                                txtApplicationStatusBottomPayable.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.charcoalGrey)));
                                applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Visible;
                                applicationStatusBotomPayableLayout.Visibility = ViewStates.Visible;
                                btnApplicationStatusPay.Visibility = ViewStates.Visible;
                            }
                            else if (applicationDetailDisplay.CTAType == DetailCTAType.PayInProgress)
                            {
                                txtApplicationStatusBottomPayableTitle.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "paymentInProgress");
                                txtApplicationStatusBottomPayableCurrency.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.lightOrange)));
                                txtApplicationStatusBottomPayable.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.lightOrange)));
                                applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Visible;
                                btnApplicationStatusPay.Visibility = ViewStates.Gone;
                            }
                            else if (applicationDetailDisplay.CTAType == DetailCTAType.Save)
                            {
                                btnPrimaryCTA.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "addToList");
                                applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Gone;
                                applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;
                                applicationStatusDetailSingleButtonLayout.Visibility = ViewStates.Visible;
                            }
                            else if (applicationDetailDisplay.CTAType == DetailCTAType.Rate)
                            {
                                btnPrimaryCTA.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "rateCTA");
                                applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Gone;
                                applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;
                                applicationStatusDetailSingleButtonLayout.Visibility = ViewStates.Visible;
                            }
                            else if (applicationDetailDisplay.CTAType == DetailCTAType.None)
                            {
                                applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Gone;
                                applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;
                                applicationStatusDetailSingleButtonLayout.Visibility = ViewStates.Gone;
                                ctaParentLayout.Visibility = ViewStates.Gone;
                            }

                            TextViewUtils.SetMuseoSans500Typeface(txtApplicationStatusMainTitle, txtApplicationStatusTitle, txtApplicationStatusBottomPayableTitle);
                            TextViewUtils.SetMuseoSans300Typeface(txtApplicationStatusSubTitle, txtApplicationStatusDetailNote, txtBCRMDownMessage);

                            if (applicationDetailDisplay.RatingDisplay != null && applicationDetailDisplay.RatingDisplay != string.Empty)
                            {
                                txtApplicationRateStar.Visibility = ViewStates.Visible;
                                imgStar.Visibility = ViewStates.Visible;
                                layoutstar.Visibility = ViewStates.Visible;
                                txtApplicationRateStar.Text = applicationDetailDisplay.RatingDisplay + " ";
                            }
                            else
                            {
                                txtApplicationRateStar.Visibility = ViewStates.Gone;
                                imgStar.Visibility = ViewStates.Gone;
                                layoutstar.Visibility = ViewStates.Gone;
                            }
                        }
                    }
                    if (extras.ContainsKey("submitRatingResponseStatus"))
                    {
                        StatusDetail statusDetails = new StatusDetail();
                        statusDetails = DeSerialze<StatusDetail>(extras.GetString("submitRatingResponseStatus"));
                        if (statusDetails != null)
                        {
                            Snackbar mSaveSnackbar = Snackbar.Make(rootview,
                            statusDetails.Message,
                            Snackbar.LengthLong);
                            View v = mSaveSnackbar.View;
                            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                            tv.SetMaxLines(5);
                            mSaveSnackbar.Show();
                        }
                    }
                    if (extras.ContainsKey("applicationRated") && extras.GetString("applicationRated") != null && extras.GetString("applicationRated") != "0")
                    {
                        txtApplicationRateStar.Visibility = ViewStates.Visible;
                        imgStar.Visibility = ViewStates.Visible;
                        layoutstar.Visibility = ViewStates.Visible;
                        txtApplicationRateStar.Text = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "youRated") + extras.GetString("applicationRated") + " ";
                    }
                }
            }
        }

        private void EvaluateReceipts()
        {
            paymentFirstReceiptLayout.Visibility = ViewStates.Gone;
            paymentSecondReceiptLayout.Visibility = ViewStates.Gone;
            paymentTaxInvoiceLayout.Visibility = ViewStates.Gone;
            try
            {
                if (applicationDetailDisplay.IsReceiptDisplayed
                    && applicationDetailDisplay.ReceiptDisplay != null
                    && applicationDetailDisplay.ReceiptDisplay.Count > 0)
                {
                    TextViewUtils.SetMuseoSans300Typeface(txtpaymentFirstReceiptDate, txtpaymentSecondReceiptDate);
                    TextViewUtils.SetMuseoSans500Typeface(txtpaymentFirstReceiptView
                        , txtpaymentSecondReceiptView, txtpaymentFirstReceiptAmount
                        , txtpaymentSecondReceiptAmount);

                    txtpaymentFirstReceiptView.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "viewReceipt");
                    txtpaymentSecondReceiptView.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "viewReceipt");

                    txtpaymentFirstReceiptDate.Text = applicationDetailDisplay.ReceiptDisplay[0].PaymentDateDisplay;
                    txtpaymentFirstReceiptAmount.Text = applicationDetailDisplay.ReceiptDisplay[0].AmountDisplay;
                    paymentFirstReceiptLayout.Clickable = true;
                    paymentFirstReceiptLayout.Click += (sender, args) =>
                    {
                        OnReceiptClick(applicationDetailDisplay.ReceiptDisplay[0].AccNumber
                            , applicationDetailDisplay.ReceiptDisplay[0].MerchantTransID);
                    };
                    paymentFirstReceiptLayout.Visibility = ViewStates.Visible;

                    if (applicationDetailDisplay.ReceiptDisplay.Count > 1)
                    {
                        txtpaymentSecondReceiptDate.Text = applicationDetailDisplay.ReceiptDisplay[1].PaymentDateDisplay;
                        txtpaymentSecondReceiptAmount.Text = applicationDetailDisplay.ReceiptDisplay[1].AmountDisplay;
                        paymentSecondReceiptLayout.Clickable = true;
                        paymentSecondReceiptLayout.Click += (sender, args) =>
                        {
                            OnReceiptClick(applicationDetailDisplay.ReceiptDisplay[1].AccNumber
                                , applicationDetailDisplay.ReceiptDisplay[1].MerchantTransID);
                        };
                        paymentSecondReceiptLayout.Visibility = ViewStates.Visible;
                    }
                }

                if (applicationDetailDisplay.IsTaxInvoiceDisplayed
                    && applicationDetailDisplay.TaxInvoiceDisplay != null)
                {
                    TextViewUtils.SetMuseoSans300Typeface(txtTaxInvoiceTitle);
                    TextViewUtils.SetMuseoSans500Typeface(txtTaxInvoiceView, txtTaxInvoiceAmount);
                    txtTaxInvoiceTitle.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "taxInvoice");
                    txtTaxInvoiceView.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "taxInvoiceView");
                    txtTaxInvoiceAmount.Text = applicationDetailDisplay.TaxInvoiceDisplay.AmountDisplay;
                    paymentTaxInvoiceLayout.Clickable = true;
                    paymentTaxInvoiceLayout.Click += (sender, args) =>
                    {
                        OnTaxInvoiceClick(applicationDetailDisplay.TaxInvoiceDisplay.SRNumber);
                    };
                    paymentTaxInvoiceLayout.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Application Status Detail");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            if (applicationDetailDisplay != null && applicationDetailDisplay.TutorialType != null)
            {
                if (!UserSessions.HasApplicationDetailShown(PreferenceManager.GetDefaultSharedPreferences(this)))
                {
                    Handler h = new Handler();
                    Action myAction = () =>
                    {

                        if (applicationDetailDisplay.TutorialType == DetailTutorialType.NoAction)
                        {
                            NewAppTutorialUtils.OnShowNewAppTutorial(this, null, PreferenceManager.GetDefaultSharedPreferences(this)
                                , this.presenter.OnGeneraNewAppTutorialNoActionList(), true);
                        }
                        else if (applicationDetailDisplay.TutorialType == DetailTutorialType.Action)
                        {
                            NewAppTutorialUtils.OnShowNewAppTutorial(this, null, PreferenceManager.GetDefaultSharedPreferences(this)
                                , this.presenter.OnGeneraNewAppTutorialActionList(), true);
                        }
                        else if (applicationDetailDisplay.TutorialType == DetailTutorialType.InProgress)
                        {
                            NewAppTutorialUtils.OnShowNewAppTutorial(this, null, PreferenceManager.GetDefaultSharedPreferences(this)
                               , this.presenter.OnGeneraNewAppTutorialInProgressList(), true);
                        }
                    };
                    h.PostDelayed(myAction, 100);
                }
            }
        }
        public int GetTopHeight()
        {
            int i = 0;

            try
            {
                int[] location1 = new int[2];
                applicationStatusMainStatusLayout.GetLocationOnScreen(location1);
                i = location1[1];
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }
        public int GetHighlightedHeight()
        {
            int i = 0;
            try
            {
                int aheight;
                aheight = applicationStatusMainStatusLayout.Height;
                i = aheight;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }


        public int GetCtaButtonHeight()
        {
            int i = 0;

            try
            {

                i = ctaParentLayout.Height;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }
        public int GetTopCtaHeight()
        {
            int i = 0;
            try
            {
                Rect offsetViewBounds = new Rect();
                //returns the visible bounds
                ctaParentLayout.GetDrawingRect(offsetViewBounds);
                // calculates the relative coordinates to the parent
                rootview.OffsetDescendantRectToMyCoords(ctaParentLayout, offsetViewBounds);
                i = offsetViewBounds.Top + (int)DPUtils.ConvertDPToPx(14f);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return i;
        }
        public void OnShowApplicationDetailTutorial(DetailTutorialType tutorialType)
        {
            if (UserSessions.HasApplicationDetailShown(PreferenceManager.GetDefaultSharedPreferences(this)))
            {
                Handler h = new Handler();
                Action myAction = () =>
                {
                    if (tutorialType == DetailTutorialType.NoAction)
                    {
                        NewAppTutorialUtils.OnShowNewAppTutorial(this, null, PreferenceManager.GetDefaultSharedPreferences(this)
                            , this.presenter.OnGeneraNewAppTutorialNoActionList(), true);
                    }
                    else if (tutorialType == DetailTutorialType.Action)
                    {
                        NewAppTutorialUtils.OnShowNewAppTutorial(this, null, PreferenceManager.GetDefaultSharedPreferences(this)
                            , this.presenter.OnGeneraNewAppTutorialActionList(), true);
                    }
                    else if (tutorialType == DetailTutorialType.InProgress)
                    {
                        NewAppTutorialUtils.OnShowNewAppTutorial(this, null, PreferenceManager.GetDefaultSharedPreferences(this)
                           , this.presenter.OnGeneraNewAppTutorialInProgressList(), true);
                    }
                };
                h.PostDelayed(myAction, 100);
            }
        }
        private void OnTaxInvoiceClick(string srNumber)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] OnTaxInvoiceClick Start");
            try
            {
                Intent viewBill = new Intent(this, typeof(ViewBillActivity));
                viewBill.PutExtra("IsTaxInvoice", true);
                viewBill.PutExtra("SRNumber", srNumber);
                StartActivity(viewBill);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void OnReceiptClick(string accountNumber, string documentNumber)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] OnReceiptClick Start");
            try
            {
                Intent viewReceipt = new Intent(this, typeof(ViewReceiptMultiAccountNewDesignActivty));
                viewReceipt.PutExtra("SELECTED_ACCOUNT_NUMBER", accountNumber);
                viewReceipt.PutExtra("DETAILED_INFO_NUMBER", documentNumber);
                viewReceipt.PutExtra("IsApplicationReceipt", true);
                viewReceipt.PutExtra("IS_OWNED_ACCOUNT", true);
                viewReceipt.PutExtra("IS_SHOW_ALL_RECEIPT", false);
                StartActivity(viewReceipt);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async void OnLinkedWithClick(object sender, EventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] OnLinkedWithClick Start");
            try
            {
                if (IsFromLinkedWith)
                {
                    Finish();
                    return;
                }
                if (ConnectionUtils.HasInternetConnection(this))
                {
                    ShowProgressDialog();
                    ApplicationDetailDisplay response = await ApplicationStatusManager.Instance.GetApplicationDetail(string.Empty
                        , applicationDetailDisplay.LinkedWithDisplay.ID
                        , applicationDetailDisplay.LinkedWithDisplay.Type
                        , applicationDetailDisplay.LinkedWithDisplay.System);
                    HideProgressDialog();
                    if (response.StatusDetail.IsSuccess)
                    {
                        Intent intent = new Intent(this, typeof(ApplicationStatusDetailActivity));
                        intent.PutExtra("applicationStatusResponse", JsonConvert.SerializeObject(response.Content));
                        intent.PutExtra("IsFromLinkedWith", true);
                        StartActivity(intent);
                    }
                    else
                    {
                        MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                            .SetTitle(response.StatusDetail.Title)
                            .SetMessage(response.StatusDetail.Message)
                            .SetCTALabel(response.StatusDetail.PrimaryCTATitle)
                            .Build();
                        errorPopup.Show();
                    }
                }
                else
                {
                    ShowNoInternetSnackbar();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                switch (item.ItemId)
                {
                    case Resource.Id.action_notification:
                        {
                            MyTNBAppToolTipBuilder removeApp = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                                .SetTitle(Utility.GetLocalizedLabel("ApplicationStatusDetails", "removeTitle"))
                                .SetMessage(Utility.GetLocalizedLabel("ApplicationStatusDetails", "removeMessage"))
                                .SetCTALabel(Utility.GetLocalizedLabel("Common", "cancel"))
                                .SetSecondaryCTALabel(Utility.GetLocalizedLabel("ApplicationStatusDetails", "confirm"))
                                .SetSecondaryCTAaction(() => OnRemoveApplication())
                                .Build();
                            removeApp.Show();
                            return true;
                        }
                }

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return base.OnOptionsItemSelected(item);
        }

        private async void OnRemoveApplication()
        {
            try
            {
                if (ConnectionUtils.HasInternetConnection(this))
                {
                    ShowProgressDialog();
                    PostRemoveApplicationResponse response = await ApplicationStatusManager.Instance.RemoveApplication(applicationDetailDisplay.ApplicationTypeCode
                        , applicationDetailDisplay.SavedApplicationID
                        , applicationDetailDisplay.System);

                    if (response.StatusDetail.IsSuccess)
                    {
                        if (response.StatusDetail.IsSuccess && UserEntity.GetActive() != null)
                        {
                            Toast.MakeText(this, response.StatusDetail.Message ?? string.Empty, ToastLength.Long).Show();
                            SetResult(Result.Ok, new Intent());
                            Finish();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, response.StatusDetail.Message ?? string.Empty, ToastLength.Long).Show();

                    }
                    HideProgressDialog();
                }
                else
                {
                    ShowNoInternetSnackbar();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateUI()
        {
        }
    }
}