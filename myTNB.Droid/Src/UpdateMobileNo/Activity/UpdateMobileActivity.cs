using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Login.Requests;
using myTNB_Android.Src.UpdateMobileNo.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using Refit;
using System;
using System.Runtime;

namespace myTNB_Android.Src.UpdateMobileNo.Activity
{
    [Activity(Label = "@string/update_mobile_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.UpdateMobile")]
    public class UpdateMobileActivity : BaseActivityCustom, UpdateMobileContract.IView
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.txtInputLayoutMobileNo)]
        TextInputLayout txtInputLayoutMobileNo;

        [BindView(Resource.Id.lblVerifyMobileNo)]
        TextView lblVerifyMobileNo;

        [BindView(Resource.Id.txtMobileNo)]
        EditText txtMobileNo;

        [BindView(Resource.Id.btnSave)]
        Button btnSave;

        UpdateMobileContract.IUserActionsListener userActionsListener;
        UpdateMobilePresenter mPresenter;

        MaterialDialog progress;
        private LoadingOverlay loadingOverlay;

        private bool forceUpdatePhoneNo = false;

        private UserAuthenticationRequest loginRequest;

        private bool fromAppLaunch = false;

        const string PAGE_ID = "UpdateMobileNumber";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                Bundle intent = Intent.Extras;

                if (intent != null)
                {
                    if (intent.ContainsKey(Constants.FORCE_UPDATE_PHONE_NO))
                    {
                        forceUpdatePhoneNo = intent.GetBoolean(Constants.FORCE_UPDATE_PHONE_NO, false);
                        //SupportActionBar.SetDisplayHomeAsUpEnabled(false);
                    }

                    if (intent.ContainsKey("LoginRequest"))
                    {
                        //loginRequest = JsonConvert.DeserializeObject<UserAuthenticationRequest>(intent.GetString("LoginRequest"));
                        loginRequest = DeSerialze<UserAuthenticationRequest>(intent.GetString("LoginRequest"));
                    }

                    if (intent.ContainsKey(Constants.FROM_APP_LAUNCH))
                    {
                        fromAppLaunch = intent.GetBoolean(Constants.FROM_APP_LAUNCH, false);
                    }

                }


                // Create your application here

                TextViewUtils.SetMuseoSans300Typeface(txtMobileNo);
                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutMobileNo);
                TextViewUtils.SetMuseoSans500Typeface(btnSave);
                TextViewUtils.SetMuseoSans300Typeface(lblVerifyMobileNo);

                lblVerifyMobileNo.Text = GetLabelByLanguage("details");
                btnSave.Text = GetLabelCommonByLanguage("next");
                txtInputLayoutMobileNo.Hint = GetLabelCommonByLanguage("mobileNo");
                progress = new MaterialDialog.Builder(this)
                    .Title(GetString(Resource.String.update_mobile_progress_title))
                    .Content(GetString(Resource.String.update_mobile_progress_content))
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();


                mPresenter = new UpdateMobilePresenter(this);
                userActionsListener.Start();

                //txtMobileNo.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
                //{
                //    if (e.HasFocus)
                //    {
                //        if (string.IsNullOrEmpty(txtMobileNo.Text))
                //        {
                //            txtMobileNo.Append("+60");
                //        }
                //    }
                //};

                txtMobileNo.TextChanged += TxtMobileNo_TextChanged;
                txtMobileNo.AddTextChangedListener(new InputFilterFormField(txtMobileNo, txtInputLayoutMobileNo));


                if (UserEntity.IsCurrentlyActive())
                {
                    UserEntity entity = UserEntity.GetActive();
                    string MobileNo = entity.MobileNo;
                    if (!MobileNo.Contains("+60"))
                    {
                        MobileNo = "+60" + MobileNo;
                    }
                    txtMobileNo.Text = MobileNo;
                }
                else if (intent.ContainsKey("PhoneNumber"))
                {
                    string MobileNo = Intent.GetStringExtra("PhoneNumber");
                    if (!MobileNo.Contains("+60"))
                    {
                        MobileNo = "+60" + MobileNo;
                    }
                    txtMobileNo.Text = string.IsNullOrEmpty(MobileNo) ? "" : MobileNo;
                }

                if (string.IsNullOrEmpty(txtMobileNo.Text))
                {
                    txtMobileNo.Append("+60");
                }
                txtMobileNo.SetFilters(new Android.Text.IInputFilter[] { new InputFilterPhoneNumber() });

                if (forceUpdatePhoneNo)
                {
                    lblVerifyMobileNo.Visibility = ViewStates.Visible;
                    SetToolBarTitle(GetLabelByLanguage("verifyDeviceTitle"));
                }
                else
                {
                    lblVerifyMobileNo.Visibility = ViewStates.Gone;
                    SetToolBarTitle(GetLabelByLanguage("updateMobileNumberTitle"));
                }

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        [Preserve]
        private void TxtMobileNo_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {


            string checkedPhoneNumber = this.userActionsListener.OnVerfiyCellularCode(e.Text.ToString());
            if (checkedPhoneNumber != txtMobileNo.Text)
            {
                txtMobileNo.Text = checkedPhoneNumber;
            }
            this.userActionsListener.OnVerifyMobile(txtMobileNo.Text.ToString(), forceUpdatePhoneNo);
        }

        [OnClick(Resource.Id.btnSave)]
        void OnSave(object sender, EventArgs eventArgs)
        {
            if (ConnectionUtils.HasInternetConnection(this))
            {
                string newMobile = txtMobileNo.Text.Trim();
                this.userActionsListener.OnUpdatePhoneNo(newMobile, loginRequest);
            }
            else
            {
                ShowNoInternetSnackbar();
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

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.UpdateMobileView;
        }

        public void SetPresenter(UpdateMobileContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override void SetToolBarTitle(string title)
        {
            base.SetToolBarTitle(title);
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

        public void ShowInvalidMobileNoError()
        {
            txtInputLayoutMobileNo.Error = Utility.GetLocalizedErrorLabel("invalid_mobileNumber");
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

        public void ShowSuccess(string newPhoneNo)
        {
            Intent nextIntent = new Intent(this, typeof(MobileTokenValidationActivity));
            nextIntent.PutExtra(Constants.NEW_PHONE_NO, newPhoneNo);
            nextIntent.PutExtra(Constants.FROM_APP_LAUNCH, fromAppLaunch);
            nextIntent.PutExtra(Constants.FORCE_UPDATE_PHONE_NO, forceUpdatePhoneNo);
            nextIntent.PutExtra("LoginRequest", JsonConvert.SerializeObject(loginRequest));
            StartActivityForResult(nextIntent, Constants.REQUEST_VERIFICATION_SMS_TOEKN_CODE);
        }

        public void ClearErrors()
        {
            txtInputLayoutMobileNo.Error = null;
        }

        public void EnableSaveButton()
        {
            btnSave.Enabled = true;
            btnSave.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);

        }

        public void DisableSaveButton()
        {
            btnSave.Enabled = false;
            btnSave.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        public void ShowMobile(string mobileNo)
        {
            txtMobileNo.Text = mobileNo;
            txtMobileNo.SetSelection(mobileNo.Length);
        }

        public override void OnBackPressed()
        {
            //if (!forceUpdatePhoneNo)
            //{
            base.OnBackPressed();
            //}
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == Constants.REQUEST_VERIFICATION_SMS_TOEKN_CODE && resultCode == Result.Ok)
                {
                    SetResult(Result.Ok);
                    Finish();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public void ShowEmptyMobileNoError()
        {
            txtInputLayoutMobileNo.Error = Utility.GetLocalizedErrorLabel("invalid_mobileNumber");
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
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Update Mobile Number");
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

        private Snackbar mNoInternetSnackbar;
        public void ShowNoInternetSnackbar()
        {
            if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
            {
                mNoInternetSnackbar.Dismiss();
            }

            mNoInternetSnackbar = Snackbar.Make(rootView, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mNoInternetSnackbar.Dismiss();
            }
            );
            View v = mNoInternetSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mNoInternetSnackbar.Show();
        }
    }
}
