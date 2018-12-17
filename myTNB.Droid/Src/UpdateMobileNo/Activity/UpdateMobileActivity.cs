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
using myTNB_Android.Src.UpdateMobileNo.MVP;
using Refit;
using Android.Support.Design.Widget;
using CheeseBind;
using AFollestad.MaterialDialogs;
using myTNB_Android.Src.Utils;
using Android.Support.V4.Content;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using Newtonsoft.Json;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Login.Requests;
using System.Runtime;

namespace myTNB_Android.Src.UpdateMobileNo.Activity
{
    [Activity(Label = "@string/update_mobile_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.UpdateMobile")]
    public class UpdateMobileActivity : BaseToolbarAppCompatActivity , UpdateMobileContract.IView
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try {
            Intent intent = Intent;
            
            if (intent != null)
            {
                if (intent.HasExtra(Constants.FORCE_UPDATE_PHONE_NO))
                {
                    forceUpdatePhoneNo = intent.GetBooleanExtra(Constants.FORCE_UPDATE_PHONE_NO, false);
                    //SupportActionBar.SetDisplayHomeAsUpEnabled(false);
                }

                if (intent.HasExtra("LoginRequest"))
                {
                    loginRequest = JsonConvert.DeserializeObject<UserAuthenticationRequest>(Intent.GetStringExtra("LoginRequest"));
                }

                if (intent.HasExtra(Constants.FROM_APP_LAUNCH))
                {
                    fromAppLaunch = intent.GetBooleanExtra(Constants.FROM_APP_LAUNCH, false);
                }

            }


            // Create your application here

            TextViewUtils.SetMuseoSans300Typeface(txtMobileNo);
            TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutMobileNo);
            TextViewUtils.SetMuseoSans500Typeface(btnSave);
            TextViewUtils.SetMuseoSans300Typeface(lblVerifyMobileNo);

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
            }else if (intent.HasExtra("PhoneNumber"))
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
                SetToolBarTitle(GetString(Resource.String.verify_mobile_no));
            }
            else
            {
                lblVerifyMobileNo.Visibility = ViewStates.Gone;
                SetToolBarTitle(GetString(Resource.String.update_mobile_activity_title));
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
            this.userActionsListener.OnVerifyMobile(e.Text.ToString(), forceUpdatePhoneNo);
        }

        [OnClick(Resource.Id.btnSave)]
        void OnSave(object sender, EventArgs eventArgs)
        {
            string newMobile = txtMobileNo.Text.Trim();
            //this.userActionsListener.OnSave(newMobile);
            this.userActionsListener.OnUpdatePhoneNo(newMobile, loginRequest);
        }

        public void HideProgress()
        {
            //if (progress != null && progress.IsShowing)
            //{
            //    progress.Dismiss();
            //}
            try {
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
            .SetAction(GetString(Resource.String.update_mobile_cancelled_exception_btn_close), delegate {

                mErrorSnackbar.Dismiss();
            }
            );
            mErrorSnackbar.Show();
        }

        public void ShowInvalidMobileNoError()
        {
            txtInputLayoutMobileNo.Error = GetString(Resource.String.update_mobile_mobile_no_error);
        }

        public void ShowProgress()
        {
            //if (progress != null && !progress.IsShowing)
            //{
            //    progress.Show();
            //}
            try {
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

            mCancelledExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.update_mobile_cancelled_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.update_mobile_cancelled_exception_btn_close), delegate {

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

            mApiExcecptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.update_mobile_api_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.update_mobile_api_exception_btn_close), delegate {

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

            mUknownExceptionSnackBar = Snackbar.Make(rootView, GetString(Resource.String.update_mobile_unknown_exception_error), Snackbar.LengthIndefinite)
            .SetAction(GetString(Resource.String.update_mobile_unknown_exception_btn_close), delegate {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
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
            btnSave.Background = ContextCompat.GetDrawable(this , Resource.Drawable.green_button_background);

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
            try {
            base.OnActivityResult(requestCode, resultCode, data);
            if(requestCode == Constants.REQUEST_VERIFICATION_SMS_TOEKN_CODE && resultCode == Result.Ok)
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
            txtInputLayoutMobileNo.Error = GetString(Resource.String.bill_related_feedback_empty_mobile_error);
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