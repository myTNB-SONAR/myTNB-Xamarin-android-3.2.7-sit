using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using CheeseBind;
using DynatraceAndroid;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Billing.MVP;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.EnergyBudgetRating.Activity;
using myTNB_Android.Src.EnergyBudgetRating.Fargment;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.ManageAccess.Activity;
using myTNB_Android.Src.ManageSupplyAccount.Activity;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.NotificationDetails.Models;
using myTNB_Android.Src.NotificationDetails.MVP;
using myTNB_Android.Src.Notifications.Models;
using myTNB_Android.Src.Rating.Model;
using myTNB_Android.Src.SSMR.SubmitMeterReading.MVP;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.ViewReceipt.Activity;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.NotificationDetails.Activity
{
    [Activity(Label = "Notification Detail", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class UserNotificationDetailActivity : BaseToolbarAppCompatActivity, UserNotificationDetailContract.IView
    {
        [BindView(Resource.Id.notificationDetailBannerImg)]
        ImageView notificationDetailBannerImg;

        [BindView(Resource.Id.notificationDetailTitle)]
        TextView notificationDetailTitle;

        [BindView(Resource.Id.notificationDetailMessage)]
        TextView notificationDetailMessage;

        [BindView(Resource.Id.rootView)]
        ViewGroup rootView;

        [BindView(Resource.Id.notificationMainLayout)]
        ScrollView notificationMainLayout;


        Models.NotificationDetails notificationDetails;
        UserNotificationData userNotificationData;
        internal static myTNB.Mobile.NotificationOpenDirectDetails Notification;
        int position;
        UserNotificationDetailPresenter mPresenter;
        AlertDialog removeDialog;
        public bool pushFromDashboard = false;
        IDTXAction dynaTrace;
        private List<RateUsQuestion> activeQuestionListNo = new List<RateUsQuestion>();
        private List<RateUsQuestion> activeQuestionListYes = new List<RateUsQuestion>();

        public override int ResourceId()
        {
            return Resource.Layout.UserNotificationDetailLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.NotificationDetailMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        protected override void OnStop()
        {
            base.OnStop();
            HideLoadingScreen();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_delete_notification:
                    removeDialog = new AlertDialog.Builder(this)

                        .SetTitle(Utility.GetLocalizedLabel("PushNotificationList", "deleteTitle"))
                        .SetMessage(Utility.GetLocalizedLabel("PushNotificationList", "deleteMessage"))
                        .SetNegativeButton(Utility.GetLocalizedCommonLabel("cancel"),
                        delegate
                        {
                            removeDialog.Dismiss();
                        })
                        .SetPositiveButton(Utility.GetLocalizedCommonLabel("ok"),
                        delegate
                        {
                            mPresenter.DeleteNotificationDetail(notificationDetails);
                        })
                        .Show();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            Intent result = new Intent();
            result.PutExtra(Constants.SELECTED_NOTIFICATION_ITEM_POSITION, position);
            result.PutExtra(Constants.ACTION_IS_READ, true);
            SetResult(Result.Ok, result);
            base.OnBackPressed();
        }

        public void ReturnToDashboard()
        {
            Finish();
        }

        public void OnClickSpan(string textMessage)
        {
            if (textMessage != null && textMessage.Contains("http"))
            {
                //Launch webview
                int startIndex = textMessage.LastIndexOf("=") + 2;
                int lastIndex = textMessage.LastIndexOf("\"");
                int lengthOfId = (lastIndex - startIndex);
                if (lengthOfId < textMessage.Length)
                {
                    string url = textMessage.Substring(startIndex, lengthOfId);
                    if (!string.IsNullOrEmpty(url))
                    {
                        Intent intent = new Intent(Intent.ActionView);
                        intent.SetData(Android.Net.Uri.Parse(url));
                        StartActivity(intent);
                    }
                }
            }
            else if (textMessage != null && textMessage.Contains("faq"))
            {
                //Lauch FAQ
                int startIndex = textMessage.LastIndexOf("=") + 1;
                int lastIndex = textMessage.LastIndexOf("}");
                int lengthOfId = (lastIndex - startIndex) + 1;
                if (lengthOfId < textMessage.Length)
                {
                    string faqid = textMessage.Substring(startIndex, lengthOfId);
                    if (!string.IsNullOrEmpty(faqid))
                    {
                        Intent faqIntent = new Intent(this, typeof(FAQListActivity));
                        faqIntent.PutExtra(Constants.FAQ_ID_PARAM, faqid);
                        StartActivity(faqIntent);
                    }
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                mPresenter = new UserNotificationDetailPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
                base.OnCreate(savedInstanceState);
                SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_DashboardLarge : Resource.Style.Theme_Dashboard);
                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM))
                    {
                        notificationDetails = DeSerialze<NotificationDetails.Models.NotificationDetails>(extras.GetString(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM));
                    }

                    if (extras.ContainsKey(Constants.SELECTED_FROMDASHBOARD_NOTIFICATION_DETAIL_ITEM))
                    {
                        Notification = DeSerialze<myTNB.Mobile.NotificationOpenDirectDetails>(extras.GetString(Constants.SELECTED_FROMDASHBOARD_NOTIFICATION_DETAIL_ITEM));
                        pushFromDashboard = true;
                    }

                    if (extras.ContainsKey(Constants.SELECTED_NOTIFICATION_LIST_ITEM))
                    {
                        userNotificationData = DeSerialze<UserNotificationData>(extras.GetString(Constants.SELECTED_NOTIFICATION_LIST_ITEM));
                        SetToolBarTitle(userNotificationData.Title);
                    }

                    position = extras.GetInt(Constants.SELECTED_NOTIFICATION_ITEM_POSITION);
                }
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                TextViewUtils.SetMuseoSans500Typeface(notificationDetailTitle);
                TextViewUtils.SetMuseoSans300Typeface(notificationDetailMessage);
                TextViewUtils.SetTextSize14(notificationDetailMessage);
                TextViewUtils.SetTextSize16(notificationDetailTitle);

                if (notificationDetails != null && notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_SMR_DISABLED_SUCCESS_ID)
                {
                    notificationMainLayout.SetBackgroundColor(Color.ParseColor("#ffffff"));
                }

                if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_80 || notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_100
                        || notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_TC || notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_RC)
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel("PushNotificationDetails", "EnergyBudgetTitle"));
                }

                if (pushFromDashboard)
                {
                    mPresenter.OnShowNotificationDetails(Notification.Type, Notification.EventId, Notification.RequestTransId);
                }
                else
                {
                    mPresenter.EvaluateDetail(notificationDetails);
                    RenderUI();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void RenderUI()
        {
            try
            {
                NotificationDetailModel detailModel = mPresenter.GetNotificationDetailModel();
                NotificationDetailCTAComponent ctaComponent = FindViewById<NotificationDetailCTAComponent>(Resource.Id.notificationCTAComponent);
                if (detailModel != null)
                {
                    notificationDetailBannerImg.Visibility = ViewStates.Visible;
                    notificationDetailTitle.Visibility = ViewStates.Visible;
                    notificationDetailMessage.Visibility = ViewStates.Visible;
                    ctaComponent.Visibility = ViewStates.Visible;

                    notificationDetailBannerImg.SetImageResource(detailModel.imageResourceBanner);

                    notificationDetailTitle.Text = detailModel.title;

                    notificationDetailMessage.TextFormatted = GetFormattedText(detailModel.message);

                    if (detailModel.message != null)
                    {
                        notificationDetailMessage = LinkRedirectionUtils
                            .Create(this, "")
                            .SetTextView(notificationDetailMessage)
                            .SetMessage(detailModel.message)
                            .Build()
                            .GetProcessedTextView();
                    }

                    if (detailModel.ctaList.Count > 0)
                    {
                        ctaComponent.Visibility = ViewStates.Visible;
                        ctaComponent.SetCTAButton(detailModel.ctaList);
                    }
                    else
                    {
                        ctaComponent.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    notificationDetailTitle.Visibility = ViewStates.Gone;
                    notificationDetailMessage.Visibility = ViewStates.Gone;
                    ctaComponent.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowNotificationListAsDeleted()
        {
            Intent result = new Intent();
            result.PutExtra(Constants.SELECTED_NOTIFICATION_ITEM_POSITION, position);
            result.PutExtra(Constants.ACTION_IS_DELETE, true);
            SetResult(Result.Ok, result);
            Finish();
        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();

            }
            );
            View v = mCancelledExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mCancelledExceptionSnackBar.Show();

        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();

            }
            );
            View v = mApiExcecptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mApiExcecptionSnackBar.Show();

        }
        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();

        }

        public void ShowLoadingScreen()
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

        public void HideLoadingScreen()
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

        public void PayNow(AccountData mSelectedAccountData)
        {
            Intent payment_activity = new Intent(this, typeof(SelectAccountsActivity));
            payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
            StartActivity(payment_activity);
        }

        public void ContactUs(WeblinkEntity entity)
        {
            if (entity.OpenWith.Equals("PHONE"))
            {
                var uri = Android.Net.Uri.Parse("tel:" + entity.Url);
                var intent = new Intent(Intent.ActionDial, uri);
                StartActivity(intent);
            }
        }

        public void ViewUsage(AccountData mSelectedAccountData)
        {
            CustomerBillingAccount.RemoveSelected();
            CustomerBillingAccount.SetSelected(mSelectedAccountData.AccountNum);

            if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_80)
            {
                CustomClassAnalytics.SetScreenNameDynaTrace(Constants.EB_view_budget_reaching);
                FirebaseAnalyticsUtils.SetScreenName(this, Constants.EB_view_budget_reaching);
            }
            else if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_100)
            {
                CustomClassAnalytics.SetScreenNameDynaTrace(Constants.EB_view_budget_reached);
                FirebaseAnalyticsUtils.SetScreenName(this, Constants.EB_view_budget_reached);
            }

            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            DashboardIntent.PutExtra("FROM_NOTIFICATION", true);
            MyTNBAccountManagement.GetInstance().SetIsAccessUsageFromNotification(true);
            StartActivity(DashboardIntent);
        }

        public void ViewTips()
        {
            CustomClassAnalytics.SetScreenNameDynaTrace(Constants.EB_view_tips);
            FirebaseAnalyticsUtils.SetScreenName(this, Constants.EB_view_tips);
            if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_80)
            {
                CustomClassAnalytics.SetScreenNameDynaTrace(Constants.EB_view_tips_reaching);
                FirebaseAnalyticsUtils.SetScreenName(this, Constants.EB_view_tips_reaching);
            }
            else if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_100)
            {
                CustomClassAnalytics.SetScreenNameDynaTrace(Constants.EB_view_tips_reached);
                FirebaseAnalyticsUtils.SetScreenName(this, Constants.EB_view_tips_reached);
            }
            MyTNBAccountManagement.GetInstance().SetIsFromViewTips(true);
            Intent webIntent = new Intent(this, typeof(BaseWebviewActivity));
            webIntent.PutExtra(Constants.IN_APP_LINK, Utility.GetLocalizedLabel("PushNotificationDetails", "viewTipsURL"));
            webIntent.PutExtra(Constants.IN_APP_TITLE, Utility.GetLocalizedLabel("PushNotificationDetails", "energyBudgetSavingTips"));
            this.StartActivity(webIntent);
        }

        public void ViewDetails(AccountData mSelectedAccountData)
        {
            Intent intent = new Intent(this, typeof(BillingDetailsActivity));
            intent.PutExtra("SELECTED_ACCOUNT", JsonConvert.SerializeObject(mSelectedAccountData));
            StartActivity(intent);
        }

        public void SubmitMeterReading(AccountData mSelectedAccountData, SMRActivityInfoResponse SMRAccountActivityInfoResponse)
        {
            Intent ssmr_submit_meter_activity = new Intent(this, typeof(SubmitMeterReadingActivity));
            ssmr_submit_meter_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
            ssmr_submit_meter_activity.PutExtra(Constants.SMR_RESPONSE_KEY, JsonConvert.SerializeObject(SMRAccountActivityInfoResponse));
            StartActivity(ssmr_submit_meter_activity);
        }

        public void EnableSelfMeterReading(AccountData mSelectedAccountData)
        {
            Intent ssmr_history_activity = new Intent(this, typeof(SSMRMeterHistoryActivity));
            ssmr_history_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
            ssmr_history_activity.PutExtra("fromNotificationDetails", true);
            StartActivity(ssmr_history_activity);
        }

        public void ViewBillHistory(AccountData mSelectedAccountData)
        {
            CustomerBillingAccount.RemoveSelected();
            CustomerBillingAccount.SetSelected(mSelectedAccountData.AccountNum);

            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            DashboardIntent.PutExtra("FROM_NOTIFICATION", true);
            DashboardIntent.PutExtra("MENU", "BillMenu");
            DashboardIntent.PutExtra("DATA", JsonConvert.SerializeObject(mSelectedAccountData));
            StartActivity(DashboardIntent);
        }

        public void ViewManageAccess(AccountData accountData)
        {
            Intent manageAccount = new Intent(this, typeof(ManageSupplyAccountActivityEdit));
            manageAccount.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            //manageAccount.PutExtra(Constants.SELECTED_ACCOUNT_POSITION, position);
            StartActivityForResult(manageAccount, Constants.MANAGE_SUPPLY_ACCOUNT_REQUEST);
        }

        public void ShowPaymentReceipt(GetPaymentReceiptResponse response)
        {
            Intent viewReceipt = new Intent(this, typeof(ViewReceiptMultiAccountNewDesignActivty));
            viewReceipt.PutExtra("ReceiptResponse", JsonConvert.SerializeObject(response));
            StartActivity(viewReceipt);
        }

        public void ShowSelectBill(AccountData mSelectedAccountData)
        {
            Intent payment_activity = new Intent(this, typeof(SelectAccountsActivity));
            payment_activity.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
            StartActivity(payment_activity);
        }

        public void ShowPaymentReceiptError()
        {
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(Utility.GetLocalizedErrorLabel("defaultErrorTitle"))
                .SetMessage(Utility.GetLocalizedErrorLabel("receiptErrorMsg"))
                .SetContentGravity(GravityFlags.Center)
                .SetCTALabel(Utility.GetLocalizedCommonLabel("ok"))
                .Build().Show();
        }

        protected override void OnPause()
        {
            base.OnPause();
            try
            {
                if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_80 || 
                    notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_100)
                {
                    dynaTrace.LeaveAction();
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
                if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_80)
                {
                    dynaTrace = DynatraceAndroid.Dynatrace.EnterAction(Constants.EB_view_notification_duration_reaching);
                    FirebaseAnalyticsUtils.SetScreenName(this, Constants.EB_view_notification_duration_reaching);
                }
                else if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_100)
                {
                    dynaTrace = DynatraceAndroid.Dynatrace.EnterAction(Constants.EB_view_notification_duration_reached);
                    FirebaseAnalyticsUtils.SetScreenName(this, Constants.EB_view_notification_duration_reached);
                }
                //dynaTrace = DynatraceAndroid.Dynatrace.EnterAction(Constants.EB_view_notification_duration);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                if (MyTNBAccountManagement.GetInstance().IsFromViewTipsPage() && notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_80)
                {
                    MyTNBAccountManagement.GetInstance().SetIsFromViewTips(false);
                    mPresenter.OnCheckFeedbackCount();
                }
                else
                {
                    MyTNBAccountManagement.GetInstance().SetIsFromViewTips(false);
                }    

                if (MyTNBAccountManagement.GetInstance().IsFinishFeedback())
                {
                    MyTNBAccountManagement.GetInstance().SetIsFinishFeedback(false);
                    ShowThankYouFeedbackTooltips();
                }
            }
            catch (Exception e)
            {
                 Utility.LoggingNonFatalError(e);
            }
        }

        public void GetFeedbackTwoQuestionsNo(GetRateUsQuestionResponse response)
        {
            try
            {
                if (response != null)
                {
                    activeQuestionListNo.Clear();
                    if (response.GetData().Count > 0)
                    {
                        foreach (RateUsQuestion que in response.GetData())
                        {
                            if (que.IsActive)
                            {
                                activeQuestionListNo.Add(que);
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void GetFeedbackTwoQuestionsYes(GetRateUsQuestionResponse response)
        {
            try
            {
                if (response != null)
                {
                    activeQuestionListYes.Clear();
                    if (response.GetData().Count > 0)
                    {
                        foreach (RateUsQuestion que in response.GetData())
                        {
                            if (que.IsActive)
                            {
                                activeQuestionListYes.Add(que);
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowFeedBackSetupPageRating()
        {
            try
            {
                SetupFeedBackFragment.Create(this, SetupFeedBackFragment.ToolTipType.NORMAL_WITH_THREE_BUTTON)
                    .SetCTALabel(Utility.GetLocalizedLabel("PushNotificationDetails", "dontAskAgain"))
                    .SetTitleOtherOne(Utility.GetLocalizedLabel("PushNotificationDetails", "likeButtonDetails"))
                    .SetTitleOtherTwo(Utility.GetLocalizedLabel("PushNotificationDetails", "dislikeButton"))
                    .SetTitle(Utility.GetLocalizedLabel("PushNotificationDetails", "feedback2Title"))
                    .SetCTAaction(() =>
                    {
                        mPresenter.OnCheckUserLeaveOut();
                    })
                    .SetYesBtnCTAaction(() =>
                    {
                        Intent intent = new Intent(this, typeof(EnergyBudgetRatingActivity));
                        intent.PutExtra("feedbackOne", "Yes");
                        intent.PutExtra("RateUsQuestionNo", JsonConvert.SerializeObject(activeQuestionListNo));
                        intent.PutExtra("RateUsQuestionYes", JsonConvert.SerializeObject(activeQuestionListYes));
                        StartActivity(intent);
                    })
                    .SetNoBtnCTAaction(() =>
                    {
                        Intent intent = new Intent(this, typeof(EnergyBudgetRatingActivity));
                        intent.PutExtra("feedbackOne", "No");
                        intent.PutExtra("RateUsQuestionNo", JsonConvert.SerializeObject(activeQuestionListNo));
                        intent.PutExtra("RateUsQuestionYes", JsonConvert.SerializeObject(activeQuestionListYes));
                        StartActivity(intent);
                    })
                    .Build().Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowThankYouFeedbackTooltips()
        {
            try
            {
                SetupFeedBackFragment.Create(this, SetupFeedBackFragment.ToolTipType.IMAGE_HEADER)
                    .SetCTALabel(Utility.GetLocalizedLabel("PushNotificationDetails", "feedback2SuccessButton"))
                    .SetTitle(Utility.GetLocalizedLabel("PushNotificationDetails", "feedback2SuccessTitle"))
                    .Build().Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

    }
}
