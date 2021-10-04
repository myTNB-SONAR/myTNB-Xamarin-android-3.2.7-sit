using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;


using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.ForgetPassword.MVP;
using myTNB_Android.Src.Utils;
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

        public void ClearErrorMessages()
        {
            if (!string.IsNullOrEmpty(txtInputLayoutEmail.Error))
            {
                txtInputLayoutEmail.Error = " ";
                txtInputLayoutEmail.ErrorEnabled = false;
            }
            if (!string.IsNullOrEmpty(textInputLayoutVerificationCode.Error))
            {
                textInputLayoutVerificationCode.Error = " ";
                textInputLayoutVerificationCode.ErrorEnabled = false;
            }
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
            if (!string.IsNullOrEmpty(txtInputLayoutEmail.Error))
            {
                txtInputLayoutEmail.Error = " ";
                txtInputLayoutEmail.ErrorEnabled = false;
            }
            txtInputLayoutEmail.Error = GetString(Resource.String.forget_password_empty_email_error);
            if (!txtInputLayoutEmail.ErrorEnabled)
                txtInputLayoutEmail.ErrorEnabled = true;
            this.SetIsClicked(false);
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
                .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mSnackBar.Dismiss(); }
                );
                View v = mSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mSnackBar.Show();
            }
            this.SetIsClicked(false);
        }

        public void ShowSuccess(string message)
        {
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
            //if (!string.IsNullOrEmpty(txtInputLayoutEmail.Error))
            //{
            //    txtInputLayoutEmail.Error = " ";
            //    txtInputLayoutEmail.ErrorEnabled = false;
            //}

            if (txtInputLayoutEmail.Error != Utility.GetLocalizedLabel("Error", "invalid_email"))
            {
                txtInputLayoutEmail.Error = Utility.GetLocalizedLabel("Error", "invalid_email");
            }


            if (!txtInputLayoutEmail.ErrorEnabled)
                txtInputLayoutEmail.ErrorEnabled = true;
            this.SetIsClicked(false);
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

                txtInputLayoutEmail.Hint = GetLabelCommonByLanguage("email");
                txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, txtInputLayoutEmail));

                TextViewUtils.SetMuseoSans500Typeface(txtEmailTitle);
                TextViewUtils.SetMuseoSans300Typeface(txtEmailLinkInfo, txtEmail, txtVerificationCode, txtGetACode);
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutEmail, textInputLayoutVerificationCode);
                TextViewUtils.SetMuseoSans500Typeface(btnSubmit);
                TextViewUtils.SetTextSize14(txtEmailLinkInfo);
                TextViewUtils.SetTextSize16(txtEmailTitle, btnSubmit);

                txtEmailTitle.Text = GetLabelByLanguage("subTitle");
                txtEmailLinkInfo.Text = GetLabelByLanguage("details");
                btnSubmit.Text = GetLabelCommonByLanguage("submit");
                txtEmail.TextChanged += TxtEmail_TextChanged;
                DisableSubmitButton();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void TxtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            // when typed there will be no error only raised when validated
            txtInputLayoutEmail.Error = null;

            if (txtEmail.Text.Length == 0)
            {
                this.DisableSubmitButton();
            }
            else
            {
                this.EnableSubmitButton();
            }


        }


        [OnClick(Resource.Id.btnSubmit)]
        void OnGetCode(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {

                bool isEmailLegit = ValidateEmail();
                if (isEmailLegit)
                {
                    this.SetIsClicked(true);
                    string email = txtEmail.Text;
                    this.userActionsListener.GetCode(Constants.APP_CONFIG.API_KEY_ID, email);
                }
                else
                {
                    this.SetIsClicked(false);
                }


            }
        }

        public void EnableSubmitButton()
        {
            btnSubmit.Enabled = true;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }

        public void DisableSubmitButton()
        {
            btnSubmit.Enabled = false;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
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
            View v = mCancelledExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
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
            View v = mApiExcecptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
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
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
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
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
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
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowEmptyCodeError()
        {
            if (!string.IsNullOrEmpty(textInputLayoutVerificationCode.Error))
            {
                textInputLayoutVerificationCode.Error = " ";
                textInputLayoutVerificationCode.ErrorEnabled = false;
            }
            if (textInputLayoutVerificationCode.Error != GetString(Resource.String.forget_password_empty_code_error))
            {
                textInputLayoutVerificationCode.Error = GetString(Resource.String.forget_password_empty_code_error);
            }


            if (!textInputLayoutVerificationCode.ErrorEnabled)
                textInputLayoutVerificationCode.ErrorEnabled = true;
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
            View v = mCodeCancelledExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
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
            View v = mCodeApiExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
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
            View v = mCodeUnknownException.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
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

        public void ShowEmptyErrorPin_1()
        {

        }

        public void ShowEmptyErrorPin_2()
        {

        }

        public void ShowEmptyErrorPin_3()
        {

        }

        public void ShowEmptyErrorPin_4()
        {

        }

        public void ShowEmptyErrorPin()
        {

        }

        private bool ValidateEmail()
        {
            try
            {
                string email = txtEmail.Text;

                if (!string.IsNullOrEmpty(email))
                {
                    if (!Patterns.EmailAddress.Matcher(email).Matches())
                    {
                        ShowInvalidEmailError();
                        DisableSubmitButton();
                        return false;
                    }
                    else
                    {
                        EnableSubmitButton();
                        ClearErrorMessages();
                        return true;
                    }
                }

                return false;

            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
                return false;
            }
        }
    }
}
