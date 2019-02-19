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
using myTNB_Android.Src.UpdatePassword.MVP;
using Refit;
using Android.Support.Design.Widget;
using CheeseBind;
using myTNB_Android.Src.Utils;
using AFollestad.MaterialDialogs;
using Android.Text;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;

namespace myTNB_Android.Src.UpdatePassword.Activity
{
    [Activity(Label = "@string/update_password_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.UpdatePassword")]
    public class UpdatePasswordActivity : BaseToolbarAppCompatActivity , UpdatePasswordContract.IView
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

        UpdatePasswordContract.IUserActionsListener userActionsListener;
        UpdatePasswordPresenter mPresenter;




        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutConfirmPassword,
                txtInputLayoutCurrentPassword,
                txtInputLayoutNewPassword);

            TextViewUtils.SetMuseoSans300Typeface(txtCurrentPassword,
                txtNewPassword,
                txtConfirmPassword);

            TextViewUtils.SetMuseoSans500Typeface(btnSave);

            txtCurrentPassword.TextChanged += TextChange;
            txtNewPassword.TextChanged += TextChange;
            txtConfirmPassword.TextChanged += TextChange;

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
            this.userActionsListener.Start();
        }

        private void TextChange(object sender, TextChangedEventArgs e)
        {

            string currentPassword = txtCurrentPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (!string.IsNullOrEmpty(currentPassword))
            {
                txtInputLayoutCurrentPassword.PasswordVisibilityToggleEnabled = true;
            }
            else
            {
                txtInputLayoutCurrentPassword.PasswordVisibilityToggleEnabled = false;
            }

            if (!string.IsNullOrEmpty(newPassword))
            {
                txtInputLayoutNewPassword.Error = GetString(Resource.String.registration_form_password_format_hint);
                txtInputLayoutNewPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomHint);
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewPassword);
                txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = true;
            }
            else
            {
                txtInputLayoutNewPassword.Error = "";
                txtInputLayoutNewPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutNewPassword);
                txtInputLayoutNewPassword.PasswordVisibilityToggleEnabled = false;
            }

            if (!string.IsNullOrEmpty(confirmPassword))
            {
                txtInputLayoutConfirmPassword.PasswordVisibilityToggleEnabled = true;
            }
            else
            {
                txtInputLayoutConfirmPassword.PasswordVisibilityToggleEnabled = false;
            }
        }

        [OnClick(Resource.Id.btnSave)]
        void OnSave(object sender, EventArgs eventArgs)
        {
            string currentPassword = txtCurrentPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            this.userActionsListener.OnSave(currentPassword , newPassword , confirmPassword);
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
            txtInputLayoutCurrentPassword.Error = GetString(Resource.String.update_password_empty_current_password); ;
        }

        public void ShowEmptyNewPassword()
        {
            txtInputLayoutNewPassword.Error = GetString(Resource.String.update_password_empty_new_password);
            txtInputLayoutNewPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
        }

        public void ShowInvalidCurrentPassword()
        {
            txtInputLayoutCurrentPassword.Error = GetString(Resource.String.update_password_invalid_current_password);
            txtInputLayoutNewPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
        }

        public void ShowInvalidNewPassword()
        {
            txtInputLayoutNewPassword.Error = GetString(Resource.String.update_password_invalid_new_password);
            txtInputLayoutNewPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
        }

        public void ShowNewPasswordNotEqualToConfirmPassword()
        {
            txtInputLayoutConfirmPassword.Error = GetString(Resource.String.update_password_invalid_confirm_password);
            txtInputLayoutNewPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.update_password_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.update_password_cancelled_exception_btn_close), delegate {

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

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.update_password_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.update_password_api_exception_btn_close), delegate {

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

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.update_password_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.update_password_unknown_exception_btn_close), delegate {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
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
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }

            loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
            loadingOverlay.Show();
        }

        public void HideProgress()
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

        private Snackbar mErrorSnackbar;
        public void ShowErrorMessage(string message)
        {
            if (mErrorSnackbar != null && mErrorSnackbar.IsShown)
            {
                mErrorSnackbar.Dismiss();
            }

            mErrorSnackbar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.update_password_cancelled_exception_btn_close), delegate {

                mErrorSnackbar.Dismiss();
            }
            );
            mErrorSnackbar.Show();
        }

        public void ShowSuccess()
        {
            SetResult(Result.Ok);
            Finish();
        }
    }
}