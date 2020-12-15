﻿using Android;
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
using myTNB_Android.Src.UpdateID.MVP;
using myTNB_Android.Src.TermsAndConditions.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.UpdateID.Models;
using myTNB_Android.Src.UpdateID.Adapter;
using Newtonsoft.Json;
using Refit;
using System;
using System.Runtime;
using myTNB_Android.Src.UpdateID.Activity;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using myTNB_Android.Src.MyAccount.Activity;
using myTNB_Android.Src.XDetailRegistrationForm.Models;
using myTNB_Android.Src.XDetailRegistrationForm.Adapter;
using myTNB_Android.Src.XDetailRegistrationForm.Activity;
using myTNB_Android.Src.Base;

namespace myTNB_Android.Src.UpdateID.Activity
{
    [Activity(Label = "@string/registration_activity_title"
      , NoHistory = false
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.DashboardHome")]
    public class UpdateIDActivity : BaseActivityCustom, UpdateIDContract.IView
    {
        private UpdateIDPresenter mPresenter;
        private UpdateIDContract.IUserActionsListener userActionsListener;

        private AlertDialog mVerificationProgressDialog;
        private AlertDialog mRegistrationProgressDialog;
        const string PAGE_ID = "UpdateID";
        private MobileNumberInputComponent mobileNumberInputComponent;
        const int COUNTRY_CODE_SELECT_REQUEST = 1;
        private readonly int SELECT_IDENTIFICATION_TYPE_REQ_CODE = 2011;
        private IdentificationType selectedIdentificationType;
        private IdentificationTypeAdapter identificationType;
        private Regex hasHyphens = new Regex(@"/(?([0-9]{3}))?([ .-]?)([0-9]{3})\2([0-9]{4})/");

        Snackbar mRegistrationSnackBar;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.Label1)]
        TextView LabelTitle;

        [BindView(Resource.Id.Label2)]
        TextView LabelDetails;

        [BindView(Resource.Id.txtICNumber)]
        EditText txtICNumber;

        [BindView(Resource.Id.textInputLayoutICNo)]
        TextInputLayout textInputLayoutICNo;

        [BindView(Resource.Id.txtAccountType)]
        TextView txtAccountType;

        [BindView(Resource.Id.selector_account_type)]
        TextView identityType;

        [BindView(Resource.Id.btnRegister)]
        Button btnRegister;

        private bool isClicked = false;
        private bool fromAddAccPage = false;

        Snackbar mUpdateIc;

        UserCredentialsEntity entity = new UserCredentialsEntity();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                if (Intent.HasExtra("fromAddAccPage"))
                {
                    fromAddAccPage = Intent.Extras.GetBoolean("fromAddAccPage", true);
                }
                this.mPresenter = new UpdateIDPresenter(this);

                txtAccountType = FindViewById<TextView>(Resource.Id.txtAccountType);
                identityType = FindViewById<TextView>(Resource.Id.selector_account_type);

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

                TextViewUtils.SetMuseoSans300Typeface(
                    txtAccountType,
                    identityType,
                    txtICNumber,
                    LabelDetails);

                TextViewUtils.SetMuseoSans300Typeface(
                    textInputLayoutICNo);

                TextViewUtils.SetMuseoSans500Typeface(btnRegister);

                txtAccountType.Text = GetLabelCommonByLanguage("idtypeTitle");
                textInputLayoutICNo.Hint = GetLabelCommonByLanguage("idNumberhint");
                SetToolBarTitle(GetLabelByLanguage("title"));
                SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                btnRegister.Text = GetLabelByLanguage("confrimButton");
                LabelTitle.Text = GetLabelByLanguage("updateIdLabelTitle");
                LabelDetails.Text = GetLabelByLanguage("updateIdLabelDetails");

                txtICNumber.AfterTextChanged += new EventHandler<AfterTextChangedEventArgs>(AddTextChangedListener);
                txtICNumber.AddTextChangedListener(new InputFilterFormField(txtICNumber, textInputLayoutICNo));

                IdentificationType Individual = new IdentificationType();
                Individual.Id = "1";
                Individual.Type = Utility.GetLocalizedLabel("Register", "mykad");
                Individual.IsSelected = true;
                selectedIdentificationType = Individual;
                identityType.Text = selectedIdentificationType.Type;
                identityType.Click += async delegate
                {
                    if (!isClicked)
                    {
                        isClicked = true;
                        Intent identificationType = new Intent(this, typeof(SelectIdentificationTypeActivity));
                        identificationType.PutExtra("selectedIdentificationType", JsonConvert.SerializeObject(selectedIdentificationType));
                        StartActivityForResult(identificationType, SELECT_IDENTIFICATION_TYPE_REQ_CODE);
                    }
                };

                txtICNumber.AddTextChangedListener(new PhoneTextWatcher(txtICNumber, identityType));
                txtICNumber.SetOnKeyListener(new KeyListener());

                MyTNBAccountManagement.GetInstance().SetIsIDUpdated(false);
                ClearFields();
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public class PhoneTextWatcher : Java.Lang.Object, ITextWatcher
        {
            public PhoneTextWatcher(EditText text, TextView idtype)
            {
                eText = text;
                idText = idtype;
            }
            private int mAfter;
            private bool mFormatting;
            private EditText eText;
            private TextView idText;


            public void AfterTextChanged(IEditable s)
            {
            }

            public void BeforeTextChanged(Java.Lang.ICharSequence s, int start, int count, int after)
            {
                mAfter = after;
            }

            public void OnTextChanged(Java.Lang.ICharSequence s, int start, int before, int count)
            {
                string Idtype = idText.Text;

                if (Idtype.Equals("IC / Mykad"))
                {
                    int len = eText.Text.Length;
                    if ((len == 6 || len == 9 || start == 5 || start == 8) && before == 0)
                    {
                        eText.Text = eText.Text + "-";
                        eText.SetSelection(eText.Text.Length);

                    }
                    if (len > 14)
                    {
                        eText.Text = eText.Text.ToString().Substring(0, 14);
                        eText.SetSelection(eText.Text.Length);
                    }
                }
            }

        }
        public class KeyListener : Java.Lang.Object, Android.Views.View.IOnKeyListener
        {
            public static int KeyDel { get; set; }
            public bool OnKey(Android.Views.View v, Android.Views.Keycode keyCode, Android.Views.KeyEvent e)
            {
                if (keyCode == Android.Views.Keycode.Del)
                    KeyDel = 1;
                else
                    KeyDel = 0;
                return false;
            }
        }

        private void AddTextChangedListener(object sender, AfterTextChangedEventArgs e)
        {
            try
            {
                string ic_no = txtICNumber.Text.ToString().Trim();
                ClearICMinimumCharactersError();
                ClearICHint();
                EnableRegisterButton();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        
        public void OnPause()
        {
            base.OnPause();
            isClicked = true;
        }

        public override int ResourceId()
        {
            return Resource.Layout.UpdateIdView;
        }

        public void ClearFields()
        {

            txtICNumber.Text = "";
            txtICNumber.ClearFocus();

        }

        public void ClearAllErrorFields()
        {
            if (!string.IsNullOrEmpty(textInputLayoutICNo.Error))
            {
                textInputLayoutICNo.Error = null;
                textInputLayoutICNo.ErrorEnabled = false;
            }
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown && IsFinishing;
        }

        public void SetPresenter(UpdateIDContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowBackScreen()
        {
            Finish();
        }

        public void ShowSuccessUpdateID()
        {
            SetResult(Result.Ok);
            Finish();
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

        public void ShowIdentificationHint()
        {
            if (textInputLayoutICNo.HelperText != Utility.GetLocalizedErrorLabel("identificationhint"))
            {
                textInputLayoutICNo.HelperText = Utility.GetLocalizedErrorLabel("identificationhint");
            }

            if (!textInputLayoutICNo.HelperTextEnabled)
                textInputLayoutICNo.HelperTextEnabled = true;
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


        [OnClick(Resource.Id.btnRegister)]
        void OnRegister(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!this.GetIsClicked())
                {
                    string Idtype = selectedIdentificationType.Id;
                    string ic_no = txtICNumber.Text.ToString().Trim();
                    this.userActionsListener.CheckRequiredFields(ic_no, Idtype);

                    bool hasExistedID = MyTNBAccountManagement.GetInstance().IsIDUpdated();

                    if (hasExistedID)
                    {
                        this.SetIsClicked(true);
                        ShowUpdateIdDialog(this, () =>
                        {
                            // _ = RunUpdateID(idtype,ic_no);
                            ShowProgress();
                            this.userActionsListener.OnUpdateIC(Idtype, ic_no);
                        });
                    }
                    else
                    {
                        DisableRegisterButton();
                        if (Idtype.Equals("1"))
                        {
                            ShowFullICError();
                        }
                        else if (Idtype.Equals("2"))
                        {
                            ShowFullArmyIdError();
                        }
                        else
                        {
                            ShowFullPassportError();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.SetIsClicked(false);
                Utility.LoggingNonFatalError(e);
            }
        }

        void ShowUpdateIdDialog(Android.App.Activity context, Action confirmAction, Action cancelAction = null)
        {
            string ic_no = txtICNumber.Text.ToString().Trim();
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.THREE_PART_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(Utility.GetLocalizedLabel("Common", "updateIdTitle"))
                        .SetSubTitle(ic_no)
                        .SetMessage(Utility.GetLocalizedLabel("Common", "updateIdMessage"))
                        .SetContentGravity(Android.Views.GravityFlags.Center)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "re_enter"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Common", "confirm"))
                        .SetSecondaryCTAaction(() =>
                        {
                            confirmAction();
                        })
                        .Build();
            tooltipBuilder.SetCTAaction(() =>
            {
                if (cancelAction != null)
                {
                    cancelAction();
                    tooltipBuilder.DismissDialog();
                }
                else
                {
                    tooltipBuilder.DismissDialog();
                }
            }).Show();
            this.SetIsClicked(false);
        }

        protected override void OnResume()
        {
            base.OnResume();
            isClicked = false;

            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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


        public void ClearErrors()
        {
            //No Impl
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

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override void SetToolBarTitle(string title)
        {
            base.SetToolBarTitle(title);
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

        public void ClearIdentificationHint()
        {
            if (!string.IsNullOrEmpty(textInputLayoutICNo.HelperText))
            {
                textInputLayoutICNo.HelperText = null;
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
                    //this.userActionsListener.OnRequestSMSPermission();
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

        public void ClearICHint()
        {
            textInputLayoutICNo.HelperTextEnabled = false;
        }

        public void ClearICMinimumCharactersError()
        {
            if (!string.IsNullOrEmpty(textInputLayoutICNo.Error))
            {
                textInputLayoutICNo.Error = null;
                textInputLayoutICNo.ErrorEnabled = false;
            }
        }

        public void ShowFullICError()
        {
            if (textInputLayoutICNo.Error != Utility.GetLocalizedErrorLabel("mykadhint"))
            {
                textInputLayoutICNo.Error = Utility.GetLocalizedErrorLabel("mykadhint");
            }

            if (!textInputLayoutICNo.ErrorEnabled)
                textInputLayoutICNo.ErrorEnabled = true;
        }

        public void ShowFullArmyIdError()
        {
            // ClearFullNameError();
            if (textInputLayoutICNo.Error != Utility.GetLocalizedErrorLabel("armyidhint"))
            {
                textInputLayoutICNo.Error = Utility.GetLocalizedErrorLabel("armyidhint");
            }

            if (!textInputLayoutICNo.ErrorEnabled)
                textInputLayoutICNo.ErrorEnabled = true;
        }

        public void ShowFullPassportError()
        {
            // ClearFullNameError();
            if (textInputLayoutICNo.Error != Utility.GetLocalizedErrorLabel("passporthint"))
            {
                textInputLayoutICNo.Error = Utility.GetLocalizedErrorLabel("passporthint");
            }

            if (!textInputLayoutICNo.ErrorEnabled)
                textInputLayoutICNo.ErrorEnabled = true;
        }

        public void ShowInvalidIdentificationError()
        {

            Utility.ShowIdentificationErrorDialog(this, () =>
            {
                ShowProgressDialog();

            });

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
                    else if (requestCode == SELECT_IDENTIFICATION_TYPE_REQ_CODE)
                    {

                        if (resultCode == Result.Ok)
                        {
                            selectedIdentificationType = JsonConvert.DeserializeObject<IdentificationType>(data.GetStringExtra("selectedIdentificationType"));
                            if (selectedIdentificationType != null)
                            {
                                identityType.Text = selectedIdentificationType.Type;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowErrorMessage(string displayMessage)
        {
            if (mUpdateIc != null && mUpdateIc.IsShown)
            {
                mUpdateIc.Dismiss();
            }

            mUpdateIc = Snackbar.Make(rootView, displayMessage, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { mUpdateIc.Dismiss(); }
            );
            View v = mUpdateIc.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            mUpdateIc.Show();
            this.SetIsClicked(false);
        }
    }
}
