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
using myTNB_Android.Src.XDetailRegistrationForm.MVP;
using myTNB_Android.Src.TermsAndConditions.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.XDetailRegistrationForm.Models;
using myTNB_Android.Src.XDetailRegistrationForm.Adapter;
using Newtonsoft.Json;
using Refit;
using System;
using System.Runtime;
using myTNB_Android.Src.XDetailRegistrationForm.Activity;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Android.Graphics;
using Android.Content.Res;
using System.Linq;
using Android.InputMethodServices;
using static myTNB_Android.Src.RegistrationForm.Activity.DetailRegistrationFormActivity.KeyListener;
using myTNB_Android.Src.Base;
using Java.Util.Regex;
using System.Collections.Generic;

namespace myTNB_Android.Src.RegistrationForm.Activity
{
    [Activity(Label = "@string/registration_activity_title"
      , NoHistory = false
              , Icon = "@drawable/ic_launcher"
      , ScreenOrientation = ScreenOrientation.Portrait
      , Theme = "@style/Theme.OwnerTenantBaseTheme")]
    public class DetailRegistrationFormActivity : BaseActivityCustom, RegisterFormContract.IView
    {
        private RegisterFormPresenter mPresenter;
        private RegisterFormContract.IUserActionsListener userActionsListener;

        private AlertDialog mVerificationProgressDialog;
        private AlertDialog mRegistrationProgressDialog;
        const string PAGE_ID = "Register";
        private MobileNumberInputComponent mobileNumberInputComponent;
        const int COUNTRY_CODE_SELECT_REQUEST = 1;
        private readonly int SELECT_IDENTIFICATION_TYPE_REQ_CODE = 2011;
        private IdentificationType selectedIdentificationType;
        private IdentificationTypeAdapter identificationType;


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

        [BindView(Resource.Id.txtAccountType)]
        TextView txtAccountType;

        [BindView(Resource.Id.selector_account_type)]
        TextView identityType;

        [BindView(Resource.Id.btnRegister)]
        Button btnRegister;

        [BindView(Resource.Id.checkboxCondition)]
        CheckBox txtboxcondition;

        [BindView(Resource.Id.txtTitleRegister)]
        TextView txtTitleRegister;

        [BindView(Resource.Id.txtBodyRegister)]
        TextView txtBodyRegister;

        private bool isClicked = false;

        UserCredentialsEntity entity = new UserCredentialsEntity();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                if (extras.ContainsKey(Constants.USER_CREDENTIALS_ENTRY))
                {
                    entity = JsonConvert.DeserializeObject<UserCredentialsEntity>(extras.GetString(Constants.USER_CREDENTIALS_ENTRY));
                }
            }

            try
            {
                this.mPresenter = new RegisterFormPresenter(this);

                txtAccountType = FindViewById<TextView>(Resource.Id.txtAccountType);
                identityType = FindViewById<TextView>(Resource.Id.selector_account_type);

               TextViewUtils.SetMuseoSans500Typeface(txtTitleRegister);

               TextViewUtils.SetMuseoSans300Typeface(
                    txtAccountType,
                    identityType,
                    txtFullName,
                    txtICNumber,
                    txtTermsConditions,
                    txtBodyRegister);

                TextViewUtils.SetMuseoSans300Typeface(
                    textInputLayoutFullName,
                    textInputLayoutICNo);

                TextViewUtils.SetMuseoSans500Typeface(btnRegister);


                TextViewUtils.SetTextSize18(txtTitleRegister, btnRegister);
                TextViewUtils.SetTextSize16(txtAccountType, identityType, txtFullName, txtICNumber, txtTermsConditions, txtBodyRegister);
                TextViewUtils.SetTextSize14(txtTermsConditions);

                txtTitleRegister.Text = Utility.GetLocalizedLabel("OneLastThing", "dtitleRegister");
                txtBodyRegister.Text = Utility.GetLocalizedLabel("OneLastThing", "dbodyRegister");
                txtAccountType.Text = Utility.GetLocalizedLabel("OneLastThing", "idtypeTitle").ToUpper();
                textInputLayoutFullName.Hint = Utility.GetLocalizedLabel("Common", "name");
                textInputLayoutICNo.Hint = Utility.GetLocalizedLabel("OneLastThing", "idNumberhint");
                txtTermsConditions.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("OneLastThing", "tnc"));
                StripUnderlinesFromLinks(txtTermsConditions);
                btnRegister.Text = Utility.GetLocalizedLabel("OneLastThing", "ctaTitleNew");

                var boxcondition = new CheckBox(this)
                {
                    ScaleX = 0.8f,
                    ScaleY = 0.8f
                };

                txtICNumber.FocusChange += txtICNumber_FocusChange;
                txtboxcondition.CheckedChange += CheckedChange;
                txtICNumber.AfterTextChanged += new EventHandler<AfterTextChangedEventArgs>(AddTextChangedListener);
                txtFullName.TextChanged += TxtFullName_TextChanged;
                txtFullName.AddTextChangedListener(new InputFilterFormField(txtFullName, textInputLayoutFullName));
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

                mobileNumberFieldContainer.RemoveAllViews();
                mobileNumberInputComponent = new MobileNumberInputComponent(this);
                mobileNumberInputComponent.SetOnTapCountryCodeAction(OnTapCountryCode);
                mobileNumberInputComponent.SetMobileNumberLabel(Utility.GetLocalizedCommonLabel("mobileNo"));
                mobileNumberInputComponent.SetSelectedCountry(CountryUtil.Instance.GetDefaultCountry());
                mobileNumberInputComponent.SetValidationAction(OnValidateMobileNumber);
                mobileNumberFieldContainer.AddView(mobileNumberInputComponent);

                ClearFields();
                ClearAllErrorFields();

                txtFullName.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.placeholder_name, 0, 0, 0);
                txtICNumber.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.placeholder_account_no, 0, 0, 0);

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

        private void TxtFullName_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool checkbox = txtboxcondition.Checked;
            string fullname = txtFullName.Text.ToString().Trim();
            string ic_no = txtICNumber.Text.ToString().Trim();
            string Idtype = selectedIdentificationType.Id;
            string mobile_no = mobileNumberInputComponent.GetMobileNumberValue();
            this.userActionsListener.validateField(fullname, ic_no, mobile_no, Idtype, checkbox);
        }

        private void AddTextChangedListener(object sender, AfterTextChangedEventArgs e)
        {
            try
            {
                bool checkbox = txtboxcondition.Checked;
                string fullname = txtFullName.Text.ToString().Trim();
                string ic_no = txtICNumber.Text.ToString().Trim();
                string Idtype = selectedIdentificationType.Id;
                string mobile_no = mobileNumberInputComponent.GetMobileNumberValue();
                ClearICMinimumCharactersError();
                //ClearICHint();
                ShowIdentificationHint();
                this.userActionsListener.validateField(fullname, ic_no, mobile_no, Idtype, checkbox);
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
            //private bool changed = false;
            private EditText eText;
            private TextView idText;


            public void AfterTextChanged(IEditable s)
            {
                int len = eText.Text.Length;
                bool dash = eText.Text.Contains("-");
                string Idtype = idText.Text;

                if (Idtype.Equals("IC / MyKad") || Idtype.Equals("Kad Pengenalan / MyKad"))
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

                if ((Idtype.Equals("IC / MyKad") || Idtype.Equals("Kad Pengenalan / MyKad")) && len > 0 && (totallenafter != 0) && !flagDel)
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
                
                if (Idtype.Equals("IC / MyKad") || Idtype.Equals("Kad Pengenalan / MyKad"))
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

                /*if (e.Action == KeyEventActions.Down && e.KeyCode == Android.Views.Keycode.Back)
                    KeyDel = 1;
                else
                    KeyDel = 0;
                return false;*/
            }
        }


        private void txtICNumber_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            try
            {
                bool checkbox = txtboxcondition.Checked;
                string Idtype = selectedIdentificationType.Id;
                string fullname = txtFullName.Text.ToString().Trim();
                string ic_no = txtICNumber.Text.ToString().Trim();
                string mobile_no = mobileNumberInputComponent.GetMobileNumberValue();

                if (!e.HasFocus)
                {
                    ClearICMinimumCharactersError();
                    ClearICHint();
                    this.userActionsListener.CheckRequiredFields(fullname, ic_no, mobile_no, Idtype, txtboxcondition.Checked);

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

        private void CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (!txtboxcondition.Checked)
            {
                txtTermsConditions.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("OneLastThing", "tnc"));
                //txtTermsConditions.TextFormatted = GetFormattedText(GetLabelByLanguage("tncNew"));
                StripUnderlinesFromLinks(txtTermsConditions);
                mobileNumberInputComponent.ClearError();

            }
            else
            {
                txtTermsConditions.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel("OneLastThing", "tnc_checked"));
                //txtTermsConditions.TextFormatted = GetFormattedText(GetLabelByLanguage("tnc_checked"));
                StripUnderlinesFromLinks(txtTermsConditions);
                if (mobileNumberInputComponent.IsTextClear())
                {
                    mobileNumberInputComponent.NewRaiseError();
                }
                else
                {
                    mobileNumberInputComponent.ClearError();
                }
            }
            string fullname = txtFullName.Text.ToString().Trim();
            string ic_no = txtICNumber.Text.ToString().Trim(); 
            string Idtype = selectedIdentificationType.Id;
            string mobile_no = mobileNumberInputComponent.GetMobileNumberValue();
            this.userActionsListener.CheckRequiredFields(fullname, ic_no, mobile_no, Idtype, txtboxcondition.Checked);



        }

        private void OnValidateMobileNumber(bool isValidated)
        {
            string fullname = txtFullName.Text.ToString().Trim();
            string ic_no = txtICNumber.Text.ToString().Trim();
            string Idtype = selectedIdentificationType.Id;
            string mobile_no = mobileNumberInputComponent.GetMobileNumberValue();
            this.userActionsListener.CheckRequiredFields(fullname, ic_no, mobile_no, Idtype, txtboxcondition.Checked);

        }

        public void OnPause()
        {
            base.OnPause();
            isClicked = true;
        }

        public override int ResourceId()
        {
            return Resource.Layout.DetailRegistrationFormView;
        }

        public void ClearFields()
        {
            txtFullName.Text = "";
            txtICNumber.Text = "";
            mobileNumberInputComponent.ClearMobileNumber();
            txtFullName.ClearFocus();
            txtICNumber.ClearFocus();
            
        }

        public void ClearAllErrorFields()
        {
            if (!string.IsNullOrEmpty(textInputLayoutICNo.Error))
            {
                textInputLayoutICNo.Error = null;
                textInputLayoutICNo.ErrorEnabled = false;
            }
            if (!string.IsNullOrEmpty(textInputLayoutICNo.HelperText))
            {
                textInputLayoutICNo.HelperText = null;
                textInputLayoutICNo.HelperTextEnabled = false;
            }
            if (!string.IsNullOrEmpty(textInputLayoutFullName.Error))
            {
                textInputLayoutFullName.Error = null;
                textInputLayoutFullName.ErrorEnabled = false;
            }
            if (!string.IsNullOrEmpty(textInputLayoutFullName.HelperText))
            {
                textInputLayoutFullName.HelperText = null;
                textInputLayoutFullName.HelperTextEnabled = false;
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


        public void ShowEmptyFullNameError()
        {
           // ClearFullNameError();
           if(textInputLayoutFullName.Error != GetString(Resource.String.registration_form_errors_empty_fullname))
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
            ClearICMinimumCharactersError();
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

            if (textInputLayoutPassword.Error != Utility.GetLocalizedErrorLabel("invalid_password")) {
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
            if(textInputLayoutEmail.Error != Utility.GetLocalizedLabel("RegisterNew", "invalidEmailTryAgain"))
            {
                textInputLayoutEmail.Error = Utility.GetLocalizedLabel("RegisterNew", "invalidEmailTryAgain");
            }
           
            if (!textInputLayoutEmail.ErrorEnabled)
                textInputLayoutEmail.ErrorEnabled = true;
        }

        public void ShowIdentificationHint()
        {
            ClearICMinimumCharactersError();
            if (textInputLayoutICNo.HelperText != Utility.GetLocalizedLabel("OneLastThing", "identificationhint"))
            {
                textInputLayoutICNo.HelperText = Utility.GetLocalizedLabel("OneLastThing", "identificationhint");
            }

            if (!textInputLayoutICNo.HelperTextEnabled)
                textInputLayoutICNo.HelperTextEnabled = true;
        }

        public void ShowInvalidICNoError()
        {
            //ClearICError();
            if(textInputLayoutICNo.Error != GetString(Resource.String.registration_form_errors_invalid_icno))
            {
                textInputLayoutICNo.Error = GetString(Resource.String.registration_form_errors_invalid_icno);
            }
           
            if (!textInputLayoutICNo.ErrorEnabled)
                textInputLayoutICNo.ErrorEnabled = true;
        }

        public void ClearICHint()
        {
            textInputLayoutICNo.HelperText = null;
            textInputLayoutICNo.HelperTextEnabled = false;
        }

        public void ClearICError()
        {
            textInputLayoutICNo.ErrorEnabled = false;
        }


        public void ShowNotEqualConfirmEmailError()
        {
            //ClearNotEqualConfirmEmailError();
            if(textInputLayoutConfirmEmail.Error != Utility.GetLocalizedErrorLabel("invalid_mismatchedEmail"))
            {
                textInputLayoutConfirmEmail.Error = Utility.GetLocalizedErrorLabel("invalid_mismatchedEmail");
            }
           
            if (!textInputLayoutConfirmEmail.ErrorEnabled)
                textInputLayoutConfirmEmail.ErrorEnabled = true;
        }

        public void ShowNotEqualConfirmPasswordError()
        {
            //ClearNotEqualConfirmPasswordError();
            if(textInputLayoutConfirmPassword.Error != Utility.GetLocalizedErrorLabel("invalid_mismatchedPassword"))
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
                    string idtype = selectedIdentificationType.Id.ToString().Trim();
                    string mobile_no = mobileNumberInputComponent.GetMobileNumberValueWithISDCode();
                    string eml_str = entity.Email.Trim();
                    string password = entity.Password.Trim();
                    string Idtype = selectedIdentificationType.Id;

                    this.userActionsListener.OnCheckID(ic_no, Idtype);

                    bool hasExistedID = MyTNBAccountManagement.GetInstance().IsIDUpdated();
                    
                    if (hasExistedID)
                    {
                        this.userActionsListener.OnAcquireToken(fName, ic_no, mobile_no, eml_str, password, idtype);
                    }
                    else
                    {
                        ShowInvalidIdentificationError();
                        DisableRegisterButton();
                        //if (Idtype.Equals("1"))
                        //{
                        //   ShowFullICError();
                        //}
                        //else if (Idtype.Equals("2"))
                        //{
                        //  ShowFullArmyIdError();
                        //}
                        //else
                        //{
                        //  ShowFullPassportError();
                        //}
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
            isClicked = false;

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
                string email = entity.Email;
                string password = entity.Password;
                string idtype = selectedIdentificationType.Id.ToString().Trim();
                ic_no = ic_no.Replace("-", string.Empty);
                this.userActionsListener.OnAcquireToken(fullname, ic_no, mobile_no, email, password,idtype);

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
                string email = entity.Email;
                string password = entity.Password;
                string idtype = selectedIdentificationType.Id.ToString().Trim();
                ic_no = ic_no.Replace("-", string.Empty);
                this.userActionsListener.OnAcquireToken(fullname, ic_no, mobile_no, email, password, idtype);

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
                string email = entity.Email;
                string password = entity.Password;
                string idtype = selectedIdentificationType.Id.ToString().Trim();
                ic_no = ic_no.Replace("-", string.Empty);
                this.userActionsListener.OnAcquireToken(fullname, ic_no, mobile_no, email, password, idtype);

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

        public void ClearIdentificationHint()
        {
            if (!string.IsNullOrEmpty(textInputLayoutICNo.HelperText))
            {
                textInputLayoutICNo.HelperText = null;
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

        public void ClearICMinimumCharactersError()
        {
            if (!string.IsNullOrEmpty(textInputLayoutICNo.Error))
            {
                textInputLayoutICNo.Error = null;
                textInputLayoutICNo.ErrorEnabled = false;
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


        //public void ShowFullNameError()
        //{
        //    // ClearFullNameError();
        //    if (textInputLayoutFullName.Error != GetString(Resource.String.name_error))
        //    {
        //        textInputLayoutFullName.Error = GetString(Resource.String.name_error);
        //    }
        //    textInputLayoutFullName.Error = GetString(Resource.String.name_error);
        //    if (!textInputLayoutFullName.ErrorEnabled)
        //        textInputLayoutFullName.ErrorEnabled = true;
        //}
       
        public void ShowFullICError()
        {
            //ClearICHint();
            if (textInputLayoutICNo.Error != Utility.GetLocalizedLabel("OneLastThing", "mykadhint"))
            {
                textInputLayoutICNo.Error = Utility.GetLocalizedLabel("OneLastThing", "mykadhint");
            }

            if (!textInputLayoutICNo.ErrorEnabled)
                textInputLayoutICNo.ErrorEnabled = true;
        }

        public void ShowFullArmyIdError()
        {
            //ClearICHint();
            if (textInputLayoutICNo.Error != Utility.GetLocalizedLabel("OneLastThing", "armyidhint")) 
            {
                textInputLayoutICNo.Error = Utility.GetLocalizedLabel("OneLastThing", "armyidhint");
            }

            if (!textInputLayoutICNo.ErrorEnabled)
                textInputLayoutICNo.ErrorEnabled = true;
        }

        public void ShowFullPassportError()
        {
            //ClearICHint();
            if (textInputLayoutICNo.Error != Utility.GetLocalizedLabel("OneLastThing", "passporthint"))
            {
                textInputLayoutICNo.Error = Utility.GetLocalizedLabel("OneLastThing", "passporthint");
            }

            if (!textInputLayoutICNo.ErrorEnabled)
                textInputLayoutICNo.ErrorEnabled = true;
        }

        public void ClearFullNameError()
        {
            textInputLayoutFullName.Error = null;
            textInputLayoutFullName.ErrorEnabled = false;
        }

        public void ShowInvalidIdentificationError()
        {
            this.SetIsClicked(false);
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
    }
}
