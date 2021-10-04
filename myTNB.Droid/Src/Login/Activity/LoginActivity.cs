using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using myTNB.Mobile.SessionCache;
using myTNB_Android.Src.AppLaunch.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.ForgetPassword.Activity;
using myTNB_Android.Src.Login.MVP;
using myTNB_Android.Src.Login.Requests;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.RegistrationForm.Activity;
using myTNB_Android.Src.ResetPassword.Activity;
using myTNB_Android.Src.UpdateMobileNo.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.IO;
using System.Runtime;

namespace myTNB_Android.Src.Login.Activity
{
    [Activity(Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.Login")]
    public class LoginActivity : BaseActivityCustom, LoginContract.IView
    {
        public readonly static string TAG = typeof(LoginActivity).Name;
        private LoginPresenter mPresenter;
        private LoginContract.IUserActionsListener userActionsListener;

        public static string urlSchemaData = "";
        public static string urlSchemaPath = "";

        private AlertDialog mProgressDialog;

        [BindView(Resource.Id.txtInputLayoutEmail)]
        TextInputLayout txtInputLayoutEmail;

        [BindView(Resource.Id.txtInputLayoutPassword)]
        TextInputLayout txtInputLayoutPassword;



        [BindView(Resource.Id.txtEmail)]
        EditText txtEmail;

        [BindView(Resource.Id.txtPassword)]
        EditText txtPassword;

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

        [BindView(Resource.Id.img_logo)]
        ImageView img_logo;

        [BindView(Resource.Id.img_display)]
        ImageView img_display;

        const string PAGE_ID = "Login";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                // Create your application here
                mPresenter = new LoginPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
                mProgressDialog = new AlertDialog.Builder(this)
                    .SetTitle(GetString(Resource.String.login_alert_dialog_title))
                    .SetMessage(GetString(Resource.String.login_alert_dialog_message))
                    .SetNegativeButton(GetString(Resource.String.login_alert_dialog_negative_button), delegate
                    {

                        if (userActionsListener != null)
                        {
                            userActionsListener.CancelLogin();
                        }
                    })
                    .SetCancelable(false)
                    .Create();

                Bundle extras = Intent?.Extras;
                if (extras != null && extras.ContainsKey("urlSchemaData"))
                {
                    urlSchemaData = extras.GetString("urlSchemaData");
                    if (extras != null && extras.ContainsKey("urlSchemaPath"))
                    {
                        urlSchemaPath = extras.GetString("urlSchemaPath");
                    }
                }

                TextViewUtils.SetMuseoSans300Typeface(chkRemeberMe, txtEmail, txtPassword, txtNoAccount);
                TextViewUtils.SetMuseoSans500Typeface(txtRegisterAccount, txtAccountLogin, txtForgotPassword);
                TextViewUtils.SetMuseoSans500Typeface(btnLogin);
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutEmail, txtInputLayoutPassword);

                btnLogin.TextSize = TextViewUtils.GetFontSize(16f);
                txtAccountLogin.TextSize = TextViewUtils.GetFontSize(16f);
                txtEmail.TextSize = TextViewUtils.GetFontSize(16f);
                chkRemeberMe.TextSize = TextViewUtils.GetFontSize(12f);
                txtPassword.TextSize = TextViewUtils.GetFontSize(16f);
                txtForgotPassword.TextSize = TextViewUtils.GetFontSize(12f);
                txtNoAccount.TextSize = TextViewUtils.GetFontSize(12f);
                txtRegisterAccount.TextSize = TextViewUtils.GetFontSize(12f);


                txtAccountLogin.Text = GetLabelByLanguage("loginTitle");
                chkRemeberMe.Text = GetLabelByLanguage("rememberEmail");
                txtForgotPassword.Text = GetLabelByLanguage("forgotPassword");
                txtNoAccount.Text = GetLabelByLanguage("dontHaveAcct");
                txtRegisterAccount.Text = GetLabelByLanguage("registerAcctNow");
                btnLogin.Text = GetLabelByLanguage("login");

                txtInputLayoutEmail.Hint = GetLabelCommonByLanguage("email");
                txtInputLayoutPassword.Hint = GetLabelCommonByLanguage("password");

                txtPassword.TextChanged += TextChange;
                txtPassword.AddTextChangedListener(new InputFilterFormField(txtPassword, txtInputLayoutPassword));
                txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, txtInputLayoutEmail));

                GenerateTopLayoutLayout();

                if (Android.OS.Build.Manufacturer.ToLower() == "samsung")
                {
                    txtEmail.LongClick += (object sender, View.LongClickEventArgs e) => onLongClick(sender, e);
                    txtPassword.LongClick += (object sender, View.LongClickEventArgs e) => onLongClick(sender, e);
                }

                ClearFields();

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            SearchApplicationTypeCache.Instance.Clear();
        }

        public void ClearFields()
        {
            ISharedPreferences sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            string savedEmail = UserSessions.GetUserEmail(sharedPreferences);
            if (!string.IsNullOrEmpty(savedEmail.Trim()))
            {
                txtEmail.Append(savedEmail.Trim());
            }
            else
            {
                txtEmail.Text = "";
            }

            txtPassword.Text = "";
            txtEmail.ClearFocus();
            txtPassword.ClearFocus();
        }

        private bool onLongClick(object sender, View.LongClickEventArgs e)
        {
            // Code to execute on item click.
            return true;
        }


        private void TextChange(object sender, TextChangedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown && !IsFinishing;
        }

        private void GenerateTopLayoutLayout()
        {
            try
            {
                LinearLayout.LayoutParams currentLogoImg = img_logo.LayoutParameters as LinearLayout.LayoutParams;

                int imgWidth = GetDeviceHorizontalScaleInPixel(0.125f);

                currentLogoImg.Height = imgWidth;
                currentLogoImg.Width = imgWidth;

                LinearLayout.LayoutParams currentDisplayLogoImg = img_display.LayoutParameters as LinearLayout.LayoutParams;

                int imgDisplayWidth = GetDeviceHorizontalScaleInPixel(0.634f);

                float heightRatio = 132f / 203f;
                int imgDisplayHeight = (int)(imgDisplayWidth * (heightRatio));

                currentDisplayLogoImg.Height = imgDisplayHeight;
                currentDisplayLogoImg.Width = imgDisplayWidth;

            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Login");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public override int ResourceId()
        {
            return Resource.Layout.LoginView;
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



        public void SetPresenter(LoginContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        private Snackbar mEmptyEmailSnackBar;
        public void ShowEmptyEmailError()
        {
            ClearErrors();
            // TODO : SHOW SNACKBAR ERROR MESSAGE
            if (mEmptyEmailSnackBar != null && mEmptyEmailSnackBar.IsShown)
            {
                mEmptyEmailSnackBar.Dismiss();
                mEmptyEmailSnackBar.Show();
            }
            else
            {
                string errorText = GetLabelByLanguage("emailRequired");
                if (!errorText.Contains("."))
                {
                    errorText = errorText + ".";
                }
                mEmptyEmailSnackBar = Snackbar.Make(rootView, errorText, Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("ok"), delegate { mEmptyEmailSnackBar.Dismiss(); }
                );
                View v = mEmptyEmailSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mEmptyEmailSnackBar.Show();
            }

            this.SetIsClicked(false);
        }

        private Snackbar mEmptyPasswordSnackBar;
        public void ShowEmptyPasswordError()
        {
            ClearErrors();
            // TODO : SHOW SNACKBAR ERROR MESSAGE
            if (mEmptyPasswordSnackBar != null && mEmptyPasswordSnackBar.IsShown)
            {
                mEmptyPasswordSnackBar.Dismiss();
                mEmptyPasswordSnackBar.Show();
            }
            else
            {
                mEmptyPasswordSnackBar = Snackbar.Make(rootView, GetLabelByLanguage("passwordRequired"), Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("ok"), delegate { mEmptyPasswordSnackBar.Dismiss(); }
                );
                View v = mEmptyPasswordSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mEmptyPasswordSnackBar.Show();
            }

            this.SetIsClicked(false);
        }

        private Snackbar mInvalidEmailSnackBar;
        public void ShowInvalidEmailError()
        {
            ClearErrors();
            // TODO : SHOW SNACKBAR ERROR MESSAGE
            if (mInvalidEmailSnackBar != null && mInvalidEmailSnackBar.IsShown)
            {
                mInvalidEmailSnackBar.Dismiss();
                mInvalidEmailSnackBar.Show();
            }
            else
            {
                string errorText = Utility.GetLocalizedErrorLabel("invalid_email");
                if (!errorText.Contains("."))
                {
                    errorText = errorText + ".";
                }

                mInvalidEmailSnackBar = Snackbar.Make(rootView, errorText, Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("ok"), delegate { mInvalidEmailSnackBar.Dismiss(); }
                );
                View v = mInvalidEmailSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mInvalidEmailSnackBar.Show();
            }

            this.SetIsClicked(false);
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
                .SetAction(Utility.GetLocalizedCommonLabel("ok"), delegate { mSnackBar.Dismiss(); }
                );
                View v = mSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);
                mSnackBar.Show();
            }

            this.SetIsClicked(false);

        }


        public void ShowDashboard()
        {
            //Guid
            Guid myuuid = Guid.NewGuid();
            LaunchViewActivity.DynatraceSessionUUID = myuuid.ToString();

            // TODO : START ACTIVITY DASHBOARD
            Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            if (!string.IsNullOrEmpty(urlSchemaData))
            {
                DashboardIntent.PutExtra("urlSchemaData", urlSchemaData);
                if (!string.IsNullOrEmpty(urlSchemaPath))
                {
                    DashboardIntent.PutExtra("urlSchemaPath", urlSchemaPath);
                }
            }
            StartActivity(DashboardIntent);
            urlSchemaPath = "";
            urlSchemaData = "";
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
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    string em_str = txtEmail.Text.ToString().Trim();
                    string pass_str = txtPassword.Text;
                    this.userActionsListener.LoginAsync(em_str, pass_str, this.DeviceId(), chkRemeberMe.Checked);
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [OnClick(Resource.Id.txtForgotPassword)]
        void OnForgetPassword(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.NavigateToForgetPassword();
            }
        }

        [OnClick(Resource.Id.txtRegisterAccount)]
        void OnRegisterAccount(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.NavigateToRegistrationForm();
            }
        }

        public void DisableLoginButton()
        {
            btnLogin.Enabled = false;
        }

        public void EnableLoginButton()
        {
            RunOnUiThread(() => btnLogin.Enabled = true);

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
                string password = txtPassword.Text;
                this.userActionsListener.LoginAsync(email, password, this.DeviceId(), chkRemeberMe.Checked);
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
                string password = txtPassword.Text;
                this.userActionsListener.LoginAsync(email, password, this.DeviceId(), chkRemeberMe.Checked);
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
                string password = txtPassword.Text;
                this.userActionsListener.LoginAsync(email, password, this.DeviceId(), chkRemeberMe.Checked);
            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();

            this.SetIsClicked(false);

        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public override string ToolbarTitle()
        {
            return "";
        }

        public void ShowResetPassword(string u_name, string enteredPass)
        {
            Intent intent = new Intent(this, typeof(ResetPasswordActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            intent.PutExtra(Constants.ENTERED_USERNAME, u_name);
            intent.PutExtra(Constants.ENTERED_PASSWORD, enteredPass);
            StartActivity(intent);
        }

        public void ShowRegisterForm()
        {
            StartActivity(typeof(RegistrationFormActivity));
        }

        public string GetCustomerAccountsStub()
        {

            var stringContent = string.Empty;
            try
            {
                var inputStream = Resources.OpenRawResource(Resource.Raw.GetCustomerBillingAccountListResponse);
                using (StreamReader sr = new StreamReader(inputStream))
                {
                    stringContent = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return stringContent;
        }

        public string GetLoginResponseStubV4()
        {

            var stringContent = string.Empty;
            try
            {
                var inputStream = Resources.OpenRawResource(Resource.Raw.UserLoginResponseV4);
                using (StreamReader sr = new StreamReader(inputStream))
                {
                    stringContent = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return stringContent;
        }

        public string GetLoginResponseStubV5()
        {

            var stringContent = string.Empty;
            try
            {
                var inputStream = Resources.OpenRawResource(Resource.Raw.UserLoginResponseV5);
                using (StreamReader sr = new StreamReader(inputStream))
                {
                    stringContent = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return stringContent;
        }

        public string GetCustomerAccountsStubV5()
        {
            var stringContent = string.Empty;
            try
            {
                var inputStream = Resources.OpenRawResource(Resource.Raw.GetCustomerBillingAccountListResponseV5);

                using (StreamReader sr = new StreamReader(inputStream))
                {
                    stringContent = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
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
            Intent intent = new Intent(this, typeof(UpdateMobileActivity));
            //updateMobileNo.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            intent.PutExtra(Constants.FORCE_UPDATE_PHONE_NO, true);
            intent.PutExtra("LoginRequest", JsonConvert.SerializeObject(request));
            intent.PutExtra("PhoneNumber", phoneNumber);
            StartActivity(intent);
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
