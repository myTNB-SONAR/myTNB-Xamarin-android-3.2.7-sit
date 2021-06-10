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
       , Theme = "@style/Theme.OwnerTenantBaseTheme")]
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

        private bool fromManualtextClear = false;


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
                TextViewUtils.SetTextSize14(txtTitleInfo);
                TextViewUtils.SetTextSize16(txtResetPasswordTitle, btnSubmit);

                txtInputLayoutNewPassword.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                    : Resource.Style.TextInputLayoutBottomErrorHint);
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

                txtNewPassword.FocusChange += FocusChange;
                txtNewPassword.TextChanged += TxtNewPassword_TextChanged;
                txtNewPassword.EditorAction += TxtNewPassword_EditorAction;
                txtNewPassword.AddTextChangedListener(new InputFilterFormField(txtNewPassword, txtInputLayoutNewPassword));
                txtNewPassword.AddTextChangedListener(new ResetPasswordNewChangeListener(txtNewPassword, txtInputLayoutNewPassword));

                txtConfirmNewPassword.FocusChange += TextConfirmChange;
                txtConfirmNewPassword.TextChanged += TxtConfirmNewPassword_TextChanged;
                txtConfirmNewPassword.EditorAction += TxtConfirmNewPassword_EditorAction;
                txtConfirmNewPassword.AddTextChangedListener(new InputFilterFormField(txtConfirmNewPassword, txtInputLayoutConfirmNewPassword));

                this.ClearErrorMessages();

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void TxtConfirmNewPassword_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e != null && (e.ActionId == Android.Views.InputMethods.ImeAction.Done ||
                e.ActionId == Android.Views.InputMethods.ImeAction.Next ||
                e.ActionId == Android.Views.InputMethods.ImeAction.Search ||
                e.ActionId == Android.Views.InputMethods.ImeAction.Send ||
                e.ActionId == Android.Views.InputMethods.ImeAction.Go))
            {
                this.validateBothPassword();
                e.Handled = true;
            }

        }

        /// <summary>
        /// Handle keyboard button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtNewPassword_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e != null && (
                e.ActionId == Android.Views.InputMethods.ImeAction.Done ||
                e.ActionId == Android.Views.InputMethods.ImeAction.Next ||
                e.ActionId == Android.Views.InputMethods.ImeAction.Search ||
                e.ActionId == Android.Views.InputMethods.ImeAction.Send ||
                e.ActionId == Android.Views.InputMethods.ImeAction.Go
                ))
            {
                this.validateBothPassword();
                e.Handled = true;
            }
        }


        /// <summary>
        /// Handle when text is null and handle show and hidden eye at password  (confirm new password) 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtConfirmNewPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            //clear error if typed
            txtInputLayoutConfirmNewPassword.Error = null;


            if (txtConfirmNewPassword.Text.Length == 0)
            {
                this.DisableSubmitButton();
            }
            else
            {
                if (!string.IsNullOrEmpty(txtNewPassword.Text) && !string.IsNullOrEmpty(txtConfirmNewPassword.Text))
                {

                    this.EnableSubmitButton();
                }

            }


            if (!string.IsNullOrEmpty(txtNewPassword.Text))
            {
                txtInputLayoutConfirmNewPassword.PasswordVisibilityToggleEnabled = true;
                txtInputLayoutConfirmNewPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
            }
            else
            {
                txtInputLayoutConfirmNewPassword.PasswordVisibilityToggleEnabled = false;
            }
            txtInputLayoutConfirmNewPassword.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayoutBottomErrorHintLarge
                : Resource.Style.TextInputLayoutBottomErrorHint);
            //this.clearErrorIfTyped_forConfirmPass(true);
        }

        private void TxtNewPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            //always clear error if typed
            txtInputLayoutNewPassword.Error = null;

            if (txtNewPassword.Text.Length == 0)
            {
                this.DisableSubmitButton();
            }
            else
            {
                if (!string.IsNullOrEmpty(txtNewPassword.Text) && !string.IsNullOrEmpty(txtConfirmNewPassword.Text))
                {

                    this.EnableSubmitButton();
                }


            }

            if (!string.IsNullOrEmpty(txtNewPassword.Text))
            {
                txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = true;
                txtInputLayoutNewPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
            }
            else
            {
                txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = false;
            }

            // this.clearErrorIfTyped_forConfirmPass(false);

        }


        private void FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                this.validateBothPassword();
            }

        }

        private bool validateBothPassword()
        {
            try
            {

                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmNewPassword.Text;
                bool isCorrect = true;

                this.DisableSubmitButton();

                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (!this.userActionsListener.CheckPasswordIsValid(newPassword))
                    {
                        this.ShowPasswordMinimumOf6CharactersError();
                        isCorrect = false;
                    }
                    else
                    {
                        this.ClearNewPasswordError();
                    }
                }
                else
                {   //disable button if no text
                    isCorrect = false;
                }


                if (!string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(confirmPassword))
                {
                    if (!newPassword.Equals(confirmPassword))
                    {
                        //if password not equals
                        this.ShowNotEqualConfirmNewPasswordToNewPasswordError();
                        isCorrect = false;
                    }
                    else
                    {
                        this.ClearConfirmPasswordError();
                    }
                }
                else
                {
                    //disable button if no text
                    isCorrect = false;
                }


                //handle button to enable or disable
                if (isCorrect == true)
                {
                    this.EnableSubmitButton();
                    return true;
                }
                else
                {
                    this.DisableSubmitButton();
                    return false;
                }


            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
                return false;
            }
        }





        private void TextConfirmChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                this.validateBothPassword();
            }

        }

        public void ChangeIsFromClear(bool isFromClear)
        {
            fromManualtextClear = isFromClear;
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
            if (txtInputLayoutConfirmNewPassword.Error != GetString(Resource.String.reset_password_empty_confirm_new_password_error))
            {

                txtInputLayoutConfirmNewPassword.Error = GetString(Resource.String.reset_password_empty_confirm_new_password_error);
            }

            if (!txtInputLayoutConfirmNewPassword.ErrorEnabled)
                txtInputLayoutConfirmNewPassword.ErrorEnabled = true;
        }

        public void ShowNotEqualConfirmNewPasswordToNewPasswordError()
        {
            //ClearConfirmPasswordError();

            if (txtInputLayoutConfirmNewPassword.Error != Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword"))
            {
                txtInputLayoutConfirmNewPassword.Error = Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword"); //fix bounce animation
            }


            if (!txtInputLayoutConfirmNewPassword.ErrorEnabled)
                txtInputLayoutConfirmNewPassword.ErrorEnabled = true;
        }

        public void ShowPasswordMinimumOf6CharactersError()
        {
            //ClearNewPasswordError();
            if (txtInputLayoutNewPassword.Error != Utility.GetLocalizedErrorLabel("invalid_password"))
            {
                txtInputLayoutNewPassword.Error = Utility.GetLocalizedErrorLabel("invalid_password");
            }

            txtInputLayoutNewPassword.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
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
            this.ChangeIsFromClear(true);
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

                    string newPassword = txtNewPassword.Text;
                    string confirmPassword = txtConfirmNewPassword.Text;
                    bool isCorrect = this.validateBothPassword();
                    if (isCorrect)
                    {

                        this.SetIsClicked(true);

                        this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, newPassword, confirmPassword, enteredPassword, enteredUserName, this.DeviceId());

                    }
                    else
                    {
                        this.SetIsClicked(false);
                    }

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
                txtInputLayoutNewPassword.Error = null;
                txtInputLayoutNewPassword.ErrorEnabled = false;
            }
        }

        private void ClearConfirmPasswordError()
        {
            if (!string.IsNullOrEmpty(txtInputLayoutConfirmNewPassword.Error))
            {
                txtInputLayoutConfirmNewPassword.Error = null;
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
