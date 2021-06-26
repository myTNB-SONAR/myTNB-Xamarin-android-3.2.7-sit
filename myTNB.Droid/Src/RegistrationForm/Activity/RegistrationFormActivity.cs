using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;


using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Common.Activity;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.CompoundView;
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
        private MobileNumberInputComponent mobileNumberInputComponent;
        const int COUNTRY_CODE_SELECT_REQUEST = 1;

        Snackbar mRegistrationSnackBar;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.txtFullName)]
        EditText txtFullName;

        [BindView(Resource.Id.txtICNumber)]
        EditText txtICNumber;

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

        [BindView(Resource.Id.textInputLayoutPassword)]
        TextInputLayout textInputLayoutPassword;

        [BindView(Resource.Id.textInputLayoutConfirmPassword)]
        TextInputLayout textInputLayoutConfirmPassword;

        [BindView(Resource.Id.mobileNumberFieldContainer)]
        LinearLayout mobileNumberFieldContainer;

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
                    txtFullName,
                    txtICNumber,
                    txtPassword,
                    txtTermsConditions);

                TextViewUtils.SetMuseoSans300Typeface(textInputLayoutConfirmEmail,
                    textInputLayoutConfirmPassword,
                    textInputLayoutEmail,
                    textInputLayoutFullName,
                    textInputLayoutICNo,
                    textInputLayoutPassword);

                TextViewUtils.SetMuseoSans500Typeface(btnRegister);
                TextViewUtils.SetTextSize12(txtTermsConditions);
                TextViewUtils.SetTextSize16(btnRegister);

                textInputLayoutFullName.Hint = GetLabelCommonByLanguage("fullname");
                textInputLayoutICNo.Hint = GetLabelCommonByLanguage("idNumber");
                textInputLayoutEmail.Hint = GetLabelCommonByLanguage("email");
                textInputLayoutConfirmEmail.Hint = GetLabelByLanguage("confirmEmail");
                textInputLayoutPassword.Hint = GetLabelCommonByLanguage("password");
                textInputLayoutConfirmPassword.Hint = GetLabelCommonByLanguage("confirmPassword");

                txtTermsConditions.TextFormatted = GetFormattedText(GetLabelByLanguage("tnc"));
                StripUnderlinesFromLinks(txtTermsConditions);
                btnRegister.Text = GetLabelByLanguage("ctaTitle");

                txtFullName.TextChanged += TextChange;
                txtICNumber.TextChanged += TextChange;
                txtEmail.TextChanged += TextChange;
                txtConfirmEmail.TextChanged += TextChange;
                txtPassword.TextChanged += TextChange;
                txtConfirmPassword.TextChanged += TextChange;

                txtFullName.AddTextChangedListener(new InputFilterFormField(txtFullName, textInputLayoutFullName));
                txtICNumber.AddTextChangedListener(new InputFilterFormField(txtICNumber, textInputLayoutICNo));
                txtEmail.AddTextChangedListener(new InputFilterFormField(txtEmail, textInputLayoutEmail));
                txtConfirmEmail.AddTextChangedListener(new InputFilterFormField(txtConfirmEmail, textInputLayoutConfirmEmail));
                txtPassword.AddTextChangedListener(new InputFilterFormField(txtPassword, textInputLayoutPassword));
                txtConfirmPassword.AddTextChangedListener(new InputFilterFormField(txtConfirmPassword, textInputLayoutConfirmPassword));


                mobileNumberFieldContainer.RemoveAllViews();
                mobileNumberInputComponent = new MobileNumberInputComponent(this);
                mobileNumberInputComponent.SetOnTapCountryCodeAction(OnTapCountryCode);
                mobileNumberInputComponent.SetMobileNumberLabel(Utility.GetLocalizedCommonLabel("mobileNo"));
                mobileNumberInputComponent.SetSelectedCountry(CountryUtil.Instance.GetDefaultCountry());
                mobileNumberInputComponent.SetValidationAction(OnValidateMobileNumber);
                mobileNumberFieldContainer.AddView(mobileNumberInputComponent);

                ClearFields();

                this.userActionsListener.Start();

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

        private void OnValidateMobileNumber(bool isValidated)
        {
            string fullname = txtFullName.Text.ToString().Trim();
            string ic_no = txtICNumber.Text.ToString().Trim();
            string mobile_no = mobileNumberInputComponent.GetMobileNumberValue();
            string email = txtEmail.Text.ToString().Trim();
            string confirm_email = txtConfirmEmail.Text.ToString().Trim();
            string password = txtPassword.Text;
            string confirm_password = txtConfirmPassword.Text;
            this.userActionsListener.CheckRequiredFields(fullname, ic_no, mobile_no, email, confirm_email, password, confirm_password);
        }

        private void TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                string fullname = txtFullName.Text.ToString().Trim();
                string ic_no = txtICNumber.Text.ToString().Trim();
                string mobile_no = mobileNumberInputComponent.GetMobileNumberValue();
                string email = txtEmail.Text.ToString().Trim();
                string confirm_email = txtConfirmEmail.Text.ToString().Trim();
                string password = txtPassword.Text;
                string confirm_password = txtConfirmPassword.Text;
                this.userActionsListener.CheckRequiredFields(fullname, ic_no, mobile_no, email, confirm_email, password, confirm_password);

                if (!string.IsNullOrEmpty(password))
                {
                    if (!this.mPresenter.CheckPasswordIsValid(password))
                    {
                        // ClearPasswordMinimumOf6CharactersError();
                        if (textInputLayoutPassword.Error != Utility.GetLocalizedErrorLabel("invalid_password"))
                        {
                            textInputLayoutPassword.Error = Utility.GetLocalizedErrorLabel("invalid_password");
                        }

                        if (!textInputLayoutPassword.ErrorEnabled)
                            textInputLayoutPassword.ErrorEnabled = true;
                    }
                    else
                    {
                        ClearPasswordMinimumOf6CharactersError();
                    }
                    TextViewUtils.SetMuseoSans300Typeface(textInputLayoutPassword);
                    textInputLayoutPassword.PasswordVisibilityToggleEnabled = true;
                    textInputLayoutPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
                }
                else
                {
                    ClearPasswordMinimumOf6CharactersError();
                    TextViewUtils.SetMuseoSans300Typeface(textInputLayoutPassword);
                    textInputLayoutPassword.PasswordVisibilityToggleEnabled = false;
                }

                if (!string.IsNullOrEmpty(confirm_password))
                {
                    textInputLayoutConfirmPassword.PasswordVisibilityToggleEnabled = true;
                    textInputLayoutConfirmPassword.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
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
            mobileNumberInputComponent.ClearMobileNumber();
            txtEmail.Text = "";
            txtConfirmEmail.Text = "";
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
            txtFullName.ClearFocus();
            txtICNumber.ClearFocus();
            txtEmail.ClearFocus();
            txtConfirmEmail.ClearFocus();
            txtPassword.ClearFocus();
            txtConfirmPassword.ClearFocus();
        }

        public void ClearAllErrorFields()
        {
            if (!string.IsNullOrEmpty(textInputLayoutEmail.Error))
            {
                textInputLayoutEmail.Error = null;
                textInputLayoutEmail.ErrorEnabled = false;
            }
            if (!string.IsNullOrEmpty(textInputLayoutICNo.Error))
            {
                textInputLayoutICNo.Error = null;
                textInputLayoutICNo.ErrorEnabled = false;
            }
            if (!string.IsNullOrEmpty(textInputLayoutConfirmEmail.Error))
            {
                textInputLayoutConfirmEmail.Error = null;
                textInputLayoutConfirmEmail.ErrorEnabled = false;
            }
            if (!string.IsNullOrEmpty(textInputLayoutPassword.Error))
            {
                textInputLayoutPassword.Error = null;
                textInputLayoutPassword.ErrorEnabled = false;
            }
            if (!string.IsNullOrEmpty(textInputLayoutConfirmPassword.Error))
            {
                textInputLayoutConfirmPassword.Error = null;
                textInputLayoutConfirmPassword.ErrorEnabled = false;
            }
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
            // ClearNotEqualConfirmEmailError();
            if (textInputLayoutConfirmEmail.Error != Utility.GetLocalizedErrorLabel("invalid_email"))
            {
                textInputLayoutConfirmEmail.Error = Utility.GetLocalizedErrorLabel("invalid_email");
            }

            if (!textInputLayoutConfirmEmail.ErrorEnabled)
                textInputLayoutConfirmEmail.ErrorEnabled = true;
        }

        public void ShowEmptyConfirmPasswordError()
        {
            // ClearNotEqualConfirmPasswordError();
            if (textInputLayoutConfirmPassword.Error != GetString(Resource.String.registration_form_errors_empty_confirm_password))
            {
                textInputLayoutConfirmPassword.Error = GetString(Resource.String.registration_form_errors_empty_confirm_password);
            }

            if (!textInputLayoutConfirmPassword.ErrorEnabled)
                textInputLayoutConfirmPassword.ErrorEnabled = true;
        }

        public void ShowEmptyEmailError()
        {
            //ClearInvalidEmailError();
            if (textInputLayoutEmail.Error != GetString(Resource.String.registration_form_errors_empty_email))
            {
                textInputLayoutEmail.Error = GetString(Resource.String.registration_form_errors_empty_email);
            }

            if (!textInputLayoutEmail.ErrorEnabled)
                textInputLayoutEmail.ErrorEnabled = true;
        }

        public void ShowEmptyFullNameError()
        {
            // ClearFullNameError();
            if (textInputLayoutFullName.Error != GetString(Resource.String.registration_form_errors_empty_fullname))
            {
                textInputLayoutFullName.Error = GetString(Resource.String.registration_form_errors_empty_fullname);
            }

            if (!textInputLayoutFullName.ErrorEnabled)
                textInputLayoutFullName.ErrorEnabled = true;
        }

        public void ShowEmptyICNoError()
        {
            // ClearICError();
            if (textInputLayoutICNo.Error != GetString(Resource.String.registration_form_errors_empty_icno))
            {
                textInputLayoutICNo.Error = GetString(Resource.String.registration_form_errors_empty_icno);
            }

            if (!textInputLayoutICNo.ErrorEnabled)
                textInputLayoutICNo.ErrorEnabled = true;
        }

        public void ShowEmptyMobileNoError()
        {
            //textInputLayoutMobileNo.Error = Utility.GetLocalizedErrorLabel("invalid_mobileNumber");
        }

        public void ShowEmptyPasswordError()
        {
            ClearPasswordMinimumOf6CharactersError();
            if (textInputLayoutPassword.Error != GetString(Resource.String.registration_form_errors_empty_password))
            {
                textInputLayoutPassword.Error = GetString(Resource.String.registration_form_errors_empty_password);
            }
            textInputLayoutPassword.Error = GetString(Resource.String.registration_form_errors_empty_password);
            if (!textInputLayoutPassword.ErrorEnabled)
                textInputLayoutPassword.ErrorEnabled = true;
        }

        public void ShowPasswordMinimumOf6CharactersError()
        {
            //ClearPasswordMinimumOf6CharactersError();

            if (textInputLayoutPassword.Error != Utility.GetLocalizedErrorLabel("invalid_password"))
            {
                textInputLayoutPassword.Error = Utility.GetLocalizedErrorLabel("invalid_password");
            }

            textInputLayoutPassword.Error = Utility.GetLocalizedErrorLabel("invalid_password");
            if (!textInputLayoutPassword.ErrorEnabled)
                textInputLayoutPassword.ErrorEnabled = true;
        }

        public void ShowInvalidMobileNoError()
        {
            //textInputLayoutMobileNo.Error = Utility.GetLocalizedErrorLabel("invalid_mobileNumber");
        }

        public void ShowInvalidEmailError()
        {
            //ClearInvalidEmailError();
            if (textInputLayoutEmail.Error != Utility.GetLocalizedErrorLabel("invalid_email"))
            {
                textInputLayoutEmail.Error = Utility.GetLocalizedErrorLabel("invalid_email");
            }

            if (!textInputLayoutEmail.ErrorEnabled)
                textInputLayoutEmail.ErrorEnabled = true;
        }

        public void ShowInvalidICNoError()
        {
            //ClearICError();
            if (textInputLayoutICNo.Error != GetString(Resource.String.registration_form_errors_invalid_icno))
            {
                textInputLayoutICNo.Error = GetString(Resource.String.registration_form_errors_invalid_icno);
            }

            if (!textInputLayoutICNo.ErrorEnabled)
                textInputLayoutICNo.ErrorEnabled = true;
        }

        public void ClearICError()
        {
            if (!string.IsNullOrEmpty(textInputLayoutICNo.Error))
            {
                textInputLayoutICNo.Error = null;
                textInputLayoutICNo.ErrorEnabled = false;
            }
        }

        public void ShowNotEqualConfirmEmailError()
        {
            //ClearNotEqualConfirmEmailError();
            if (textInputLayoutConfirmEmail.Error != Utility.GetLocalizedErrorLabel("invalid_mismatchedEmail"))
            {
                textInputLayoutConfirmEmail.Error = Utility.GetLocalizedErrorLabel("invalid_mismatchedEmail");
            }

            if (!textInputLayoutConfirmEmail.ErrorEnabled)
                textInputLayoutConfirmEmail.ErrorEnabled = true;
        }

        public void ShowNotEqualConfirmPasswordError()
        {
            //ClearNotEqualConfirmPasswordError();
            if (textInputLayoutConfirmPassword.Error != Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword"))
            {
                textInputLayoutConfirmPassword.Error = Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword");
            }

            if (!textInputLayoutConfirmPassword.ErrorEnabled)
                textInputLayoutConfirmPassword.ErrorEnabled = true;
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
                    string mobile_no = mobileNumberInputComponent.GetMobileNumberValueWithISDCode();
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
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mRegistrationSnackBar.Dismiss(); }
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
                string mobile_no = mobileNumberInputComponent.GetMobileNumberValueWithISDCode();
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
                string mobile_no = mobileNumberInputComponent.GetMobileNumberValueWithISDCode();
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
                string mobile_no = mobileNumberInputComponent.GetMobileNumberValueWithISDCode();
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
            //textInputLayoutMobileNo.Error = null;
        }

        public void ClearInvalidEmailError()
        {
            if (!string.IsNullOrEmpty(textInputLayoutEmail.Error))
            {
                textInputLayoutEmail.Error = null;
                textInputLayoutEmail.ErrorEnabled = false;
            }
        }

        public void ClearNotEqualConfirmEmailError()
        {
            if (!string.IsNullOrEmpty(textInputLayoutConfirmEmail.Error))
            {
                textInputLayoutConfirmEmail.Error = null;
                textInputLayoutConfirmEmail.ErrorEnabled = false;
            }
        }

        public void ClearPasswordMinimumOf6CharactersError()
        {
            if (!string.IsNullOrEmpty(textInputLayoutPassword.Error))
            {
                textInputLayoutPassword.Error = null;
                textInputLayoutPassword.ErrorEnabled = false;
            }
        }

        public void ClearNotEqualConfirmPasswordError()
        {
            if (!string.IsNullOrEmpty(textInputLayoutConfirmPassword.Error))
            {
                textInputLayoutConfirmPassword.Error = null;
                textInputLayoutConfirmPassword.ErrorEnabled = false;
            }
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
            // ClearFullNameError();
            if (textInputLayoutFullName.Error != GetString(Resource.String.name_error))
            {
                textInputLayoutFullName.Error = GetString(Resource.String.name_error);
            }
            textInputLayoutFullName.Error = GetString(Resource.String.name_error);
            if (!textInputLayoutFullName.ErrorEnabled)
                textInputLayoutFullName.ErrorEnabled = true;
        }

        public void ClearFullNameError()
        {
            textInputLayoutFullName.Error = null;
            textInputLayoutFullName.ErrorEnabled = false;
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

        public void StripUnderlinesFromLinks(TextView textView)
        {
            var spannable = new SpannableStringBuilder(textView.TextFormatted);
            var spans = spannable.GetSpans(0, spannable.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));
            foreach (URLSpan span in spans)
            {
                var start = spannable.GetSpanStart(span);
                var end = spannable.GetSpanEnd(span);
                spannable.RemoveSpan(span);
                var newSpan = new URLSpanNoUnderline(span.URL);
                spannable.SetSpan(newSpan, start, end, 0);
            }
            textView.TextFormatted = spannable;
        }

        class URLSpanNoUnderline : URLSpan
        {
            public URLSpanNoUnderline(string url) : base(url)
            {
            }

            public override void UpdateDrawState(TextPaint ds)
            {
                base.UpdateDrawState(ds);
                ds.UnderlineText = false;
            }
        }

        private void OnTapCountryCode()
        {
            Intent intent = new Intent(this, typeof(SelectCountryActivity));
            StartActivityForResult(intent, COUNTRY_CODE_SELECT_REQUEST);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (resultCode == Result.Ok)
                {
                    if (requestCode == COUNTRY_CODE_SELECT_REQUEST)
                    {
                        string dataString = data.GetStringExtra(Constants.SELECT_COUNTRY_CODE);
                        Country selectedCountry = JsonConvert.DeserializeObject<Country>(dataString);
                        mobileNumberInputComponent.SetSelectedCountry(selectedCountry);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
