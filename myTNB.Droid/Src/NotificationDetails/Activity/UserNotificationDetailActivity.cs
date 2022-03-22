using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Net.Http;
using Android.OS;
using Android.Preferences;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using CheeseBind;
using DynatraceAndroid;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Billing.MVP;
using myTNB_Android.Src.Bills.AccountStatement;
using myTNB_Android.Src.Bills.AccountStatement.Activity;
using myTNB_Android.Src.Bills.NewBillRedesign;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.EnergyBudgetRating.Activity;
using myTNB_Android.Src.EnergyBudgetRating.Fargment;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.ManageAccess.Activity;
using myTNB_Android.Src.ManageSupplyAccount.Activity;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.NotificationDetails.Models;
using myTNB_Android.Src.NotificationDetails.MVP;
using myTNB_Android.Src.Notifications.Models;
using myTNB_Android.Src.Rating.Model;
using myTNB_Android.Src.RewardDetail.MVP;
using myTNB_Android.Src.SSMR.SubmitMeterReading.MVP;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.ViewReceipt.Activity;
using myTNB_Android.Src.WhatsNewDetail.MVP;
using Newtonsoft.Json;
using Refit;

using Constant = myTNB_Android.Src.Utils.LinkRedirection.LinkRedirection.Constants;
using Screen = myTNB_Android.Src.Utils.LinkRedirection.LinkRedirection.ScreenEnum;

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

        [BindView(Resource.Id.webViewNoti)]
        WebView webView;

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

        public void ShowUpateApp()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                if (WeblinkEntity.HasRecord("DROID"))
                {
                    WeblinkEntity entity = WeblinkEntity.GetByCode("DROID");
                    try
                    {
                        string[] array = entity.Url.Split(new[] { "?id=" }, StringSplitOptions.None);

                        if (array.Length > 1)
                        {
                            string id = array[1];
                            var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=" + id));
                            // we need to add this, because the activity is in a new context.
                            // Otherwise the runtime will block the execution and throw an exception
                            intent.AddFlags(ActivityFlags.NewTask);

                            Application.Context.StartActivity(intent);
                        }
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                        var uri = Android.Net.Uri.Parse(entity.Url);
                        var intent = new Intent(Intent.ActionView, uri);
                        StartActivity(intent);
                    }
                }
            }
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
                SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.PUSH_NOTIF_DETAILS, LanguageConstants.PushNotificationDetails.NOTIF_TITLE_DEFAULT));
                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM))
                    {
                        notificationDetails = DeSerialze<NotificationDetails.Models.NotificationDetails>(extras.GetString(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM));

                        if (!string.IsNullOrEmpty(notificationDetails.HeaderTitle))
                        {
                            SetToolBarTitle(notificationDetails.HeaderTitle);
                        }
                    }

                    if (extras.ContainsKey(Constants.SELECTED_FROMDASHBOARD_NOTIFICATION_DETAIL_ITEM))
                    {
                        Notification = DeSerialze<myTNB.Mobile.NotificationOpenDirectDetails>(extras.GetString(Constants.SELECTED_FROMDASHBOARD_NOTIFICATION_DETAIL_ITEM));
                        pushFromDashboard = true;
                    }

                    if (extras.ContainsKey(Constants.SELECTED_NOTIFICATION_LIST_ITEM))
                    {
                        userNotificationData = DeSerialze<UserNotificationData>(extras.GetString(Constants.SELECTED_NOTIFICATION_LIST_ITEM));
                    }

                    position = extras.GetInt(Constants.SELECTED_NOTIFICATION_ITEM_POSITION);
                }
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);
                TextViewUtils.SetMuseoSans500Typeface(notificationDetailTitle);
                TextViewUtils.SetMuseoSans300Typeface(notificationDetailMessage);
                TextViewUtils.SetTextSize14(notificationDetailMessage);
                TextViewUtils.SetTextSize16(notificationDetailTitle);

                if (notificationDetails != null && notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_SMR_DISABLED_SUCCESS_ID)
                {
                    notificationMainLayout.SetBackgroundColor(Color.ParseColor("#ffffff"));
                }

                if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_80
                    || notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_100
                    || notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_TC
                    || notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_ENERGY_BUDGET_RC)
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.PUSH_NOTIF_DETAILS, LanguageConstants.PushNotificationDetails.NOTIF_TITLE_ENERGY_BUDGET));
                }
                else if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_SERVICE_DISTRUPT_OUTAGE
                    || notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_SERVICE_DISTRUPT_INPROGRESS
                    || notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_SERVICE_DISTRUPT_RESTORATION)
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.PUSH_NOTIF_DETAILS, LanguageConstants.PushNotificationDetails.NOTIF_TITLE_SRVC_DISTRUPTION));
                }
                else if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_SERVICE_DISTRUPT_UPDATE_NOW)
                {
                    SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.PUSH_NOTIF_DETAILS, LanguageConstants.PushNotificationDetails.NOTIF_TITLE_DEFAULT));
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

                    string dynatraceTag = string.Empty;
                    if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_APP_UPDATE ||
                        notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_APP_UPDATE_2 ||
                        notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_APP_UPDATE_OT)
                    {
                        dynatraceTag = myTNB.Mobile.DynatraceConstants.BR.CTAs.Notifications.Update;
                    }
                    else if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_DBR_EBILL
                       || notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_DBR_EMAIL_REMOVED)
                    {
                        dynatraceTag = myTNB.Mobile.DynatraceConstants.BR.CTAs.Notifications.Combined_Comms_In_App_Non_Owner;
                    }

                    if (detailModel.message != null)
                    {
                        webView.Visibility = ViewStates.Visible;
                        notificationDetailMessage.Visibility = ViewStates.Gone;
                        webView.HorizontalScrollBarEnabled = false;

                        webView.SetWebChromeClient(new WebChromeClient());
                        webView.Settings.SetPluginState(WebSettings.PluginState.On);
                        webView.SetWebViewClient(new MyTNBWebViewClients(this, string.Empty, string.Empty));
                        WebSettings websettings = webView.Settings;
                        websettings.JavaScriptEnabled = true;
                        websettings.LoadWithOverviewMode = true;
                        websettings.AllowFileAccess = true;
                        websettings.AllowContentAccess = true;

                        if (notificationDetails != null && notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_SMR_DISABLED_SUCCESS_ID)
                        {
                            notificationMainLayout.SetBackgroundColor(Color.ParseColor("#ffffff"));
                            webView.SetBackgroundColor(Color.ParseColor("#ffffff"));
                        }
                        else
                        {
                            webView.SetBackgroundColor(Color.ParseColor("#F9F9F9"));
                        }
                        webView.LoadDataWithBaseURL("file:///android_asset",
                        getHtmlData(this, detailModel.message),
                        "text/html; charset=UTF-8", null, "about:blank");

                        //notificationDetailMessage = LinkRedirectionUtils
                        //.Create(this, string.Empty)
                        //.SetTextView(notificationDetailMessage)
                        //.SetMessage(detailModel.message)
                        //.Build(string.Empty)
                        //.GetProcessedTextView();
                    }

                    if (detailModel.ctaList.Count > 0)
                    {
                        ctaComponent.Visibility = ViewStates.Visible;
                        ctaComponent.SetCTAButton(detailModel.ctaList);

                        if (notificationDetails != null)
                        {
                            if (notificationDetails.BCRMNotificationTypeId == Constants.BCRM_NOTIFICATION_SERVICE_DISTRUPT_UPDATE_NOW)
                            {
                                ctaComponent.Visibility = ViewStates.Visible;
                                ctaComponent.SetCustomCTAButton(detailModel.ctaList);
                            }
                        }
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

        private string getHtmlData(Context context, string data)
        {
            string[] whatsnewid;
            string newWord = "";
            string newWordImg = "";
            if (data.Contains("<a"))
            {
                whatsnewid = data.Split("<a");
                int arraylist = 1;
                foreach (var item in whatsnewid)
                {
                    if (whatsnewid.Length > arraylist)
                    {
                        arraylist++;
                        newWord = newWord + item + "<a style=\"text-decoration:none; color:#1c79ca; font-weight: bold\" ";
                    }
                    else
                    {
                        newWord = newWord + item;
                    }
                }
            }
            else
            {
                newWord = data;
            }

            if (newWord.Contains("<img"))
            {

                whatsnewid = newWord.Split("<img");
                int arraylist = 1;
                foreach (var item in whatsnewid)
                {
                    if (whatsnewid.Length > arraylist)
                    {
                        arraylist++;
                        newWordImg = newWordImg + item + "<img style=\"max-width: 100%\" ";
                    }
                    else
                    {
                        newWordImg = newWordImg + item;
                    }
                }
                newWord = newWordImg;
            }

            try
            {
                string fontSize = TextViewUtils.IsLargeFonts ? "font-size: 18px;" : "font-size: 14px;";
                string htmlData = "<html>" + "<head>" + "<style type=\"text/css\">" + "@font-face {" + "font-family: MyFont;" + "src: url(\"file:///android_asset/fonts/MuseoSans_300.otf\")" + "}" +
                    "body {" + "font-family: MyFont;" + fontSize + "text-align: left;" + "color:#49494a;" + "}," + "</style>" + "</head>" + "<body>" + newWord + "</body>" + "</html>";
                return htmlData;
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
                return data;
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

        public void ViewManageAccess(AccountData mSelectedAccountData)
        {
            Intent manageAccount = new Intent(this, typeof(ManageSupplyAccountActivityEdit));
            manageAccount.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
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

        public void ViewAccountStatement(AccountData mSelectedAccountData, string statementPeriod)
        {
            Intent acctStmntLoadingIntent = new Intent(this, typeof(AccountStatementLoadingActivity));
            acctStmntLoadingIntent.PutExtra(AccountStatementConstants.STATEMENT_PERIOD, statementPeriod);
            acctStmntLoadingIntent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(mSelectedAccountData));
            StartActivity(acctStmntLoadingIntent);
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

        public class MyTNBWebViewClients : WebViewClient
        {
            private UserNotificationDetailActivity mActivity;
            private ProgressBar progressBar;
            private string mHeaderTitle;
            private Screen TargetScreen = Screen.None;
            private RewardServiceImpl mApi;
            bool shouldOverride = false;

            public static List<string> RedirectTypeList = new List<string> {
            "inAppBrowser=",
            "externalBrowser=",
            "tel=",
            "whatsnew=",
            "faq=",
            "reward=",
            "http",
            "tel:",
            "whatsnewid=",
            "faqid=",
            "rewardid=",
            "inAppScreen="
            };

            public MyTNBWebViewClients(UserNotificationDetailActivity mActivity, string title, string dynatrace)
            {
                this.mActivity = mActivity;
                this.mHeaderTitle = title;
            }

            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                if (ConnectionUtils.HasInternetConnection(mActivity))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(url))
                        {
                            //for:
                            //"inAppBrowser="
                            //"externalBrowser="
                            //"http"
                            if (url.Contains(RedirectTypeList[0])
                                || url.Contains(RedirectTypeList[1])
                                || url.Contains(RedirectTypeList[6]))
                            {
                                string uri = url;
                                if (url.Contains(RedirectTypeList[0]))
                                {
                                    uri = url.Split(RedirectTypeList[0])[1];
                                }
                                else if (url.Contains(RedirectTypeList[1]))
                                {
                                    uri = url.Split(RedirectTypeList[1])[1];
                                }

                                string compareText = uri.ToLower();

                                if (!compareText.Contains("http"))
                                {
                                    uri = "http://" + uri;
                                }

                                //External Browser
                                if (url.Contains(RedirectTypeList[1]))
                                {
                                    Intent intent = new Intent(Intent.ActionView);
                                    intent.SetData(Android.Net.Uri.Parse(uri));
                                    mActivity.StartActivity(intent);
                                }
                                //In App Browser
                                else
                                {
                                    if (compareText.Contains(".pdf") && !compareText.Contains("docs.google"))
                                    {
                                        Intent webIntent = new Intent(mActivity, typeof(BasePDFViewerActivity));
                                        webIntent.PutExtra(Constants.IN_APP_LINK, uri);
                                        webIntent.PutExtra(Constants.IN_APP_TITLE, this.mHeaderTitle);
                                        mActivity.StartActivity(webIntent);
                                    }
                                    else if (compareText.Contains(".jpeg") || compareText.Contains(".jpg") || compareText.Contains(".png"))
                                    {
                                        Intent webIntent = new Intent(mActivity, typeof(BaseFullScreenImageViewActivity));
                                        webIntent.PutExtra(Constants.IN_APP_LINK, uri);
                                        webIntent.PutExtra(Constants.IN_APP_TITLE, this.mHeaderTitle);
                                        mActivity.StartActivity(webIntent);
                                    }
                                    else
                                    {
                                        Intent webIntent = new Intent(mActivity, typeof(BaseWebviewActivity));
                                        webIntent.PutExtra(Constants.IN_APP_LINK, uri);
                                        webIntent.PutExtra(Constants.IN_APP_TITLE, this.mHeaderTitle);
                                        mActivity.StartActivity(webIntent);
                                    }
                                }
                            }
                            //for:
                            //"tel="
                            //"tel:"
                            else if (url.Contains(RedirectTypeList[2])
                                        || url.Contains(RedirectTypeList[7]))
                            {
                                string phonenum = url;
                                if (url.Contains(RedirectTypeList[2]))
                                {
                                    phonenum = url.Split(RedirectTypeList[2])[1];
                                }
                                if (!string.IsNullOrEmpty(phonenum))
                                {
                                    if (!phonenum.Contains("tel:"))
                                    {
                                        phonenum = "tel:" + phonenum;
                                    }

                                    var call = Android.Net.Uri.Parse(phonenum);
                                    var callIntent = new Intent(Intent.ActionView, call);
                                    mActivity.StartActivity(callIntent);
                                }
                            }
                            //for:
                            //"whatsnew="
                            //"whatsnewid="
                            else if (url.Contains(RedirectTypeList[3])
                                        || url.Contains(RedirectTypeList[8]))
                            {
                                string whatsnewid = url;
                                if (url.Contains(RedirectTypeList[3]))
                                {
                                    whatsnewid = url.Split(RedirectTypeList[3])[1];
                                }
                                else if (url.Contains(RedirectTypeList[8]))
                                {
                                    whatsnewid = url.Split(RedirectTypeList[8])[1];
                                }

                                if (!string.IsNullOrEmpty(whatsnewid))
                                {
                                    if (!whatsnewid.Contains("{"))
                                    {
                                        whatsnewid = "{" + whatsnewid;
                                    }

                                    if (!whatsnewid.Contains("}"))
                                    {
                                        whatsnewid = whatsnewid + "}";
                                    }

                                    WhatsNewEntity wtManager = new WhatsNewEntity();

                                    WhatsNewEntity item = wtManager.GetItem(whatsnewid);

                                    if (item != null)
                                    {
                                        if (!item.Read)
                                        {
                                            UpdateWhatsNewRead(item.ID, true);
                                        }

                                        Intent activity = new Intent(mActivity, typeof(WhatsNewDetailActivity));
                                        activity.PutExtra(Constants.WHATS_NEW_DETAIL_ITEM_KEY, whatsnewid);
                                        activity.PutExtra(Constants.WHATS_NEW_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("Tabbar", "promotion"));
                                        mActivity.StartActivity(activity);
                                    }
                                }
                            }
                            //for:
                            //"faq="
                            //"faqid="
                            else if (url.Contains(RedirectTypeList[4])
                                        || url.Contains(RedirectTypeList[9]))
                            {
                                string faqid = url;
                                if (url.Contains(RedirectTypeList[4]))
                                {
                                    faqid = url.Split(RedirectTypeList[4])[1];
                                }
                                else if (url.Contains(RedirectTypeList[9]))
                                {
                                    faqid = url.Split(RedirectTypeList[9])[1];
                                }

                                if (!string.IsNullOrEmpty(faqid))
                                {
                                    if (!faqid.Contains("{"))
                                    {
                                        faqid = "{" + faqid;
                                    }

                                    if (!faqid.Contains("}"))
                                    {
                                        faqid = faqid + "}";
                                    }

                                    if (faqid.Contains("\""))
                                    {
                                        faqid = faqid.Replace("\"", string.Empty);
                                    }

                                    if (faqid.Contains("\\"))
                                    {
                                        faqid = faqid.Replace("\\", string.Empty);
                                    }

                                    Intent faqIntent = new Intent(mActivity, typeof(FAQListActivity));
                                    faqIntent.PutExtra(Constants.FAQ_ID_PARAM, faqid);
                                    mActivity.StartActivity(faqIntent);
                                }
                            }
                            //for:
                            //"reward="
                            //"rewardid="
                            else if (url.Contains(RedirectTypeList[5])
                                        || url.Contains(RedirectTypeList[10]))
                            {
                                string rewardid = url;
                                if (url.Contains(RedirectTypeList[5]))
                                {
                                    rewardid = url.Split(RedirectTypeList[5])[1];
                                }
                                else if (url.Contains(RedirectTypeList[10]))
                                {
                                    rewardid = url.Split(RedirectTypeList[10])[1];
                                }

                                if (!string.IsNullOrEmpty(rewardid))
                                {
                                    if (!rewardid.Contains("{"))
                                    {
                                        rewardid = "{" + rewardid;
                                    }

                                    if (!rewardid.Contains("}"))
                                    {
                                        rewardid = rewardid + "}";
                                    }

                                    RewardsEntity wtManager = new RewardsEntity();

                                    RewardsEntity item = wtManager.GetItem(rewardid);

                                    if (item != null)
                                    {
                                        if (!item.Read)
                                        {
                                            UpdateRewardRead(item.ID, true);
                                        }

                                        Intent activity = new Intent(mActivity, typeof(RewardDetailActivity));
                                        activity.PutExtra(Constants.REWARD_DETAIL_ITEM_KEY, rewardid);
                                        activity.PutExtra(Constants.REWARD_DETAIL_TITLE_KEY, Utility.GetLocalizedLabel("Tabbar", "rewards"));
                                        mActivity.StartActivity(activity);
                                    }
                                }
                            }
                            //for:
                            //"inAppScreen="
                            else if (url.Contains(RedirectTypeList[11]))
                            {
                                var targetScreen = GetTargetInAppScreen(url);
                                if (targetScreen.Contains(Screen.NewBillDesignComms.ToString()))
                                {
                                    TargetScreen = Screen.NewBillDesignComms;
                                }
                                NavigateToTargetScreen(TargetScreen);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                    shouldOverride = true;
                }
                return shouldOverride;
            }

            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
            }

            public override void OnPageFinished(WebView view, string url)
            {
            }

            public override bool OnRenderProcessGone(WebView view, RenderProcessGoneDetail detail)
            {
                return true;
            }

            public override void OnReceivedError(WebView view, ClientError errorCode, string description, string failingUrl)
            {
                try
                {
                    string message = "Please check your internet connection.";
                    if (ConnectionUtils.HasInternetConnection(mActivity))
                    {
                        switch (errorCode)
                        {
                            case ClientError.FileNotFound:
                                message = "File Not Found."; break;
                            case ClientError.Authentication:
                                message = "Authetication Error."; break;
                            case ClientError.FailedSslHandshake:
                                message = "SSL Handshake Failed."; break;
                            case ClientError.Unknown:
                                message = "Unkown Error."; break;
                        }
                        ShowErrorMessage(failingUrl);
                    }
                    else
                    {
                        ShowErrorMessage(failingUrl);
                    }
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            public override void OnReceivedSslError(WebView view, SslErrorHandler handler, SslError error)
            {
                handler.Proceed();
            }

            public override void OnLoadResource(WebView view, string url)
            {
                if (!ConnectionUtils.HasInternetConnection(mActivity))
                {
                    view.StopLoading();
                }
            }

            private void UpdateWhatsNewRead(string itemID, bool flag)
            {
                try
                {
                    DateTime currentDate = DateTime.UtcNow;
                    WhatsNewEntity wtManager = new WhatsNewEntity();
                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                    string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
                    if (!flag)
                    {
                        formattedDate = "";

                    }
                    wtManager.UpdateReadItem(itemID, flag, formattedDate);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            private void UpdateRewardRead(string itemID, bool flag)
            {
                try
                {
                    DateTime currentDate = DateTime.UtcNow;
                    RewardsEntity wtManager = new RewardsEntity();
                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                    string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
                    if (!flag)
                    {
                        formattedDate = "";

                    }
                    wtManager.UpdateReadItem(itemID, flag, formattedDate);

                    _ = OnUpdateReward(itemID);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            private async Task OnUpdateReward(string itemID)
            {
                try
                {
                    // Update api calling
                    RewardsEntity wtManager = new RewardsEntity();
                    RewardsEntity currentItem = wtManager.GetItem(itemID);

                    UserInterface currentUsrInf = new UserInterface()
                    {
                        eid = UserEntity.GetActive().Email,
                        sspuid = UserEntity.GetActive().UserID,
                        did = UserEntity.GetActive().DeviceId,
                        ft = FirebaseTokenEntity.GetLatest().FBToken,
                        lang = LanguageUtil.GetAppLanguage().ToUpper(),
                        sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                        sec_auth_k2 = "",
                        ses_param1 = "",
                        ses_param2 = ""
                    };

                    string rewardId = currentItem.ID;
                    rewardId = rewardId.Replace("{", "");
                    rewardId = rewardId.Replace("}", "");

                    AddUpdateRewardModel currentReward = new AddUpdateRewardModel()
                    {
                        Email = UserEntity.GetActive().Email,
                        RewardId = rewardId,
                        Read = currentItem.Read,
                        ReadDate = !string.IsNullOrEmpty(currentItem.ReadDateTime) ? currentItem.ReadDateTime + " +00:00" : "",
                        Favourite = currentItem.IsSaved,
                        FavUpdatedDate = !string.IsNullOrEmpty(currentItem.IsSavedDateTime) ? currentItem.IsSavedDateTime + " +00:00" : "",
                        Redeemed = currentItem.IsUsed,
                        RedeemedDate = !string.IsNullOrEmpty(currentItem.IsUsedDateTime) ? currentItem.IsUsedDateTime + " +00:00" : ""
                    };

                    AddUpdateRewardRequest request = new AddUpdateRewardRequest()
                    {
                        usrInf = currentUsrInf,
                        reward = currentReward
                    };

                    AddUpdateRewardResponse response = await this.mApi.AddUpdateReward(request, new System.Threading.CancellationTokenSource().Token);

                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }


            private string GetTargetInAppScreen(string path)
            {
                string value = string.Empty;
                string pattern = string.Format(Constant.Pattern, Constant.InAppScreenKey);
                Regex regex = new Regex(pattern);
                Match match = regex.Match(path);
                if (match.Success)
                {
                    value = match.Value.Replace(string.Format(Constant.ReplaceKey, Constant.InAppScreenKey), string.Empty);
                }

                return value;
            }

            private void NavigateToTargetScreen(Screen targetScreen)
            {
                switch (targetScreen)
                {
                    case Screen.NewBillDesignComms:
                        {
                            Intent nbrDiscoverMoreIntent = new Intent(mActivity, typeof(NBRDiscoverMoreActivity));
                            mActivity.StartActivityForResult(nbrDiscoverMoreIntent, Constants.NEW_BILL_REDESIGN_REQUEST_CODE);
                        }
                        break;
                    default:
                        break;
                }
            }

            private Snackbar mErrorMessageSnackBar;
            public void ShowErrorMessage(string failingUrl)
            {
                //if (mErrorMessageSnackBar != null && mErrorMessageSnackBar.IsShown)
                //{
                //    mErrorMessageSnackBar.Dismiss();
                //}

                //mErrorMessageSnackBar = Snackbar.Make(Context
                //    , Utility.GetLocalizedErrorLabel("noDataConnectionMessage")
                //    , Snackbar.LengthIndefinite)
                //    .SetAction(Utility.GetLocalizedLabel("Common", "tryAgain")
                //        , delegate
                //        {
                //            mErrorMessageSnackBar.Dismiss();
                //        });
                //View v = mErrorMessageSnackBar.View;
                //TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                //tv.SetMaxLines(5);
                //mErrorMessageSnackBar.Show();
            }

            //  TODO: AndroidX Temporary Fix for Android 5,5.1 
            //  TODO: AndroidX Due to this: https://github.com/xamarin/AndroidX/issues/131
            //public override AssetManager Assets =>
            //    (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop && Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.M)
            //    ? Resources.Assets : base.Assets;
        }

    }
}
