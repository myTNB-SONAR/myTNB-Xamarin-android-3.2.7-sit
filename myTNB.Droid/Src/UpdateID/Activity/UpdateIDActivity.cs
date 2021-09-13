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
      , Theme = "@style/Theme.OwnerTenantBaseTheme")]
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
       //private bool fromIDFlag = false;

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

                TextViewUtils.SetTextSize14(txtAccountType, identityType, txtICNumber, LabelDetails);
                TextViewUtils.SetTextSize16(btnRegister);

                txtAccountType.Text = Utility.GetLocalizedLabel("OneLastThing", "idtypeTitle").ToUpper();
                textInputLayoutICNo.Hint = Utility.GetLocalizedLabel("OneLastThing", "idNumberhint");
                SetToolBarTitle(Utility.GetLocalizedLabel("UpdateID", "title"));
                //SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                btnRegister.Text = Utility.GetLocalizedLabel("UpdateID", "confrimButton");
                LabelTitle.Text = Utility.GetLocalizedLabel("UpdateID", "updateIdLabelTitle");
                LabelDetails.Text = Utility.GetLocalizedLabel("UpdateID", "updateIdLabelDetails");


                //txtAccountType.Text = GetLabelCommonByLanguage("idtypeTitle");
                //textInputLayoutICNo.Hint = GetLabelCommonByLanguage("idNumberhint");
                //SetToolBarTitle(GetLabelByLanguage("title"));
                //SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
                //btnRegister.Text = GetLabelByLanguage("confrimButton");
                //LabelTitle.Text = GetLabelByLanguage("updateIdLabelTitle");
                //LabelDetails.Text = GetLabelByLanguage("updateIdLabelDetails");

                txtICNumber.FocusChange += txtICNumber_FocusChange;
                txtICNumber.AfterTextChanged += new EventHandler<AfterTextChangedEventArgs>(AddTextChangedListener);
                txtICNumber.AddTextChangedListener(new InputFilterFormField(txtICNumber, textInputLayoutICNo));
                txtICNumber.InputType = InputTypes.ClassNumber;

                IdentificationType Individual = new IdentificationType();
                Individual.Id = "1";
                Individual.Type = Utility.GetLocalizedLabel("OneLastThing", "mykad");
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
                ClearAllErrorFields();

                txtICNumber.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.placeholder_account_no, 0, 0, 0);
                this.userActionsListener.Start();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void txtICNumber_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            try
            {
                
                string Idtype = selectedIdentificationType.Id;
                string ic_no = txtICNumber.Text.ToString().Trim();
                

                if (!e.HasFocus)
                {
                    ClearICMinimumCharactersError();
                    ClearICHint();
                    this.userActionsListener.CheckRequiredFields( ic_no, Idtype);

                    if (ic_no.Equals(""))
                    {
                        ClearICMinimumCharactersError();
                        ClearICHint();
                    }
                    else
                    {
                        if (Idtype.Equals("1"))
                        {
                            ic_no = ic_no.Replace("-", string.Empty);
                            if (!this.mPresenter.CheckIdentificationIsValid(ic_no))
                            {
                                ClearICHint();
                                ShowFullICError();
                            }
                            //else
                            //{
                            //    ShowIdentificationHint();
                            //}
                        }
                        else if (Idtype.Equals("2"))
                        {
                            if (!this.mPresenter.CheckArmyIdIsValid(ic_no))
                            {
                                ClearICHint();
                                ShowFullArmyIdError();
                            }
                            //else
                            //{
                            //    ShowIdentificationHint();
                            //}
                        }
                        else if (Idtype.Equals("3"))
                        {
                            if (!this.mPresenter.CheckPassportIsValid(ic_no))
                            {
                                ClearICHint();
                                ShowFullPassportError();
                            }
                            //else
                            //{
                            //    ShowIdentificationHint();
                            //}
                        }
                        else
                        {
                            ClearICMinimumCharactersError();
                            ClearICHint();
                        }

                    }
                }
                else
                {
                    ShowIdentificationHint();
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }

        }

        public class PhoneTextWatcher : Java.Lang.Object, ITextWatcher
        {
            public PhoneTextWatcher(EditText text, TextView idtype)
            {
                eText = text;
                idText = idtype;
            }
            private bool flagDel = false;
            private EditText eText;
            private TextView idText;


            public void AfterTextChanged(IEditable s)
            {
                int len = eText.Text.Length;
                bool dash = eText.Text.Contains("-");
                string Idtype = idText.Text;

                if (Idtype.Equals("IC / MyKad"))
                {
                    if (len == 12 && !dash)
                    {
                        string first6digit = eText.Text.Substring(0, 6);
                        string digit78 = eText.Text.Substring(eText.Text.Length - 6, 2);
                        string lastdigit = eText.Text.Substring(eText.Text.Length - 4);
                        eText.Text = first6digit + "-" + digit78 + "-" + lastdigit;
                        eText.SetSelection(eText.Text.Length);
                    }
                }
                else if (Idtype.Equals("Army / Police ID") || Idtype.Equals("Kad Pengenalan Tentera / Polis"))
                {
                    eText.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(50) });
                }
                else if (Idtype.Equals("Passport") || Idtype.Equals("Pasport"))
                {
                    eText.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(50) });
                }
            }

            public void BeforeTextChanged(Java.Lang.ICharSequence s, int start, int count, int after)
            {
                int len = eText.Text.Length;
                int totallength = eText.SelectionStart;
                int totallenafter = len - totallength;
                string Idtype = idText.Text;

                if (Idtype.Equals("IC / Mykad") && len > 0 && (totallenafter != 0) && !flagDel)
                {
                    flagDel = true;
                    KeyListener.KeyDel = 0;
                    eText.Text = s.ToString();
                    eText.SetSelection(eText.Text.Length);
                    //string input = s.ToString();
                    //char a = input[totallength - 1];
                    //string b = a.ToString();
                    //if (b.Equals("-"))
                    //{
                    //    eText.Text = s.ToString();
                    //    eText.SetSelection(eText.Text.Length);
                    //}
                }
            }

            public void OnTextChanged(Java.Lang.ICharSequence s, int start, int before, int count)
            {
                string Idtype = idText.Text;
                if (Idtype.Equals("IC / MyKad"))
                {
                    eText.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(14) });
                    if (!flagDel)
                    {
                        int len = eText.Text.Length;
                        if ((len == 6 || len == 9 || start == 5 || start == 8) && before == 0)
                        {
                            eText.Text = eText.Text + "-";
                            eText.SetSelection(eText.Text.Length);
                        }

                        if (len == 7 && before == 0)
                        {
                            string first6digit = eText.Text.Substring(0, 6);
                            string last1digit = eText.Text.Substring(eText.Text.Length - 1);
                            eText.Text = first6digit + "-" + last1digit;
                            eText.SetSelection(eText.Text.Length);
                        }

                        if (len == 10 && before == 0)
                        {
                            string first9digit = eText.Text.Substring(0, 9);
                            string last1digit = eText.Text.Substring(eText.Text.Length - 1);
                            eText.Text = first9digit + "-" + last1digit;
                            eText.SetSelection(eText.Text.Length);
                        }
                    }
                    flagDel = false;
                }
                else if (Idtype.Equals("Army / Police ID") || Idtype.Equals("Kad Pengenalan Tentera / Polis"))
                {
                    eText.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(50) });
                }
                else if (Idtype.Equals("Passport") || Idtype.Equals("Pasport"))
                {
                    eText.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(50) });
                }
            }

        }
        public class KeyListener : Java.Lang.Object, Android.Views.View.IOnKeyListener
        {
            public static int KeyDel { get; set; }
            public bool OnKey(Android.Views.View v, Android.Views.Keycode keyCode, Android.Views.KeyEvent e)
            {
                if (keyCode == Android.Views.Keycode.Del || keyCode == Android.Views.Keycode.Back)
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
                string Idtype = selectedIdentificationType.Id;
                ClearICMinimumCharactersError();
                ClearICHint();
                //EnableRegisterButton();
                this.userActionsListener.validateField(ic_no,Idtype);
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

        public void ShowInvalidIdentificationError()
        {
            this.SetIsClicked(false);
            Utility.ShowIdentificationErrorDialog(this, () =>
            {
                ShowProgressDialog();
            });

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
                    this.SetIsClicked(true);
                    string Idtype = selectedIdentificationType.Id;
                    string ic_no = txtICNumber.Text.ToString().Trim();

                    this.userActionsListener.OnCheckID(ic_no, Idtype);
                    //this.userActionsListener.CheckRequiredFields(ic_no, Idtype);

                    bool hasExistedID = MyTNBAccountManagement.GetInstance().IsIDUpdated();

                    if (hasExistedID)
                    {
                        //this.userActionsListener.OnUpdateIC(Idtype, ic_no);
                        ShowUpdateIdDialog(this, () =>
                        {
                            // _ = RunUpdateID(idtype,ic_no);
                            ShowProgress();
                            this.userActionsListener.OnUpdateIC(Idtype, ic_no);
                        });
                    }
                    else
                    {
                        ShowInvalidIdentificationError();
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
                        .SetTitle(Utility.GetLocalizedLabel("CompleteYourProfile", "updateIdTitle"))
                        .SetSubTitle(ic_no)
                        .SetMessage(Utility.GetLocalizedLabel("CompleteYourProfile", "updateIdMessage"))
                        .SetContentGravity(Android.Views.GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("CompleteYourProfile", "re_enter"))
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
            if (textInputLayoutICNo.Error != Utility.GetLocalizedLabel("OneLastThing", "mykadhint"))
            {
                textInputLayoutICNo.Error = Utility.GetLocalizedLabel("OneLastThing", "mykadhint");
            }

            if (!textInputLayoutICNo.ErrorEnabled)
                textInputLayoutICNo.ErrorEnabled = true;
        }

        public void ShowFullArmyIdError()
        {
            // ClearFullNameError();
            if (textInputLayoutICNo.Error != Utility.GetLocalizedLabel("OneLastThing", "armyidhint"))
            {
                textInputLayoutICNo.Error = Utility.GetLocalizedLabel("OneLastThing", "armyidhint");
            }

            if (!textInputLayoutICNo.ErrorEnabled)
                textInputLayoutICNo.ErrorEnabled = true;
        }

        public void ShowFullPassportError()
        {
            // ClearFullNameError();
            if (textInputLayoutICNo.Error != Utility.GetLocalizedLabel("OneLastThing", "passporthint"))
            {
                textInputLayoutICNo.Error = Utility.GetLocalizedLabel("OneLastThing", "passporthint");
            }

            if (!textInputLayoutICNo.ErrorEnabled)
                textInputLayoutICNo.ErrorEnabled = true;
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
                                txtICNumber.Text = "";
                                if (selectedIdentificationType.Id.Equals("1"))
                                {
                                    txtICNumber.InputType = InputTypes.ClassNumber;
                                }
                                else
                                {
                                    string digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890abcdefghijklmnopqrstuvwxyz"; // or any characters you want to allow
                                    txtICNumber.KeyListener = Android.Text.Method.DigitsKeyListener.GetInstance(digits);
                                    txtICNumber.SetRawInputType(InputTypes.ClassText);
                                }
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
