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
using myTNB_Android.Src.ForgetPassword.Activity;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.RegisterValidation;
using myTNB_Android.Src.XEmailRegistrationForm.Models;
using myTNB_Android.Src.XEmailRegistrationForm.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Runtime;
using myTNB_Android.Src.PreLogin.Activity;
using myTNB_Android.Src.RegistrationForm.Activity;
using System.Threading.Tasks;
using Android.Util;
using System.Timers;
using static Android.Resource;

namespace myTNB_Android.Src.XEmailRegistrationForm.Activity
{
    [Activity(Label = "@string/registration_activity_title"
      , NoHistory = false
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class EmailRegistrationFormActivity : BaseActivityCustom, EmailRegisterFormContract.IView, ITextWatcher
    {
        private EmailRegisterFormPresenter mPresenter;
        private EmailRegisterFormContract.IUserActionsListener userActionsListener;

        private AlertDialog mVerificationProgressDialog;
        private AlertDialog mRegistrationProgressDialog;
        const string PAGE_ID = "Register";

        Snackbar mRegistrationSnackBar;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.txtEmailReg)]
        EditText txtEmailReg;

        [BindView(Resource.Id.txtPasswordReg)]
        EditText txtPasswordReg;
       
        [BindView(Resource.Id.textInputLayoutEmailReg)]
        TextInputLayout textInputLayoutEmailReg;

        [BindView(Resource.Id.textInputLayoutPasswordReg)]
        TextInputLayout textInputLayoutPasswordReg;

        [BindView(Resource.Id.btnNext)]
        Button btnNext;

        [BindView(Resource.Id.txtTitleRegister)]
        TextView txtTitleRegister;

        [BindView(Resource.Id.txtBodyRegister)]
        TextView txtBodyRegister;

        Timer searchTimer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            try
            {
                this.mPresenter = new EmailRegisterFormPresenter(this);


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

                TextViewUtils.SetMuseoSans500Typeface(txtTitleRegister);

                TextViewUtils.SetMuseoSans300Typeface(
                    txtEmailReg,
                    txtPasswordReg,
                    txtBodyRegister
                   );

                TextViewUtils.SetMuseoSans300Typeface(
                    textInputLayoutEmailReg,
                    textInputLayoutPasswordReg);

                TextViewUtils.SetMuseoSans500Typeface(btnNext);

                TextViewUtils.SetTextSize18(txtTitleRegister, btnNext);
                TextViewUtils.SetTextSize16(txtEmailReg, txtPasswordReg, txtBodyRegister);

                txtTitleRegister.Text = Utility.GetLocalizedLabel("RegisterNew", "etitleRegister");
                txtBodyRegister.Text = Utility.GetLocalizedLabel("RegisterNew", "ebodyRegister");
                textInputLayoutEmailReg.Hint = Utility.GetLocalizedLabel("Common", "emailAddress");
                textInputLayoutPasswordReg.Hint = Utility.GetLocalizedLabel("Common", "password");
                btnNext.Text = Utility.GetLocalizedLabel("Common", "next");


                //txtTitleRegister.Text = GetLabelCommonByLanguage("etitleRegister");
                //txtBodyRegister.Text = GetLabelCommonByLanguage("ebodyRegister");
                //textInputLayoutEmailReg.Hint = GetLabelCommonByLanguage("email_address");
                //textInputLayoutPasswordReg.Hint = GetLabelCommonByLanguage("password");
                //btnNext.Text = GetLabelCommonByLanguage("next");

                txtEmailReg.FocusChange += txtEmailReg_FocusChange;
                txtPasswordReg.FocusChange += txtPasswordReg_FocusChange;
                txtPasswordReg.AddTextChangedListener(new InputFilterFormField(txtPasswordReg, textInputLayoutPasswordReg));
                txtEmailReg.AddTextChangedListener(new InputFilterFormField(txtEmailReg, textInputLayoutEmailReg));
                txtPasswordReg.TextChanged += TxtPasswordReg_TextChanged;
                txtEmailReg.TextChanged += TxtEmailReg_TextChanged;

                this.userActionsListener.Start();

                ClearFields();
                ClearAllErrorFields();

                txtEmailReg.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.placeholder_email_new, 0, 0, 0);
                txtPasswordReg.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.placeholder_password_new, 0, 0, 0);

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown && !IsFinishing;
        }

        private void TxtEmailReg_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ShowEmailHint();
                ButtonEnable();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void TxtPasswordReg_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ShowPasswordHint();
                ButtonEnable();
                string password = txtPasswordReg.Text.ToString().Trim();
                string email = txtEmailReg.Text.ToString().Trim();

                if (password.Length > 0)
                {
                    // Your code here
                    if (!string.IsNullOrEmpty(password))
                    {
                        textInputLayoutPasswordReg.PasswordVisibilityToggleEnabled = true;
                        textInputLayoutPasswordReg.SetPasswordVisibilityToggleDrawable(Resource.Drawable.selector_password_right_icon);
                    }
                    else
                    {
                        textInputLayoutPasswordReg.PasswordVisibilityToggleEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void txtPasswordReg_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                string email = txtEmailReg.Text.ToString().Trim();
                string password = txtPasswordReg.Text.ToString().Trim();
                this.userActionsListener.validateEmailAndPassword(email, password);

            }
            else
            {
                ShowPasswordHint();
            }
        }

        private void txtEmailReg_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                string email = txtEmailReg.Text.ToString().Trim();
                string password = txtPasswordReg.Text.ToString().Trim();
                this.userActionsListener.validateEmailAndPassword(email, password);
            }
            else
            {
                ShowEmailHint();
            }
        }

        public void ButtonEnable()
        {
            string password = txtPasswordReg.Text.ToString().Trim();
            string email = txtEmailReg.Text.ToString().Trim();

            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(email))
            {
                EnableRegisterButton();
            }
        }

        public override int ResourceId()
        {
            return Resource.Layout.EmailRegistrationView;
        }

        public void SetPresenter(EmailRegisterFormContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowRegister(UserCredentialsEntity userCredentialEntity)
        {
            // TODO : ADD START ACTIVITY REGISTER ACTIVITY
            Intent registrationdetail = new Intent(this, typeof(DetailRegistrationFormActivity));
            registrationdetail.PutExtra(Constants.USER_CREDENTIALS_ENTRY, JsonConvert.SerializeObject(userCredentialEntity));
            StartActivity(registrationdetail);
            //StartActivity(typeof(DetailRegistrationFormActivity));

        }

        [OnClick(Resource.Id.btnNext)]
        void OnRegister(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    string eml_str = txtEmailReg.Text.ToString().Trim();
                    string password = txtPasswordReg.Text;
                    bool valid = this.userActionsListener.validateEmailAndPassword(eml_str, password);
                    if (valid)
                    {
                        this.SetIsClicked(true);
                        this.userActionsListener.OnAcquireToken(eml_str, password);
                    }
                }
                this.SetIsClicked(false);
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }


        public void ClearFields()
        {

            txtEmailReg.Text = "";
            txtPasswordReg.Text = "";

            txtEmailReg.ClearFocus();
            txtPasswordReg.ClearFocus();
        }

        public void ClearAllErrorFields()
        {
            if (!string.IsNullOrEmpty(textInputLayoutEmailReg.Error))
            {
                textInputLayoutEmailReg.Error = null;
                textInputLayoutEmailReg.ErrorEnabled = false;
            }
          
          
            if (!string.IsNullOrEmpty(textInputLayoutPasswordReg.Error))
            {
                textInputLayoutPasswordReg.Error = null;
                textInputLayoutPasswordReg.ErrorEnabled = false;
            }

            if (!string.IsNullOrEmpty(textInputLayoutEmailReg.HelperText))
            {
                textInputLayoutEmailReg.HelperText = null;
                textInputLayoutEmailReg.HelperTextEnabled = false;
            }


            if (!string.IsNullOrEmpty(textInputLayoutPasswordReg.HelperText))
            {
                textInputLayoutPasswordReg.HelperText = null;
                textInputLayoutPasswordReg.HelperTextEnabled = false;
            }

        }
      
        public void ShowBackScreen()
        {
            Finish();
        }
    
        public void ShowEmptyEmailError()
        {
            //ClearInvalidEmailError();
            if(textInputLayoutEmailReg.Error != GetString(Resource.String.registration_form_errors_empty_email))
            {
                textInputLayoutEmailReg.Error = GetString(Resource.String.registration_form_errors_empty_email);
            }
        
            if (!textInputLayoutEmailReg.ErrorEnabled)
                textInputLayoutEmailReg.ErrorEnabled = true;
        }

       
        public void ShowEmptyPasswordError()
        {
            ClearPasswordMinimumOf6CharactersError();
            if (textInputLayoutPasswordReg.Error != GetString(Resource.String.registration_form_errors_empty_password))
            {
                textInputLayoutPasswordReg.Error = GetString(Resource.String.registration_form_errors_empty_password);
            }
            textInputLayoutPasswordReg.Error = GetString(Resource.String.registration_form_errors_empty_password);
            if (!textInputLayoutPasswordReg.ErrorEnabled)
                textInputLayoutPasswordReg.ErrorEnabled = true;
        }

        public void ShowPasswordMinimumOf6CharactersError()
        {
            ClearInvalidPasswordHint();
            textInputLayoutPasswordReg.Error = null;
            if (textInputLayoutPasswordReg.Error != Utility.GetLocalizedErrorLabel("invalid_password")) {
                textInputLayoutPasswordReg.Error = Utility.GetLocalizedErrorLabel("invalid_password");
            }
            if (!textInputLayoutPasswordReg.ErrorEnabled)
                textInputLayoutPasswordReg.ErrorEnabled = true;
        }

        public void ShowPasswordHint()
        {
            ClearPasswordMinimumOf6CharactersError();
            if (textInputLayoutPasswordReg.HelperText != Utility.GetLocalizedLabel("RegisterNew", "passwordHint"))
            {
                textInputLayoutPasswordReg.HelperText = Utility.GetLocalizedLabel("RegisterNew", "passwordHint");
            }

            if (!textInputLayoutPasswordReg.HelperTextEnabled)
                textInputLayoutPasswordReg.HelperTextEnabled = true;
        }

        public void ShowInvalidEmailError()
        {
            ClearInvalidEmailHint();
            textInputLayoutEmailReg.Error = null;
            if (textInputLayoutEmailReg.Error != Utility.GetLocalizedErrorLabel("invalid_email"))
            {
                textInputLayoutEmailReg.Error = Utility.GetLocalizedErrorLabel("invalid_email");
            }
            if (!textInputLayoutEmailReg.ErrorEnabled)
                textInputLayoutEmailReg.ErrorEnabled = true;
        }

        public void ShowEmailHint()
        {
            ClearInvalidEmailError(); 
            if (textInputLayoutEmailReg.HelperText != Utility.GetLocalizedLabel("RegisterNew", "EmailHint"))
            {
                textInputLayoutEmailReg.HelperText = Utility.GetLocalizedLabel("RegisterNew", "EmailHint");
            }

            if (!textInputLayoutEmailReg.HelperTextEnabled)
                textInputLayoutEmailReg.HelperTextEnabled = true;
        }

        // private Snackbar mSnackBar;
        public void ShowInvalidEmailPasswordError()
         {
            string selectedAction = "1";
            this.SetIsClicked(false);
            Utility.ShowEmailErrorDialog(this, selectedAction, () =>
            {
                //ShowProgressDialog();
                if (selectedAction.Equals("1"))
                {
                    ShowForgetPassword();
                }

            });

        }

        public void ShowEmptyEmailErrorNew()
        {
            ClearInvalidEmailHint();
            textInputLayoutEmailReg.Error = null;
            if (textInputLayoutEmailReg.Error != Utility.GetLocalizedLabel("RegisterNew", "emailRequired"))
            {
                textInputLayoutEmailReg.Error = Utility.GetLocalizedLabel("RegisterNew", "emailRequired");
            }

            if (!textInputLayoutEmailReg.ErrorEnabled)
                textInputLayoutEmailReg.ErrorEnabled = true;
        }

        public void ShowEmptyPasswordErrorNew()
        {
            ClearPasswordMinimumOf6CharactersError();
            ClearInvalidPasswordHint();
            textInputLayoutPasswordReg.Error = null;
            if (textInputLayoutPasswordReg.Error != Utility.GetLocalizedLabel("RegisterNew", "passwordRequired"))
            {
                textInputLayoutPasswordReg.Error = Utility.GetLocalizedLabel("RegisterNew", "passwordRequired");
            }
            if (!textInputLayoutPasswordReg.ErrorEnabled)
                textInputLayoutPasswordReg.ErrorEnabled = true;
        }

        public void ShowForgetPassword()
        {
            // TODO : START ACTIVITY FORGET PASSWORD
            StartActivity(typeof(ForgetPasswordActivity));
        }

        public void ClearErrors()
        {
            this.textInputLayoutEmailReg.Error = null;
            this.textInputLayoutPasswordReg.Error = null;
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
               
                string email = txtEmailReg.Text;
                string password = txtPasswordReg.Text;
                
                this.userActionsListener.OnAcquireToken(email, password);

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
                
                string email = txtEmailReg.Text;
                string password = txtPasswordReg.Text;
                
                this.userActionsListener.OnAcquireToken(email,  password);

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
               
               
                string email = txtEmailReg.Text;
                string password = txtPasswordReg.Text;
                
                this.userActionsListener.OnAcquireToken( email,  password);

            }
            );
            mUknownExceptionSnackBar.Show();
            this.SetIsClicked(false);

        }

        private Snackbar mCCErrorSnakebar;
        public void ShowCCErrorSnakebar()
        {
            try
            {
                if (mCCErrorSnakebar != null && mCCErrorSnakebar.IsShown)
                {
                    mCCErrorSnakebar.Dismiss();
                }

                mCCErrorSnakebar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
                .SetAction(GetLabelCommonByLanguage("ok"), delegate
                {

                    mCCErrorSnakebar.Dismiss();
                }
                );
                View v = mCCErrorSnakebar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(6);
                mCCErrorSnakebar.Show();
                this.SetIsClicked(false);
            }
            catch (System.Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
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
            btnNext.Enabled = true;
            btnNext.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
        }

        public void DisableRegisterButton()
        {
            btnNext.Enabled = false;
            btnNext.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

       

        public void ClearInvalidEmailError()
        {
            if (!string.IsNullOrEmpty(textInputLayoutEmailReg.Error))
            {
                textInputLayoutEmailReg.Error = null;
                textInputLayoutEmailReg.ErrorEnabled = false;
            }
            else
            {
                textInputLayoutEmailReg.Error = null;
                textInputLayoutEmailReg.ErrorEnabled = false;
            }
        }

        public void ClearInvalidEmailHint()
        {
            textInputLayoutEmailReg.HelperText = null;
            textInputLayoutEmailReg.HelperTextEnabled = false;
        }

        public void ClearPasswordMinimumOf6CharactersError()
        {
            if (!string.IsNullOrEmpty(textInputLayoutPasswordReg.Error))
            {
                textInputLayoutPasswordReg.Error = null;
                textInputLayoutPasswordReg.ErrorEnabled = false;
            }
            else
            {
                textInputLayoutPasswordReg.Error = null;
                textInputLayoutPasswordReg.ErrorEnabled = false;
            }

        }

        public void ClearInvalidPasswordHint()
        {
            textInputLayoutPasswordReg.HelperText = null;
            textInputLayoutPasswordReg.HelperTextEnabled = false;
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
                //this.userActionsListener.NavigateToTermsAndConditions();
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

        public void AfterTextChanged(IEditable s)
        {
            throw new NotImplementedException();

        }

        public void BeforeTextChanged(Java.Lang.ICharSequence s, int start, int count, int after)
        {
            throw new NotImplementedException();
        }

        public void OnTextChanged(Java.Lang.ICharSequence s, int start, int before, int count)
        {
            throw new NotImplementedException();
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

    }
}
