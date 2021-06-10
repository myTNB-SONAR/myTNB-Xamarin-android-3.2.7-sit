using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;


using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Common.Activity;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Login.Requests;
using myTNB_Android.Src.SSMR.Util;
using myTNB_Android.Src.UpdateMobileNo.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Runtime;

namespace myTNB_Android.Src.UpdateMobileNo.Activity
{
    [Activity(Label = "@string/update_mobile_activity_title"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class UpdateMobileActivity : BaseActivityCustom, UpdateMobileContract.IView
    {
        [BindView(Resource.Id.rootView)]
        LinearLayout rootView;

        [BindView(Resource.Id.lblVerifyMobileNo)]
        TextView lblVerifyMobileNo;

        [BindView(Resource.Id.btnSave)]
        Button btnSave;

        [BindView(Resource.Id.mobileNumberFieldContainer)]
        LinearLayout mobileNumberFieldContainer;

        [BindView(Resource.Id.infoLabelContainer)]
        LinearLayout infoLabelContainer;

        [BindView(Resource.Id.infoLabel)]
        TextView lblInfoLabel;

        UpdateMobileContract.IUserActionsListener userActionsListener;
        UpdateMobilePresenter mPresenter;

        MaterialDialog progress;

        private bool forceUpdatePhoneNo = false;

        private UserAuthenticationRequest loginRequest;

        private bool fromAppLaunch = false;

        const string PAGE_ID = "UpdateMobileNumber";

        const int COUNTRY_CODE_SELECT_REQUEST = 1;

        private MobileNumberInputComponent mobileNumberInputComponent;
        private string dialogTitle, dialogMessage, dialogBtnLabel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_UpdateMobileLarge : Resource.Style.Theme_UpdateMobile);
            try
            {
                Bundle intent = Intent.Extras;

                if (intent != null)
                {
                    if (intent.ContainsKey(Constants.FORCE_UPDATE_PHONE_NO))
                    {
                        forceUpdatePhoneNo = intent.GetBoolean(Constants.FORCE_UPDATE_PHONE_NO, false);
                    }

                    if (intent.ContainsKey("LoginRequest"))
                    {
                        loginRequest = DeSerialze<UserAuthenticationRequest>(intent.GetString("LoginRequest"));
                    }

                    if (intent.ContainsKey(Constants.FROM_APP_LAUNCH))
                    {
                        fromAppLaunch = intent.GetBoolean(Constants.FROM_APP_LAUNCH, false);
                    }
                }

                TextViewUtils.SetMuseoSans500Typeface(btnSave, lblInfoLabel);
                TextViewUtils.SetMuseoSans300Typeface(lblVerifyMobileNo);
                btnSave.Text = GetLabelCommonByLanguage("next");
                lblInfoLabel.Text = GetLabelByLanguage("infoTitle");
                lblVerifyMobileNo.Text = GetLabelByLanguage("details");
                TextViewUtils.SetTextSize12(lblInfoLabel);
                TextViewUtils.SetTextSize14(lblVerifyMobileNo);
                TextViewUtils.SetTextSize16(btnSave);

                progress = new MaterialDialog.Builder(this)
                    .Title(GetString(Resource.String.update_mobile_progress_title))
                    .Content(GetString(Resource.String.update_mobile_progress_content))
                    .Progress(true, 0)
                    .Cancelable(false)
                    .Build();


                mPresenter = new UpdateMobilePresenter(this);
                userActionsListener.Start();

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

                //SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                //SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

                infoLabelContainer.Click += delegate
                {
                    ShowWhatIsThisDialog();
                };

                dialogTitle = GetLabelByLanguage("infoPopupTitle");
                dialogMessage = GetLabelByLanguage("infoPopupMessage");
                dialogBtnLabel = GetLabelCommonByLanguage("gotIt");

                mobileNumberFieldContainer.RemoveAllViews();
                mobileNumberInputComponent = new MobileNumberInputComponent(this);
                mobileNumberInputComponent.SetOnTapCountryCodeAction(OnTapCountryCode);
                mobileNumberInputComponent.SetMobileNumberLabel(Utility.GetLocalizedCommonLabel("mobileNo"));
                mobileNumberInputComponent.SetSelectedCountry(CountryUtil.Instance.GetDefaultCountry());
                mobileNumberInputComponent.SetValidationAction(OnValidateMobileNumber);
                mobileNumberFieldContainer.AddView(mobileNumberInputComponent);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ShowWhatIsThisDialog()
        {
            MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(dialogTitle)
                .SetMessage(dialogMessage)
                .SetCTALabel(dialogBtnLabel)
                .Build().Show();
        }

        private void OnValidateMobileNumber(bool isValidated)
        {
            if (isValidated)
            {
                EnableSaveButton();
            }
            else
            {
                DisableSaveButton();
            }
        }

        private void OnTapCountryCode()
        {
            Intent intent = new Intent(this, typeof(SelectCountryActivity));
            StartActivityForResult(intent, COUNTRY_CODE_SELECT_REQUEST);
        }

        [OnClick(Resource.Id.btnSave)]
        void OnSave(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                if (ConnectionUtils.HasInternetConnection(this))
                {
                    string newMobile = mobileNumberInputComponent.GetMobileNumberValueWithISDCode();
                    this.userActionsListener.OnUpdatePhoneNo(newMobile, loginRequest, forceUpdatePhoneNo);
                }
                else
                {
                    ShowNoInternetSnackbar();
                }
            }
        }

        public void HideProgress()
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
            tv.SetMaxLines(7);
            mErrorSnackbar.Show();
            this.SetIsClicked(false);
        }

        public void ShowInvalidMobileNoError()
        {
            //No Impl
        }

        public void ShowProgress()
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
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mApiExcecptionSnackBar.Dismiss();
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
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();

            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();
            this.SetIsClicked(false);
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
            //No Impl
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
            //No Impl
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (resultCode == Result.Ok)
                {
                    if (requestCode == Constants.REQUEST_VERIFICATION_SMS_TOEKN_CODE)
                    {
                        SetResult(Result.Ok);
                        Finish();
                    }
                    else if (requestCode == COUNTRY_CODE_SELECT_REQUEST)
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

        public void ShowEmptyMobileNoError()
        {
            //No Impl
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
            this.SetIsClicked(false);
        }
    }
}
