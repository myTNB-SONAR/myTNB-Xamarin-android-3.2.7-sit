using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using myTNB_Android.Src.Base.Activity;
using Android;
using myTNB_Android.Src.AppLaunch.MVP;
using myTNB_Android.Src.AppLaunch.Models;
using Android.Util;
using myTNB_Android.Src.WalkThrough;
using Refit;
using Android.Support.Design.Widget;
using CheeseBind;
using myTNB_Android.Src.myTNBMenu.Activity;
using Android.Preferences;
using myTNB_Android.Src.PreLogin.Activity;
using myTNB_Android.Src.ResetPassword.Activity;
using myTNB_Android.Src.Utils;
using Firebase.Iid;
using myTNB_Android.Src.Database.Model;
using Android.Gms.Common;
using AFollestad.MaterialDialogs;
using myTNB_Android.Src.Notifications.Activity;
using Android.Support.V4.Content;
using myTNB_Android.Src.LogoutEnd.Activity;
using myTNB_Android.Src.Login.Activity;
using myTNB_Android.Src.UpdateMobileNo.Activity;
using System.Runtime;

namespace myTNB_Android.Src.AppLaunch.Activity
{
    [Activity(Label = "@string/app_name"
        , NoHistory = true
        , MainLauncher = true
              , Icon = "@drawable/ic_launcher"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Launch")]
    public class LaunchViewActivity : BaseAppCompatActivity , AppLaunchContract.IView
    {
        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.txt_app_version)]
        TextView txt_app_version;

        public static readonly string TAG = typeof(LaunchViewActivity).Name;
        private AppLaunchPresenter mPresenter;
        private AppLaunchContract.IUserActionsListener userActionsListener;

        private string savedTimeStamp = "0000000";

        bool hasBeenCalled = false;

        MaterialDialog appUpdateDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                mPresenter = new AppLaunchPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));

                if (Intent != null && Intent.Extras != null && Intent.Extras.ContainsKey("Email"))
                {
                    string email = Intent.Extras.GetString("Email");
                    UserSessions.SetHasNotification(PreferenceManager.GetDefaultSharedPreferences(this));
                    UserSessions.SaveUserEmailNotification(PreferenceManager.GetDefaultSharedPreferences(this), email);
                }



                Log.Debug(TAG, "InstanceID token: " + FirebaseInstanceId.Instance.Token);
                if (FirebaseTokenEntity.HasLatest())
                {
                    var tokenEntity = FirebaseTokenEntity.GetLatest();
                    if (tokenEntity != null)
                    {
                        Log.Debug(TAG, "Refresh token: " + tokenEntity.FBToken);
                    }
                }

                TextViewUtils.SetMuseoSans300Typeface(txt_app_version);
                //try
                //{

                //    PackageInfo info = this.PackageManager.GetPackageInfo("com.mytnb.mytnb", Android.Content.PM.PackageInfoFlags.Activities);
                //    if (info != null)
                //    {
                //        txt_app_version.Text = GetString(Resource.String.text_app_version) +" "+ info.VersionName;
                //    }
                //}
                //catch (System.Exception e)
                //{
                //    Log.Debug("Package Manager", e.StackTrace);
                //    txt_app_version.Visibility = ViewStates.Gone;
                //}
                //Hide version number text from splash 
                txt_app_version.Visibility = ViewStates.Gone;

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
                TextViewUtils.SetMuseoSans300Typeface(txtDialogMessage);
                TextViewUtils.SetMuseoSans500Typeface(txtDialogTitle, btnUpdateNow);
                btnUpdateNow.Click += delegate
                {
                    OnAppUpdateClick();
                };
            } catch(Exception e) {
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
                Log.Debug(TAG,"Account Type = " +  accountTypeList[i].ToString());
            }
        }

        public void ShowWalkThrough()
        {
            userActionsListener.GetSavedTimeStamp();
            //RunOnUiThread(() => StartActivity(typeof(WalkThroughActivity)));
            
        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionApiException(ApiException apiException)
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
                .SetAction(GetString(Resource.String.app_launch_http_exception_btn_retry), delegate {

                    mApiExcecptionSnackBar.Dismiss();
                    hasBeenCalled = false;
                    this.userActionsListener.Start();
                }
                );
                mApiExcecptionSnackBar.Show();
            }
        }

        private Snackbar mUnknownExceptionSnackBar;
        public void ShowRetryOptionUknownException(Exception unkownException)
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
                .SetAction(GetString(Resource.String.app_launch_unknown_exception_btn_retry), delegate {

                    mUnknownExceptionSnackBar.Dismiss();
                    hasBeenCalled = false;
                    this.userActionsListener.Start();
                }
                );
                mUnknownExceptionSnackBar.Show();
            }
        }

        public void ShowDashboard()
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
        }

        public void ShowPreLogin()
        {
            Intent PreLoginIntent = new Intent(this, typeof(PreLoginActivity));
            PreLoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(PreLoginIntent);
        }

        public void ShowResetPassword()
        {
            Intent ResetPasswordIntent = new Intent(this, typeof(ResetPasswordActivity));
            ResetPasswordIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            ResetPasswordIntent.PutExtra(Constants.FROM_ACTIVITY, LaunchViewActivity.TAG);
            StartActivity(ResetPasswordIntent);
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }


        public void ShowDeviceNotSupported()
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

        public async void ShowPlayServicesIsAvailable()
        {
            await GoogleApiAvailability.Instance.MakeGooglePlayServicesAvailableAsync(this);
        }

        public int PlayServicesResultCode()
        {
            return GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
        }

        public void ShowPlayServicesErrorDialog(int resultCode)
        {
            GoogleApiAvailability.Instance.GetErrorDialog(this , resultCode , Constants.PLAY_SERVICES_RESOLUTION_REQUEST).Show();
            hasBeenCalled = true;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            this.userActionsListener.OnActivityResult(requestCode , resultCode , data);
            
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
            if (!hasBeenCalled)
            {
                userActionsListener.Start();
                hasBeenCalled = true;
            }

        }

        public void ShowNotification()
        {
            Intent notificationIntent = new Intent(this, typeof(NotificationActivity));
            notificationIntent.PutExtra(Constants.HAS_NOTIFICATION, true);
            StartActivity(notificationIntent);
        }

        public void ShowNotificationCount(int count)
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

        public void OnSavedTimeStampRecievd(string timestamp)
        {
            if (timestamp != null)
            {
                savedTimeStamp = timestamp;
            }
            this.userActionsListener.OnGetTimeStamp();
        }

        public void OnTimeStampRecieved(string timestamp)
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

        public void OnSiteCoreServiceFailed(string message)
        {
            
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            this.userActionsListener.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void RequestSMSPermission()
        {
            RequestPermissions(new string[] { Manifest.Permission.ReceiveSms }, Constants.RUNTIME_PERMISSION_SMS_REQUEST_CODE);
        }
        private Snackbar mSnackBar;
        public void ShowSMSPermissionRationale()
        {
            if (mSnackBar != null && mSnackBar.IsShown)
            {
                mSnackBar.Dismiss();
                mSnackBar.Show();
            }
            else
            {
                mSnackBar = Snackbar.Make(rootView, GetString(Resource.String.runtime_permission_sms_received_rationale), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.runtime_permission_dialog_btn_show), delegate {
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
            ME.Leolin.Shortcutbadger.ShortcutBadger.RemoveCount(this.ApplicationContext);
            Intent logout = new Intent(this, typeof(LoginActivity));
            StartActivity(logout);
        }

        public void ShowUpdateAvailable()
        {
            if(appUpdateDialog != null)
            {
                appUpdateDialog.Show();
            }
        }

        public void OnAppUpdateClick()
        {
            WeblinkEntity weblinkEntity = WeblinkEntity.GetByCode("DROID");
            if (weblinkEntity != null)
            {
                var uri = Android.Net.Uri.Parse(weblinkEntity.Url);
                Intent intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            }
        }

        public void ShowUpdatePhoneNumber(string phoneNumber)
        {
            Intent updateMobileNo = new Intent(this, typeof(UpdateMobileActivity));
            //updateMobileNo.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            updateMobileNo.PutExtra(Constants.FORCE_UPDATE_PHONE_NO, true);
            updateMobileNo.PutExtra(Constants.FROM_APP_LAUNCH, true);
            updateMobileNo.PutExtra("PhoneNumber", phoneNumber);
            StartActivity(updateMobileNo);
        }


        public override void OnTrimMemory(TrimMemory level)
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
    }
}