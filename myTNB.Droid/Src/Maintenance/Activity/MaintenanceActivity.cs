using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;

using Android.Views;
using Android.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.AppLaunch.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Maintenance.MVP;
using myTNB_Android.Src.Utils;
using Refit;
using System;

namespace myTNB_Android.Src.Maintenance.Activity
{
    [Activity(Label = "@string/app_name"
       , NoHistory = true
              , Icon = "@drawable/ic_launcher"
       , LaunchMode = LaunchMode.SingleInstance
       , ScreenOrientation = ScreenOrientation.Portrait
              , Theme = "@style/Theme.Dashboard")]
    public class MaintenanceActivity : BaseAppCompatActivity, MaintenanceContract.IView
    {
        [BindView(Resource.Id.rootView)]
        RelativeLayout rootView;

        [BindView(Resource.Id.maintenance_heading)]
        TextView txtHeading;

        [BindView(Resource.Id.maintenance_content)]
        TextView txtContent;

        [BindView(Resource.Id.maintenance_image)]
        ImageView imgMaintenance;

        public static readonly string TAG = typeof(MaintenanceActivity).Name;
        private MaintenancePresenter mPresenter;
        private MaintenanceContract.IUserActionsListener userActionsListener;

        bool hasBeenCalled = false;

        bool firstInitiated = false;

        private string title = "";

        private string message = "";

        public override int ResourceId()
        {
            return Resource.Layout.MaintenanceView;
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);
            try
            {
                TextViewUtils.SetMuseoSans300Typeface(txtContent);
                TextViewUtils.SetMuseoSans500Typeface(txtHeading);
                TextViewUtils.SetTextSize16(txtContent);
                TextViewUtils.SetTextSize24(txtHeading);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            if (Intent != null && Intent.Extras != null && Intent.Extras.ContainsKey(Constants.MAINTENANCE_TITLE_KEY) && Intent.Extras.ContainsKey(Constants.MAINTENANCE_MESSAGE_KEY))
            {
                try
                {
                    title = Intent.Extras.GetString(Constants.MAINTENANCE_TITLE_KEY);
                    message = Intent.Extras.GetString(Constants.MAINTENANCE_MESSAGE_KEY);
                    txtHeading.Text = title;
                    txtContent.Text = message;
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            else
            {
                try
                {
                    txtHeading.Text = "";
                    txtContent.Text = "";
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            try
            {
                mPresenter = new MaintenancePresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));

                if (!hasBeenCalled)
                {
                    userActionsListener.Start();
                    hasBeenCalled = true;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();


        }

        public void OnUpdateMaintenanceWord(string mTitle, string mMessage)
        {
            try
            {
                if (!string.IsNullOrEmpty(mTitle))
                {
                    title = mTitle;
                    txtHeading.Text = title;
                }

                if (!string.IsNullOrEmpty(mMessage))
                {
                    message = mMessage;
                    txtContent.Text = message;
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
            this.userActionsListener.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "App Under Maintenance");
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

        public void SetPresenter(MaintenanceContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowLaunchViewActivity()
        {
            Intent LaunchViewIntent = new Intent(this, typeof(LaunchViewActivity));
            LaunchViewActivity.MAKE_INITIAL_CALL = true;
            LaunchViewIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(LaunchViewIntent);
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

        private Snackbar mNoInternetSnackbar;
        public void ShowNoInternetSnackbar()
        {
            if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
            {
                mNoInternetSnackbar.Dismiss();
            }

            mNoInternetSnackbar = Snackbar.Make(rootView, GetString(Resource.String.no_internet_connection), Snackbar.LengthIndefinite)
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
    }
}
