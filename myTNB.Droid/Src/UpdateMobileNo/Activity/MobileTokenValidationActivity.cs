﻿using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Login.Requests;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.RegisterValidation.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressButton;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Refit;
using System;
using System.Linq;
using System.Runtime;

namespace myTNB_Android.Src.UpdateMobileNo
{
    [Activity(Label = "@string/registration_validation_activity_title"
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.RegisterValidation")]
    public class MobileTokenValidationActivity : BaseToolbarAppCompatActivity, MobileTokenValidationContract.IView, ProgressGenerator.OnProgressListener
    {

        private MobileTokenValidationPresenter mPresenter;
        private MobileTokenValidationContract.IUserActionsListener userActionsListener;
        private ProgressGenerator progressGenerator;

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

        //PinDisplayerSMSReceiver pinDisplayerSMSReceiver;

        MaterialDialog registrationDialog;
        private LoadingOverlay loadingOverlay;

        private string newPhoneNo;

        private UserAuthenticationRequest loginRequest;

        private bool fromAppLaunch = false;
        private bool verifyPhone = false;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                registrationDialog = new MaterialDialog.Builder(this)
                    .Title(Resource.String.registration_validation_progress_title)
                    .Content(Resource.String.registration_validation_progress_content)
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();

                Bundle extras = Intent.Extras;

                if (extras != null)
                {
                    if (extras.ContainsKey("LoginRequest"))
                    {
                        //loginRequest = JsonConvert.DeserializeObject<UserAuthenticationRequest>(extras.GetString("LoginRequest"));
                        loginRequest = DeSerialze<UserAuthenticationRequest>(extras.GetString("LoginRequest"));
                    }
                    newPhoneNo = extras.GetString(Constants.NEW_PHONE_NO);
                    fromAppLaunch = extras.GetBoolean(Constants.FROM_APP_LAUNCH, false);
                    verifyPhone = extras.GetBoolean(Constants.FORCE_UPDATE_PHONE_NO, false);
                }

                // Create your application here
                this.mPresenter = new MobileTokenValidationPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));

                progressGenerator = new ProgressGenerator(this)
                {
                    ProgressSlice = 100f / 30f,
                    MaxCounter = 30

                };

                TextViewUtils.SetMuseoSans300Typeface(txtInfoTitle, txtDidntReceive);
                TextViewUtils.SetMuseoSans300Typeface(txtNumber_1, txtNumber_2, txtNumber_3, txtNumber_4);
                TextViewUtils.SetMuseoSans500Typeface(btnResend, OnCompleteResend);

                txtInfoTitle.Text = GetString(Resource.String.verfiy_mobile_validation_pin_info_wildcard, newPhoneNo);

                txtNumber_1.TextChanged += TxtNumber_1_TextChanged;
                txtNumber_2.TextChanged += TxtNumber_2_TextChanged;
                txtNumber_3.TextChanged += TxtNumber_3_TextChanged;
                txtNumber_4.TextChanged += TxtNumber_4_TextChanged;

                //pinDisplayerSMSReceiver = new PinDisplayerSMSReceiver(txtNumber_1 , txtNumber_2 , txtNumber_3 , txtNumber_4);

                Snackbar mPinSentInfo = Snackbar.Make(rootView,
                    GetString(Resource.String.registration_validation_snackbar_sms_sent_msg),
                    Snackbar.LengthLong);
                mPinSentInfo.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
            CheckValidPin();
        }

        private void CheckValidPin()
        {
            try
            {
                string txt_1 = txtNumber_1.Text;
                string txt_2 = txtNumber_2.Text;
                string txt_3 = txtNumber_3.Text;
                string txt_4 = txtNumber_4.Text;
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

                this.userActionsListener.OnVerifyToken(txt_1, txt_2, txt_3, txt_4, newPhoneNo, loginRequest, fromAppLaunch, verifyPhone);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            //try {
            //if (pinDisplayerSMSReceiver != null)
            //{
            //    UnregisterReceiver(pinDisplayerSMSReceiver);
            //}

            //}
            //catch (Exception e)
            //{
            //    Utility.LoggingNonFatalError(e);
            //}
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                //if (pinDisplayerSMSReceiver != null)
                //{
                //    RegisterReceiver(pinDisplayerSMSReceiver , new IntentFilter("com.myTNB.smsReceiver"));
                //}
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                if (progressGenerator != null)
                {
                    progressGenerator.OnDestroy();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            base.OnDestroy();
        }

        [OnClick(Resource.Id.re_send_btn)]
        void OnResend(object sender, EventArgs eventArgs)
        {
            // TODO : UPDATE THIS TO RESEND ASYNC
            this.userActionsListener.ResendAsync(newPhoneNo);

        }

        #region OnCompleteListener implementation

        void ProgressGenerator.OnProgressListener.OnComplete()
        {
            try
            {
                btnResend.Text = GetString(Resource.String.registration_validation_btn_resend) + "(30)";
                //btnResend.SetCompoundDrawablesWithIntrinsicBounds(GetDrawable(Resource.Drawable.ic_button_resend_loaded) , null , null , null );
                btnResend.Visibility = ViewStates.Gone;
                OnCompleteResend.Visibility = ViewStates.Visible;
                btnResend.Text = GetString(Resource.String.registration_validation_btn_resend);
                btnResend.SetCompoundDrawablesWithIntrinsicBounds(GetDrawable(Resource.Drawable.ic_button_resend_loading), null, null, null);
                btnResend.SetTextColor(Resources.GetColor(Resource.Color.freshGreen));
                progressGenerator.Progress = 0;
                this.userActionsListener.OnComplete();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }





        #endregion

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.RegistrationValidationView;
        }

        public void SetPresenter(MobileTokenValidationContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowInvalidPin()
        {
            //throw new NotImplementedException();
        }

        public void ShowResendPin()
        {
            //throw new NotImplementedException();
        }

        public void ShowSMSPinInfo()
        {
            //throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Resource.Menu.RegisterValidationMenu, menu);
        //    return base.OnCreateOptionsMenu(menu);
        //}

        //public override bool OnOptionsItemSelected(IMenuItem item)
        //{
        //    switch (item.ItemId)
        //    {
        //        case Resource.Id.action_next:
        //            this.mPresenter.OnNavigateToAccountListActivity();
        //            break;

        //    }
        //    return base.OnOptionsItemSelected(item);
        //}

        public void ShowAccountListActivity()
        {
            Intent LinkAccountIntent = new Intent(this, typeof(LinkAccountActivity));
            LinkAccountIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(LinkAccountIntent);
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void StartProgress()
        {
            OnCompleteResend.Visibility = ViewStates.Gone;
            btnResend.Visibility = ViewStates.Visible;
            btnResend.SetCompoundDrawablesWithIntrinsicBounds(GetDrawable(Resource.Drawable.ic_button_resend_loading), null, null, null);
            btnResend.SetTextColor(Resources.GetColor(Resource.Color.freshGreen));
            progressGenerator.Progress = 0;
            progressGenerator.Start(btnResend, this);
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

        public void OnProgress(int count)
        {

            //if (count >= 15)
            //{
            //    btnResend.SetCompoundDrawablesWithIntrinsicBounds(GetDrawable(Resource.Drawable.ic_button_resend_loaded), null, null, null);
            //    btnResend.SetTextColor(Resources.GetColor(Resource.Color.white));

            //}
            btnResend.Text = GetString(Resource.String.registration_validation_btn_resend) + "(" + Math.Abs(count - 30) + ")";
        }

        public void ShowEmptyErrorPin_1()
        {
            txtInputLayoutNumber_1.Error = GetString(Resource.String.registration_validation_empty_error_1);
        }

        public void ShowEmptyErrorPin_2()
        {
            txtInputLayoutNumber_2.Error = GetString(Resource.String.registration_validation_empty_error_2);
        }

        public void ShowEmptyErrorPin_3()
        {
            txtInputLayoutNumber_3.Error = GetString(Resource.String.registration_validation_empty_error_3);
        }

        public void ShowEmptyErrorPin_4()
        {
            txtInputLayoutNumber_4.Error = GetString(Resource.String.registration_validation_empty_error_4);
        }

        public void ClearErrors()
        {
            txtInputLayoutNumber_1.Error = null;
            txtInputLayoutNumber_2.Error = null;
            txtInputLayoutNumber_3.Error = null;
            txtInputLayoutNumber_4.Error = null;
        }

        private Snackbar mCancelledExceptionSnackBar;
        public void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException)
        {
            if (mCancelledExceptionSnackBar != null && mCancelledExceptionSnackBar.IsShown)
            {
                mCancelledExceptionSnackBar.Dismiss();
            }

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.registration_validation_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.registration_validation_cancelled_exception_btn_close), delegate
            {

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

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.registration_validation_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.registration_validation_api_exception_btn_close), delegate
            {

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

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.registration_validation_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.registration_validation_unknown_exception_btn_close), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            mUknownExceptionSnackBar.Show();

        }
        private Snackbar mSnackBar;
        public void ShowError(string errorMessage)
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
                .SetAction(GetString(Resource.String.registration_validation_snackbar_btn_close), delegate { mSnackBar.Dismiss(); }
                );
                mSnackBar.Show();
            }


        }

        public bool IsStillCountingDown()
        {
            return !btnResend.Enabled;
        }

        public void ShowSnackbarError(int resourceStringId)
        {
            if (mSnackBar != null && mSnackBar.IsShown)
            {
                mSnackBar.Dismiss();
                mSnackBar.Show();
            }
            else
            {
                mSnackBar = Snackbar.Make(rootView, GetString(resourceStringId), Snackbar.LengthIndefinite)
                .SetAction(GetString(Resource.String.registration_validation_snackbar_btn_close), delegate { mSnackBar.Dismiss(); }
                );
                mSnackBar.Show();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            this.userActionsListener.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void RequestSMSPermission()
        {
            RequestPermissions(new string[] { Manifest.Permission.ReceiveSms }, Constants.RUNTIME_PERMISSION_SMS_REQUEST_CODE);
        }

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
                .SetAction(GetString(Resource.String.registration_validation_snackbar_btn_close), delegate { mSnackBar.Dismiss(); }
                );
                mSnackBar.Show();
            }
        }

        public bool IsGrantedSMSReceivePermission()
        {
            return ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReceiveSms) == (int)Permission.Granted;
        }

        public bool ShouldShowSMSReceiveRationale()
        {
            return ShouldShowRequestPermissionRationale(Manifest.Permission.ReceiveSms);
        }
        public string GetDeviceId()
        {
            return this.DeviceId();
        }

        public void ShowRegistrationProgress()
        {
            //if (registrationDialog != null && !registrationDialog.IsShowing)
            //{
            //    registrationDialog.Show();
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

        public void HideRegistrationProgress()
        {
            //if (registrationDialog != null && registrationDialog.IsShowing)
            //{
            //    registrationDialog.Dismiss();
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

        public void ShowDashboardMyAccount()
        {
            //SetResult(Result.Ok);
            //Finish();
            Intent DashboardIntent = new Intent(this, typeof(DashboardActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            DashboardIntent.PutExtra(Constants.FORCE_UPDATE_PHONE_NO, true);
            StartActivity(DashboardIntent);
        }

        public void ShowRetryLoginUnknownException(String error)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, error, Snackbar.LengthIndefinite)
            .SetAction("Retry", delegate
            {
                CheckValidPin();
                mUknownExceptionSnackBar.Dismiss();

            }
            );
            mUknownExceptionSnackBar.Show();
        }

        public void ShowUpdatePhoneResultOk()
        {
            SetResult(Result.Ok);
            Finish();
        }

        public void ShowDashboard()
        {
            Intent DashboardIntent = new Intent(this, typeof(DashboardActivity));
            DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(DashboardIntent);
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
    }
}