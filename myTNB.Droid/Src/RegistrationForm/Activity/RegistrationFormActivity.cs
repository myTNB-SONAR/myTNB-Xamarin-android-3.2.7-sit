using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.RegisterValidation;
using myTNB_Android.Src.RegistrationForm.Models;
using myTNB_Android.Src.RegistrationForm.MVP;
using myTNB_Android.Src.TermsAndConditions.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Runtime;

namespace myTNB_Android.Src.RegistrationForm.Activity
{
    [Activity(Label = "@string/registration_activity_title"
      , NoHistory = false
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.RegisterForm")]
    public class RegistrationFormActivity : BaseActivityCustom, RegisterFormContract.IView
    {
        private RegisterFormPresenter mPresenter;
        private RegisterFormContract.IUserActionsListener userActionsListener;

        private AlertDialog mVerificationProgressDialog;
        private AlertDialog mRegistrationProgressDialog;
        const string PAGE_ID = "Register";

        Snackbar mRegistrationSnackBar;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.txtFullName)]
        EditText txtFullName;

        [BindView(Resource.Id.txtICNumber)]
        EditText txtICNumber;

        [BindView(Resource.Id.txtMobileNumber)]
        EditText txtMobileNumber;

        [BindView(Resource.Id.txtEmail)]
        EditText txtEmail;

        [BindView(Resource.Id.txtConfirmEmail)]
        EditText txtConfirmEmail;

        [BindView(Resource.Id.txtPassword)]
        EditText txtPassword;

        [BindView(Resource.Id.txtConfirmPassword)]
        EditText txtConfirmPassword;

        [BindView(Resource.Id.txtTermsConditions)]
        TextView txtTermsConditions;

        [BindView(Resource.Id.textInputLayoutFullName)]
        TextInputLayout textInputLayoutFullName;

        [BindView(Resource.Id.textInputLayoutEmail)]
        TextInputLayout textInputLayoutEmail;

        [BindView(Resource.Id.textInputLayoutConfirmEmail)]
        TextInputLayout textInputLayoutConfirmEmail;

        [BindView(Resource.Id.textInputLayoutICNo)]
        TextInputLayout textInputLayoutICNo;

        [BindView(Resource.Id.textInputLayoutMobileNo)]
        TextInputLayout textInputLayoutMobileNo;

        [BindView(Resource.Id.textInputLayoutPassword)]
        TextInputLayout textInputLayoutPassword;

        [BindView(Resource.Id.textInputLayoutConfirmPassword)]
        TextInputLayout textInputLayoutConfirmPassword;



        [BindView(Resource.Id.btnRegister)]
        Button btnRegister;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            try
            {
                this.mPresenter = new RegisterFormPresenter(this);


                mVerificationProgressDialog = new AlertDialog.Builder(this)
                    .SetTitle(GetString(Resource.String.verification_alert_dialog_title))
                    .SetMessage(GetString(Resource.String.verification_alert_dialog_message))
                    .SetCancelable(false)
                    .Create();

                mRegistrationProgressDialog = new AlertDialog.Builder(this)
                     .SetTitle(GetString(Resource.String.registration_alert_dialog_title))
                     .SetMessage(GetString(Resource.String.registration_alert_dialog_message))
                     .SetCancelable(false)
                     .Create();

                TextViewUtils.SetMuseoSans300Typeface(txtConfirmEmail,
                    txtConfirmPassword,
                    txtEmail,
                    txtMobileNumber,
                    txtFullName,
                    txtICNumber,
                    txtPassword,
                    txtTermsConditions);

                TextViewUtils.SetMuseoSans300Typeface(textInputLayoutConfirmEmail,
                    textInputLayoutConfirmPassword,
                    textInputLayoutEmail,
                    textInputLayoutFullName,
                    textInputLayoutMobileNo,
                    textInputLayoutICNo,
                    textInputLayoutPassword);

                TextViewUtils.SetMuseoSans500Typeface(btnRegister);

                textInputLayoutFullName.Hint = GetLabelCommonByLanguage("fullname");
                textInputLayoutICNo.Hint = GetLabelCommonByLanguage("idNumber");
                textInputLayoutMobileNo.Hint = GetLabelCommonByLanguage("mobileNo");
                textInputLayoutEmail.Hint = GetLabelCommonByLanguage("email");
                textInputLayoutConfirmEmail.Hint = GetLabelByLanguage("confirmEmail");
                textInputLayoutPassword.Hint = GetLabelCommonByLanguage("password");
                textInputLayoutConfirmPassword.Hint = Utility.GetLocalizedLabel("ResetPassword", "confirmNewPassword");

                txtTermsConditions.TextFormatted = GetFormattedText(GetLabelByLanguage("tnc"));
                ClickSpan clickableSpan = new ClickSpan();
                clickableSpan.Click += delegate
                {
                    if (!this.GetIsClicked())
                    {
                        this.SetIsClicked(true);
                        this.userActionsListener.NavigateToTermsAndConditions();
                    }
                };
                txtTermsConditions.TextFormatted = Utility.GetFormattedURLString(clickableSpan, txtTermsConditions.TextFormatted);
                txtTermsConditions.MovementMethod = new LinkMovementMethod();
                btnRegister.Text = GetLabelByLanguage("ctaTitle");

                //var inputFilter = new InputFilterPhoneNumber();
                //txtMobileNumber.AddTextChangedListener(inputFilter);
                //txtMobileNumber.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
                //{
                //    if (e.HasFocus)
                //    {
                //        if (string.IsNullOrEmpty(txtMobileNumber.Text))
                //        {
                //            txtMobileNumber.Append("+60");
                //        }

                //    }

                //};

                ClearFields();

                txtFullName.TextChanged += TextChange;
                txtICNumber.TextChanged += TextChange;
                txtMobileNumber.TextChanged += TextChange;
                txtEmail.TextChanged += TextChange;
                txtConfirmEmail.TextChanged += TextChange;
                txtPassword.TextChanged += TextChange;
                txtConfirmPassword.TextChanged += TextChange;

                txtFullName.AddTextChangedListener(new InputFilterFormField(txtFullName, textInputLayoutFullName));
                txtICNumber.AddTextChangedListener(new InputFilterFormField(txtICNumber, textInputLayoutICNo));
                txtMobileNumber.AddTextChangedListener(new InputFilterFormField(txtMobileNumber, textInputLayoutMobileNo));
                txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, textInputLayoutEmail));
                txtConfirmEmail.AddTextChangedListener(new InputFilterFormField(txtConfirmEmail, textInputLayoutConfirmEmail));
                txtPassword.AddTextChangedListener(new InputFilterFormField(txtPassword, textInputLayoutPassword));
                txtConfirmPassword.AddTextChangedListener(new InputFilterFormField(txtConfirmPassword, textInputLayoutConfirmPassword));

                this.userActionsListener.Start();

                txtMobileNumber.Append("+60");
                txtMobileNumber.SetFilters(new Android.Text.IInputFilter[] { new InputFilterPhoneNumber() });
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            //#if DEBUG
            //            txtFullName.Text = "David Montecillo";
            //            txtICNumber.Text = "123131312";
            //            txtMobileNumber.Text = "639299920799";
            //            txtEmail.Text = "montecillodavid.acn1001@gmail.com";
            //            txtConfirmEmail.Text = "montecillodavid.acn1001@gmail.com";
            //            txtPassword.Text = "password123";
            //            txtConfirmPassword.Text = "password123";
            //#endif
        }


        private void TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                string fullname = txtFullName.Text.ToString().Trim();
                string ic_no = txtICNumber.Text.ToString().Trim();
                string mobile_no = txtMobileNumber.Text.ToString().Trim();
                string email = txtEmail.Text.ToString().Trim();
                string confirm_email = txtConfirmEmail.Text.ToString().Trim();
                string password = txtPassword.Text;
                string confirm_password = txtConfirmPassword.Text;
                this.userActionsListener.CheckRequiredFields(fullname, ic_no, mobile_no, email, confirm_email, password, confirm_password);

                if (!string.IsNullOrEmpty(password))
                {
                    textInputLayoutPassword.Error = GetString(Resource.String.registration_form_password_format_hint);
                    textInputLayoutPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomHint);
                    TextViewUtils.SetMuseoSans300Typeface(textInputLayoutPassword);
                    textInputLayoutPassword.PasswordVisibilityToggleEnabled = true;
                }
                else
                {
                    textInputLayoutPassword.Error = "";
                    textInputLayoutPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                    TextViewUtils.SetMuseoSans300Typeface(textInputLayoutPassword);
                    textInputLayoutPassword.PasswordVisibilityToggleEnabled = false;
                }

                if (!string.IsNullOrEmpty(confirm_password))
                {
                    textInputLayoutConfirmPassword.PasswordVisibilityToggleEnabled = true;
                }
                else
                {
                    textInputLayoutConfirmPassword.PasswordVisibilityToggleEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }



        public override int ResourceId()
        {
            return Resource.Layout.RegistrationFormView;
        }

        public void ClearFields()
        {
            txtFullName.Text = "";
            txtICNumber.Text = "";
            txtMobileNumber.Text = "";
            txtEmail.Text = "";
            txtConfirmEmail.Text = "";
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
            txtFullName.ClearFocus();
            txtICNumber.ClearFocus();
            txtMobileNumber.ClearFocus();
            txtEmail.ClearFocus();
            txtConfirmEmail.ClearFocus();
            txtPassword.ClearFocus();
            txtConfirmPassword.ClearFocus();
        }

        public void ClearAllErrorFields()
        {
            textInputLayoutEmail.Error = null;
            textInputLayoutICNo.Error = null;
            textInputLayoutMobileNo.Error = null;
            textInputLayoutEmail.Error = null;
            textInputLayoutConfirmEmail.Error = null;
            textInputLayoutPassword.Error = null;
            textInputLayoutConfirmPassword.Error = null;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }


        public void SetPresenter(RegisterFormContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowBackScreen()
        {
            Finish();
        }

        public void ShowEmptyConfirmEmailError()
        {
            textInputLayoutConfirmEmail.Error = GetString(Resource.String.registration_form_errors_empty_confirm_email);
        }

        public void ShowEmptyConfirmPasswordError()
        {
            textInputLayoutConfirmPassword.Error = GetString(Resource.String.registration_form_errors_empty_confirm_password);
        }

        public void ShowEmptyEmailError()
        {
            textInputLayoutEmail.Error = null;
            textInputLayoutEmail.Error = GetString(Resource.String.registration_form_errors_empty_email);
        }

        public void ShowEmptyFullNameError()
        {
            textInputLayoutFullName.Error = GetString(Resource.String.registration_form_errors_empty_fullname);
        }

        public void ShowEmptyICNoError()
        {
            textInputLayoutICNo.Error = GetString(Resource.String.registration_form_errors_empty_icno);
        }

        public void ShowEmptyMobileNoError()
        {
            textInputLayoutMobileNo.Error = GetString(Resource.String.registration_form_errors_empty_mobile_no);
        }

        public void ShowEmptyPasswordError()
        {
            textInputLayoutPassword.Error = GetString(Resource.String.registration_form_errors_empty_password);
        }

        public void ShowPasswordMinimumOf6CharactersError()
        {
            textInputLayoutPassword.Error = Utility.GetLocalizedErrorLabel("invalid_password");
            textInputLayoutPassword.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
        }

        public void ShowInvalidMobileNoError()
        {
            textInputLayoutMobileNo.Error = GetString(Resource.String.registration_form_errors_invalid_mobile_no);
        }

        public void ShowInvalidEmailError()
        {
            textInputLayoutEmail.Error = Utility.GetLocalizedErrorLabel("invalid_email");
        }

        public void ShowInvalidICNoError()
        {
            textInputLayoutICNo.Error = GetString(Resource.String.registration_form_errors_invalid_icno);
        }

        public void ShowNotEqualConfirmEmailError()
        {
            textInputLayoutConfirmEmail.Error = Utility.GetLocalizedErrorLabel("invalid_mismatchedEmail");
        }

        public void ShowNotEqualConfirmPasswordError()
        {
            textInputLayoutConfirmPassword.Error = GetString(Resource.String.registration_form_errors_not_equal_confirm_password);
        }

        public void ShowTermsAndConditions()
        {
            StartActivity(typeof(TermsAndConditionActivity));
        }


        [OnClick(Resource.Id.btnRegister)]
        void OnRegister(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    this.SetIsClicked(true);
                    string fName = txtFullName.Text.ToString().Trim();
                    string ic_no = txtICNumber.Text.ToString().Trim();
                    string mobile_no = txtMobileNumber.Text.ToString().Trim();
                    string eml_str = txtEmail.Text.ToString().Trim();
                    string confirm_email = txtConfirmEmail.Text.ToString().Trim();
                    string password = txtPassword.Text;
                    string confirm_password = txtConfirmPassword.Text;
                    this.userActionsListener.OnAcquireToken(fName, ic_no, mobile_no, eml_str, confirm_email, password, confirm_password);
                }
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }


        [OnClick(Resource.Id.txtTermsConditions)]
        void OnTermsConditions(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.NavigateToTermsAndConditions();
            }
        }


        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Register New User");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowVerificationCodeProgressDialog()
        {
            try
            {
                if (this.mVerificationProgressDialog != null && !this.mVerificationProgressDialog.IsShowing)
                {
                    this.mVerificationProgressDialog.Show();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideVerificationCodeProgressDialog()
        {
            try
            {
                if (this.mVerificationProgressDialog != null && this.mVerificationProgressDialog.IsShowing)
                {
                    this.mVerificationProgressDialog.Dismiss();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowRegistrationProgressDialog()
        {
            try
            {
                if (this.mRegistrationProgressDialog != null && !this.mRegistrationProgressDialog.IsShowing)
                {
                    this.mRegistrationProgressDialog.Show();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideRegistrationProgressDialog()
        {
            try
            {
                if (this.mRegistrationProgressDialog != null && this.mRegistrationProgressDialog.IsShowing)
                {
                    this.mRegistrationProgressDialog.Dismiss();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



        public void ShowRegisterValidation(UserCredentialsEntity userCredentialEntity)
        {
            Intent registrationValidation = new Intent(this, typeof(RegisterValidationActivity));
            registrationValidation.PutExtra(Constants.USER_CREDENTIALS_ENTRY, JsonConvert.SerializeObject(userCredentialEntity));
            StartActivity(registrationValidation);
        }



        public void ShowInvalidAcquiringTokenThruSMS(string errorMessage)
        {
            if (mRegistrationSnackBar != null && mRegistrationSnackBar.IsShown)
            {
                mRegistrationSnackBar.Dismiss();
            }

            mRegistrationSnackBar = Snackbar.Make(rootView, errorMessage, Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.registration_failed_btn_close), delegate { mRegistrationSnackBar.Dismiss(); }
            );
            View v = mRegistrationSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mRegistrationSnackBar.Show();
            this.SetIsClicked(false);
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
                string fullname = txtFullName.Text;
                string ic_no = txtICNumber.Text;
                string mobile_no = txtMobileNumber.Text;
                string email = txtEmail.Text;
                string confirm_email = txtConfirmEmail.Text;
                string password = txtPassword.Text;
                string confirm_password = txtConfirmPassword.Text;
                this.userActionsListener.OnAcquireToken(fullname, ic_no, mobile_no, email, confirm_email, password, confirm_password);

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
                string fullname = txtFullName.Text;
                string ic_no = txtICNumber.Text;
                string mobile_no = txtMobileNumber.Text;
                string email = txtEmail.Text;
                string confirm_email = txtConfirmEmail.Text;
                string password = txtPassword.Text;
                string confirm_password = txtConfirmPassword.Text;
                this.userActionsListener.OnAcquireToken(fullname, ic_no, mobile_no, email, confirm_email, password, confirm_password);

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
                string fullname = txtFullName.Text;
                string ic_no = txtICNumber.Text;
                string mobile_no = txtMobileNumber.Text;
                string email = txtEmail.Text;
                string confirm_email = txtConfirmEmail.Text;
                string password = txtPassword.Text;
                string confirm_password = txtConfirmPassword.Text;
                this.userActionsListener.OnAcquireToken(fullname, ic_no, mobile_no, email, confirm_email, password, confirm_password);

            }
            );
            mUknownExceptionSnackBar.Show();
            this.SetIsClicked(false);

        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override bool TelephonyPermissionRequired()
        {
            return false;
        }

        public override void Ready()
        {
            base.Ready();
        }

        public void EnableRegisterButton()
        {
            btnRegister.Enabled = true;
            btnRegister.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }

        public void DisableRegisterButton()
        {
            btnRegister.Enabled = false;
            btnRegister.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public void ClearInvalidMobileError()
        {
            textInputLayoutMobileNo.Error = null;
        }

        public void ClearInvalidEmailError()
        {
            textInputLayoutEmail.Error = null;
        }

        public void ClearNotEqualConfirmEmailError()
        {
            textInputLayoutConfirmEmail.Error = null;
        }

        public void ClearPasswordMinimumOf6CharactersError()
        {
            textInputLayoutPassword.Error = null;
        }

        public void ClearNotEqualConfirmPasswordError()
        {
            textInputLayoutConfirmPassword.Error = null;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                this.userActionsListener.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void RequestSMSPermission()
        {
            RequestPermissions(new string[] { Manifest.Permission.ReceiveSms }, Constants.RUNTIME_PERMISSION_SMS_REQUEST_CODE);
        }

        private Snackbar mSnackBar;
        public void ShowSMSPermissionRationale()
        {
            if (mSnackBar != null && mSnackBar.IsShown)
            {
                mSnackBar.Dismiss();
                mSnackBar.Show();
            }
            else
            {
                mSnackBar = Snackbar.Make(rootView, GetString(Resource.String.runtime_permission_sms_received_rationale), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.runtime_permission_dialog_btn_show), delegate
                {
                    mSnackBar.Dismiss();
                    this.userActionsListener.OnRequestSMSPermission();
                }
                );

                View v = mSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                if (tv != null)
                {
                    tv.SetMaxLines(5);
                }
                mSnackBar.Show();
            }
            this.SetIsClicked(false);
        }

        public bool IsGrantedSMSReceivePermission()
        {
            return ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReceiveSms) == (int)Permission.Granted;
        }

        public bool ShouldShowSMSReceiveRationale()
        {
            return ShouldShowRequestPermissionRationale(Manifest.Permission.ReceiveSms);
        }


        public void ShowFullNameError()
        {
            textInputLayoutFullName.Error = GetString(Resource.String.name_error);
        }

        public void ClearFullNameError()
        {
            textInputLayoutFullName.Error = null;
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

        public void OnClickSpan(string textMessage)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.userActionsListener.NavigateToTermsAndConditions();
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }

    class ClickSpan : ClickableSpan
    {
        public Action<View> Click;
        public override void OnClick(View widget)
        {
            if (Click != null)
            {
                Click(widget);
            }
        }

        public override void UpdateDrawState(TextPaint ds)
        {
            base.UpdateDrawState(ds);
            ds.UnderlineText = false;
        }
    }
}
