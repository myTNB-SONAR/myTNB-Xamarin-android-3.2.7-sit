using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.MVP;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Maintenance.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Notifications.Activity;
using myTNB_Android.Src.PreLogin.Activity;
using myTNB_Android.Src.ResetPassword.Activity;
using myTNB_Android.Src.UpdateMobileNo.Activity;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Runtime;
using Android.Text;
using myTNB.SitecoreCMS.Model;
using System.Globalization;
using Android.Graphics.Drawables;
using myTNB_Android.Src.NewWalkthrough.MVP;
using myTNB_Android.Src.MyTNBService.Response;
using Firebase.DynamicLinks;
using Google.Android.Material.Snackbar;
using AndroidX.Core.Content;
using myTNB.Mobile;
using myTNB;
using myTNB.Mobile.SessionCache;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP;
using Newtonsoft.Json;
using Firebase.Iid;

namespace myTNB_Android.Src.AppLaunch.Activity
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

        private AppLaunchMasterDataResponse cacheResponseData = null;

        private string urlSchemaData = "";
        private string urlSchemaPath = "";
        private Snackbar mSnackBar;
        private Snackbar mNoInternetSnackbar;
        private Snackbar mUnknownExceptionSnackBar;

        private AppLaunchNavigation currentNavigation = AppLaunchNavigation.Nothing;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Utility.SetAppUpdateId(this);
            LanguageUtil.SetInitialAppLanguage();
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

            try
            {
                if (Intent != null && Intent.Extras != null)
                {
                    if (Intent.Extras.ContainsKey("Type"))
                    {
                        string notifType = Intent.Extras.GetString("Type");
                        UserSessions.SaveNotificationType(PreferenceManager.GetDefaultSharedPreferences(this), notifType);
                        if (notifType.ToUpper() == ApplicationStatusNotificationModel.TYPE_APPLICATIONDETAILS
                            && Intent.Extras.ContainsKey("SaveApplicationID")
                            && Intent.Extras.ContainsKey("ApplicationID")
                            && Intent.Extras.ContainsKey("ApplicationType"))
                        {
                            string saveID = Intent.Extras.GetString("SaveApplicationID");
                            string applicationID = Intent.Extras.GetString("ApplicationID");
                            string applicationType = Intent.Extras.GetString("ApplicationType");
                            UserSessions.SetApplicationStatusNotification(saveID, applicationID, applicationType);
                        }
                        else
                        {
                            UserSessions.SetHasNotification(PreferenceManager.GetDefaultSharedPreferences(this));
                        }
                    }

                    if (Intent.Extras.ContainsKey("Email"))
                    {
                        string email = Intent.Extras.GetString("Email");
                        UserSessions.SaveUserEmailNotification(PreferenceManager.GetDefaultSharedPreferences(this), email);
                        if (!"APPLICATIONSTATUS".Equals(UserSessions.GetNotificationType(PreferenceManager.GetDefaultSharedPreferences(this)).ToUpper()))
                        {
                            UserSessions.SetHasNotification(PreferenceManager.GetDefaultSharedPreferences(this));
                        }
                    }

                    // Get CategoryBrowsable intent data
                    var data = Intent?.Data?.EncodedAuthority;
                    if (!string.IsNullOrEmpty(data))
                    {
                        urlSchemaData = data;
                        urlSchemaPath = Intent?.Data?.EncodedPath;
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
                if (UserSessions.HasUpdateSkipped(PreferenceManager.GetDefaultSharedPreferences(this)))
                {
                    Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
                    DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    if (!string.IsNullOrEmpty(urlSchemaData))
                    {
                        DashboardIntent.PutExtra("urlSchemaData", urlSchemaData);
                        if (!string.IsNullOrEmpty(urlSchemaPath))
                        {
                            DashboardIntent.PutExtra("urlSchemaPath", urlSchemaPath);
                        }
                    }
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
                       , notificationObj.ApplicationType);

                if (detailResponse.StatusDetail.IsSuccess)
                {
                    Intent applicationStatusDetailIntent = new Intent(this, typeof(ApplicationStatusDetailActivity));
                    applicationStatusDetailIntent.PutExtra("applicationStatusResponse", JsonConvert.SerializeObject(detailResponse.Content));
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

        public void ShowResetPassword()
        {
            if (isAppLaunchSiteCoreDone && isAppLaunchLoadSuccessful && !isAppLaunchDone)
            {
                isAppLaunchDone = true;
                Intent ResetPasswordIntent = new Intent(this, typeof(ResetPasswordActivity));
                ResetPasswordIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                ResetPasswordIntent.PutExtra(Constants.FROM_ACTIVITY, LaunchViewActivity.TAG);
                StartActivity(ResetPasswordIntent);
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
            if (isAppLaunchSiteCoreDone && isAppLaunchLoadSuccessful && !isAppLaunchDone)
            {
                isAppLaunchDone = true;
                Intent notificationIntent = new Intent(this, typeof(NotificationActivity));
                notificationIntent.PutExtra(Constants.HAS_NOTIFICATION, true);
                StartActivity(notificationIntent);
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
                    OnAppUpdateClick();
                };

                if (IsActive())
                    appUpdateDialog.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

        public void ShowMaintenance(AppLaunchMasterDataResponse masterDataResponse)
        {
            try
            {
                cacheResponseData = masterDataResponse;
                if (isAppLaunchSiteCoreDone && isAppLaunchLoadSuccessful && !isAppLaunchDone)
                {
                    isAppLaunchDone = true;
                    Intent maintenanceScreen = new Intent(this, typeof(MaintenanceActivity));
                    maintenanceScreen.PutExtra(Constants.MAINTENANCE_TITLE_KEY, masterDataResponse.Response.DisplayTitle);
                    maintenanceScreen.PutExtra(Constants.MAINTENANCE_MESSAGE_KEY, masterDataResponse.Response.DisplayMessage);
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
                        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                        GC.Collect();
                        break;
                    default:
                        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
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

            Android.Net.Uri deepLink = null;
            if (pendingResult != null)
            {
                deepLink = pendingResult.Link;
                string deepLinkUrl = deepLink.ToString();
                if (!string.IsNullOrEmpty(deepLinkUrl))
                {
                    if (deepLinkUrl.Contains("rewards"))
                    {
                        urlSchemaData = "rewards";
                        string id = deepLinkUrl.Substring(deepLinkUrl.LastIndexOf("=") + 1);
                        urlSchemaPath = "rewardId=" + id;
                    }
                    else if (deepLinkUrl.Contains("whatsnew"))
                    {
                        urlSchemaData = "whatsnew";
                        string id = deepLinkUrl.Substring(deepLinkUrl.LastIndexOf("=") + 1);
                        urlSchemaPath = "whatsNewId=" + id;
                    }
                    else if (deepLinkUrl.Contains("applicationListing"))
                    {
                        urlSchemaData = "applicationListing";
                    }
                    else if (deepLinkUrl.Contains("applicationDetails"))
                    {
                        urlSchemaData = "applicationDetails";
                        ApplicationDetailsDeeplinkCache.Instance.SetData(deepLinkUrl);
                    }
                }
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
    }
}