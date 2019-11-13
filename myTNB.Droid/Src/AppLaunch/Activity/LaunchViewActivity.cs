﻿using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Firebase.Iid;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.MVP;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Login.Activity;
using myTNB_Android.Src.Maintenance.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Notifications.Activity;
using myTNB_Android.Src.PreLogin.Activity;
using myTNB_Android.Src.ResetPassword.Activity;
using myTNB_Android.Src.UpdateMobileNo.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.WalkThrough;
using Refit;
using System;
using System.Collections.Generic;
using System.Runtime;
using myTNB_Android.Src.Maintenance.Activity;
using Android.Text;
using myTNB.SitecoreCMS.Model;
using System.Threading;
using System.Globalization;
using Android.Graphics.Drawables;
using myTNB_Android.Src.NewWalkthrough.MVP;
using myTNB_Android.Src.Base;

namespace myTNB_Android.Src.AppLaunch.Activity
{
    [Activity(Label = "@string/app_name"
        , NoHistory = true
        , MainLauncher = true
              , Icon = "@drawable/ic_launcher"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Launch")]
    public class LaunchViewActivity : BaseAppCompatActivity, AppLaunchContract.IView
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

        private MasterDataResponse cacheResponse = null;

        private AppLaunchNavigation currentNavigation = AppLaunchNavigation.Nothing;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (Intent != null && Intent.Extras != null)
                {
                    if (Intent.Extras.ContainsKey("Type"))
                    {
                        string notifType = Intent.Extras.GetString("Type");
                        UserSessions.SetHasNotification(PreferenceManager.GetDefaultSharedPreferences(this));
                        UserSessions.SaveNotificationType(PreferenceManager.GetDefaultSharedPreferences(this), notifType);
                    }

                    if (Intent.Extras.ContainsKey("Email"))
                    {
                        string email = Intent.Extras.GetString("Email");
                        UserSessions.SetHasNotification(PreferenceManager.GetDefaultSharedPreferences(this));
                        UserSessions.SaveUserEmailNotification(PreferenceManager.GetDefaultSharedPreferences(this), email);
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
                    mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.app_launch_http_exception_error), Snackbar.LengthIndefinite)
                    .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
                    {

                        mApiExcecptionSnackBar.Dismiss();
                        hasBeenCalled = false;
                        this.userActionsListener.Start();
                    }
                    );
                    mApiExcecptionSnackBar.Show();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Snackbar mUnknownExceptionSnackBar;
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
                    mUnknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.app_launch_unknown_exception_error), Snackbar.LengthIndefinite)
                    .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
                    {

                        mUnknownExceptionSnackBar.Dismiss();
                        hasBeenCalled = false;
                        this.userActionsListener.Start();
                    }
                    );
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
                    .PositiveText(GetString(Resource.String.app_launch_device_not_supported_btn_close))
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
                    Log.Debug(TAG, "InstanceID token: " + FirebaseInstanceId.Instance.Token);
                    if (FirebaseTokenEntity.HasLatest())
                    {
                        var tokenEntity = FirebaseTokenEntity.GetLatest();
                        if (tokenEntity != null)
                        {
                            Log.Debug(TAG, "Refresh token: " + tokenEntity.FBToken);
                        }
                    }

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
                    ShowMaintenance(cacheResponse);
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
                RunOnUiThread(() => StartActivity(typeof(WalkThroughActivity)));
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
        private Snackbar mSnackBar;
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

        public void ShowMaintenance(MasterDataResponse masterDataResponse)
        {
            try
            {
                cacheResponse = masterDataResponse;
                if (isAppLaunchSiteCoreDone && isAppLaunchLoadSuccessful && !isAppLaunchDone)
                {
                    isAppLaunchDone = true;
                    Intent maintenanceScreen = new Intent(this, typeof(MaintenanceActivity));
                    maintenanceScreen.PutExtra(Constants.MAINTENANCE_TITLE_KEY, masterDataResponse.Data.DisplayTitle);
                    maintenanceScreen.PutExtra(Constants.MAINTENANCE_MESSAGE_KEY, masterDataResponse.Data.DisplayMessage);
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


        private Snackbar mNoInternetSnackbar;
        public void ShowNoInternetSnackbar()
        {
            if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
            {
                mNoInternetSnackbar.Dismiss();
            }

            mNoInternetSnackbar = Snackbar.Make(rootView, GetString(Resource.String.no_internet_connection), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.dashboard_chartview_data_not_available_no_internet_btn_close), delegate
            {

                mNoInternetSnackbar.Dismiss();
            }
            );
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
                            this.Window.SetBackgroundDrawable(GetDrawable(Resource.Drawable.launch_screen));
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
                                        secondMilli = (int) (float.Parse(item.ShowForSeconds, CultureInfo.InvariantCulture.NumberFormat) * 1000);
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
                                        this.Window.SetBackgroundDrawable(bitmapDrawable);
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

            string msg = "Sorry, something went wrong. Please try again later.";

            mSomethingWrongExceptionSnackBar = Snackbar.Make(rootView, msg, Snackbar.LengthIndefinite)
            .SetAction("Ok", delegate
            {
                mSomethingWrongExceptionSnackBar.Dismiss();
            }
            );
            mSomethingWrongExceptionSnackBar.Show();
        }
    }
}
