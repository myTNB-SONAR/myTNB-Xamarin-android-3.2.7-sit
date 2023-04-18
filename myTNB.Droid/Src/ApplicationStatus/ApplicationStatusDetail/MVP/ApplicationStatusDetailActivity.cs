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
using Android.Runtime;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB.Mobile.API.DisplayModel.Scheduler;
using myTNB.Mobile.API.Managers.Scheduler;
using myTNB_Android.Src.AppointmentScheduler.AppointmentSelect.MVP;
using Android.Text;
using Android.Content.PM;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.NotificationDetails.Models;

using MyHomeModel = myTNB_Android.Src.MyHome.Model.MyHomeModel;
using myTNB_Android.Src.MyHome;
using myTNB_Android.Src.MyHome.Activity;
using static Android.Graphics.ColorSpace;
using myTNB_Android.Src.DeviceCache;
using System.Threading.Tasks;
using System.Reflection;
using myTNB.Mobile.Constants;
using Xamarin.Facebook;
using static myTNB_Android.Src.myTNBMenu.Models.SMUsageHistoryData;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP
{
    [Activity(Label = "Application Details", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.AppointmentScheduler")]
    public class ApplicationStatusDetailActivity : BaseActivityCustom, ApplicationStatusDetailContract.IView
    {
        [BindView(Resource.Id.rootview)]
        LinearLayout rootview;

        const string PAGE_ID = "ApplicationStatusDetails";
        public ApplicationStatusDetailProgressAdapter adapter;
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

        [BindView(Resource.Id.applicationStatusDetailNonLoginLayout)]
        LinearLayout applicationStatusDetailNonLoginLayout;

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

        [BindView(Resource.Id.txtAppointmentSet)]
        TextView txtAppointmentSet;

        [BindView(Resource.Id.cannotReschedulTooltip)]
        LinearLayout cannotReschedulTooltip;

        [BindView(Resource.Id.txtCannotRescheduleTooltip)]
        TextView txtCannotRescheduleTooltip;

        private bool IsFromLinkedWith = false;
        private Snackbar mNoInternetSnackbar;
        private Snackbar mErrorSnackbar;
        private bool IsPush = false;

        [OnClick(Resource.Id.btnPrimaryCTA)]
        internal void OnPrimaryCTAClick(object sender, EventArgs args)
        {
            try
            {
                if (applicationDetailDisplay != null)
                {
                    if (applicationDetailDisplay.CTAType == DetailCTAType.NewAppointment)
                    {
                        OnSetAppointment("NewAppointment");
                        FirebaseAnalyticsUtils.LogClickEvent(this, "Set Appointment Button Clicked");
                    }
                    else if (applicationDetailDisplay.CTAType == DetailCTAType.Reschedule)
                    {
                        OnSetAppointment("Reschedule");
                        FirebaseAnalyticsUtils.LogClickEvent(this, "Set Reschedule Appointment Button Clicked");
                    }
                    else if (applicationDetailDisplay.CTAType == DetailCTAType.Save)
                    {
                        SaveApplication();
                        FirebaseAnalyticsUtils.LogClickEvent(this, "Save Button Clicked");
                    }
                    else if (applicationDetailDisplay.CTAType == DetailCTAType.CustomerRating)
                    {
                        OnCustomerRating();
                        FirebaseAnalyticsUtils.LogClickEvent(this, "Customer Rating Button Clicked");
                        DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.CTAs.Details.NC_Customer_Rating);
                    }
                    else if (applicationDetailDisplay.CTAType == DetailCTAType.SubmitApplicationRating)
                    {
                        DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.CTAs.Details.NC_Rate_Our_Service);

                        ShowProgressDialog();
                        Task.Run(() =>
                        {
                            _ = GetAccessToken(Constants.APPLICATION_STATUS_SUBMIT_APPLICATION_RATING_REQUEST_CODE, string.Empty);
                        });
                    }
                    else if (applicationDetailDisplay.CTAType == DetailCTAType.ContractorRating)
                    {
                        DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.CTAs.Details.NC_Contractor_Rating);

                        Intent webIntent = new Intent(this, typeof(BaseWebviewActivity));
                        webIntent.PutExtra(Constants.IN_APP_LINK, applicationDetailDisplay.ContractorRatingURL);
                        webIntent.PutExtra(Constants.IN_APP_TITLE, Utility.GetLocalizedLabel("ApplicationStatusDetails", "rateContractor"));
                        webIntent.PutExtra("action", "contractorRating");
                        StartActivityForResult(webIntent, Constants.APPLICATION_STATUS_RATING_REQUEST_CODE);
                        FirebaseAnalyticsUtils.LogClickEvent(this, "Contractor Rating Button Clicked");
                    }
                    else if (applicationDetailDisplay.CTAType == DetailCTAType.StartApplication
                        || applicationDetailDisplay.CTAType == DetailCTAType.ReapplyNow
                        || applicationDetailDisplay.CTAType == DetailCTAType.ReuploadDocument)
                    {
                        if (applicationDetailDisplay != null && applicationDetailDisplay.MyHomeDetails != null)
                        {
                            string dynatraceCTA = string.Empty;
                            switch(applicationDetailDisplay.CTAType)
                            {
                                case DetailCTAType.StartApplication:
                                    dynatraceCTA = DynatraceConstants.ApplicationStatus.CTAs.Details.NC_Start_Application;
                                    break;
                                case DetailCTAType.ReapplyNow:
                                    dynatraceCTA = DynatraceConstants.ApplicationStatus.CTAs.Details.NC_Reappoint_Contractor_Reapply_Now;
                                    break;
                                case DetailCTAType.ReuploadDocument:
                                    //TODO: Add dynatraceCTA for Reupload Document
                                    break;
                                default:
                                    break;
                            }

                            DynatraceHelper.OnTrack(dynatraceCTA);

                            ShowProgressDialog();
                            Task.Run(() =>
                            {
                                _ = GetAccessToken(Constants.APPLICATION_STATUS_START_RESUME_REQUEST_CODE, string.Empty);
                            });
                        }
                        else
                        {
                            ShowGenericErrorPopUp();
                        }
                    }
                    else if (applicationDetailDisplay.CTAType == DetailCTAType.DeleteApplication)
                    {
                        this.SetIsClicked(true);
                        DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.Screens.Details.NC_Delete_Draft_PopUp);

                        MyTNBAppToolTipBuilder deleteDraftPopUp = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_IMAGE_BUTTON)
                            .SetSecondaryHeaderImage(Resource.Drawable.ic_display_validation_success)
                            .SetTitle(Utility.GetLocalizedLabel(LanguageConstants.APPLICATION_STATUS_DETAILS, ApplicationStatusDetails.PopUps.I18N_DeleteNCDraftTitle))
                            .SetMessage(Utility.GetLocalizedLabel(LanguageConstants.APPLICATION_STATUS_DETAILS, ApplicationStatusDetails.PopUps.I18N_DeleteNCDraftMessage))
                            .SetPrimaryButtonDrawable(Resource.Drawable.blue_button_solid_background)
                            .SetSecondaryButtonDrawable(Resource.Drawable.blue_outline_round_button_background)
                            .SetCTALabel(Utility.GetLocalizedLabel(LanguageConstants.APPLICATION_STATUS_DETAILS, ApplicationStatusDetails.PopUps.I18N_DeleteNCDraftSure))
                            .SetCTAaction(() => OnDeleteDraft())
                            .SetSecondaryCTALabel(Utility.GetLocalizedLabel(LanguageConstants.APPLICATION_STATUS_DETAILS, ApplicationStatusDetails.PopUps.I18N_DeleteNCDraftCancel))
                            .SetSecondaryCTAaction(() =>
                            {
                                DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.CTAs.Details.NC_Delete_Draft_Cancel);
                                this.SetIsClicked(false);
                            })
                            .Build();
                        deleteDraftPopUp.Show();
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private async Task GetAccessToken(int resultCode, string cancelUrl)
        {
            UserEntity user = UserEntity.GetActive();
            string accessToken = await AccessTokenManager.Instance.GetUserServiceAccessToken(user.UserID);
            AccessTokenCache.Instance.SaveUserServiceAccessToken(this, accessToken);
            if (accessToken.IsValid())
            {
                this.RunOnUiThread(() =>
                {
                    HideProgressDialog();

                    if (applicationDetailDisplay.MyHomeDetails != null)
                    {
                        MyHomeModel myHomeModel = new MyHomeModel()
                        {
                            SSODomain = applicationDetailDisplay.MyHomeDetails.SSODomain,
                            OriginURL = applicationDetailDisplay.MyHomeDetails.OriginURL,
                            RedirectURL = applicationDetailDisplay.MyHomeDetails.RedirectURL,
                            CancelURL = cancelUrl
                        };

                        Intent micrositeActivity = new Intent(this, typeof(MyHomeMicrositeActivity));
                        micrositeActivity.PutExtra(MyHomeConstants.ACCESS_TOKEN, accessToken);
                        micrositeActivity.PutExtra(MyHomeConstants.MYHOME_MODEL, JsonConvert.SerializeObject(myHomeModel));
                        StartActivityForResult(micrositeActivity, resultCode);
                    }
                    else
                    {
                        ShowGenericErrorPopUp();
                    }
                });
            }
            else
            {
                this.RunOnUiThread(() =>
                {
                    HideProgressDialog();
                    ShowGenericErrorPopUp();
                });
            }
        }

        private void OnDeleteDraft()
        {
            this.SetIsClicked(false);
            
            DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.CTAs.Details.NC_Delete_Draft_Im_Sure);

            ShowProgressDialog();
            Task.Run(() =>
            {
                _ = PostDeleteDraft();
            });
        }

        private async Task PostDeleteDraft()
        {
            if (applicationDetailDisplay.ApplicationDetail != null &&
                applicationDetailDisplay.ApplicationDetail.ReferenceNo.IsValid())
            {
                string refNo = applicationDetailDisplay.ApplicationDetail.ReferenceNo;
                UserEntity user = UserEntity.GetActive();
                var response = await myTNB.Mobile.AWS.ApplicationStatusManager.Instance.PostDeleteNCDraft(refNo, user.UserID, AccessTokenCache.Instance.GetUserServiceAccessToken(this));
                if (response != null &&
                    response.StatusDetail != null &&
                    response.StatusDetail.IsSuccess)
                {
                    AccessTokenCache.Instance.SaveUserServiceAccessToken(this, response.StatusDetail.AccessToken);
                    RunOnUiThread(() =>
                    {
                        HideProgressDialog();
                        Intent intent = new Intent();
                        intent.PutExtra(Constants.DELETE_DRAFT_MESSAGE, response.StatusDetail.Message);
                        SetResult(Result.Ok, intent);
                        Finish();
                    });
                }
                else
                {
                    RunOnUiThread(() =>
                    {
                        HideProgressDialog();
                        ShowErrorSnackbar(response.StatusDetail.Message);
                    });
                }
            }
        }

        [OnClick(Resource.Id.btnViewActivityLog)]
        internal void OnViewActivityLog(object sender, EventArgs e)
        {
            DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.CTAs.Details.Activity_Log);
            ViewActivityLog();
        }

        [OnClick(Resource.Id.btnApplicationStatusViewBill)]
        internal void OnPaymentViewDetails(object sender, EventArgs e)
        {
            if (applicationDetailDisplay != null)
            {
                if (applicationDetailDisplay.CTAType == DetailCTAType.ResumeApplication)
                {
                    this.SetIsClicked(true);
                    DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.CTAs.Details.NC_Delete);
                    DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.Screens.Details.NC_Delete_Draft_PopUp);
                    
                    MyTNBAppToolTipBuilder deleteDraftPopUp = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_IMAGE_BUTTON)
                            .SetSecondaryHeaderImage(Resource.Drawable.ic_display_validation_success)
                            .SetTitle(Utility.GetLocalizedLabel(LanguageConstants.APPLICATION_STATUS_DETAILS, ApplicationStatusDetails.PopUps.I18N_DeleteNCDraftTitle))
                            .SetMessage(Utility.GetLocalizedLabel(LanguageConstants.APPLICATION_STATUS_DETAILS, ApplicationStatusDetails.PopUps.I18N_DeleteNCDraftMessage))
                            .SetPrimaryButtonDrawable(Resource.Drawable.blue_button_solid_background)
                            .SetSecondaryButtonDrawable(Resource.Drawable.blue_outline_round_button_background)
                            .SetCTALabel(Utility.GetLocalizedLabel(LanguageConstants.APPLICATION_STATUS_DETAILS, ApplicationStatusDetails.PopUps.I18N_DeleteNCDraftSure))
                            .SetCTAaction(() => OnDeleteDraft())
                            .SetSecondaryCTALabel(Utility.GetLocalizedLabel(LanguageConstants.APPLICATION_STATUS_DETAILS, ApplicationStatusDetails.PopUps.I18N_DeleteNCDraftCancel))
                            .SetSecondaryCTAaction(() =>
                            {
                                DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.CTAs.Details.NC_Delete_Draft_Cancel);
                                this.SetIsClicked(false);
                            })
                            .Build();
                    deleteDraftPopUp.Show();
                    return;
                }
            }

            Intent applicationStatusDetailPaymentIntent = new Intent(this, typeof(ApplicationStatusDetailPaymentActivity));
            applicationStatusDetailPaymentIntent.PutExtra("applicationDetailDisplay", JsonConvert.SerializeObject(applicationDetailDisplay));

            StartActivity(applicationStatusDetailPaymentIntent);
        }

        [OnClick(Resource.Id.btnApplicationStatusPay)]
        internal void OnPay(object sender, EventArgs e)
        {
            if (applicationDetailDisplay != null)
            {
                if (applicationDetailDisplay.CTAType == DetailCTAType.ResumeApplication)
                {
                    if (applicationDetailDisplay.MyHomeDetails != null)
                    {
                        DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.CTAs.Details.NC_Resume);
                        ShowProgressDialog();
                        Task.Run(() =>
                        {
                            _ = GetAccessToken(Constants.APPLICATION_STATUS_START_RESUME_REQUEST_CODE, AWSConstants.ApplicationStatusLandingCancelURL);
                        });
                    }
                    return;
                }
            }

            System.Diagnostics.Debug.WriteLine("[DEBUG] OnPay");
            if (!this.GetIsClicked())
            {
                DownTimeEntity pgXEntity = DownTimeEntity.GetByCode(Constants.PG_SYSTEM);
                if (!Utility.IsEnablePayment())
                {
                    if (pgXEntity != null)
                    {
                        Utility.ShowBCRMDOWNTooltip(this, pgXEntity, () =>
                        {
                            this.SetIsClicked(false);
                        });
                    }
                }
                else
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
                    StartActivityForResult(intent, PaymentActivity.SELECT_PAYMENT_ACTIVITY_CODE);
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

        [OnClick(Resource.Id.cannotReschedulTooltip)]
        void OnCannotRescheduleTooltipClick(object sender, EventArgs eventArgs)
        {
            try
            {
                string textTitle = Utility.GetLocalizedLabel("ApplicationStatusDetails", "cannotRescheduleTitle");
                string textMessage = Utility.GetLocalizedLabel("ApplicationStatusDetails", "cannotRescheduleMessage");

                MyTNBAppToolTipBuilder cannotRescheduleTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                    .SetTitle(textTitle)
                    .SetMessage(textMessage)
                    .SetCTALabel(Utility.GetLocalizedCommonLabel("ok"))
                    .Build();
                cannotRescheduleTooltip.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowApplicationPopupMessage(Android.App.Activity context, StatusDetail statusDetail)
        {
            MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(statusDetail.Title)
                .SetMessage(statusDetail.Message)
                .SetCTALabel(statusDetail.PrimaryCTATitle)
                .Build();
            whereisMyacc.Show();
        }

        public async void OnCustomerRating()
        {
            ShowProgressDialog();
            try
            {
                customerRatingMasterResponse = await RatingManager.Instance.GetCustomerRatingMaster();
                if (!customerRatingMasterResponse.StatusDetail.IsSuccess)
                {
                    ShowApplicationPopupMessage(this, customerRatingMasterResponse.StatusDetail);
                }
                else
                {
                    Intent rating_activity = new Intent(this, typeof(ApplicationRatingActivity));
                    rating_activity.PutExtra("applicationDetailDisplay", JsonConvert.SerializeObject(applicationDetailDisplay));
                    rating_activity.PutExtra("customerRatingMasterResponse", JsonConvert.SerializeObject(customerRatingMasterResponse));
                    StartActivityForResult(rating_activity, Constants.APPLICATION_STATUS_RATING_REQUEST_CODE);
                }
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
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

        public async void OnSetAppointment(string appointment)
        {
            ShowProgressDialog();
            try
            {
                string businessArea = applicationDetailDisplay.BusinessArea ?? string.Empty;
                SchedulerDisplay response = await ScheduleManager.Instance.GetAvailableAppointment(businessArea);
                if (response.StatusDetail.IsSuccess)
                {
                    Intent intent = new Intent(this, typeof(AppointmentSelectActivity));
                    intent.PutExtra("applicationDetailDisplay", JsonConvert.SerializeObject(applicationDetailDisplay));
                    intent.PutExtra("newAppointmentResponse", JsonConvert.SerializeObject(response));
                    intent.PutExtra("appointment", appointment);
                    StartActivityForResult(intent, Constants.APPLICATION_STATUS_DETAILS_SCHEDULER_REQUEST_CODE);
                }
                else
                {
                    ShowApplicationPopupMessage(this, response.StatusDetail);
                }
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
            HideProgressDialog();
        }

        private void ViewActivityLog()
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
                            ToastUtils.OnDisplayToast(this, postSaveApplicationResponse.StatusDetail.Message ?? string.Empty);
                            SetResult(Result.Ok, new Intent());
                            Finish();
                        }
                        else
                        {
                            bool isTowButtons = !string.IsNullOrEmpty(postSaveApplicationResponse.StatusDetail.SecondaryCTATitle)
                                && !string.IsNullOrWhiteSpace(postSaveApplicationResponse.StatusDetail.SecondaryCTATitle);
                            MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, isTowButtons
                                    ? MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON
                                    : MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                                .SetTitle(postSaveApplicationResponse.StatusDetail.Title)
                                .SetMessage(postSaveApplicationResponse.StatusDetail.Message)
                                .SetCTALabel(postSaveApplicationResponse.StatusDetail.PrimaryCTATitle);

                            if (isTowButtons)
                            {
                                whereisMyacc.SetSecondaryCTALabel(postSaveApplicationResponse.StatusDetail.SecondaryCTATitle)
                                    .SetSecondaryCTAaction(() => ShowStatusLanding());
                            }
                            whereisMyacc.Build();
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

        public void ShowErrorSnackbar(string message)
        {
            if (mErrorSnackbar != null && mErrorSnackbar.IsShown)
            {
                mErrorSnackbar.Dismiss();
            }

            mErrorSnackbar = Snackbar.Make(rootview, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {
                mErrorSnackbar.Dismiss();
            }
            );
            View v = mErrorSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            TextView tvA = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_action);
            TextViewUtils.SetMuseoSans500Typeface(tv, tvA);
            TextViewUtils.SetTextSize16(tv, tvA);
            tv.SetMaxLines(5);
            mErrorSnackbar.Show();
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

        public override void OnBackPressed()
        {
            string dynatraceTag = DynatraceConstants.ApplicationStatus.CTAs.Details.Back;
            if (applicationDetailDisplay != null)
            {
                switch (applicationDetailDisplay.CTAType)
                {
                    case DetailCTAType.ReapplyNow:
                        dynatraceTag = DynatraceConstants.ApplicationStatus.CTAs.Details.NC_Reappoint_Contractor_Back;
                        break;
                    case DetailCTAType.SubmitApplicationRating:
                        dynatraceTag = DynatraceConstants.ApplicationStatus.CTAs.Details.NC_Rate_Our_Servie_Back;
                        break;
                    case DetailCTAType.CustomerRating:
                        dynatraceTag = DynatraceConstants.ApplicationStatus.CTAs.Details.NC_Customer_Rating_Back;
                        break;
                    case DetailCTAType.ContractorRating:
                        dynatraceTag = DynatraceConstants.ApplicationStatus.CTAs.Details.NC_Contractor_Rating_Back;
                        break;
                    default:
                        break;
                }
            }

            DynatraceHelper.OnTrack(dynatraceTag);

            if (IsPush)
            {
                Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
                DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(DashboardIntent);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetToolBarTitle(Utility.GetLocalizedLabel("ApplicationStatusDetails", "title"));
            presenter = new ApplicationStatusDetailPresenter(this);
            applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Gone;
            applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;
            btnPrimaryCTA.Visibility = ViewStates.Visible;
            linkedWithLayout.Visibility = ViewStates.Gone;
            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            applicationStatusStatusListRecyclerView.SetLayoutManager(layoutManager);
            applicationStatusStatusListRecyclerView.SetAdapter(adapter);

            txtAppointmentSet.Visibility = ViewStates.Gone;
            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);

            applicationStatusAdditionalListRecyclerView.SetLayoutManager(layoutManager);
            applicationStatusAdditionalListRecyclerView.SetAdapter(subAdapter);

            TextViewUtils.SetMuseoSans300Typeface(txtApplicationStatusUpdated, txtApplicationStatusDetail, txtApplicationRateStar, txtAppointmentSet);

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
                if (extras.ContainsKey("isPush"))
                {
                    IsPush = extras.GetBoolean("isPush");
                }
                if (extras != null)
                {
                    if (extras.ContainsKey(MyHomeConstants.APPLICATION_DETAIL_RESPONSE))
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
                        cannotReschedulTooltip.Visibility = ViewStates.Gone;
                        applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Gone;
                        applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;

                        applicationDetailDisplay = new GetApplicationStatusDisplay();
                        applicationDetailDisplay = JsonConvert.DeserializeObject<GetApplicationStatusDisplay>(extras.GetString("applicationStatusResponse"));
                        applicationDetailDisplay.IsSchedulerEnable = !MyTNBAccountManagement.GetInstance().IsAppointmentDisabled;
                        MyTNBAccountManagement.GetInstance().SetIsTNGEnable(applicationDetailDisplay.IsTNGEnableApplicationStatus);

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

                            if (applicationDetailDisplay.CTAType == DetailCTAType.NewAppointment
                                || applicationDetailDisplay.CTAType == DetailCTAType.Reschedule)
                            {
                                txtAppointmentSet.Visibility = ViewStates.Visible;
                                txtAppointmentSet.Text = Build.VERSION.SdkInt >= BuildVersionCodes.N
                                    ? Html.FromHtml(applicationDetailDisplay.CTAMessage, FromHtmlOptions.ModeLegacy).ToString()
                                    : Html.FromHtml(applicationDetailDisplay.CTAMessage).ToString();
                                btnPrimaryCTA.Text = applicationDetailDisplay.CTAType == DetailCTAType.NewAppointment
                                    ? Utility.GetLocalizedLabel("ApplicationStatusDetails", "setAppointmentNowCTA")
                                    : Utility.GetLocalizedLabel("ApplicationStatusDetails", "rescheduleCTA");

                                applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Gone;
                                applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;
                                applicationStatusDetailSingleButtonLayout.Visibility = ViewStates.Visible;

                                if (applicationDetailDisplay.CTAType == DetailCTAType.Reschedule)
                                {
                                    btnPrimaryCTA.Enabled = true;
                                    btnPrimaryCTA.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.freshGreen));
                                    btnPrimaryCTA.Background = ContextCompat.GetDrawable(this, Resource.Drawable.light_green_outline_button_background);
                                }
                            }
                            else if (applicationDetailDisplay.CTAType == DetailCTAType.RescheduleDisabled)
                            {
                                txtCannotRescheduleTooltip.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "cannotReschedule");

                                txtAppointmentSet.Text = Build.VERSION.SdkInt >= BuildVersionCodes.N
                                    ? Html.FromHtml(applicationDetailDisplay.CTAMessage, FromHtmlOptions.ModeLegacy).ToString()
                                    : Html.FromHtml(applicationDetailDisplay.CTAMessage).ToString();
                                btnPrimaryCTA.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "rescheduleCTA");

                                btnPrimaryCTA.Enabled = false;
                                btnPrimaryCTA.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.silverChalice));
                                btnPrimaryCTA.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_outline);

                                txtAppointmentSet.Visibility = ViewStates.Visible;
                                cannotReschedulTooltip.Visibility = ViewStates.Visible;
                                applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Gone;
                                applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;
                                applicationStatusDetailSingleButtonLayout.Visibility = ViewStates.Visible;

                                TextViewUtils.SetMuseoSans500Typeface(txtCannotRescheduleTooltip);
                                TextViewUtils.SetTextSize12(txtCannotRescheduleTooltip);
                            }
                            else if (applicationDetailDisplay.CTAType == DetailCTAType.PayOffline)
                            {
                                txtApplicationStatusBottomPayableCurrency.Text = "RM";
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

                                if (!applicationDetailDisplay.IsPaymentEnabled)
                                {
                                    btnApplicationStatusPay.Visibility = ViewStates.Visible;
                                    btnApplicationStatusPay.Enabled = false;
                                    btnApplicationStatusPay.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.white));
                                    btnApplicationStatusPay.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
                                }

                                if (!Utility.IsEnablePayment())
                                {
                                    btnApplicationStatusPay.Visibility = ViewStates.Visible;
                                    btnApplicationStatusPay.Enabled = true;
                                    btnApplicationStatusPay.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.white));
                                    btnApplicationStatusPay.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
                                }
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
                            else if (applicationDetailDisplay.CTAType == DetailCTAType.CustomerRating
                                || applicationDetailDisplay.CTAType == DetailCTAType.ContractorRating)
                            {
                                btnPrimaryCTA.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "rateCTA");
                                applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Gone;
                                applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;
                                applicationStatusDetailSingleButtonLayout.Visibility = ViewStates.Visible;

                                if (applicationDetailDisplay.CTAType == DetailCTAType.CustomerRating)
                                {
                                    DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.Screens.Details.NC_Customer_Rating);
                                }
                                else if (applicationDetailDisplay.CTAType == DetailCTAType.ContractorRating)
                                {
                                    DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.Screens.Details.NC_Contractor_Rating);
                                }
                            }
                            else if (applicationDetailDisplay.CTAType == DetailCTAType.StartApplication)
                            {
                                if (applicationDetailDisplay.CTAMessage.IsValid())
                                {
                                    txtAppointmentSet.Visibility = ViewStates.Visible;
                                    txtAppointmentSet.Text = Build.VERSION.SdkInt >= BuildVersionCodes.N
                                   ? Html.FromHtml(applicationDetailDisplay.CTAMessage, FromHtmlOptions.ModeLegacy).ToString()
                                   : Html.FromHtml(applicationDetailDisplay.CTAMessage).ToString();
                                }

                                applicationStatusDetailSingleButtonLayout.Visibility = ViewStates.Visible;
                                btnPrimaryCTA.Text = GetLabelByLanguage(ApplicationStatusDetails.CTATitles.I18N_StartApplication);
                                btnPrimaryCTA.Enabled = true;
                                btnPrimaryCTA.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_round_button_background);

                                DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.Screens.Details.NC_Start_Electricity);
                            }
                            else if (applicationDetailDisplay.CTAType == DetailCTAType.ReapplyNow)
                            {
                                if (applicationDetailDisplay.CTAMessage.IsValid())
                                {
                                    txtAppointmentSet.Visibility = ViewStates.Visible;
                                    txtAppointmentSet.Text = Build.VERSION.SdkInt >= BuildVersionCodes.N
                                   ? Html.FromHtml(applicationDetailDisplay.CTAMessage, FromHtmlOptions.ModeLegacy).ToString()
                                   : Html.FromHtml(applicationDetailDisplay.CTAMessage).ToString();
                                }

                                applicationStatusDetailSingleButtonLayout.Visibility = ViewStates.Visible;
                                btnPrimaryCTA.Text = GetLabelByLanguage(ApplicationStatusDetails.CTATitles.I18N_ReaapplyNow);
                                btnPrimaryCTA.Enabled = true;
                                btnPrimaryCTA.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_round_button_background);

                                DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.Screens.Details.NC_Reappoint_Contractor);
                            }
                            else if (applicationDetailDisplay.CTAType == DetailCTAType.ReuploadDocument)
                            {
                                if (applicationDetailDisplay.CTAMessage.IsValid())
                                {
                                    txtAppointmentSet.Visibility = ViewStates.Visible;
                                    txtAppointmentSet.Text = Build.VERSION.SdkInt >= BuildVersionCodes.N
                                   ? Html.FromHtml(applicationDetailDisplay.CTAMessage, FromHtmlOptions.ModeLegacy).ToString()
                                   : Html.FromHtml(applicationDetailDisplay.CTAMessage).ToString();
                                }

                                applicationStatusDetailSingleButtonLayout.Visibility = ViewStates.Visible;
                                btnPrimaryCTA.Text = GetLabelByLanguage(ApplicationStatusDetails.CTATitles.I18N_UpdateNow);
                                btnPrimaryCTA.Enabled = true;
                                btnPrimaryCTA.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_round_button_background);
                            }
                            else if (applicationDetailDisplay.CTAType == DetailCTAType.DeleteApplication)
                            {
                                if (applicationDetailDisplay.CTAMessage.IsValid())
                                {
                                    txtAppointmentSet.Visibility = ViewStates.Visible;
                                    txtAppointmentSet.Text = Build.VERSION.SdkInt >= BuildVersionCodes.N
                                   ? Html.FromHtml(applicationDetailDisplay.CTAMessage, FromHtmlOptions.ModeLegacy).ToString()
                                   : Html.FromHtml(applicationDetailDisplay.CTAMessage).ToString();
                                }

                                applicationStatusDetailSingleButtonLayout.Visibility = ViewStates.Visible;
                                btnPrimaryCTA.Text = GetLabelByLanguage(ApplicationStatusDetails.CTATitles.I18N_Delete);
                                btnPrimaryCTA.Enabled = true;
                                btnPrimaryCTA.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.tomato));
                                btnPrimaryCTA.Background = ContextCompat.GetDrawable(this, Resource.Drawable.red_outline_round_button_background);
                            }
                            else if (applicationDetailDisplay.CTAType == DetailCTAType.ResumeApplication)
                            {
                                applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Visible;

                                btnApplicationStatusViewBill.Text = GetLabelByLanguage(ApplicationStatusDetails.CTATitles.I18N_Delete);
                                btnApplicationStatusViewBill.Enabled = true;
                                btnApplicationStatusViewBill.SetTextColor(ContextCompat.GetColorStateList(this, Resource.Color.tomato));
                                btnApplicationStatusViewBill.Background = ContextCompat.GetDrawable(this, Resource.Drawable.red_outline_round_button_background);

                                btnApplicationStatusPay.Text = GetLabelByLanguage(ApplicationStatusDetails.CTATitles.I18N_Resume);
                                btnApplicationStatusPay.Enabled = true;
                                btnApplicationStatusPay.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_round_button_background);
                            }
                            else if (applicationDetailDisplay.CTAType == DetailCTAType.SubmitApplicationRating)
                            {
                                btnPrimaryCTA.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", ApplicationStatusDetails.CTATitles.I18N_NCSubmissionRateUs);
                                btnPrimaryCTA.Enabled = true;
                                btnPrimaryCTA.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_round_button_background);

                                applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Gone;
                                applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;
                                applicationStatusDetailSingleButtonLayout.Visibility = ViewStates.Visible;

                                DynatraceHelper.OnTrack(DynatraceConstants.ApplicationStatus.Screens.Details.NC_Rate_Our_Service);
                            }
                            else if (applicationDetailDisplay.CTAType == DetailCTAType.None)
                            {
                                applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Gone;
                                applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;
                                applicationStatusDetailSingleButtonLayout.Visibility = ViewStates.Gone;
                                ctaParentLayout.Visibility = ViewStates.Gone;
                            }
                            TextViewUtils.SetMuseoSans500Typeface(txtApplicationStatusMainTitle, txtApplicationStatusTitle, txtApplicationStatusBottomPayableTitle);
                            TextViewUtils.SetMuseoSans300Typeface(txtApplicationStatusSubTitle, txtApplicationStatusDetailNote, txtBCRMDownMessage, txtAppointmentSet);

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
                            ToastUtils.OnDisplayToast(this
                                , statusDetails?.Message ?? Utility.GetLocalizedLabel("ApplicationStatusDetails", "rateSuccessMessage"));
                        }
                    }
                    if (extras.ContainsKey("applicationRated")
                        && extras.GetString("applicationRated") != null
                        && extras.GetString("applicationRated") != "0")
                    {
                        txtApplicationRateStar.Visibility = ViewStates.Visible;
                        imgStar.Visibility = ViewStates.Visible;
                        layoutstar.Visibility = ViewStates.Visible;
                        txtApplicationRateStar.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "youRated") + extras.GetString("applicationRated") + " ";
                        ctaParentLayout.Visibility = ViewStates.Gone;
                    }
                }

                TextViewUtils.SetMuseoSans500Typeface(txtApplicationStatusMainTitle, txtApplicationStatusTitle, txtApplicationStatusBottomPayableTitle, btnViewActivityLog, howDoISeeApplicaton, btnPrimaryCTA
                    , btnApplicationStatusViewBill, btnApplicationStatusPay);
                TextViewUtils.SetMuseoSans300Typeface(txtApplicationStatusSubTitle, txtApplicationStatusDetailNote);
                TextViewUtils.SetTextSize10(txtLinkedWithHeader);
                TextViewUtils.SetTextSize12(txtApplicationStatusUpdated, txtApplicationStatusDetail, txtLinkedWithView
                    , txtApplicationStatusDetailNote, howDoISeeApplicaton);
                TextViewUtils.SetTextSize13(txtApplicationStatusBottomPayableCurrency);
                TextViewUtils.SetTextSize14(txtApplicationStatusSubTitle, txtLinkedWithReferencNo, txtApplicationStatusDetail
                    , txtApplicationStatusBottomPayableTitle);
                TextViewUtils.SetTextSize16(txtApplicationStatusTitle, txtApplicationStatusMainTitle, btnViewActivityLog
                    , btnPrimaryCTA, btnApplicationStatusViewBill, btnApplicationStatusPay);
                TextViewUtils.SetTextSize25(txtApplicationStatusBottomPayable);
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
                if (!UserSessions.HasApplicationDetailShown(PreferenceManager.GetDefaultSharedPreferences(this)))//STUB
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

        public int GetProgressDateCount()
        {
            int i = 0;

            try
            {
                i = adapter.statusDateCount;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
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

        public int GetapplicationStatusDetailNonLoginLayout()
        {
            int i = 0;
            try
            {
                i = applicationStatusDetailNonLoginLayout.Height;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }

        public int GetRecyclerViewHeight()
        {

            int i = 0;
            try
            {
                i = applicationStatusStatusListRecyclerView.Height;

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
                            ToastUtils.OnDisplayToast(this, response.StatusDetail.Message ?? string.Empty);
                            SetResult(Result.Ok, new Intent());
                            Finish();
                        }
                    }
                    else
                    {
                        MyTNBAppToolTipBuilder removeFailPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                            .SetTitle(response.StatusDetail.Title)
                            .SetMessage(response.StatusDetail.Message)
                            .SetCTALabel(Utility.GetLocalizedCommonLabel("ok"))
                            .Build();
                        removeFailPopup.Show();
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

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok && requestCode == Constants.APPLICATION_STATUS_RATING_REQUEST_CODE)
            {
                if (data != null && data.Extras is Bundle extras && extras != null)
                {
                    if (extras.ContainsKey("applicationRated")
                        && extras.GetString("applicationRated") != null
                        && extras.GetString("applicationRated") != "0")
                    {
                        txtApplicationRateStar.Visibility = ViewStates.Visible;
                        imgStar.Visibility = ViewStates.Visible;
                        layoutstar.Visibility = ViewStates.Visible;
                        txtApplicationRateStar.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "youRated") + extras.GetString("applicationRated") + " ";
                        ctaParentLayout.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    this.RunOnUiThread(() =>
                    {
                        ShowProgressDialog();
                    });

                    Task.Run(() =>
                    {
                        _ = GetApplicationDetail(UpdateType.ContractorRating);
                    });
                }
            }
            else if (resultCode == Result.Ok && requestCode == Constants.APPLICATION_STATUS_DETAILS_SCHEDULER_REQUEST_CODE)
            {
                SetResult(Result.Ok);
                Finish();
            }
            else if (resultCode == Result.Ok && requestCode == PaymentActivity.SELECT_PAYMENT_ACTIVITY_CODE)
            {

            }
            else if (resultCode == Result.Ok && requestCode == Constants.APPLICATION_STATUS_START_RESUME_REQUEST_CODE)
            {
                if (data != null && data.Extras is Bundle extras && extras != null)
                {
                    if (extras.ContainsKey(MyHomeConstants.BACK_TO_HOME))
                    {
                        bool backToHome = extras.GetBoolean(MyHomeConstants.BACK_TO_HOME);
                        if (backToHome)
                        {
                            string toastMessage = string.Empty;
                            if (extras.ContainsKey(MyHomeConstants.CANCEL_TOAST_MESSAGE))
                            {
                                toastMessage = extras.GetString(MyHomeConstants.CANCEL_TOAST_MESSAGE);
                            }

                            Intent resultIntent = new Intent();
                            resultIntent.PutExtra(MyHomeConstants.BACK_TO_HOME, true);
                            resultIntent.PutExtra(MyHomeConstants.CANCEL_TOAST_MESSAGE, toastMessage);
                            SetResult(Result.Ok, resultIntent);
                            Finish();
                        }
                    }
                    else if (extras.ContainsKey(MyHomeConstants.BACK_TO_APPLICATION_STATUS_LANDING))
                    {
                        bool backToStatusLanding = extras.GetBoolean(MyHomeConstants.BACK_TO_APPLICATION_STATUS_LANDING);
                        if (backToStatusLanding)
                        {
                            string toastMessage = string.Empty;
                            if (extras.ContainsKey(MyHomeConstants.CANCEL_TOAST_MESSAGE))
                            {
                                toastMessage = extras.GetString(MyHomeConstants.CANCEL_TOAST_MESSAGE);
                            }

                            Intent resultIntent = new Intent();
                            resultIntent.PutExtra(MyHomeConstants.BACK_TO_APPLICATION_STATUS_LANDING, true);
                            resultIntent.PutExtra(MyHomeConstants.CANCEL_TOAST_MESSAGE, toastMessage);
                            SetResult(Result.Ok, resultIntent);
                            Finish();
                        }
                    }
                }
            }
            else if (resultCode == Result.Ok && requestCode == Constants.APPLICATION_STATUS_SUBMIT_APPLICATION_RATING_REQUEST_CODE)
            {
                if (data != null && data.Extras is Bundle extras && extras != null)
                {
                    if (extras.ContainsKey(Constants.APPLICATION_STATUS_DETAIL_RELOAD))
                    {
                        bool toReload = extras.GetBoolean(Constants.APPLICATION_STATUS_DETAIL_RELOAD);
                        if (toReload)
                        {
                            this.RunOnUiThread(() =>
                            {
                                ShowProgressDialog();
                            });

                            string toastMessage = string.Empty;
                            if (extras.ContainsKey(Constants.APPLICATION_STATUS_DETAIL_RATED_TOAST_MESSAGE))
                            {
                                toastMessage = extras.GetString(Constants.APPLICATION_STATUS_DETAIL_RATED_TOAST_MESSAGE);
                            }

                            Task.Run(() =>
                            {
                                _ = GetApplicationDetail(UpdateType.SubmitApplicationRating, toastMessage);
                            });
                        }
                    }
                }
            }
        }

        private async Task GetApplicationDetail(UpdateType updateType, string toastMessage = "")
        {
            ApplicationDetailDisplay response = await ApplicationStatusManager.Instance.GetApplicationDetail(applicationDetailDisplay.SavedApplicationID
                , applicationDetailDisplay.ApplicationDetail.ApplicationId
                , applicationDetailDisplay.ApplicationTypeCode
                , applicationDetailDisplay.System);

            this.RunOnUiThread(() =>
            {
                HideProgressDialog();

                if (toastMessage.IsValid())
                {
                    ToastUtils.OnDisplayToast(this, toastMessage);
                }

                if (response.StatusDetail.IsSuccess && response.Content != null)
                {
                    applicationDetailDisplay.ApplicationRatingDetail = response.Content.ApplicationRatingDetail;
                    if (updateType == UpdateType.ContractorRating)
                    {
                        if (response.Content.RatingDisplay != null
                            && response.Content.RatingDisplay != string.Empty)
                        {
                            txtApplicationRateStar.Visibility = ViewStates.Visible;
                            imgStar.Visibility = ViewStates.Visible;
                            layoutstar.Visibility = ViewStates.Visible;
                            txtApplicationRateStar.Text = response.Content.RatingDisplay + " ";
                        }
                        else
                        {
                            txtApplicationRateStar.Visibility = ViewStates.Gone;
                            imgStar.Visibility = ViewStates.Gone;
                            layoutstar.Visibility = ViewStates.Gone;
                        }
                        if (response.Content.IsContractorRating)
                        {
                            btnPrimaryCTA.Text = Utility.GetLocalizedLabel("ApplicationStatusDetails", "rateCTA");
                            applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Gone;
                            applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;
                            ctaParentLayout.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Gone;
                            applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;
                            applicationStatusDetailSingleButtonLayout.Visibility = ViewStates.Gone;
                            ctaParentLayout.Visibility = ViewStates.Gone;
                        }
                    }
                    else if (updateType == UpdateType.SubmitApplicationRating)
                    {
                        applicationStatusDetailDoubleButtonLayout.Visibility = ViewStates.Gone;
                        applicationStatusBotomPayableLayout.Visibility = ViewStates.Gone;
                        applicationStatusDetailSingleButtonLayout.Visibility = ViewStates.Gone;
                        ctaParentLayout.Visibility = ViewStates.Gone;
                    }
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
            });
        }

        private enum UpdateType
        {
            ContractorRating,
            SubmitApplicationRating
        }
    }
}