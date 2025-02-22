﻿using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Firebase.DynamicLinks;
using Firebase.Iid;
using Google.Android.Material.Snackbar;
using myTNB.AndroidApp.Src.AppLaunch.Models;
using myTNB.AndroidApp.Src.AppLaunch.MVP;
using myTNB.AndroidApp.Src.ApplicationStatus.ApplicationStatusDetail.MVP;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.DeviceCache;
using myTNB.AndroidApp.Src.Login.Activity;
using myTNB.AndroidApp.Src.Maintenance.Activity;
using myTNB.AndroidApp.Src.ManageBillDelivery.MVP;
using myTNB.AndroidApp.Src.myTNBMenu.Activity;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.NewWalkthrough.MVP;
using myTNB.AndroidApp.Src.NotificationDetails.Activity;
using myTNB.AndroidApp.Src.Notifications.Activity;
using myTNB.AndroidApp.Src.OverVoltageFeedback.Activity;
using myTNB.AndroidApp.Src.PreLogin.Activity;
using myTNB.AndroidApp.Src.UpdateMobileNo.Activity;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.Utils.Deeplink;
using myTNB.AndroidApp.Src.Utils.Notification;
using myTNB.Mobile;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.SessionCache;
using myTNB.SitecoreCMS.Model;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace myTNB.AndroidApp.Src.AppLaunch.Activity
{
    [Activity(Label = "@string/app_name"
        , NoHistory = true
        , MainLauncher = true
              , Icon = "@drawable/ic_launcher"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Launch")]
    [IntentFilter(new[] { Android.Content.Intent.ActionView },
            DataScheme = "mytnbapp",
            Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable })]
    public class LaunchViewActivity : BaseAppCompatActivity, AppLaunchContract.IView, Android.Gms.Tasks.IOnSuccessListener, Android.Gms.Tasks.IOnFailureListener
    {
        [BindView(Resource.Id.rootView)]
        RelativeLayout rootView;
        public static bool FcmPushNotificationFlagFromBackground;

        public static readonly string TAG = typeof(LaunchViewActivity).Name;
        private AppLaunchPresenter mPresenter;
        private AppLaunchContract.IUserActionsListener userActionsListener;

        private string savedTimeStamp = "0000000";

        private string savedAppLaunchTimeStamp = "0000000";

        bool hasBeenCalled = false;

        MaterialDialog appUpdateDialog;

        public static bool MAKE_INITIAL_CALL = true;

        private bool isAppLaunchSiteCoreDone = false;
        private bool isAppLaunchLoadSuccessful = false;
        private bool isAppLaunchDone = false;
        private bool userClick = false;

        private AppLaunchMasterDataResponseAWS cacheResponseData = null;

        string ClaimId = "";

        private string urlSchemaData = "";
        private string urlSchemaPath = "";
        private string userID = "";
        private Snackbar mSnackBar;
        private Snackbar mNoInternetSnackbar;
        private Snackbar mUnknownExceptionSnackBar;

        private AppLaunchNavigation currentNavigation = AppLaunchNavigation.Nothing;
        public static string DynatraceSessionUUID;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //UUID number for dynatrace webview navigation	
            Guid myuuid = Guid.NewGuid();
            DynatraceSessionUUID = myuuid.ToString();

            Utility.SetAppUpdateId(this);
            LanguageUtil.SetInitialAppLanguage();

            try
            {
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)                                          //Starting android 13 asking notification permission
                {
                    if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications) == (int)Permission.Granted)
                    {
                        UserSessions.SaveUserNotificationFirstTimeInstallFlag(PreferenceManager.GetDefaultSharedPreferences(this), true);
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }

            try
            {
                FirebaseDynamicLinks.Instance
                    .GetDynamicLink(Intent)
                    .AddOnSuccessListener(this, this)
                    .AddOnFailureListener(this, this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
#pragma warning disable CS0618 // Type or member is obsolete
            try
            {
                if (Intent != null && Intent.Extras != null)
                {
                    if (UserEntity.IsCurrentlyActive())
                    {
                        NotificationUtil.Instance.SaveData(Intent.Extras);
                    }

                    if (!NotificationUtil.Instance.PushMapId.IsValid())
                    {
                        if (Intent.Extras.ContainsKey("Type"))
                        {
                            string notifType = Intent.Extras.GetString("Type");
                            UserSessions.SaveNotificationType(PreferenceManager.GetDefaultSharedPreferences(this), notifType);
                            if (notifType.ToUpper() == MobileConstants.PushNotificationTypes.DBR_Owner)
                            {
                                string accountNumber = Intent.Extras.GetString("AccountNumber");
                                UserSessions.DBROwnerNotificationAccountNumber = accountNumber ?? string.Empty;
                            }
                            else
                            {
                                UserSessions.SetHasNotification(PreferenceManager.GetDefaultSharedPreferences(this));
                            }
                        }

                        if (Intent.Extras.ContainsKey("Type") && Intent.Extras.ContainsKey("RequestTransId") && Intent.Extras.ContainsKey("EventId"))
                        {
                            string type = Intent.Extras.GetString(NotificationOpenDirectDetails.TYPE);
                            string requestTransID = Intent.Extras.GetString(NotificationOpenDirectDetails.PARAM_REQUESTTRANSID);
                            string eventID = Intent.Extras.GetString(NotificationOpenDirectDetails.Param_EVENTID);
                            UserSessions.SetNotification(type, requestTransID, eventID);
                        }

                        if (Intent.Extras.ContainsKey("Email"))
                        {
                            string email = Intent.Extras.GetString("Email");
                            UserSessions.SaveUserEmailNotification(PreferenceManager.GetDefaultSharedPreferences(this), email);
                            if (PreferenceManager.GetDefaultSharedPreferences(this) != null)
                            {
                                if (UserSessions.GetNotificationType(PreferenceManager.GetDefaultSharedPreferences(this))?.ToUpper() != null)
                                {
                                    if (!"APPLICATIONSTATUS".Equals(UserSessions.GetNotificationType(PreferenceManager.GetDefaultSharedPreferences(this)).ToUpper()))
                                    {
                                        UserSessions.SetHasNotification(PreferenceManager.GetDefaultSharedPreferences(this));
                                    }
                                }
                                else if (Intent.Extras.ContainsKey("AccountNum"))
                                {
                                    UserSessions.SetHasNotification(PreferenceManager.GetDefaultSharedPreferences(this));
                                }
                            }
                        }

                        if (Intent.Extras.ContainsKey("claimId"))
                        {
                            ClaimId = Intent.Extras.GetString("claimId");
                            currentNavigation = AppLaunchNavigation.Notification;
                        }
                    }
                    else
                    {
                        UserSessions.SetHasNotification(PreferenceManager.GetDefaultSharedPreferences(this));

                        if (Intent.Extras.ContainsKey("Type"))
                        {
                            string notifType = Intent.Extras.GetString("Type");
                            UserSessions.SaveNotificationType(PreferenceManager.GetDefaultSharedPreferences(this), notifType);
                        }

                        if (Intent.Extras.ContainsKey("Email"))
                        {
                            string email = Intent.Extras.GetString("Email");
                            UserSessions.SaveUserEmailNotification(PreferenceManager.GetDefaultSharedPreferences(this), email);
                        }
                    }
                }
                UserSessions.SetUploadFileNameCounter(PreferenceManager.GetDefaultSharedPreferences(this), 1);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }


        public override int ResourceId()
        {
            return Resource.Layout.LaunchView;
        }

        public void SetPresenter(AppLaunchContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        /// <summary>
        /// Testing purposes to show account type listing
        /// </summary>
        /// <param name="accountTypeList"></param>
        public void ShowAccountTypes(List<AccountType> accountTypeList)
        {
            for (int i = 0; i < accountTypeList.Count; i++)
            {
                Log.Debug(TAG, "Account Type = " + accountTypeList[i].ToString());
            }
        }

        public void ShowWalkThrough()
        {
            try
            {
                if (isAppLaunchSiteCoreDone && isAppLaunchLoadSuccessful && !isAppLaunchDone)
                {
                    isAppLaunchDone = true;
                    Intent WalkthroughIntent = new Intent(this, typeof(NewWalkthroughActivity));
                    WalkthroughIntent.PutExtra(Constants.APP_NAVIGATION_KEY, AppLaunchNavigation.Walkthrough.ToString());
                    StartActivity(WalkthroughIntent);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionApiException(ApiException apiException)
        {
            try
            {
                // TODO : PROVIDE EXCEPTION DESCRIPTION
                // TODO : SHOW SNACKBAR ERROR MESSAGE
                if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
                {
                    mApiExcecptionSnackBar.Dismiss();
                    mApiExcecptionSnackBar.Show();
                }
                else
                {
                    mApiExcecptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                    .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
                    {

                        mApiExcecptionSnackBar.Dismiss();
                        hasBeenCalled = false;
                        this.userActionsListener.Start();
                    }
                    );
                    View v = mApiExcecptionSnackBar.View;
                    TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                    tv.SetMaxLines(5);
                    mApiExcecptionSnackBar.Show();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowRetryOptionUknownException(Exception unkownException)
        {
            try
            {
                // TODO : PROVIDE EXCEPTION DESCRIPTION
                // TODO : SHOW SNACKBAR ERROR MESSAGE
                if (mUnknownExceptionSnackBar != null && mUnknownExceptionSnackBar.IsShown)
                {
                    mUnknownExceptionSnackBar.Dismiss();
                    mUnknownExceptionSnackBar.Show();
                }
                else
                {
                    mUnknownExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                    .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
                    {

                        mUnknownExceptionSnackBar.Dismiss();
                        hasBeenCalled = false;
                        this.userActionsListener.Start();
                    }
                    );
                    View v = mUnknownExceptionSnackBar.View;
                    TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                    tv.SetMaxLines(5);
                    mUnknownExceptionSnackBar.Show();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowDashboard()
        {
            if (isAppLaunchSiteCoreDone && isAppLaunchLoadSuccessful && !isAppLaunchDone)
            {
                isAppLaunchDone = true;
                if (UserSessions.HasUpdateSkipped(PreferenceManager.GetDefaultSharedPreferences(this)) || (!MyTNBAccountManagement.GetInstance().IsWalkthroughAppUpdate()))
                {
                    Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
                    DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(DashboardIntent);
                }
                else
                {
                    Intent WalkthroughIntent = new Intent(this, typeof(NewWalkthroughActivity));
                    WalkthroughIntent.PutExtra(Constants.APP_NAVIGATION_KEY, AppLaunchNavigation.Dashboard.ToString());
                    StartActivity(WalkthroughIntent);
                }
            }
        }

        public void ShowExceptionDashboard()
        {
            MyTNBAccountManagement.GetInstance().SetFromLoginPage(true);
            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);

        }

        public async void ShowApplicationStatusDetails()
        {
            SearchApplicationTypeResponse searchApplicationTypeResponse = SearchApplicationTypeCache.Instance.GetData();
            if (searchApplicationTypeResponse == null)
            {
                searchApplicationTypeResponse = await ApplicationStatusManager.Instance.SearchApplicationType("16", UserEntity.GetActive() != null);
                if (searchApplicationTypeResponse != null
                    && searchApplicationTypeResponse.StatusDetail != null
                    && searchApplicationTypeResponse.StatusDetail.IsSuccess)
                {
                    SearchApplicationTypeCache.Instance.SetData(searchApplicationTypeResponse);
                }
            }

            if (searchApplicationTypeResponse != null
                && searchApplicationTypeResponse.StatusDetail != null
                && searchApplicationTypeResponse.StatusDetail.IsSuccess)
            {
                ApplicationStatusNotificationModel notificationObj = UserSessions.ApplicationStatusNotification;
                ApplicationDetailDisplay detailResponse = await ApplicationStatusManager.Instance.GetApplicationDetail(notificationObj.SaveApplicationID
                    , notificationObj.ApplicationID
                    , notificationObj.ApplicationType
                    , UserEntity.GetActive().UserID ?? string.Empty
                    , UserEntity.GetActive().Email ?? string.Empty
                    , notificationObj.System);

                if (detailResponse.StatusDetail.IsSuccess)
                {
                    Intent applicationStatusDetailIntent = new Intent(this, typeof(ApplicationStatusDetailActivity));
                    applicationStatusDetailIntent.PutExtra("applicationStatusResponse", JsonConvert.SerializeObject(detailResponse.Content));
                    applicationStatusDetailIntent.PutExtra("isPush", true);
                    StartActivity(applicationStatusDetailIntent);
                }
                else
                {
                    ShowDashboard();
                }
            }
            else
            {
                ShowDashboard();
            }
        }

        public async void OnShowManageBillDelivery()
        {
            bool isDBREnabled = DBRUtility.Instance.IsAccountEligible;
            if (!isDBREnabled)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(this);
#pragma warning restore CS0618 // Type or member is obsolete
                if (EligibilityManager.Instance.IsEnabled(AWSConstants.Services.GetEligibility)
                    && preferences.GetString(MobileConstants.SharePreferenceKey.GetEligibilityData, string.Empty) is string encryptedData
                    && !string.IsNullOrEmpty(encryptedData)
                    && !string.IsNullOrWhiteSpace(encryptedData))
                {
                    GetEligibilityResponse data = SecurityManager.Instance.Decrypt<GetEligibilityResponse>(encryptedData);
                    EligibilitySessionCache.Instance.SetData(data);
                    //Use data or any EligibilitySessionCache functionality
                }
                isDBREnabled = DBRUtility.Instance.IsAccountEligible;
            }
            if (!isDBREnabled
                || string.IsNullOrEmpty(UserSessions.DBROwnerNotificationAccountNumber)
                || string.IsNullOrWhiteSpace(UserSessions.DBROwnerNotificationAccountNumber))
            {
                ShowDashboard();
                return;
            }
            try
            {
                ShowProgressDialog();
                if (isDBREnabled
                    && CustomerBillingAccount.List() is List<CustomerBillingAccount> accountList
                    && accountList != null
                    && accountList.Count > 0
                    && accountList.FindIndex(y => y.AccNum == UserSessions.DBROwnerNotificationAccountNumber) is int caIndex
                    && caIndex > -1)
                {
                    if (!AccessTokenCache.Instance.HasTokenSaved(this))
                    {
                        string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                        AccessTokenCache.Instance.SaveAccessToken(this, accessToken);
                    }
                    var isOwner = accountList.Find(x => { return x.AccNum == UserSessions.DBROwnerNotificationAccountNumber; }).isOwned;
                    GetBillRenderingResponse? billRenderingResponse = await DBRManager.Instance.GetBillRendering(UserSessions.DBROwnerNotificationAccountNumber
                        , AccessTokenCache.Instance.GetAccessToken(this), isOwner);
                    if (billRenderingResponse != null
                        && billRenderingResponse.StatusDetail != null
                        && billRenderingResponse.StatusDetail.IsSuccess
                        && billRenderingResponse.Content != null
                        && billRenderingResponse.Content.DBRType != MobileEnums.DBRTypeEnum.None)
                    {
                        //For tenant checking DBR
                        List<string> dBRCAs = DBRUtility.Instance.GetCAList();
                        // PostBREligibilityIndicatorsResponse billRenderingTenantResponse = await DBRManager.Instance.PostBREligibilityIndicators(dBRCAs, UserEntity.GetActive().UserID, AccessTokenCache.Instance.GetAccessToken(this));

                        Intent intent = new Intent(this, typeof(ManageBillDeliveryActivity));
                        intent.PutExtra("billRenderingResponse", JsonConvert.SerializeObject(billRenderingResponse));
                        // intent.PutExtra("billRenderingTenantResponse", JsonConvert.SerializeObject(billRenderingTenantResponse));
                        intent.PutExtra("accountNumber", UserSessions.DBROwnerNotificationAccountNumber);
                        StartActivity(intent);
                    }
                    else
                    {
                        ShowDashboard();
                    }
                }
                else
                {
                    ShowDashboard();
                }
                HideProgressDialog();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
                ShowDashboard();
            }
        }

        public void ShowPreLogin()
        {
            if (isAppLaunchSiteCoreDone && isAppLaunchLoadSuccessful && !isAppLaunchDone)
            {
                isAppLaunchDone = true;
                if (UserSessions.HasUpdateSkipped(PreferenceManager.GetDefaultSharedPreferences(this)))
                {
                    Intent PreLoginIntent = new Intent(this, typeof(PreLoginActivity));
                    PreLoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(PreLoginIntent);
                }
                else
                {
                    Intent WalkthroughIntent = new Intent(this, typeof(NewWalkthroughActivity));
                    WalkthroughIntent.PutExtra(Constants.APP_NAVIGATION_KEY, AppLaunchNavigation.PreLogin.ToString());
                    StartActivity(WalkthroughIntent);
                }
            }
        }

        public void ShowLogin(string urlSchemaData)
        {
            if (isAppLaunchSiteCoreDone && isAppLaunchLoadSuccessful && !isAppLaunchDone)
            {
                isAppLaunchDone = true;
                if (UserSessions.HasUpdateSkipped(PreferenceManager.GetDefaultSharedPreferences(this)))
                {
                    //Intent LoginIntent = new Intent(this, typeof(LoginActivity));
                    //LoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    //StartActivity(LoginIntent);
                    string fromlink = "fromlink";
                    Intent login = new Intent(this, typeof(LoginActivity));
                    login.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    if (!string.IsNullOrEmpty(urlSchemaData))
                    {
                        login.PutExtra(urlSchemaData, true);
                        if (!string.IsNullOrEmpty(urlSchemaPath))
                        {
                            login.PutExtra(urlSchemaData, true);
                            login.PutExtra(fromlink, true);
                            //login.PutExtra("userID", userID);
                            SetResult(Result.Ok, login);
                        }
                    }
                    StartActivity(login);

                }
                else
                {
                    Intent WalkthroughIntent = new Intent(this, typeof(NewWalkthroughActivity));
                    WalkthroughIntent.PutExtra(Constants.APP_NAVIGATION_KEY, AppLaunchNavigation.PreLogin.ToString());
                    StartActivity(WalkthroughIntent);
                }
            }
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        public void ShowDeviceNotSupported()
        {
            try
            {
                new MaterialDialog.Builder(this)
                    .Title(GetString(Resource.String.app_launch_device_not_supported_title))
                    .Content(GetString(Resource.String.app_launch_device_not_supported_content))
                    .Cancelable(false)
                    .PositiveText(Utility.GetLocalizedCommonLabel("close"))
                    .OnPositive(delegate
                    {
                        Finish();
                    })
                    .Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void ShowPlayServicesIsAvailable()
        {
            try
            {
                await GoogleApiAvailability.Instance.MakeGooglePlayServicesAvailableAsync(this);
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
                FirebaseAnalyticsUtils.SetScreenName(this, "App Launch");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public int PlayServicesResultCode()
        {
            return GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
        }

        public void ShowPlayServicesErrorDialog(int resultCode)
        {
            try
            {
                GoogleApiAvailability.Instance.GetErrorDialog(this, resultCode, Constants.PLAY_SERVICES_RESOLUTION_REQUEST).Show();
                hasBeenCalled = true;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                this.userActionsListener.OnActivityResult(requestCode, resultCode, data);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override bool TelephonyPermissionRequired()
        {
            return false;
        }

        /// <summary>
        /// Starts the process(access api) when view is rendered fully and runtime permission is already allowed
        /// </summary>
        public override void Ready()
        {
            base.Ready();
        }

        protected override void OnStart()
        {
            base.OnStart();

            isAppLaunchDone = false;
            isAppLaunchSiteCoreDone = false;
            isAppLaunchLoadSuccessful = false;

            try
            {
                mPresenter = new AppLaunchPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
            }
            catch (Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }

            try
            {
                if (AppLaunchUtils.GetAppLaunch() != null)
                {
                    SetCustomAppLaunchImage(AppLaunchUtils.GetAppLaunch());
                }
                else
                {
                    if (!isAppLaunchSiteCoreDone)
                    {
                        this.userActionsListener.GetSavedAppLaunchTimeStamp();
                    }
                }
            }
            catch (Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }

            try
            {
                currentNavigation = AppLaunchNavigation.Nothing;
                if (ConnectionUtils.HasInternetConnection(this))
                {
#if DEBUG
                    Log.Debug(TAG, "InstanceID token: " + FirebaseInstanceId.Instance.Token);
                    if (FirebaseTokenEntity.HasLatest())
                    {
                        var tokenEntity = FirebaseTokenEntity.GetLatest();
                        if (tokenEntity != null)
                        {
                            Log.Debug(TAG, "Refresh token: " + tokenEntity.FBToken);
                        }
                    }
#endif
                    if (!hasBeenCalled)
                    {
                        userActionsListener.Start();
                        hasBeenCalled = true;
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

        public void ShowNotification()
        {
            if (!string.IsNullOrEmpty(ClaimId))
            {
                FcmPushNotificationFlagFromBackground = true;
                isAppLaunchDone = true;
                Intent Intent = new Intent(this, typeof(OverVoltageFeedbackDetailActivity));
                Intent.AddFlags(ActivityFlags.ClearTop);
                Intent.PutExtra("ClaimId", ClaimId);
                StartActivity(Intent);
            }
            else if (isAppLaunchSiteCoreDone && isAppLaunchLoadSuccessful && !isAppLaunchDone)
            {
                isAppLaunchDone = true;
                Intent notificationIntent = new Intent(this, typeof(NotificationActivity));
                notificationIntent.PutExtra(Constants.HAS_NOTIFICATION, true);
                StartActivity(notificationIntent);
            }
        }

        public void ShowNotificationDetails()
        {
            var usrsession = UserSessions.Notification;
            mPresenter.OnShowNotificationDetails(usrsession.Type, usrsession.EventId, usrsession.RequestTransId);
        }

        public void ShowDetails(NotificationDetails.Models.NotificationDetails details)
        {
            try
            {
                CustomClassAnalytics.SetScreenNameDynaTrace(Constants.EB_in_app_notification);
                FirebaseAnalyticsUtils.SetScreenName(this, Constants.EB_in_app_notification);
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
            isAppLaunchDone = true;
            Intent notificationDetails = new Intent(this, typeof(UserNotificationDetailActivity));
            notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM, JsonConvert.SerializeObject(details));
            notificationDetails.PutExtra(Constants.FROM_APP_LAUNCH, true);
            StartActivityForResult(notificationDetails, Constants.NOTIFICATION_DETAILS_REQUEST_CODE);
        }

        public void ShowProgress()
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

        public void ShowNotificationCount(int count)
        {
            try
            {
                if (count <= 0)
                {
                    ME.Leolin.Shortcutbadger.ShortcutBadger.RemoveCount(this.ApplicationContext);
                }
                else
                {
                    ME.Leolin.Shortcutbadger.ShortcutBadger.ApplyCount(this.ApplicationContext, count);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSavedTimeStampRecievd(string timestamp)
        {
            try
            {
                if (timestamp != null)
                {
                    savedTimeStamp = timestamp;
                }
                this.userActionsListener.OnGetTimeStamp();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSavedAppLaunchTimeStampRecievd(string timestamp)
        {
            try
            {
                if (timestamp != null)
                {
                    savedAppLaunchTimeStamp = timestamp;
                }
                this.userActionsListener.OnGetAppLaunchTimeStamp();
            }
            catch (Exception e)
            {
                this.userActionsListener.OnGetAppLaunchCache();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnGoAppLaunchEvent()
        {
            if (isAppLaunchSiteCoreDone && isAppLaunchLoadSuccessful)
            {
                if (currentNavigation == AppLaunchNavigation.Notification)
                {
                    ShowNotification();
                }
                else if (currentNavigation == AppLaunchNavigation.Logout)
                {
                    ShowLogout();
                }
                else if (currentNavigation == AppLaunchNavigation.Dashboard)
                {
                    ShowDashboard();
                }
                else if (currentNavigation == AppLaunchNavigation.PreLogin)
                {
                    ShowPreLogin();
                }
                else if (currentNavigation == AppLaunchNavigation.Walkthrough)
                {
                    ShowWalkThrough();
                }
                else if (currentNavigation == AppLaunchNavigation.Maintenance)
                {
                    ShowMaintenance(cacheResponseData);
                }
                else if (currentNavigation == AppLaunchNavigation.Login)
                {
                    ShowLogin(urlSchemaData);
                }

            }

        }

        public void OnTimeStampRecieved(string timestamp)
        {
            try
            {
                if (timestamp != null)
                {
                    if (timestamp.Equals(savedTimeStamp))
                    {
                        MyTNBApplication.siteCoreUpdated = false;
                    }
                    else
                    {
                        MyTNBApplication.siteCoreUpdated = true;
                    }
                }
                else
                {
                    MyTNBApplication.siteCoreUpdated = true;
                }
                // RunOnUiThread(() => StartActivity(typeof(WalkThroughActivity)));
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnAppLaunchTimeStampRecieved(string timestamp)
        {
            try
            {
                if (timestamp != null)
                {
                    if (timestamp.Equals(savedAppLaunchTimeStamp))
                    {
                        this.userActionsListener.OnGetAppLaunchCache();
                    }
                    else
                    {
                        this.userActionsListener.OnGetAppLaunchItem();
                    }
                }
                else
                {
                    this.userActionsListener.OnGetAppLaunchCache();
                }
            }
            catch (Exception e)
            {
                this.userActionsListener.OnGetAppLaunchCache();
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSiteCoreServiceFailed(string message)
        {

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                this.userActionsListener.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void RequestSMSPermission()
        {
            try
            {
                RequestPermissions(new string[] { Manifest.Permission.ReceiveSms }, Constants.RUNTIME_PERMISSION_SMS_REQUEST_CODE);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowSMSPermissionRationale()
        {
            try
            {
                if (mSnackBar != null && mSnackBar.IsShown)
                {
                    mSnackBar.Dismiss();
                    mSnackBar.Show();
                }
                else
                {
                    mSnackBar = Snackbar.Make(rootView, GetString(Resource.String.runtime_permission_sms_received_rationale), Snackbar.LengthIndefinite)
                    .SetAction(GetString(Resource.String.runtime_permission_dialog_btn_show), delegate
                    {
                        mSnackBar.Dismiss();
                        hasBeenCalled = false;
                        this.userActionsListener.OnRequestSMSPermission();
                    }
                    );

                    View v = mSnackBar.View;
                    TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                    if (tv != null)
                    {
                        tv.SetMaxLines(5);
                    }
                    mSnackBar.Show();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool IsGrantedSMSReceivePermission()
        {
            return ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReceiveSms) == (int)Permission.Granted;
        }

        public bool ShouldShowSMSReceiveRationale()
        {
            return ShouldShowRequestPermissionRationale(Manifest.Permission.ReceiveSms);
        }

        public void ShowLogout()
        {
            try
            {
                if (isAppLaunchSiteCoreDone && isAppLaunchLoadSuccessful && !isAppLaunchDone)
                {
                    isAppLaunchDone = true;
                    if (UserSessions.HasUpdateSkipped(PreferenceManager.GetDefaultSharedPreferences(this)))
                    {
                        ME.Leolin.Shortcutbadger.ShortcutBadger.RemoveCount(this.ApplicationContext);
                        Intent PreLoginIntent = new Intent(this, typeof(PreLoginActivity));
                        PreLoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                        StartActivity(PreLoginIntent);
                    }
                    else
                    {
                        Intent WalkthroughIntent = new Intent(this, typeof(NewWalkthroughActivity));
                        WalkthroughIntent.PutExtra(Constants.APP_NAVIGATION_KEY, AppLaunchNavigation.Logout.ToString());
                        StartActivity(WalkthroughIntent);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowUpdateAvailable(string title, string message, string btnLabel)
        {
            try
            {
                appUpdateDialog = new MaterialDialog.Builder(this)
                    .CustomView(Resource.Layout.AppUpdateDialog, false)
                    .Cancelable(false)
                    .CanceledOnTouchOutside(false)
                    .Build();

                View dialogView = appUpdateDialog.Window.DecorView;
                dialogView.SetBackgroundResource(Android.Resource.Color.Transparent);

                TextView txtDialogTitle = appUpdateDialog.FindViewById<TextView>(Resource.Id.txtTitle);
                TextView txtDialogMessage = appUpdateDialog.FindViewById<TextView>(Resource.Id.txtMessage);
                TextView btnUpdateNow = appUpdateDialog.FindViewById<TextView>(Resource.Id.txtUpdate);
                txtDialogTitle.Text = title;
                txtDialogMessage.Text = message;
                btnUpdateNow.Text = btnLabel;

                TextViewUtils.SetTextSize14(txtDialogMessage);
                TextViewUtils.SetTextSize16(txtDialogTitle);
                TextViewUtils.SetTextSize18(btnUpdateNow);

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    txtDialogMessage.TextFormatted = Html.FromHtml(message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtDialogMessage.TextFormatted = Html.FromHtml(message);
                }
                TextViewUtils.SetMuseoSans300Typeface(txtDialogMessage);
                TextViewUtils.SetMuseoSans500Typeface(txtDialogTitle, btnUpdateNow);
                btnUpdateNow.Click += delegate
                {
                    var versionNow = new string(DeviceIdUtils.GetAppVersionName().Where(c => !char.IsLetter(c)).ToArray());
                    DynatraceHelper.OnTrack(DynatraceConstants.AppUpdate.Force.ForceUpdate_ClickUpdate);
                    DynatraceHelper.OnTrack(DynatraceConstants.AppUpdate.Force.ForceUpdate_VersionBeforeUpdate + " " + versionNow);

                    OnAppUpdateClick();
                };

                if (IsActive())
                {
                    appUpdateDialog.Show();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowUpdateAvailableWithRequirement(string title, string message, string btnYes, string btnNo)
        {
            try
            {
                appUpdateDialog = new MaterialDialog.Builder(this)
                    .CustomView(Resource.Layout.AppUpdateDialogYesNo, false)
                    .Cancelable(false)
                    .CanceledOnTouchOutside(false)
                    .Build();

                View dialogView = appUpdateDialog.Window.DecorView;
                dialogView.SetBackgroundResource(Android.Resource.Color.Transparent);

                TextView txtDialogTitle = appUpdateDialog.FindViewById<TextView>(Resource.Id.txtTitle);
                TextView txtDialogMessage = appUpdateDialog.FindViewById<TextView>(Resource.Id.txtMessage);
                TextView btnNolabel = appUpdateDialog.FindViewById<TextView>(Resource.Id.txtBtnPrimary);
                TextView btnYeslabel = appUpdateDialog.FindViewById<TextView>(Resource.Id.txtBtnSecondary);
                txtDialogTitle.Text = title;
                txtDialogMessage.Text = message;
                btnNolabel.Text = btnNo;
                btnYeslabel.Text = btnYes;

                TextViewUtils.SetTextSize14(txtDialogMessage);
                TextViewUtils.SetTextSize16(txtDialogTitle);
                TextViewUtils.SetTextSize18(btnNolabel, btnYeslabel);

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    txtDialogMessage.TextFormatted = Html.FromHtml(message, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtDialogMessage.TextFormatted = Html.FromHtml(message);
                }
                TextViewUtils.SetMuseoSans300Typeface(txtDialogMessage);
                TextViewUtils.SetMuseoSans500Typeface(txtDialogTitle, btnYeslabel);
                btnYeslabel.Click += delegate
                {
                    var versionNow = new string(DeviceIdUtils.GetAppVersionName().Where(c => !char.IsLetter(c)).ToArray());
                    DynatraceHelper.OnTrack(DynatraceConstants.AppUpdate.Recommend.RecommendAppUpdate_ClickYes);
                    DynatraceHelper.OnTrack(DynatraceConstants.AppUpdate.Recommend.RecommendAppUpdate_VersionBeforeUpdate + " " + versionNow);
                    userClick = true;
                    OnAppUpdateClick();
                };

                btnNolabel.Click += delegate
                {
                    DynatraceHelper.OnTrack(DynatraceConstants.AppUpdate.Recommend.RecommendAppUpdate_ClickNo);
                    appUpdateDialog.Dismiss();
                    userClick = true;
                    mPresenter.Start();
                };
                if (IsActive())
                {
                    appUpdateDialog.Show();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool UserCancelUpdate()
        {
            return userClick;
        }

        public void OnAppUpdateClick()
        {
            try
            {
                WeblinkEntity weblinkEntity = WeblinkEntity.GetByCode("DROID");
                if (weblinkEntity != null)
                {
                    var uri = Android.Net.Uri.Parse(weblinkEntity.Url);
                    Intent intent = new Intent(Intent.ActionView, uri);
                    StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowUpdatePhoneNumber(string phoneNumber)
        {
            try
            {
                Intent updateMobileNo = new Intent(this, typeof(UpdateMobileActivity));
                updateMobileNo.PutExtra(Constants.FORCE_UPDATE_PHONE_NO, true);
                updateMobileNo.PutExtra(Constants.FROM_APP_LAUNCH, true);
                updateMobileNo.PutExtra("PhoneNumber", phoneNumber);
                StartActivity(updateMobileNo);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowMaintenance(AppLaunchMasterDataResponseAWS masterDataResponse)
        {
            try
            {
                cacheResponseData = masterDataResponse;
                if (isAppLaunchSiteCoreDone && isAppLaunchLoadSuccessful && !isAppLaunchDone)
                {
                    isAppLaunchDone = true;
                    Intent maintenanceScreen = new Intent(this, typeof(MaintenanceActivity));
                    maintenanceScreen.PutExtra(Constants.MAINTENANCE_TITLE_KEY, masterDataResponse.DisplayTitle);
                    maintenanceScreen.PutExtra(Constants.MAINTENANCE_MESSAGE_KEY, masterDataResponse.DisplayMessage);
                    StartActivity(maintenanceScreen);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                base.OnTrimMemory(level);

                switch (level)
                {
                    case TrimMemory.RunningLow:
                        // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                        GC.Collect();
                        break;
                    default:
                        // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                        GC.Collect();
                        break;
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

            mNoInternetSnackbar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mNoInternetSnackbar.Dismiss();
            }
            );
            View v = mNoInternetSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mNoInternetSnackbar.Show();
        }

        public void SetAppLaunchSuccessfulFlag(bool flag, AppLaunchNavigation navigationWay)
        {
            isAppLaunchLoadSuccessful = flag;
            currentNavigation = navigationWay;
        }

        public void SetAppLaunchSiteCoreDoneFlag(bool flag)
        {
            isAppLaunchSiteCoreDone = flag;
        }

        public bool GetAppLaunchSiteCoreDoneFlag()
        {
            return isAppLaunchSiteCoreDone;
        }

        public void SetDefaultAppLaunchImage()
        {
            try
            {
                if (!isAppLaunchSiteCoreDone)
                {
                    try
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                this.Window.SetBackgroundDrawable(GetDrawable(Resource.Drawable.launch_screen));
                            }
                            catch (Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }
                        });
                    }
                    catch (Exception ne)
                    {
                        Utility.LoggingNonFatalError(ne);
                    }

                    this.userActionsListener.OnWaitSplashScreenDisplay(1000);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void RenderAppLaunchImage(AppLaunchModel item)
        {
            try
            {
                int secondMilli = 0;
                try
                {
                    secondMilli = (int)(float.Parse(item.ShowForSeconds, CultureInfo.InvariantCulture.NumberFormat) * 1000);
                }
                catch (Exception nea)
                {
                    Utility.LoggingNonFatalError(nea);
                }

                if (secondMilli == 0)
                {
                    try
                    {
                        secondMilli = Int32.Parse(item.ShowForSeconds) * 1000;
                    }
                    catch (Exception nea)
                    {
                        Utility.LoggingNonFatalError(nea);
                    }
                }

                var bitmapDrawable = new BitmapDrawable(item.ImageBitmap);
                RunOnUiThread(() =>
                {
                    try
                    {
                        this.Window.SetBackgroundDrawable(bitmapDrawable);
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });

                this.userActionsListener.OnWaitSplashScreenDisplay(secondMilli);
            }
            catch (Exception ne)
            {
                SetDefaultAppLaunchImage();
                Utility.LoggingNonFatalError(ne);
            }
        }

        public void SetCustomAppLaunchImage(AppLaunchModel item)
        {
            try
            {
                if (!isAppLaunchSiteCoreDone)
                {
                    try
                    {
                        if (item.ImageBitmap != null)
                        {
                            DateTime startDateTime = DateTime.ParseExact(item.StartDateTime, "yyyyMMddTHHmmss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                            DateTime stopDateTime = DateTime.ParseExact(item.EndDateTime, "yyyyMMddTHHmmss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                            DateTime nowDateTime = DateTime.Now;
                            int startResult = DateTime.Compare(nowDateTime, startDateTime);
                            int endResult = DateTime.Compare(nowDateTime, stopDateTime);
                            if (startResult >= 0 && endResult <= 0)
                            {
                                try
                                {
                                    int secondMilli = 0;
                                    try
                                    {
                                        secondMilli = (int)(float.Parse(item.ShowForSeconds, CultureInfo.InvariantCulture.NumberFormat) * 1000);
                                    }
                                    catch (Exception nea)
                                    {
                                        Utility.LoggingNonFatalError(nea);
                                    }

                                    if (secondMilli == 0)
                                    {
                                        try
                                        {
                                            secondMilli = Int32.Parse(item.ShowForSeconds) * 1000;
                                        }
                                        catch (Exception nea)
                                        {
                                            Utility.LoggingNonFatalError(nea);
                                        }
                                    }

                                    var bitmapDrawable = new BitmapDrawable(item.ImageBitmap);
                                    RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            this.Window.SetBackgroundDrawable(bitmapDrawable);
                                        }
                                        catch (Exception ex)
                                        {
                                            Utility.LoggingNonFatalError(ex);
                                        }
                                    });

                                    this.userActionsListener.OnWaitSplashScreenDisplay(secondMilli);
                                }
                                catch (Exception ne)
                                {
                                    SetDefaultAppLaunchImage();
                                    Utility.LoggingNonFatalError(ne);
                                }
                            }
                            else
                            {
                                SetDefaultAppLaunchImage();
                            }
                        }
                        else
                        {
                            SetDefaultAppLaunchImage();
                        }
                    }
                    catch (Exception ne)
                    {
                        SetDefaultAppLaunchImage();
                        Utility.LoggingNonFatalError(ne);
                    }
                }
            }
            catch (Exception e)
            {
                SetDefaultAppLaunchImage();
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mSomethingWrongExceptionSnackBar;
        public void ShowSomethingWrongException()
        {
            if (mSomethingWrongExceptionSnackBar != null && mSomethingWrongExceptionSnackBar.IsShown)
            {
                mSomethingWrongExceptionSnackBar.Dismiss();

            }

            string msg = Utility.GetLocalizedErrorLabel("defaultErrorMessage");

            mSomethingWrongExceptionSnackBar = Snackbar.Make(rootView, msg, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("ok"), delegate
            {
                mSomethingWrongExceptionSnackBar.Dismiss();
            }
            );
            View snackbarView = mSomethingWrongExceptionSnackBar.View;
            TextView tv = snackbarView.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(3);
            mSomethingWrongExceptionSnackBar.Show();
        }

        void Android.Gms.Tasks.IOnSuccessListener.OnSuccess(Java.Lang.Object result)
        {
            PendingDynamicLinkData pendingResult = result.JavaCast<PendingDynamicLinkData>();

            if (pendingResult != null)
            {
                Android.Net.Uri deepLink = null;
                deepLink = pendingResult.Link;
                string deepLinkUrl = deepLink.ToString();
                if (!string.IsNullOrEmpty(deepLinkUrl))
                {
                    if (deepLinkUrl.Contains("UpdateUserStatusActivate"))
                    {
                        urlSchemaData = "UpdateUserStatusActivate";

                        Regex regex = new Regex("\\bUpdateUserStatusActivate.*\\b");
                        Match match = regex.Match(deepLinkUrl);
                        string value = match.Value.Replace("UpdateUserStatusActivate", "");
                        string userID = value.Replace(@"/", "");
                        bool flag = UserSessions.GetLoginflag(PreferenceManager.GetDefaultSharedPreferences(this));

                        if (flag)
                        {
                            this.userActionsListener.UpdateUserStatusActivate(userID);
                        }
                        else
                        {
                            UserSessions.DoflagDynamicLink(PreferenceManager.GetDefaultSharedPreferences(this));
                            UserSessions.UpdateUserIDEmailVerified(PreferenceManager.GetDefaultSharedPreferences(this));
                            UserSessions.SaveUserIDEmailVerified(PreferenceManager.GetDefaultSharedPreferences(this), userID);
                            SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Login);
                            ShowLogin(urlSchemaData);

                        }
                    }
                    else if (deepLinkUrl.Contains("UpdateUserStatusDeactivate"))
                    {

                        urlSchemaData = "UpdateUserStatusDeactivate";

                        Regex regex = new Regex("\\bUpdateUserStatusDeactivate.*\\b");
                        Match match = regex.Match(deepLinkUrl);
                        string value = match.Value.Replace("UpdateUserStatusDeactivate", "");
                        string userID = value.Replace(@"/", "");
                        bool flag = UserSessions.GetLoginflag(PreferenceManager.GetDefaultSharedPreferences(this));

                        if (flag)
                        {
                            this.userActionsListener.UpdateUserStatusDeactivate(userID);
                        }
                        else
                        {

                            UserSessions.DoflagDynamicLink(PreferenceManager.GetDefaultSharedPreferences(this));
                            UserSessions.UpdateUserIDEmailVerified(PreferenceManager.GetDefaultSharedPreferences(this));
                            UserSessions.SaveUserIDEmailVerified(PreferenceManager.GetDefaultSharedPreferences(this), userID);
                            SetAppLaunchSuccessfulFlag(true, AppLaunchNavigation.Login);
                            ShowLogin(urlSchemaData);
                        }

                    }
                }

                DeeplinkUtil.Instance.InitiateDeepLink(pendingResult.Link);
            }
        }

        void Android.Gms.Tasks.IOnFailureListener.OnFailure(Java.Lang.Exception e)
        {
            Utility.LoggingNonFatalError(e);
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

        //private Snackbar mCancelledExceptionSnackBar;
        //public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        //{
        //    if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
        //    {
        //        mCancelledExceptionSnackBar.Dismiss();
        //    }

        //    mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
        //    .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
        //    {

        //        mCancelledExceptionSnackBar.Dismiss();
        //        string email = txtEmail.Text;
        //        string password = txtPassword.Text;
        //        this.userActionsListener.LoginAsync(email, password, this.DeviceId(), chkRemeberMe.Checked);
        //    }
        //    );
        //    View v = mCancelledExceptionSnackBar.View;
        //    TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
        //    tv.SetMaxLines(5);
        //    mCancelledExceptionSnackBar.Show();

        //    this.SetIsClicked(false);

        //}
        //private Snackbar mApiExcecptionSnackBar;
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
            });
            View v = mApiExcecptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mApiExcecptionSnackBar.Show();
            this.SetIsClicked(false);
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
            this.SetIsClicked(false);
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
            this.SetIsClicked(false);
        }

        public override bool NotificationPemissionRequired()
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)                                          //Starting android 13 asking notification permission
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications) == (int)Permission.Granted)
                {
                    UserSessions.SaveUserNotificationFirstTimeInstallFlag(PreferenceManager.GetDefaultSharedPreferences(this), true);
                    return false;
                }
                else if (!UserSessions.GetUserNotificationFirstTimeInstallFlag(PreferenceManager.GetDefaultSharedPreferences(this)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

}
