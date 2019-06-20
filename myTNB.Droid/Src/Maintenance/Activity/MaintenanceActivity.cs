using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AppLaunch.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Maintenance.MVP;
using myTNB_Android.Src.Utils;
using Refit;

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

            if (Intent != null && Intent.Extras != null && Intent.Extras.ContainsKey(Constants.MAINTENANCE_TITLE_KEY) && Intent.Extras.ContainsKey(Constants.MAINTENANCE_MESSAGE_KEY))
            {
                try
                {
                    string title = Intent.Extras.GetString(Constants.MAINTENANCE_TITLE_KEY);
                    string message = Intent.Extras.GetString(Constants.MAINTENANCE_MESSAGE_KEY);
                    txtHeading.Text = title;
                    txtContent.Text = message;

                    TextViewUtils.SetMuseoSans300Typeface(txtContent);
                    TextViewUtils.SetMuseoSans500Typeface(txtHeading);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }

            try
            {
                if (ConnectionUtils.HasInternetConnection(this))
                {
                    mPresenter = new MaintenancePresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));

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

        protected override void OnStart()
        {
            base.OnStart();


        }

        protected override void OnResume()
        {
            base.OnResume();
            this.userActionsListener.OnResume();
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
                    .SetAction(GetString(Resource.String.app_launch_unknown_exception_btn_retry), delegate {

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

        private Snackbar mNoInternetSnackbar;
        public void ShowNoInternetSnackbar()
        {
            if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
            {
                mNoInternetSnackbar.Dismiss();
            }

            mNoInternetSnackbar = Snackbar.Make(rootView, GetString(Resource.String.no_internet_connection), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.dashboard_chartview_data_not_available_no_internet_btn_close), delegate {

                mNoInternetSnackbar.Dismiss();
            }
            );
            mNoInternetSnackbar.Show();
        }
    }
}