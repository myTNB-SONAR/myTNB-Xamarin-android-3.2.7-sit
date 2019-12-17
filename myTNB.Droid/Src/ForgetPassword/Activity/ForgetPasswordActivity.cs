﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.ForgetPassword.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Refit;
using System;
using System.Runtime;

namespace myTNB_Android.Src.ForgetPassword.Activity
{
    [Activity(Label = "@string/forget_password_activity_title"
              , Icon = "@drawable/ic_launcher"
   , ScreenOrientation = ScreenOrientation.Portrait
   , Theme = "@style/Theme.ForgetPassword")]
    public class ForgetPasswordActivity : BaseActivityCustom, ForgetPasswordContract.IView
    {

        private ForgetPasswordPresenter mPresenter;
        private ForgetPasswordContract.IUserActionsListener userActionsListener;

        private AlertDialog mProgressDialog;
        private AlertDialog mVerificationProgressDialog;


        [BindView(Resource.Id.txtInputLayoutEmail)]
        TextInputLayout txtInputLayoutEmail;

        [BindView(Resource.Id.textInputLayoutVerificationCode)]
        TextInputLayout textInputLayoutVerificationCode;

        [BindView(Resource.Id.txtEmail)]
        EditText txtEmail;

        [BindView(Resource.Id.txtVerificationCode)]
        EditText txtVerificationCode;

        [BindView(Resource.Id.txtEnterEmailTitle)]
        TextView txtEmailTitle;

        [BindView(Resource.Id.txtEmailLinkInfo)]
        TextView txtEmailLinkInfo;

        [BindView(Resource.Id.txtGetACode)]
        TextView txtGetACode;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.btnSubmit)]
        Button btnSubmit;

        private Snackbar mSnackBar;
        const string PAGE_ID = "ForgotPassword";
        private LoadingOverlay loadingOverlay;

        public void ClearErrorMessages()
        {
            txtInputLayoutEmail.Error = null;
            textInputLayoutVerificationCode.Error = null;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.ForgetPasswordView;
        }

        public void SetPresenter(ForgetPasswordContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowEmptyEmailError()
        {
            txtInputLayoutEmail.Error = GetString(Resource.String.forget_password_empty_email_error);
        }

        public void ShowError(string errorMessage)
        {
            if (mSnackBar != null && mSnackBar.IsShown)
            {
                mSnackBar.Dismiss();
                mSnackBar.Show();
            }
            else
            {
                mSnackBar = Snackbar.Make(rootView, errorMessage, Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.forget_password_btn_close), delegate { mSnackBar.Dismiss(); }
                );
                mSnackBar.Show();
            }
            this.SetIsClicked(false);
        }

        public void ShowSuccess(string message)
        {
            //if (mSnackBar != null && mSnackBar.IsShown)
            //{
            //    mSnackBar.Dismiss();
            //    mSnackBar.Show();
            //}
            //else
            //{
            //    mSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            //    .SetAction(GetString(Resource.String.forget_password_btn_close), delegate { mSnackBar.Dismiss(); }
            //    );
            //    View v = mSnackBar.View;
            //    TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            //    tv.SetMaxLines(5);
            //    mSnackBar.Show();
            //}

            Intent nextInent = new Intent(this, typeof(VerificationCodeActivity));
            nextInent.PutExtra("email", txtEmail.Text);
            StartActivity(nextInent);
            this.Finish();

        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Forgot Password");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowInvalidEmailError()
        {
            txtInputLayoutEmail.Error = Utility.GetLocalizedLabel("Error", "invalid_email");
        }

        public void ShowCodeVerifiedSuccess()
        {

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);



            try
            {
                // Create your application here
                mPresenter = new ForgetPasswordPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
                mProgressDialog = new AlertDialog.Builder(this)
                   .SetTitle(GetString(Resource.String.forget_password_progress_dialog_title))
                   .SetMessage(GetString(Resource.String.forget_password_progress_dialog_message))
                   .SetCancelable(false)
                   .Create();

                mVerificationProgressDialog = new AlertDialog.Builder(this)
                   .SetTitle(GetString(Resource.String.forget_password_get_code_progress_dialog_title))
                   .SetMessage(GetString(Resource.String.forget_password_get_code_progress_dialog_message))
                   .SetCancelable(false)
                   .Create();


                TextViewUtils.SetMuseoSans500Typeface(txtEmailTitle);

                TextViewUtils.SetMuseoSans300Typeface(txtEmailLinkInfo, txtEmail, txtVerificationCode, txtGetACode);

                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutEmail, textInputLayoutVerificationCode);

                TextViewUtils.SetMuseoSans500Typeface(btnSubmit);

                txtEmailTitle.Text = GetLabelByLanguage("subTitle");
                txtEmailLinkInfo.Text = GetLabelByLanguage("details");
                txtInputLayoutEmail.Hint = GetLabelCommonByLanguage("email");
                btnSubmit.Text = GetLabelCommonByLanguage("submit");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        //[OnClick(Resource.Id.btnSubmit)]
        //void OnSubmit(object sender, EventArgs eventArgs)
        //{
        //    string email = txtEmail.Text;
        //    string code = txtVerificationCode.Text;
        //    this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, email , email , code);
        //}

        [OnClick(Resource.Id.btnSubmit)]
        void OnGetCode(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                string email = txtEmail.Text;
                this.userActionsListener.GetCode(Constants.APP_CONFIG.API_KEY_ID, email);
            }
        }

        public void EnableSubmitButton()
        {
            btnSubmit.Enabled = true;
        }

        public void DisableSubmitButton()
        {
            btnSubmit.Enabled = false;
        }

        public void ShowProgressDialog()
        {
            try
            {
                if (mProgressDialog != null && !mProgressDialog.IsShowing)
                {
                    mProgressDialog.Show();
                }
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
                if (mProgressDialog != null && mProgressDialog.IsShowing)
                {
                    mProgressDialog.Dismiss();
                }
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
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();
                string email = txtEmail.Text;
                string code = txtVerificationCode.Text;
                this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, email, email, code);
            }
            );
            mCancelledExceptionSnackBar.Show();
            this.SetIsClicked(false);

        }

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();
                string email = txtEmail.Text;
                string code = txtVerificationCode.Text;
                this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, email, email, code);
            }
            );
            mApiExcecptionSnackBar.Show();
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
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();
                string email = txtEmail.Text;
                string code = txtVerificationCode.Text;
                this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, email, email, code);
            }
            );
            mUknownExceptionSnackBar.Show();
            this.SetIsClicked(false);
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowGetCodeProgressDialog()
        {
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

        public void HideGetCodeProgressDialog()
        {
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

        public void ShowEmptyCodeError()
        {
            textInputLayoutVerificationCode.Error = GetString(Resource.String.forget_password_empty_code_error);
        }

        private Snackbar mCodeCancelledExceptionSnackBar;
        public void ShowRetryOptionsCodeCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCodeCancelledExceptionSnackBar != null && mCodeCancelledExceptionSnackBar.IsShown)
            {
                mCodeCancelledExceptionSnackBar.Dismiss();
            }

            mCodeCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mCodeCancelledExceptionSnackBar.Dismiss();
                string email = txtEmail.Text;
                this.userActionsListener.GetCode(Constants.APP_CONFIG.API_KEY_ID, email);
            }
            );
            mCodeCancelledExceptionSnackBar.Show();
            this.SetIsClicked(false);
        }

        private Snackbar mCodeApiExceptionSnackBar;
        public void ShowRetryOptionsCodeApiException(ApiException apiException)
        {
            if (mCodeApiExceptionSnackBar != null && mCodeApiExceptionSnackBar.IsShown)
            {
                mCodeApiExceptionSnackBar.Dismiss();
            }

            mCodeApiExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mCodeApiExceptionSnackBar.Dismiss();
                string email = txtEmail.Text;
                this.userActionsListener.GetCode(Constants.APP_CONFIG.API_KEY_ID, email);
            }
            );
            mCodeApiExceptionSnackBar.Show();
            this.SetIsClicked(false);
        }
        private Snackbar mCodeUnknownException;
        public void ShowRetryOptionsCodeUnknownException(Exception exception)
        {
            if (mCodeUnknownException != null && mCodeUnknownException.IsShown)
            {
                mCodeUnknownException.Dismiss();
            }

            mCodeUnknownException = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mCodeUnknownException.Dismiss();
                string email = txtEmail.Text;
                this.userActionsListener.GetCode(Constants.APP_CONFIG.API_KEY_ID, email);
            }
            );
            mCodeUnknownException.Show();
            this.SetIsClicked(false);
        }

        public void ClearTextFields()
        {

            txtEmail.Text = "";
            txtVerificationCode.Text = "";
        }

        public void StartProgress()
        {
            //throw new NotImplementedException();
        }

        public void EnableResendButton()
        {
            //throw new NotImplementedException();
        }

        public void DisableResendButton()
        {
            //throw new NotImplementedException();
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

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
