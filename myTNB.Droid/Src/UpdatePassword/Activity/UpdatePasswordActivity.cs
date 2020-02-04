using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.UpdatePassword.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Refit;
using System;
using System.Runtime;

namespace myTNB_Android.Src.UpdatePassword.Activity
{
    [Activity(Label = "@string/update_password_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.UpdatePassword")]
    public class UpdatePasswordActivity : BaseActivityCustom, UpdatePasswordContract.IView
    {

        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.txtInputLayoutCurrentPassword)]
        TextInputLayout txtInputLayoutCurrentPassword;

        [BindView(Resource.Id.txtInputLayoutNewPassword)]
        TextInputLayout txtInputLayoutNewPassword;

        [BindView(Resource.Id.txtInputLayoutConfirmPassword)]
        TextInputLayout txtInputLayoutConfirmPassword;

        [BindView(Resource.Id.txtCurrentPassword)]
        EditText txtCurrentPassword;

        [BindView(Resource.Id.txtNewPassword)]
        EditText txtNewPassword;

        [BindView(Resource.Id.txtConfirmPassword)]
        EditText txtConfirmPassword;

        [BindView(Resource.Id.btnSave)]
        Button btnSave;


        MaterialDialog progress;
        private LoadingOverlay loadingOverlay;
        private string PAGE_ID = "UpdatePassword";

        UpdatePasswordContract.IUserActionsListener userActionsListener;
        UpdatePasswordPresenter mPresenter;




        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutConfirmPassword,
                    txtInputLayoutCurrentPassword,
                    txtInputLayoutNewPassword);

                TextViewUtils.SetMuseoSans300Typeface(txtCurrentPassword,
                    txtNewPassword,
                    txtConfirmPassword);

                txtInputLayoutCurrentPassword.Hint = GetLabelByLanguage("currentPassword");
                txtInputLayoutNewPassword.Hint = GetLabelByLanguage("newPassword");
                txtInputLayoutConfirmPassword.Hint = GetLabelByLanguage("confirmNewPassword");

                txtInputLayoutCurrentPassword.ErrorEnabled = false;
                txtInputLayoutNewPassword.ErrorEnabled = false;
                txtInputLayoutConfirmPassword.ErrorEnabled = false;

                txtInputLayoutNewPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                txtInputLayoutConfirmPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);

                TextViewUtils.SetMuseoSans500Typeface(btnSave);
                btnSave.Text = GetLabelCommonByLanguage("save");

                txtCurrentPassword.TextChanged += OnCurrentPasswordTextChange;
                txtNewPassword.TextChanged += OnNewPasswordTextChange;
                txtConfirmPassword.TextChanged += OnConfirmPasswordTextChange;

                txtCurrentPassword.AddTextChangedListener(new InputFilterFormField(txtCurrentPassword, txtInputLayoutCurrentPassword));
                txtNewPassword.AddTextChangedListener(new InputFilterFormField(txtNewPassword, txtInputLayoutNewPassword));
                txtConfirmPassword.AddTextChangedListener(new InputFilterFormField(txtConfirmPassword, txtInputLayoutConfirmPassword));

                progress = new MaterialDialog.Builder(this)
                .Title(GetString(Resource.String.update_password_progress_title))
                .Content(GetString(Resource.String.update_password_progress_content))
                .Progress(true, 0)
                .Cancelable(false)
                .Build();


                this.mPresenter = new UpdatePasswordPresenter(this);
                EnableOnSubmitButton(false);
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void OnCurrentPasswordTextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                string currentPassword = txtCurrentPassword.Text;
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmPassword.Text;

                if (!string.IsNullOrEmpty(currentPassword))
                {
                    txtInputLayoutCurrentPassword.PasswordVisibilityToggleEnabled = true;
                    txtInputLayoutCurrentPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
                }
                else
                {
                    txtInputLayoutCurrentPassword.PasswordVisibilityToggleEnabled = false;
                }

                if (!txtInputLayoutNewPassword.ErrorEnabled && !txtInputLayoutConfirmPassword.ErrorEnabled)
                {
                    EnableOnSubmitButton(!string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(confirmPassword));
                }
                else
                {
                    EnableOnSubmitButton(false);
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void OnNewPasswordTextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                string currentPassword = txtCurrentPassword.Text;
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmPassword.Text;

                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (!mPresenter.CheckPasswordIsValid(newPassword))
                    {
                        txtInputLayoutNewPassword.Error = Utility.GetLocalizedErrorLabel("invalid_password");
                        txtInputLayoutNewPassword.ErrorEnabled = true;
                    }
                    else
                    {
                        txtInputLayoutNewPassword.Error = null;
                        txtInputLayoutNewPassword.ErrorEnabled = false;
                    }
                    txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = true;
                    txtInputLayoutNewPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
                }
                else
                {
                    txtInputLayoutNewPassword.Error = null;
                    txtInputLayoutNewPassword.ErrorEnabled = false;
                    txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = false;
                }

                if (!string.IsNullOrEmpty(confirmPassword))
                {
                    if (!newPassword.Equals(confirmPassword))
                    {
                        txtInputLayoutConfirmPassword.Error = Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword");
                        txtInputLayoutConfirmPassword.ErrorEnabled = true;
                    }
                    else
                    {
                        txtInputLayoutConfirmPassword.Error = null;
                        txtInputLayoutConfirmPassword.ErrorEnabled = false;
                    }
                    txtInputLayoutConfirmPassword.PasswordVisibilityToggleEnabled = true;
                    txtInputLayoutConfirmPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
                }
                else
                {
                    txtInputLayoutConfirmPassword.Error = null;
                    txtInputLayoutConfirmPassword.ErrorEnabled = false;
                    txtInputLayoutConfirmPassword.PasswordVisibilityToggleEnabled = false;
                }

                if (!txtInputLayoutNewPassword.ErrorEnabled && !txtInputLayoutConfirmPassword.ErrorEnabled)
                {
                    EnableOnSubmitButton(!string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(confirmPassword));
                }
                else
                {
                    EnableOnSubmitButton(false);
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void OnConfirmPasswordTextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                string currentPassword = txtCurrentPassword.Text;
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmPassword.Text;

                if (!string.IsNullOrEmpty(confirmPassword))
                {
                    if (!newPassword.Equals(confirmPassword))
                    {
                        txtInputLayoutConfirmPassword.Error = Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword");
                        txtInputLayoutConfirmPassword.ErrorEnabled = true;
                    }
                    else
                    {
                        txtInputLayoutConfirmPassword.Error = null;
                        txtInputLayoutConfirmPassword.ErrorEnabled = false;
                    }
                    txtInputLayoutConfirmPassword.PasswordVisibilityToggleEnabled = true;
                    txtInputLayoutConfirmPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
                }
                else
                {
                    txtInputLayoutConfirmPassword.Error = null;
                    txtInputLayoutConfirmPassword.ErrorEnabled = false;
                    txtInputLayoutConfirmPassword.PasswordVisibilityToggleEnabled = false;
                }

                if (!txtInputLayoutNewPassword.ErrorEnabled && !txtInputLayoutConfirmPassword.ErrorEnabled)
                {
                    EnableOnSubmitButton(!string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(confirmPassword));
                }
                else
                {
                    EnableOnSubmitButton(false);
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.btnSave)]
        void OnSave(object sender, EventArgs eventArgs)
        {
            try
            {
                string currentPassword = txtCurrentPassword.Text;
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmPassword.Text;
                this.userActionsListener.OnSave(currentPassword, newPassword, confirmPassword);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.UpdatePasswordView;
        }

        public void SetPresenter(UpdatePasswordContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowEmptyConfirmPassword()
        {
            txtInputLayoutConfirmPassword.Error = GetString(Resource.String.update_password_empty_confirm_new_password);
        }

        public void ShowEmptyCurrentPassword()
        {
            txtInputLayoutCurrentPassword.Error = GetString(Resource.String.update_password_empty_current_password);
        }

        public void ShowEmptyNewPassword()
        {
            txtInputLayoutNewPassword.Error = GetString(Resource.String.update_password_empty_new_password);
        }

        public void ShowInvalidCurrentPassword()
        {
            txtInputLayoutCurrentPassword.Error = GetString(Resource.String.update_password_invalid_current_password);
        }

        public void ShowInvalidNewPassword()
        {
            txtInputLayoutNewPassword.Error = Utility.GetLocalizedErrorLabel("invalid_password");
        }

        public void ShowNewPasswordNotEqualToConfirmPassword()
        {
            txtInputLayoutConfirmPassword.Error = GetString(Resource.String.update_password_invalid_confirm_password);
        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mCancelledExceptionSnackBar.Dismiss();
            }
            );
            View v = mCancelledExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
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
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();
            }
            );
            View v = mApiExcecptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
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
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();

        }

        public void ClearErrors()
        {
            txtInputLayoutConfirmPassword.Error = null;
            txtInputLayoutCurrentPassword.Error = null;
            txtInputLayoutNewPassword.Error = null;
        }

        public void ShowProgress()
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

        public void HideProgress()
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

        private Snackbar mErrorSnackbar;
        public void ShowErrorMessage(string message)
        {
            if (mErrorSnackbar != null && mErrorSnackbar.IsShown)
            {
                mErrorSnackbar.Dismiss();
            }

            mErrorSnackbar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mErrorSnackbar.Dismiss();
            }
            );
            View v = mErrorSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mErrorSnackbar.Show();
        }

        public void ShowSuccess()
        {
            SetResult(Result.Ok);
            Finish();
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

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                FirebaseAnalyticsUtils.SetScreenName(this, "Change Password");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public void EnableOnSubmitButton(bool IsEnable)
        {
            btnSave.Enabled = IsEnable;
            btnSave.Background = IsEnable ? ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background) : ContextCompat.GetDrawable(this,
                Resource.Drawable.silver_chalice_button_background);
        }
    }
}
