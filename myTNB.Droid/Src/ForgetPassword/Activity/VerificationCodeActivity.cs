using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;

using Android.Text;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.ForgetPassword.MVP;
using myTNB.Android.Src.Utils;
using myTNB.Android.Src.Utils.Custom.ProgressButton;
using Refit;
using System;
using System.Linq;
using System.Runtime;
using myTNB.Android.Src.ForgotPasswordVerificationCodeSuccess.Activity;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using AndroidX.Core.Content;

namespace myTNB.Android.Src.ForgetPassword.Activity
{
    [Activity(Label = "@string/forget_password_verfication_code_activity_title"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.RegisterValidation")]
    public class VerificationCodeActivity : BaseActivityCustom, ForgetPasswordContract.IView, ProgressGenerator.OnProgressListener
    {
        private ForgetPasswordPresenter mPresenter;
        private ForgetPasswordContract.IUserActionsListener userActionsListener;
        private ProgressGenerator progressGenerator;

        private MaterialDialog mVerificationProgressDialog;

        private Snackbar mSnackBar;



        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.btnResend)]
        ResendButton btnResend;

        [BindView(Resource.Id.re_send_btn)]
        Button OnCompleteResend;

        [BindView(Resource.Id.txtInfoTitle)]
        TextView txtInfoTitle;

        [BindView(Resource.Id.txtDidntReceive)]
        TextView txtDidntReceive;

        [BindView(Resource.Id.txtNumber_1)]
        EditText txtNumber_1;

        [BindView(Resource.Id.txtNumber_2)]
        EditText txtNumber_2;

        [BindView(Resource.Id.txtNumber_3)]
        EditText txtNumber_3;

        [BindView(Resource.Id.txtNumber_4)]
        EditText txtNumber_4;

        [BindView(Resource.Id.txtInputLayoutNumber_1)]
        TextInputLayout txtInputLayoutNumber_1;

        [BindView(Resource.Id.txtInputLayoutNumber_2)]
        TextInputLayout txtInputLayoutNumber_2;

        [BindView(Resource.Id.txtInputLayoutNumber_3)]
        TextInputLayout txtInputLayoutNumber_3;

        [BindView(Resource.Id.txtInputLayoutNumber_4)]
        TextInputLayout txtInputLayoutNumber_4;

        [BindView(Resource.Id.txtErrorPin)]
        TextView txtErrorPin;

        private string email;
        private bool resendCalled = false;

        const string PAGE_ID = "EnterCode";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                mPresenter = new ForgetPasswordPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));

                mVerificationProgressDialog = new MaterialDialog.Builder(this)
                    .Title(Resource.String.forget_password_validation_progress_title)
                    .Content(Resource.String.forget_password_validation_progress_message)
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();

                Bundle extras = Intent.Extras;
                email = extras.GetString("email");

                txtInfoTitle.Text = string.Format(GetLabelByLanguage("details"), email);
                TextViewUtils.SetMuseoSans300Typeface(txtInfoTitle, txtDidntReceive);
                TextViewUtils.SetMuseoSans300Typeface(txtNumber_1, txtNumber_2, txtNumber_3, txtNumber_4);
                TextViewUtils.SetMuseoSans500Typeface(btnResend, OnCompleteResend);
                TextViewUtils.SetMuseoSans300Typeface(txtErrorPin);
                TextViewUtils.SetTextSize12(txtErrorPin);
                TextViewUtils.SetTextSize16(txtInfoTitle, txtDidntReceive);
                TextViewUtils.SetTextSize18(OnCompleteResend, btnResend);
                TextViewUtils.SetTextSize22(txtNumber_1, txtNumber_2, txtNumber_3, txtNumber_4);

                txtDidntReceive.Text = GetLabelByLanguage("emailNotReceived");
                btnResend.Text = Utility.GetLocalizedLabel("Common", "resend");
                OnCompleteResend.Text = Utility.GetLocalizedLabel("Common", "resend");
                txtErrorPin.Text = Utility.GetLocalizedErrorLabel("invalid_pin");
                txtErrorPin.Visibility = ViewStates.Gone;

                txtNumber_1.TextChanged += TxtNumber_1_TextChanged;
                txtNumber_2.TextChanged += TxtNumber_2_TextChanged;
                txtNumber_3.TextChanged += TxtNumber_3_TextChanged;
                txtNumber_4.TextChanged += TxtNumber_4_TextChanged;

                progressGenerator = new ProgressGenerator(this)
                {
                    ProgressSlice = 100f / 30f,
                    MaxCounter = 30

                };
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            //ShowSuccess("An SMS containing activation pin has been send to your number.");
        }


        private void TxtNumber_1_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (e.Text.Count() == 1)
                {
                    txtNumber_1.ClearFocus();
                    txtNumber_2.RequestFocus();
                }
                CheckValidPin();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }


        private void TxtNumber_2_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (e.Text.Count() == 1)
                {
                    txtNumber_2.ClearFocus();
                    txtNumber_3.RequestFocus();
                }
                CheckValidPin();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void TxtNumber_3_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (e.Text.Count() == 1)
                {
                    txtNumber_3.ClearFocus();
                    txtNumber_4.RequestFocus();
                }
                CheckValidPin();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void TxtNumber_4_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                CheckValidPin();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void CheckValidPin()
        {
            ClearErrors();

            string txt_1 = txtNumber_1.Text;
            string txt_2 = txtNumber_2.Text;
            string txt_3 = txtNumber_3.Text;
            string txt_4 = txtNumber_4.Text;
            try
            {
                if (TextUtils.IsEmpty(txt_1) || !TextUtils.IsDigitsOnly(txt_1))
                {
                    return;
                }

                if (TextUtils.IsEmpty(txt_2) || !TextUtils.IsDigitsOnly(txt_2))
                {
                    return;
                }

                if (TextUtils.IsEmpty(txt_3) || !TextUtils.IsDigitsOnly(txt_3))
                {
                    return;
                }

                if (TextUtils.IsEmpty(txt_3) || !TextUtils.IsDigitsOnly(txt_3))
                {
                    return;
                }

                if (TextUtils.IsEmpty(txt_4) || !TextUtils.IsDigitsOnly(txt_4))
                {
                    // ShowEmptyErrorPin_4();
                    return;
                }

                string code = txt_1 + "" + txt_2 + "" + txt_3 + "" + txt_4;
                this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, email, email, code);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.re_send_btn)]
        void OnResend(object sender, EventArgs eventArgs)
        {
            resendCalled = true;
            mPresenter.ResendCode(email);
        }

        public void ShowEmptyErrorPin()
        {
            txtInputLayoutNumber_1.Error = " ";
            txtInputLayoutNumber_2.Error = " ";
            txtInputLayoutNumber_3.Error = " ";
            txtInputLayoutNumber_4.Error = " ";
            txtErrorPin.Visibility = ViewStates.Visible;
        }

        public void ClearErrors()
        {
            txtInputLayoutNumber_1.Error = null;
            txtInputLayoutNumber_2.Error = null;
            txtInputLayoutNumber_3.Error = null;
            txtInputLayoutNumber_4.Error = null;
            txtErrorPin.Visibility = ViewStates.Gone;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.ForgotPasswordEnterCodeView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowEmptyEmailError()
        {

        }

        public void ShowEmptyCodeError()
        {

        }

        public void ShowInvalidEmailError()
        {

        }

        public void ShowError(string errorMessage)
        {
            if (mSnackBar != null && mSnackBar.IsShown)
            {
                mSnackBar.Dismiss();
            }


            mSnackBar = Snackbar.Make(rootView, errorMessage, Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mSnackBar.Dismiss(); }
            );
            View v = mSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mSnackBar.Show();

        }

        public void ShowSuccess(string message)
        {

            if (mSnackBar != null && mSnackBar.IsShown)
            {
                mSnackBar.Dismiss();
            }
            mSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {
                mSnackBar.Dismiss();
                if (!resendCalled)
                {
                    this.Finish();
                }
                else
                {
                    resendCalled = false;
                }
            }
            );
            View v = mSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mSnackBar.Show();






        }

        public void ShowCodeVerifiedSuccess()
        {
            Intent intent = new Intent(this, typeof(ForgotPasswordVerificationCodeSuccessActivity));
            intent.PutExtra("email", email);
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(intent);
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Verification Code (Email)");
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

        public void ShowGetCodeProgressDialog()
        {

        }

        public void HideGetCodeProgressDialog()
        {

        }

        public void ClearErrorMessages()
        {

        }

        public void ClearTextFields()
        {

        }

        public void EnableSubmitButton()
        {

        }

        public void DisableSubmitButton()
        {

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
                string txt_1 = txtNumber_1.Text;
                string txt_2 = txtNumber_2.Text;
                string txt_3 = txtNumber_3.Text;
                string txt_4 = txtNumber_4.Text;
                string code = txt_1 + "" + txt_2 + "" + txt_3 + "" + txt_4;
                this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, email, email, code);
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
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();
                string txt_1 = txtNumber_1.Text;
                string txt_2 = txtNumber_2.Text;
                string txt_3 = txtNumber_3.Text;
                string txt_4 = txtNumber_4.Text;
                string code = txt_1 + "" + txt_2 + "" + txt_3 + "" + txt_4;
                this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, email, email, code);
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
            .SetAction(Utility.GetLocalizedCommonLabel("retry"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();
                string txt_1 = txtNumber_1.Text;
                string txt_2 = txtNumber_2.Text;
                string txt_3 = txtNumber_3.Text;
                string txt_4 = txtNumber_4.Text;
                string code = txt_1 + "" + txt_2 + "" + txt_3 + "" + txt_4;
                this.userActionsListener.Submit(Constants.APP_CONFIG.API_KEY_ID, email, email, code);
            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();
        }

        public void ShowRetryOptionsCodeCancelledException(System.OperationCanceledException operationCanceledException)
        {

        }

        public void ShowRetryOptionsCodeApiException(ApiException apiException)
        {

        }

        public void ShowRetryOptionsCodeUnknownException(Exception exception)
        {

        }

        public void SetPresenter(ForgetPasswordContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }


        #region OnCompleteListener implementation

        void ProgressGenerator.OnProgressListener.OnComplete()
        {
            try
            {
                btnResend.Text = Utility.GetLocalizedLabel("Common", "resend") + "(30)";
                //btnResend.SetCompoundDrawablesWithIntrinsicBounds(GetDrawable(Resource.Drawable.ic_button_resend_loaded), null, null, null);
                btnResend.Visibility = ViewStates.Gone;
                OnCompleteResend.Visibility = ViewStates.Visible;
                btnResend.Text = Utility.GetLocalizedLabel("Common", "resend");
                btnResend.SetCompoundDrawablesWithIntrinsicBounds(GetDrawable(Resource.Drawable.ic_button_resend_loading), null, null, null);
                btnResend.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.freshGreen)));
                progressGenerator.Progress = 0;
                this.userActionsListener.OnComplete();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }





        #endregion

        public void OnComplete()
        {

        }

        public void OnProgress(int count)
        {
            //if (count >= 15)
            //{
            //    btnResend.SetCompoundDrawablesWithIntrinsicBounds(GetDrawable(Resource.Drawable.ic_button_resend_loaded), null, null, null);
            //    btnResend.SetTextColor(Resources.GetColor(Resource.Color.white));

            //}
            btnResend.Text = Utility.GetLocalizedLabel("Common", "resend") + "(" + Math.Abs(count - 30) + ")";
        }

        public void StartProgress()
        {
            try
            {
                OnCompleteResend.Visibility = ViewStates.Gone;
                btnResend.Visibility = ViewStates.Visible;
                btnResend.SetCompoundDrawablesWithIntrinsicBounds(GetDrawable(Resource.Drawable.ic_button_resend_loading), null, null, null);
                btnResend.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.freshGreen)));
                progressGenerator.Progress = 0;
                progressGenerator.Start(btnResend, this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void EnableResendButton()
        {
            btnResend.Enabled = true;
            btnResend.Clickable = true;
        }

        public void DisableResendButton()
        {
            btnResend.Enabled = false;
            btnResend.Clickable = false;
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

        public void ShowEmptyErrorPin_1()
        {
            txtInputLayoutNumber_1.Error = Utility.GetLocalizedErrorLabel("invalid_pin");
        }

        public void ShowEmptyErrorPin_2()
        {
            txtInputLayoutNumber_2.Error = Utility.GetLocalizedErrorLabel("invalid_pin");
        }

        public void ShowEmptyErrorPin_3()
        {
            txtInputLayoutNumber_3.Error = Utility.GetLocalizedErrorLabel("invalid_pin");
        }

        public void ShowEmptyErrorPin_4()
        {
            txtInputLayoutNumber_4.Error = Utility.GetLocalizedErrorLabel("invalid_pin");
        }

        public void ShowEmailResendSuccess()
        {
            //throw new NotImplementedException();
        }
       
        public void ShowEmailUpdateSuccess(string message, string email)
        {
            //throw new NotImplementedException();
        }
    }
}
