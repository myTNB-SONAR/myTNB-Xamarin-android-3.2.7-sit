using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AppLaunch.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Login.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.ResetPassword.MVP;
using myTNB_Android.Src.ResetPasswordSuccess.Activity;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Runtime;

namespace myTNB_Android.Src.ResetPassword.Activity
{
    [Activity(Label = "@string/reset_password_activity_title"
              , Icon = "@drawable/ic_launcher"
       , ScreenOrientation = ScreenOrientation.Portrait
       , Theme = "@style/Theme.ResetPassword")]
    public class ResetPasswordActivity : BaseToolbarAppCompatActivity, ResetPasswordContract.IView
    {
        [BindView(Resource.Id.txtResetPasswordTitle)]
        TextView txtResetPasswordTitle;


        [BindView(Resource.Id.txtNewPassword)]
        TextView txtNewPassword;

        [BindView(Resource.Id.txtConfirmNewPassword)]
        TextView txtConfirmNewPassword;

        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtInputLayoutNewPassword)]
        TextInputLayout txtInputLayoutNewPassword;

        [BindView(Resource.Id.txtInputLayoutConfirmNewPassword)]
        TextInputLayout txtInputLayoutConfirmNewPassword;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.btnSubmit)]
        Button btnSubmit;

        private AlertDialog mProgressDialog;
        private string fromActivity;


        private ResetPasswordPresenter mPresenter;
        private ResetPasswordContract.IUserActionsListener userActionsListener;

        string enteredPassword, enteredUserName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            try
            {


                this.DisableSubmitButton();

                mPresenter = new ResetPasswordPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));

                TextViewUtils.SetMuseoSans500Typeface(btnSubmit);

                TextViewUtils.SetMuseoSans300Typeface(txtTitleInfo, txtNewPassword, txtConfirmNewPassword);

                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewPassword, txtInputLayoutConfirmNewPassword);

                mProgressDialog = new AlertDialog.Builder(this)
                    .SetTitle(GetString(Resource.String.reset_password_alert_dialog_title))
                    .SetMessage(GetString(Resource.String.reset_password_alert_dialog_message))
                    .SetCancelable(false)
                    .Create();

                if (savedInstanceState != null)
                {
                    enteredPassword = savedInstanceState.GetString(Constants.ENTERED_PASSWORD, enteredPassword);
                    enteredUserName = savedInstanceState.GetString(Constants.ENTERED_USERNAME, enteredUserName);
                }
                else
                {
                    enteredPassword = Intent.Extras.GetString(Constants.ENTERED_PASSWORD, null);
                    enteredUserName = Intent.Extras.GetString(Constants.ENTERED_USERNAME, null);
                }

                txtNewPassword.TextChanged += TextChange;
                txtConfirmNewPassword.TextChanged += TextChange;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmNewPassword.Text;
                //bool isError = true;


                //if (!string.IsNullOrEmpty(newPassword))
                //{

                //    txtInputLayoutNewPassword.Error = GetString(Resource.String.registration_form_password_format_hint);
                //    txtInputLayoutNewPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomHint);
                //    TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewPassword);
                //    txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = true;
                //}
                //else
                //{
                //    txtInputLayoutNewPassword.Error = "";
                //    txtInputLayoutNewPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                //    TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewPassword);
                //    txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = false;
                //}

                //if (!string.IsNullOrEmpty(confirmPassword))
                //{
                //    txtInputLayoutConfirmNewPassword.PasswordVisibilityToggleEnabled = true;
                //    isError = false;
                //}
                //else
                //{
                //    txtInputLayoutConfirmNewPassword.PasswordVisibilityToggleEnabled = false;
                //}

                //if (isError)
                //{
                //    this.DisableSubmitButton();
                //}
                //else
                //{
                //    this.EnableSubmitButton();
                //}

                this.DisableSubmitButton();
                this.ClearErrorMessages();
                
                // validation new password
                if (!string.IsNullOrEmpty(newPassword))
                {
                    txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = true;
                    if (!this.userActionsListener.CheckPasswordIsValid(newPassword))
                    {
                        this.ShowPasswordMinimumOf6CharactersError();
                    }
                    else
                    {
                        this.ClearErrorMessages();
                    }
                }
                else
                {
                    txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = false;

                }

                // validation confirm password
                if (!string.IsNullOrEmpty(confirmPassword) || (!string.IsNullOrEmpty(confirmPassword) && !string.IsNullOrEmpty(newPassword)))
                {
                    txtInputLayoutConfirmNewPassword.PasswordVisibilityToggleEnabled = true;
                    if (!newPassword.Equals(confirmPassword))
                    {
                        this.ShowNotEqualConfirmNewPasswordToNewPasswordError();
                    }
                    else
                    {
                        this.ClearErrorMessages();
                        this.EnableSubmitButton();
                    }
                    
                }
                else
                {
                    txtInputLayoutConfirmNewPassword.PasswordVisibilityToggleEnabled = false;

                }





            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public override int ResourceId()
        {
            return Resource.Layout.ResetPasswordView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Reset Password");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowEmptyNewPasswordError()
        {
            txtInputLayoutNewPassword.Error = GetString(Resource.String.reset_password_empty_new_password_error);
        }

        public void ShowEmptyConfirmNewPasswordError()
        {
            txtInputLayoutConfirmNewPassword.Error = GetString(Resource.String.reset_password_empty_confirm_new_password_error);
        }

        public void ShowNotEqualConfirmNewPasswordToNewPasswordError()
        {
            txtInputLayoutConfirmNewPassword.Error = GetString(Resource.String.reset_password_confirm_password_does_not_match_error);
        }

        public void ShowPasswordMinimumOf6CharactersError()
        {
            txtInputLayoutNewPassword.Error = GetString(Resource.String.registration_form_password_format_hint);
            txtInputLayoutNewPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
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
        private Snackbar mSnackBar;
        public void ShowSuccessMessage(string message)
        {
            if (mSnackBar != null && mSnackBar.IsShown)
            {
                mSnackBar.Dismiss();
            }

            mSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.reset_password_validation_snackbar_btn_close), delegate { mSnackBar.Dismiss(); }
            );
            mSnackBar.Show();
        }

        public void ShowErrorMessage(string errorMessage)
        {
            if (mSnackBar != null && mSnackBar.IsShown)
            {
                mSnackBar.Dismiss();
            }

            mSnackBar = Snackbar.Make(rootView, errorMessage, Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.reset_password_validation_snackbar_btn_close), delegate { mSnackBar.Dismiss(); }
            );
            mSnackBar.Show();
        }

        public void DisableSubmitButton()
        {
            btnSubmit.Enabled = false;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public void EnableSubmitButton()
        {
            btnSubmit.Enabled = true;
            btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.reset_password_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.reset_password_cancelled_exception_btn_retry), delegate
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

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.reset_password_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.reset_password_api_exception_btn_retry), delegate
            {

                mApiExcecptionSnackBar.Dismiss();
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmNewPassword.Text;
                this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, newPassword, confirmPassword, enteredPassword, enteredUserName, this.DeviceId());
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
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmNewPassword.Text;
                this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, newPassword, confirmPassword, enteredPassword, enteredUserName, this.DeviceId());

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.reset_password_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.reset_password_unknown_exception_btn_retry), delegate
            {

                mUknownExceptionSnackBar.Dismiss();
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmNewPassword.Text;
                this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, newPassword, confirmPassword, enteredPassword, enteredUserName, this.DeviceId());
            }
            );
            mUknownExceptionSnackBar.Show();

        }

        public void SetPresenter(ResetPasswordContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public void ClearTextFields()
        {
            txtNewPassword.Text = "";
            txtConfirmNewPassword.Text = "";
        }

        [OnClick(Resource.Id.btnSubmit)]
        void OnSubmit(object sender, EventArgs eventArgs)
        {
            try
            {
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmNewPassword.Text;
                this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, newPassword, confirmPassword, enteredPassword, enteredUserName, this.DeviceId());
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ClearErrorMessages()
        {
            txtInputLayoutNewPassword.Error = "";
            txtInputLayoutConfirmNewPassword.Error = "";
        }

        public override void OnBackPressed()
        {
            try
            {
                if (fromActivity != null && (fromActivity.Equals(LaunchViewActivity.TAG) || fromActivity.Equals(LoginActivity.TAG)))
                {
                    // TODO : START ACTIVITY DASHBOARD
                    if (UserSessions.HasResetFlag(PreferenceManager.GetDefaultSharedPreferences(this)))
                    {
                        Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
                        DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                        StartActivity(DashboardIntent);
                    }
                    else
                    {
                        ShowResetPasswordSuccess();
                    }

                }
                else
                {
                    base.OnBackPressed();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowResetPasswordSuccess()
        {
            Intent intent = new Intent(this, typeof(ResetPasswordSuccessActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(intent);

        }

        public override bool TelephonyPermissionRequired()
        {
            return false;
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