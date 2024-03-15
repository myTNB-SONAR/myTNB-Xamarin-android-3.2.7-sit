using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;


using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.UpdatePassword.MVP;
using myTNB.AndroidApp.Src.Utils;
using Refit;
using System;
using System.Runtime;

namespace myTNB.AndroidApp.Src.UpdatePassword.Activity
{
    [Activity(Label = "@string/update_password_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.OwnerTenantBaseTheme")]
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
        private string PAGE_ID = "UpdatePassword";

        UpdatePasswordContract.IUserActionsListener userActionsListener;
        UpdatePasswordPresenter mPresenter;




        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_UpdatePasswordLarge : Resource.Style.Theme_UpdatePassword);
            try
            {
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutConfirmPassword,
                    txtInputLayoutCurrentPassword,
                    txtInputLayoutNewPassword);

                TextViewUtils.SetMuseoSans300Typeface(txtCurrentPassword,
                    txtNewPassword,
                    txtConfirmPassword);

               // TextViewUtils.SetTextSize12(txtCurrentPassword, txtNewPassword, txtConfirmPassword);
               

                txtInputLayoutCurrentPassword.Hint = GetLabelByLanguage("currentPassword");
                txtInputLayoutNewPassword.Hint = GetLabelByLanguage("newPassword");
                txtInputLayoutConfirmPassword.Hint = GetLabelByLanguage("confirmNewPassword");

                //txtInputLayoutCurrentPassword.ErrorEnabled = false;
                //txtInputLayoutNewPassword.ErrorEnabled = false;
                //txtInputLayoutConfirmPassword.ErrorEnabled = false;

                txtInputLayoutCurrentPassword.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
                txtInputLayoutNewPassword.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
                txtInputLayoutConfirmPassword.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);

                TextViewUtils.SetMuseoSans500Typeface(btnSave);
                TextViewUtils.SetTextSize14(btnSave, txtCurrentPassword, txtNewPassword, txtConfirmPassword);

                btnSave.Text = GetLabelCommonByLanguage("save");

                txtCurrentPassword.FocusChange += TxtCurrentPassword_FocusChange1; //OnCurrentPasswordTextChange;
                txtCurrentPassword.TextChanged += TxtCurrentPassword_TextChanged;
                txtCurrentPassword.EditorAction += TxtCurrentPassword_EditorAction;

                txtNewPassword.FocusChange += TxtNewPassword_FocusChange; //OnNewPasswordTextChange;
                txtNewPassword.TextChanged += TxtNewPassword_TextChanged;
                txtNewPassword.EditorAction += TxtNewPassword_EditorAction;

                txtConfirmPassword.FocusChange += TxtConfirmPassword_FocusChange;  // OnConfirmPasswordTextChange;
                txtConfirmPassword.TextChanged += TxtConfirmPassword_TextChanged;
                txtConfirmPassword.EditorAction += TxtConfirmPassword_EditorAction;

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

        private void TxtConfirmPassword_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TxtConfirmPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtInputLayoutConfirmPassword.Error = null;


            if (!string.IsNullOrEmpty(txtConfirmPassword.Text))
            {
                txtInputLayoutConfirmPassword.PasswordVisibilityToggleEnabled = true;
                txtInputLayoutConfirmPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
            }
            else
            {
                ClearConfirmPasswordError();
                txtInputLayoutConfirmPassword.PasswordVisibilityToggleEnabled = false;
            }

            this.EnableButtonIfAllFilled();
        }

        private void TxtNewPassword_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TxtNewPassword_TextChanged(object sender, TextChangedEventArgs e)
        {

            txtInputLayoutNewPassword.Error = null;

            if (!string.IsNullOrEmpty(txtNewPassword.Text))
            {
                txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = true;
                txtInputLayoutNewPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
            }
            else
            {

                txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = false;
            }

            this.EnableButtonIfAllFilled();
        }

        private void TxtCurrentPassword_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TxtCurrentPassword_TextChanged(object sender, TextChangedEventArgs e)
        {  
            //when typed error is hide
            txtInputLayoutCurrentPassword.Error = null;

            if (!string.IsNullOrEmpty(txtCurrentPassword.Text))
            {
                txtInputLayoutCurrentPassword.PasswordVisibilityToggleEnabled = true;
                txtInputLayoutCurrentPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
            }
            else
            {
                txtInputLayoutCurrentPassword.PasswordVisibilityToggleEnabled = false;
            }

            this.EnableButtonIfAllFilled();

        }

        private void TxtConfirmPassword_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                this.ValidateAllPassword();
            }
        }

        private void TxtNewPassword_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                this.ValidateAllPassword();
            }
        }

        private void TxtCurrentPassword_FocusChange1(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                this.ValidateAllPassword();
            }
        }

        private void TxtCurrentPassword_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                this.ValidateAllPassword();
            }
        }

        private void EnableButtonIfAllFilled()
        {
            if(!string.IsNullOrEmpty(txtCurrentPassword.Text) && !string.IsNullOrEmpty(txtNewPassword.Text) && !string.IsNullOrEmpty(txtConfirmPassword.Text))
            {
                this.EnableOnSubmitButton(true);
            }
        }


        private bool ValidateAllPassword()
        {
            try {

                string currentPassword = txtCurrentPassword.Text;
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmPassword.Text;

                this.EnableOnSubmitButton(false);

                bool enableSubmitButton = true;

                if (!string.IsNullOrEmpty(currentPassword))
                {
                    // no implementation for password exist
                }
                else
                {   
                    // current password is required
                    enableSubmitButton = false;
                }

                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (!mPresenter.CheckPasswordIsValid(newPassword))
                    {
                        if (txtInputLayoutNewPassword.Error != Utility.GetLocalizedErrorLabel("invalid_password"))
                        {   //to ensure error were not bouncing
                            txtInputLayoutNewPassword.Error = Utility.GetLocalizedErrorLabel("invalid_password");
                        }

                        //password is not valid
                        enableSubmitButton = false;
                    }
                    else
                    {
                        ClearNewPasswordError();
                    }
                }
                else
                {
                    //new password is required
                    enableSubmitButton = false;
                }

                if (!string.IsNullOrEmpty(confirmPassword))
                {
                    if (!newPassword.Equals(confirmPassword))
                    {
                        //raised error
                        if (txtInputLayoutConfirmPassword.Error != Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword"))
                        {
                            txtInputLayoutConfirmPassword.Error = Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword");
                        }

                        //password was not same
                        enableSubmitButton = false;
                    }
                    else
                    {   
                        ClearConfirmPasswordError();
                    }

                }


                if (enableSubmitButton)
                {
                    this.EnableOnSubmitButton(true);
                    return true;
                }
                else
                {
                    this.EnableOnSubmitButton(false);
                    return false;
                }


            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
                return false;
            }
        }


        //private void OnCurrentPasswordTextChange(object sender, View.FocusChangeEventArgs e)
        //{
        //    try
        //    {

        //        if (!e.HasFocus)
        //        {
        //            string currentPassword = txtCurrentPassword.Text;
        //            string newPassword = txtNewPassword.Text;
        //            string confirmPassword = txtConfirmPassword.Text;

        //            if (!string.IsNullOrEmpty(currentPassword))
        //            {
        //                txtInputLayoutCurrentPassword.PasswordVisibilityToggleEnabled = true;
        //                txtInputLayoutCurrentPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
        //            }
        //            else
        //            {
        //                txtInputLayoutCurrentPassword.PasswordVisibilityToggleEnabled = false;
        //            }

        //            if (!txtInputLayoutNewPassword.ErrorEnabled && !txtInputLayoutConfirmPassword.ErrorEnabled)
        //            {
        //                EnableOnSubmitButton(!string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(confirmPassword));
        //            }
        //            else
        //            {
        //                EnableOnSubmitButton(false);
        //            }
        //        }

               
        //    }
        //    catch (Exception ex)
        //    {
        //        Utility.LoggingNonFatalError(ex);
        //    }
        //}

        //private void OnNewPasswordTextChange(object sender, View.FocusChangeEventArgs e)
        //{
        //    if (!e.HasFocus)
        //    {
        //        try
        //        {
        //            string currentPassword = txtCurrentPassword.Text;
        //            string newPassword = txtNewPassword.Text;
        //            string confirmPassword = txtConfirmPassword.Text;

        //            if (!string.IsNullOrEmpty(newPassword))
        //            {
        //                if (!mPresenter.CheckPasswordIsValid(newPassword))
        //                {
        //                    //ClearNewPasswordError();

        //                    if (txtInputLayoutNewPassword.Error != Utility.GetLocalizedErrorLabel("invalid_password"))
        //                    {
        //                        txtInputLayoutNewPassword.Error = Utility.GetLocalizedErrorLabel("invalid_password");
        //                    }

        //                    if (!txtInputLayoutNewPassword.ErrorEnabled)
        //                        txtInputLayoutNewPassword.ErrorEnabled = true;
        //                }
        //                else
        //                {
        //                    ClearNewPasswordError();
        //                }
        //                txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = true;
        //                txtInputLayoutNewPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
        //            }
        //            else
        //            {
        //                ClearNewPasswordError();
        //                txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = false;
        //            }

        //            if (!string.IsNullOrEmpty(confirmPassword))
        //            {
        //                if (!newPassword.Equals(confirmPassword))
        //                {
        //                    //ClearConfirmPasswordError();
        //                    if (txtInputLayoutConfirmPassword.Error != Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword"))
        //                    {
        //                        txtInputLayoutConfirmPassword.Error = Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword");
        //                    }

        //                    if (!txtInputLayoutConfirmPassword.ErrorEnabled)
        //                        txtInputLayoutConfirmPassword.ErrorEnabled = true;
        //                }
        //                else
        //                {
        //                    ClearConfirmPasswordError();
        //                }
        //                txtInputLayoutConfirmPassword.PasswordVisibilityToggleEnabled = true;
        //                txtInputLayoutConfirmPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
        //            }
        //            else
        //            {
        //                ClearConfirmPasswordError();
        //                txtInputLayoutConfirmPassword.PasswordVisibilityToggleEnabled = false;
        //            }

        //            if (!txtInputLayoutNewPassword.ErrorEnabled && !txtInputLayoutConfirmPassword.ErrorEnabled)
        //            {
        //                EnableOnSubmitButton(!string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(confirmPassword));
        //            }
        //            else
        //            {
        //                EnableOnSubmitButton(false);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Utility.LoggingNonFatalError(ex);
        //        }
        //    }
           
        //}

        //private void OnConfirmPasswordTextChange(object sender, View.FocusChangeEventArgs e)
        //{
        //    if (!e.HasFocus)
        //    {
        //        try
        //        {
        //            string currentPassword = txtCurrentPassword.Text;
        //            string newPassword = txtNewPassword.Text;
        //            string confirmPassword = txtConfirmPassword.Text;

        //            if (!string.IsNullOrEmpty(confirmPassword))
        //            {
        //                if (!newPassword.Equals(confirmPassword))
        //                {
        //                    // ClearConfirmPasswordError();
        //                    if (txtInputLayoutConfirmPassword.Error != Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword"))
        //                    {
        //                        txtInputLayoutConfirmPassword.Error = Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword");
        //                    }

        //                    if (!txtInputLayoutConfirmPassword.ErrorEnabled)
        //                        txtInputLayoutConfirmPassword.ErrorEnabled = true;
        //                }
        //                else
        //                {
        //                    ClearConfirmPasswordError();
        //                }
        //                txtInputLayoutConfirmPassword.PasswordVisibilityToggleEnabled = true;
        //                txtInputLayoutConfirmPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
        //            }
        //            else
        //            {
        //                ClearConfirmPasswordError();
        //                txtInputLayoutConfirmPassword.PasswordVisibilityToggleEnabled = false;
        //            }

        //            if (!txtInputLayoutNewPassword.ErrorEnabled && !txtInputLayoutConfirmPassword.ErrorEnabled)
        //            {
        //                EnableOnSubmitButton(!string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(confirmPassword));
        //            }
        //            else
        //            {
        //                EnableOnSubmitButton(false);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Utility.LoggingNonFatalError(ex);
        //        }
        //    }
          
        //}

        [OnClick(Resource.Id.btnSave)]
        void OnSave(object sender, EventArgs eventArgs)
        {
            try
            {
                string currentPassword = txtCurrentPassword.Text;
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmPassword.Text;

                bool isPasswordPass = this.ValidateAllPassword();

                if (isPasswordPass)
                {
                    this.userActionsListener.OnSave(currentPassword, newPassword, confirmPassword);
                }

               
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
            ClearConfirmPasswordError();
            txtInputLayoutConfirmPassword.Error = GetString(Resource.String.update_password_empty_confirm_new_password);
            if (!txtInputLayoutConfirmPassword.ErrorEnabled)
                txtInputLayoutConfirmPassword.ErrorEnabled = true;
        }

        public void ShowEmptyCurrentPassword()
        {
            ClearCurrentPasswordError();
            txtInputLayoutCurrentPassword.Error = GetString(Resource.String.update_password_empty_current_password);
            if (!txtInputLayoutCurrentPassword.ErrorEnabled)
                txtInputLayoutCurrentPassword.ErrorEnabled = true;
        }

        public void ShowEmptyNewPassword()
        {
            ClearNewPasswordError();
            txtInputLayoutNewPassword.Error = GetString(Resource.String.update_password_empty_new_password);
            if (!txtInputLayoutNewPassword.ErrorEnabled)
                txtInputLayoutNewPassword.ErrorEnabled = true;
        }

        public void ShowInvalidCurrentPassword()
        {
            ClearCurrentPasswordError();
            txtInputLayoutCurrentPassword.Error = GetString(Resource.String.update_password_invalid_current_password);
            if (!txtInputLayoutCurrentPassword.ErrorEnabled)
                txtInputLayoutCurrentPassword.ErrorEnabled = true;
        }

        public void ShowInvalidNewPassword()
        {
            ClearNewPasswordError();
            txtInputLayoutNewPassword.Error = Utility.GetLocalizedErrorLabel("invalid_password");
            if (!txtInputLayoutNewPassword.ErrorEnabled)
                txtInputLayoutNewPassword.ErrorEnabled = true;
        }

        public void ShowNewPasswordNotEqualToConfirmPassword()
        {
            ClearConfirmPasswordError();
            txtInputLayoutConfirmPassword.Error = GetString(Resource.String.update_password_invalid_confirm_password);
            if (!txtInputLayoutConfirmPassword.ErrorEnabled)
                txtInputLayoutConfirmPassword.ErrorEnabled = true;
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
            ClearConfirmPasswordError();
            ClearCurrentPasswordError();
            ClearNewPasswordError();
        }

        private void ClearConfirmPasswordError()
        {
            if (!string.IsNullOrEmpty(txtInputLayoutConfirmPassword.Error))
            {
                txtInputLayoutConfirmPassword.Error = null;
                txtInputLayoutConfirmPassword.ErrorEnabled = false;
            }
        }

        private void ClearCurrentPasswordError()
        {
            if (!string.IsNullOrEmpty(txtInputLayoutCurrentPassword.Error))
            {
                txtInputLayoutCurrentPassword.Error = null;
                txtInputLayoutCurrentPassword.ErrorEnabled = false;
            }
        }

        private void ClearNewPasswordError()
        {
            if (!string.IsNullOrEmpty(txtInputLayoutNewPassword.Error))
            {
                txtInputLayoutNewPassword.Error = null;
                txtInputLayoutNewPassword.ErrorEnabled = false;
            }
        }

        public void ShowProgress()
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

        public void HideProgress()
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
