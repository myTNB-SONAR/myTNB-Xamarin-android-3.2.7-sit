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
using myTNB_Android.Src.Login.MVP;
using CheeseBind;
using Android.Support.Design.Widget;
using Android.Content.PM;
using Refit;
using myTNB_Android.Src.ForgetPassword.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.myTNBMenu.Activity;
using Android.Preferences;
using myTNB_Android.Src.ResetPassword.Activity;
using myTNB_Android.Src.RegistrationForm.Activity;
using System.IO;
using Android.Text;
using myTNB_Android.Src.UpdateMobileNo.Activity;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using myTNB_Android.Src.Login.Requests;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Login.Activity
{
    [Activity( NoHistory = true
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Login")]
    public class LoginActivity : BaseToolbarAppCompatActivity , LoginContract.IView
    {
        public readonly static string TAG = typeof(LoginActivity).Name;
        private LoginPresenter mPresenter;
        private LoginContract.IUserActionsListener userActionsListener;

        private AlertDialog mProgressDialog;

        [BindView(Resource.Id.txtInputLayoutEmail)]
        TextInputLayout txtInputLayoutEmail;

        [BindView(Resource.Id.txtInputLayoutPassword)]
        TextInputLayout txtInputLayoutPassword;



        [BindView(Resource.Id.txtEmail)]
        EditText txtEmail;

        [BindView(Resource.Id.txtPassword)]
        EditText txtPassword;

        [BindView(Resource.Id.txtWelcomeBack)]
        TextView txtWelcomeBack;

        [BindView(Resource.Id.txtAccountLogin)]
        TextView txtAccountLogin;

        [BindView(Resource.Id.txtNoAccount)]
        TextView txtNoAccount;

        [BindView(Resource.Id.txtRegisterAccount)]
        TextView txtRegisterAccount;

        [BindView(Resource.Id.txtForgotPassword)]
        TextView txtForgotPassword;

        [BindView(Resource.Id.btnLogin)]
        Button btnLogin;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.chk_remember_me)]
        CheckBox chkRemeberMe;

        private LoadingOverlay loadingOverlay;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            mPresenter = new LoginPresenter(this , PreferenceManager.GetDefaultSharedPreferences(this));
            mProgressDialog = new AlertDialog.Builder(this)
                .SetTitle(GetString(Resource.String.login_alert_dialog_title))
                .SetMessage(GetString(Resource.String.login_alert_dialog_message))
                .SetNegativeButton(GetString(Resource.String.login_alert_dialog_negative_button), delegate {
                        
                    if (userActionsListener != null)
                    {
                        userActionsListener.CancelLogin();
                    }
                })
                .SetCancelable(false)
                .Create();

            TextViewUtils.SetMuseoSans500Typeface(txtWelcomeBack);
            TextViewUtils.SetMuseoSans300Typeface(txtAccountLogin  , txtEmail , txtPassword , txtNoAccount , txtForgotPassword );
            TextViewUtils.SetMuseoSans500Typeface(txtRegisterAccount);
            TextViewUtils.SetMuseoSans500Typeface(btnLogin);
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutEmail , txtInputLayoutPassword);
            TextViewUtils.SetMuseoSans500Typeface(chkRemeberMe);

            txtPassword.TextChanged += TextChange;
            txtPassword.AddTextChangedListener(new InputFilterFormField(txtPassword, txtInputLayoutPassword));
            txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, txtInputLayoutEmail));

            ISharedPreferences sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            string savedEmail = UserSessions.GetUserEmail(sharedPreferences);
            txtEmail.Append(savedEmail);
        }

        private void TextChange(object sender, TextChangedEventArgs e)
        {
            string password = txtPassword.Text;
            if (!string.IsNullOrEmpty(password))
            {
                
                txtInputLayoutPassword.PasswordVisibilityToggleEnabled = true;
                txtInputLayoutPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
            }
            else
            {
                txtInputLayoutPassword.PasswordVisibilityToggleEnabled = false;
            }
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown && !IsFinishing;
        }

        public override int ResourceId()
        {
            return Resource.Layout.LoginView;
        }

        public void HideProgressDialog()
        {
            //if (mProgressDialog != null && mProgressDialog.IsShowing)
            //{
            //    mProgressDialog.Dismiss();
            //}
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }
        }

        public void ShowProgressDialog()
        {
            //if (mProgressDialog != null && !mProgressDialog.IsShowing)
            //{
            //    mProgressDialog.Show();
            //}
            if (loadingOverlay != null && loadingOverlay.IsShowing)
            {
                loadingOverlay.Dismiss();
            }

            loadingOverlay = new LoadingOverlay(this, Resource.Style.LoadingOverlyDialogStyle);
            loadingOverlay.Show();
        }



        public void SetPresenter(LoginContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowEmptyEmailError()
        {
            ClearErrors();
            this.txtInputLayoutEmail.Error = GetString(Resource.String.login_validation_email_empty_error);
        }

        public void ShowEmptyPasswordError()
        {
            ClearErrors();
            this.txtInputLayoutPassword.Error = GetString(Resource.String.login_validation_password_empty_error);
        }

        public void ShowInvalidEmailError()
        {
            ClearErrors();
            this.txtInputLayoutEmail.Error = GetString(Resource.String.login_validation_email_invalid_error);
        }
        private Snackbar mSnackBar;
        public void ShowInvalidEmailPasswordError(string errorMessage)
        {
            ClearErrors();
            // TODO : SHOW SNACKBAR ERROR MESSAGE
            if (mSnackBar != null && mSnackBar.IsShown)
            {
                mSnackBar.Dismiss();
                mSnackBar.Show();
            }
            else
            {
                mSnackBar = Snackbar.Make(rootView, errorMessage, Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.login_validation_snackbar_btn_close), delegate { mSnackBar.Dismiss(); }
                );
                mSnackBar.Show();
            }


           
        }
    

        public void ShowDashboard()
        {
            // TODO : START ACTIVITY DASHBOARD
            Intent DashboardIntent = new Intent(this, typeof(DashboardActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
        }

        public void ShowForgetPassword()
        {
            // TODO : START ACTIVITY FORGET PASSWORD
            StartActivity(typeof(ForgetPasswordActivity));
        }

        public void ClearErrors()
        {
            this.txtInputLayoutEmail.Error = null;
            this.txtInputLayoutPassword.Error = null;
        }


        [OnClick(Resource.Id.btnLogin)]
        void OnLogin(object sender, EventArgs e)
        {
            string email = txtEmail.Text.ToString().Trim();
            string password = txtPassword.Text;
            this.userActionsListener.LoginAsync(email , password , this.DeviceId(), chkRemeberMe.Checked);
        }

        [OnClick(Resource.Id.txtForgotPassword)]
        void OnForgetPassword(object sender , EventArgs eventArgs)
        {
            this.userActionsListener.NavigateToForgetPassword();
        }

        [OnClick(Resource.Id.txtRegisterAccount)]
        void OnRegisterAccount(object sender , EventArgs eventArgs)
        {
            this.userActionsListener.NavigateToRegistrationForm();
        }

        public void DisableLoginButton()
        {
            btnLogin.Enabled = false;
        }

        public void EnableLoginButton()
        {
            RunOnUiThread( () => btnLogin.Enabled = true );
            
        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.login_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.login_cancelled_exception_btn_retry), delegate {

                mCancelledExceptionSnackBar.Dismiss();
                string email = txtEmail.Text;
                string password = txtPassword.Text;
                this.userActionsListener.LoginAsync(email, password, this.DeviceId(), chkRemeberMe.Checked);
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

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.login_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.login_api_exception_btn_retry), delegate {

                mApiExcecptionSnackBar.Dismiss();
                string email = txtEmail.Text;
                string password = txtPassword.Text;
                this.userActionsListener.LoginAsync(email, password, this.DeviceId(), chkRemeberMe.Checked);
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

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.login_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.login_unknown_exception_btn_retry), delegate {

                mUknownExceptionSnackBar.Dismiss();
                string email = txtEmail.Text;
                string password = txtPassword.Text;
                this.userActionsListener.LoginAsync(email, password, this.DeviceId(), chkRemeberMe.Checked);
            }
            );
            mUknownExceptionSnackBar.Show();

        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public override string ToolbarTitle()
        {
            return "";
        }

        public void ShowResetPassword(string username , string enteredPassword)
        {
            Intent ResetPasswordIntent = new Intent(this, typeof(ResetPasswordActivity));
            ResetPasswordIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            ResetPasswordIntent.PutExtra(Constants.ENTERED_USERNAME , username);
            ResetPasswordIntent.PutExtra(Constants.ENTERED_PASSWORD, enteredPassword);
            StartActivity(ResetPasswordIntent);
        }

        public void ShowRegisterForm()
        {
            StartActivity(typeof(RegistrationFormActivity));
        }

        public string GetCustomerAccountsStub()
        {
            var inputStream = Resources.OpenRawResource(Resource.Raw.GetCustomerBillingAccountListResponse);
            var stringContent = string.Empty;

            using (StreamReader sr = new StreamReader(inputStream))
            {
                stringContent = sr.ReadToEnd();
            }

            return stringContent;
        }

        public string GetLoginResponseStubV4()
        {
            var inputStream = Resources.OpenRawResource(Resource.Raw.UserLoginResponseV4);
            var stringContent = string.Empty;

            using (StreamReader sr = new StreamReader(inputStream))
            {
                stringContent = sr.ReadToEnd();
            }

            return stringContent;
        }

        public string GetLoginResponseStubV5()
        {
            var inputStream = Resources.OpenRawResource(Resource.Raw.UserLoginResponseV5);
            var stringContent = string.Empty;

            using (StreamReader sr = new StreamReader(inputStream))
            {
                stringContent = sr.ReadToEnd();
            }

            return stringContent;
        }

        public string GetCustomerAccountsStubV5()
        {
            var inputStream = Resources.OpenRawResource(Resource.Raw.GetCustomerBillingAccountListResponseV5);
            var stringContent = string.Empty;

            using (StreamReader sr = new StreamReader(inputStream))
            {
                stringContent = sr.ReadToEnd();
            }

            return stringContent;
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        public void ShowNotificationCount(int count)
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

        public void ShowUpdatePhoneNumber(UserAuthenticationRequest request, string phoneNumber)
        {
            Intent updateMobileNo = new Intent(this, typeof(UpdateMobileActivity));
            //updateMobileNo.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            updateMobileNo.PutExtra(Constants.FORCE_UPDATE_PHONE_NO, true);
            updateMobileNo.PutExtra("LoginRequest", JsonConvert.SerializeObject(request));
            updateMobileNo.PutExtra("PhoneNumber", phoneNumber);
            StartActivity(updateMobileNo);
        }
    }
}