using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;


using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
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
    public class ResetPasswordActivity : BaseActivityCustom, ResetPasswordContract.IView
    {
        [BindView(Resource.Id.txtResetPasswordTitle)]
        TextView txtResetPasswordTitle;


        [BindView(Resource.Id.txtNewPassword)]
        EditText txtNewPassword;

        [BindView(Resource.Id.txtConfirmNewPassword)]
        EditText txtConfirmNewPassword;

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
        const string PAGE_ID = "ResetPassword";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            try
            {


                this.DisableSubmitButton();

                mPresenter = new ResetPasswordPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));

                TextViewUtils.SetMuseoSans500Typeface(btnSubmit, txtResetPasswordTitle);

                TextViewUtils.SetMuseoSans300Typeface(txtTitleInfo, txtNewPassword, txtConfirmNewPassword);

                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewPassword, txtInputLayoutConfirmNewPassword);

                txtResetPasswordTitle.Text = GetLabelByLanguage("subTitle");
                txtTitleInfo.Text = GetLabelByLanguage("details");
                txtInputLayoutNewPassword.Hint = GetLabelByLanguage("newPassword");
                txtInputLayoutConfirmNewPassword.Hint = GetLabelByLanguage("confirmNewPassword");
                btnSubmit.Text = GetLabelCommonByLanguage("submit");

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

                txtNewPassword.TextChanged += TextNewChange;
                txtNewPassword.AddTextChangedListener(new InputFilterFormField(txtNewPassword, txtInputLayoutNewPassword));
                txtConfirmNewPassword.TextChanged += TextConfirmChange;
                txtConfirmNewPassword.AddTextChangedListener(new InputFilterFormField(txtConfirmNewPassword, txtInputLayoutConfirmNewPassword));

                this.ClearErrorMessages();

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void TextNewChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmNewPassword.Text;

                this.DisableSubmitButton();
               // this.ClearErrorMessages();

                // validation new password
                if (!string.IsNullOrEmpty(newPassword))
                {
                    txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = true;
                    TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewPassword);
                    txtInputLayoutNewPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
                   // ClearNewPasswordError();
                    if (txtInputLayoutNewPassword.Error != Utility.GetLocalizedErrorLabel("invalid_password")) {
                        txtInputLayoutNewPassword.Error = Utility.GetLocalizedErrorLabel("invalid_password");  //fix bounce issue
                    }
                    txtInputLayoutNewPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                    if (!txtInputLayoutNewPassword.ErrorEnabled)
                        txtInputLayoutNewPassword.ErrorEnabled = true;
                    if (!this.userActionsListener.CheckPasswordIsValid(newPassword))
                    {
                        this.ShowPasswordMinimumOf6CharactersError();
                    }
                    else
                    {
                        
                        this.ClearNewPasswordError();
                    }
                }
                else
                {
                    txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = false;
                }

                // validation confirm password
                if (!string.IsNullOrEmpty(confirmPassword) || (!string.IsNullOrEmpty(confirmPassword) && !string.IsNullOrEmpty(newPassword)))
                {
                    TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutConfirmNewPassword);
                    txtInputLayoutConfirmNewPassword.PasswordVisibilityToggleEnabled = true;
                    txtInputLayoutConfirmNewPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
                   // ClearConfirmPasswordError();
                    if(txtInputLayoutConfirmNewPassword.Error != Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword"))
                    {
                     txtInputLayoutConfirmNewPassword.Error = Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword");  //fix bounce issue
                    }
                    txtInputLayoutConfirmNewPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                    if (!txtInputLayoutConfirmNewPassword.ErrorEnabled)
                        txtInputLayoutConfirmNewPassword.ErrorEnabled = true;
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

        private void TextConfirmChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmNewPassword.Text;

                this.DisableSubmitButton();
               // this.ClearErrorMessages();

                // validation confirm password
                if (!string.IsNullOrEmpty(confirmPassword) || (!string.IsNullOrEmpty(confirmPassword) && !string.IsNullOrEmpty(newPassword)))
                {
                    txtInputLayoutConfirmNewPassword.PasswordVisibilityToggleEnabled = true;
                    TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutConfirmNewPassword);
                    txtInputLayoutConfirmNewPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
                   // ClearConfirmPasswordError();
                    if (txtInputLayoutConfirmNewPassword.Error != Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword")) {
                        txtInputLayoutConfirmNewPassword.Error = Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword"); //fix bounce issue
                    }
                    
                    txtInputLayoutConfirmNewPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                    if (!txtInputLayoutConfirmNewPassword.ErrorEnabled)
                        txtInputLayoutConfirmNewPassword.ErrorEnabled = true;
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
            //ClearNewPasswordError();

            if (txtInputLayoutNewPassword.Error != GetString(Resource.String.reset_password_empty_new_password_error))
            {
                txtInputLayoutNewPassword.Error = GetString(Resource.String.reset_password_empty_new_password_error);
            }

           
            if (!txtInputLayoutNewPassword.ErrorEnabled)
                txtInputLayoutNewPassword.ErrorEnabled = true;
        }

        public void ShowEmptyConfirmNewPasswordError()
        {
            //ClearConfirmPasswordError();
            if (txtInputLayoutConfirmNewPassword.Error != GetString(Resource.String.reset_password_empty_confirm_new_password_error)) {

                txtInputLayoutConfirmNewPassword.Error = GetString(Resource.String.reset_password_empty_confirm_new_password_error);
            }
   
            if (!txtInputLayoutConfirmNewPassword.ErrorEnabled)
                txtInputLayoutConfirmNewPassword.ErrorEnabled = true;
        }

        public void ShowNotEqualConfirmNewPasswordToNewPasswordError()
        {
            //ClearConfirmPasswordError();

            if(txtInputLayoutConfirmNewPassword.Error != Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword"))
            {
                txtInputLayoutConfirmNewPassword.Error = Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword"); //fix bounce animation
            }

          
            if (!txtInputLayoutConfirmNewPassword.ErrorEnabled)
                txtInputLayoutConfirmNewPassword.ErrorEnabled = true;
        }

        public void ShowPasswordMinimumOf6CharactersError()
        {
            //ClearNewPasswordError();
            if(txtInputLayoutNewPassword.Error != Utility.GetLocalizedErrorLabel("invalid_password"))
            {
                txtInputLayoutNewPassword.Error = Utility.GetLocalizedErrorLabel("invalid_password");
            }
            
            txtInputLayoutNewPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
            if (!txtInputLayoutNewPassword.ErrorEnabled)
                txtInputLayoutNewPassword.ErrorEnabled = true;
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
        private Snackbar mSnackBar;
        public void ShowSuccessMessage(string message)
        {
            if (mSnackBar != null && mSnackBar.IsShown)
            {
                mSnackBar.Dismiss();
            }

            mSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mSnackBar.Dismiss(); }
            );
            mSnackBar.Show();
            this.SetIsClicked(false);
        }

        public void ShowErrorMessage(string errorMessage)
        {
            if (mSnackBar != null && mSnackBar.IsShown)
            {
                mSnackBar.Dismiss();
            }

            mSnackBar = Snackbar.Make(rootView, errorMessage, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mSnackBar.Dismiss(); }
            );
            mSnackBar.Show();
            this.SetIsClicked(false);
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

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();

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
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmNewPassword.Text;
                this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, newPassword, confirmPassword, enteredPassword, enteredUserName, this.DeviceId());
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
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmNewPassword.Text;
                this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, newPassword, confirmPassword, enteredPassword, enteredUserName, this.DeviceId());

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmNewPassword.Text;
                this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, newPassword, confirmPassword, enteredPassword, enteredUserName, this.DeviceId());
            }
            );
            mUknownExceptionSnackBar.Show();
            this.SetIsClicked(false);

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
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    string newPassword = txtNewPassword.Text;
                    string confirmPassword = txtConfirmNewPassword.Text;
                    this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, newPassword, confirmPassword, enteredPassword, enteredUserName, this.DeviceId());
                }
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ClearErrorMessages()
        {
            ClearNewPasswordError();
            ClearConfirmPasswordError();
        }

        private void ClearNewPasswordError()
        {
            if (!string.IsNullOrEmpty(txtInputLayoutNewPassword.Error))
            {
                txtInputLayoutNewPassword.Error = " ";  //fix bouncing
                txtInputLayoutNewPassword.ErrorEnabled = false;
            }
        }

        private void ClearConfirmPasswordError()
        {
            if (!string.IsNullOrEmpty(txtInputLayoutConfirmNewPassword.Error))
            {
                txtInputLayoutConfirmNewPassword.Error = " ";
                txtInputLayoutConfirmNewPassword.ErrorEnabled = false;
            }
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

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
