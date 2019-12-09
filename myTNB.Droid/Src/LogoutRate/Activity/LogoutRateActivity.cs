using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.LogoutEnd.Activity;
using myTNB_Android.Src.LogoutRate.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Refit;
using System;
using System.Runtime;

namespace myTNB_Android.Src.LogoutRate.Activity
{
    [Activity(Label = "@string/logout_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Logout")]
    public class LogoutRateActivity : BaseAppCompatActivity, LogoutRateContract.IView
    {
        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;


        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        //[BindView(Resource.Id.txtContentInfo)]
        //TextView txtContentInfo;

        [BindView(Resource.Id.btnSubmit)]
        Button btnSubmit;

        LogoutRateContract.IUserActionsListener userActionsListener;
        LogoutRatePresenter mPresenter;

        MaterialDialog progress;

        private LoadingOverlay loadingOverlay;

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.LogoutRateView;
        }

        public void SetPresenter(LogoutRateContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowLogoutSuccess()
        {
            Intent LogoutIntent = new Intent(this, typeof(LogoutEndActivity));
            LogoutIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(LogoutIntent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                progress = new MaterialDialog.Builder(this)
                .Title(GetString(Resource.String.logout_rate_progress_title))
                .Content(GetString(Resource.String.logout_rate_progress_content))
                .Progress(true, 0)
                .Cancelable(false)
                .Build();


                //TextViewUtils.SetMuseoSans300Typeface(txtContentInfo);
                TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo, btnSubmit);
                // Create your application here

                mPresenter = new LogoutRatePresenter(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnSubmit)]
        void OnSubmit(object sender, EventArgs eventArgs)
        {
            // TODO : ADD START ACTIVITY FOR LOGOUT END
            this.userActionsListener.OnLogout(this.DeviceId());
        }

        [OnClick(Resource.Id.btnClose)]
        void OnClose(object sender, EventArgs eventArgs)
        {
            Finish();
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Log Out Rating");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.logout_rate_cancelled_exception_btn_close), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();
            }
            );
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
            .SetAction(GetString(Resource.String.logout_rate_api_exception_btn_close), delegate
            {

                mApiExcecptionSnackBar.Dismiss();
            }
            );
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
            .SetAction(GetString(Resource.String.logout_rate_unknown_exception_btn_close), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            mUknownExceptionSnackBar.Show();

        }

        public void ShowProgressDialog()
        {
            //if (progress != null && !progress.IsShowing)
            //{
            //    progress.Show();
            //}
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            //if (progress != null && progress.IsShowing)
            //{
            //    progress.Dismiss();
            //}
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowErrorMessage(string message)
        {
            Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
                        .SetAction(GetString(Resource.String.logout_rate_btn_close),
                         (view) =>
                         {

                             // EMPTY WILL CLOSE SNACKBAR
                         }
                        ).Show();
        }

        public override bool TelephonyPermissionRequired()
        {
            return false;
        }

        public override void Ready()
        {
            base.Ready();
            this.userActionsListener.Start();
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