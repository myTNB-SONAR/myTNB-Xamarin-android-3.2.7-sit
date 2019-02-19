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
using myTNB_Android.Src.Base.Activity;
using Android.Content.PM;
using CheeseBind;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.LogoutRate.MVP;
using myTNB_Android.Src.LogoutEnd.Activity;
using Android.Support.Design.Widget;
using Refit;
using AFollestad.MaterialDialogs;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;

namespace myTNB_Android.Src.LogoutRate.Activity
{
    [Activity(Label = "@string/logout_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Logout")]
    public class LogoutRateActivity : BaseAppCompatActivity , LogoutRateContract.IView
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

            progress = new MaterialDialog.Builder(this)
            .Title(GetString(Resource.String.logout_rate_progress_title))
            .Content(GetString(Resource.String.logout_rate_progress_content))
            .Progress(true, 0)
            .Cancelable(false)
            .Build();


            //TextViewUtils.SetMuseoSans300Typeface(txtContentInfo);
            TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo , btnSubmit);
            // Create your application here

            mPresenter = new LogoutRatePresenter(this);
            
        }

        [OnClick(Resource.Id.btnSubmit)]
        void OnSubmit(object sender , EventArgs eventArgs)
        {
            // TODO : ADD START ACTIVITY FOR LOGOUT END
            this.userActionsListener.OnLogout(this.DeviceId());
        }

        [OnClick(Resource.Id.btnClose)]
        void OnClose(object sender, EventArgs eventArgs)
        {
            Finish();
        }


        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.logout_rate_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.logout_rate_cancelled_exception_btn_close), delegate {

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

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.logout_rate_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.logout_rate_api_exception_btn_close), delegate {

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

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.logout_rate_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.logout_rate_unknown_exception_btn_close), delegate {

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
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }

            loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
            loadingOverlay.Show();
        }

        public void HideProgressDialog()
        {
            //if (progress != null && progress.IsShowing)
            //{
            //    progress.Dismiss();
            //}
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
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
    }
}