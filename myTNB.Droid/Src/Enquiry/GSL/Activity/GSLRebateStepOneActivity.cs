using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.TextField;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.Common;
using myTNB.Android.Src.Common.Activity;
using myTNB.Android.Src.Common.Model;
using myTNB.Android.Src.CompoundView;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.Enquiry.GSL.MVP;
using myTNB.Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using static Android.Views.View;

namespace myTNB.Android.Src.Enquiry.GSL.Activity
{
    [Activity(Label = "GSL Rebate Step One"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.Enquiry")]
    public class GSLRebateStepOneActivity : BaseToolbarAppCompatActivity, GSLRebateStepOneContract.IView
    {
        [BindView(Resource.Id.gslStepOnePageTitle)]
        TextView gslStepOnePageTitle;

        [BindView(Resource.Id.txtRebateTypeTitle)]
        TextView txtRebateTypeTitle;

        [BindView(Resource.Id.txtRebatetType)]
        TextView txtRebatetType;

        [BindView(Resource.Id.selectorGSLTypeOfRebate)]
        TextView selectorGSLTypeOfRebate;

        [BindView(Resource.Id.txtTenantInfoTitle)]
        TextView txtTenantInfoTitle;

        [BindView(Resource.Id.txtGSLTenantFullNameLayout)]
        TextInputLayout txtGSLTenantFullNameLayout;

        [BindView(Resource.Id.txtGSLTenantFullName)]
        EditText txtGSLTenantFullName;

        [BindView(Resource.Id.txtGSLTenantEmailLayout)]
        TextInputLayout txtGSLTenantEmailLayout;

        [BindView(Resource.Id.txtGSLTenantEmail)]
        EditText txtGSLTenantEmail;

        [BindView(Resource.Id.tenantGSLMobileNumberContainer)]
        LinearLayout tenantGSLMobileNumberContainer;

        [BindView(Resource.Id.gslTenantInfoContainer)]
        LinearLayout gslTenantInfoContainer;

        [BindView(Resource.Id.gslStepOnebtnNext)]
        Button gslStepOnebtnNext;

        private MobileNumberInputComponent mobileNumberInputComponent;

        private GSLRebateStepOneContract.IUserActionsListener userActionsListener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                _ = new GSLRebateStepOnePresenter(this);
                this.userActionsListener?.OnInitialize();

                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.ACCOUNT_NUMBER))
                    {
                        this.userActionsListener?.SetAccountNumber(extras.GetString(Constants.ACCOUNT_NUMBER));
                    }
                    if (extras.ContainsKey(Constants.IS_OWNER))
                    {
                        this.userActionsListener?.SetIsOwner(extras.GetBoolean(Constants.IS_OWNER));
                        this.userActionsListener?.SaveAccountInfo();
                        gslTenantInfoContainer.Visibility = this.userActionsListener.GetGSLRebateModel().IsOwner ? ViewStates.Gone : ViewStates.Visible;
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_HEADER_TITLE));

            TextViewUtils.SetMuseoSans300Typeface(txtGSLTenantFullNameLayout, txtGSLTenantEmailLayout);
            TextViewUtils.SetMuseoSans300Typeface(txtRebatetType, selectorGSLTypeOfRebate);
            TextViewUtils.SetMuseoSans300Typeface(txtGSLTenantFullName, txtGSLTenantEmail);

            TextViewUtils.SetMuseoSans500Typeface(gslStepOnePageTitle, txtRebateTypeTitle, txtTenantInfoTitle, gslStepOnebtnNext);

            TextViewUtils.SetTextSize10(txtRebatetType);
            TextViewUtils.SetTextSize12(gslStepOnePageTitle);
            TextViewUtils.SetTextSize16(txtRebateTypeTitle, txtGSLTenantFullName, txtGSLTenantEmail, selectorGSLTypeOfRebate, txtTenantInfoTitle, gslStepOnebtnNext);

            var stepTitleString = string.Format(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_STEP_TITLE), 1, 4);
            gslStepOnePageTitle.Text = stepTitleString;

            txtGSLTenantFullNameLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
            txtGSLTenantEmailLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);

            txtGSLTenantFullName.TextChanged += FullNameTextChanged;
            txtGSLTenantFullName.FocusChange += FullNameTextOnFocusChanged;
            txtGSLTenantFullName.AddTextChangedListener(new InputFilterFormField(txtGSLTenantFullName, txtGSLTenantFullNameLayout));

            txtGSLTenantEmail.TextChanged += EmailTextChanged;
            txtGSLTenantEmail.FocusChange += EmailTextOnFocusChanged;
            txtGSLTenantEmail.AddTextChangedListener(new InputFilterFormField(txtGSLTenantEmail, txtGSLTenantEmailLayout));

            Drawable fullNameIcon = ContextCompat.GetDrawable(this, Resource.Drawable.placeholder_name);
            txtGSLTenantFullName.SetCompoundDrawablesWithIntrinsicBounds(fullNameIcon, null, null, null);

            Drawable emailIcon = ContextCompat.GetDrawable(this, Resource.Drawable.placeholder_email);
            txtGSLTenantEmail.SetCompoundDrawablesWithIntrinsicBounds(emailIcon, null, null, null);

            tenantGSLMobileNumberContainer.RemoveAllViews();
            mobileNumberInputComponent = new MobileNumberInputComponent(this);
            mobileNumberInputComponent.SetOnTapCountryCodeAction(OnTapCountryCode);
            mobileNumberInputComponent.SetMobileNumberLabel(Utility.GetLocalizedCommonLabel(LanguageConstants.Common.MOBILE_NO));
            mobileNumberInputComponent.SetSelectedCountry(CountryUtil.Instance.GetDefaultCountry());
            mobileNumberInputComponent.SetValidationAction(OnValidateMobileNumber);
            tenantGSLMobileNumberContainer.AddView(mobileNumberInputComponent);

            txtRebateTypeTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_CLAIM_TITLE);
            txtRebatetType.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_REBATE_TYPE_TITLE);
            txtTenantInfoTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_TENANT_INFO_TITLE);

            txtGSLTenantFullNameLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.FULL_NAME_HINT);
            txtGSLTenantEmailLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.EMAIL_HINT);

            txtGSLTenantFullNameLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
            txtGSLTenantEmailLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);

            gslStepOnebtnNext.Text = Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.NEXT);

            UpdateSelectedRebateType(this.userActionsListener.GetDefaultSelectedRebateType());
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.GSLRebateStepOneView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public override bool CameraPermissionRequired()
        {
            return true;
        }

        public override bool StoragePermissionRequired()
        {
            return true;
        }

        public void SetPresenter(GSLRebateStepOneContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public void PrepopulateTenantFields()
        {
            if (UserEntity.IsCurrentlyActive())
            {
                string name = UserEntity.GetActive().DisplayName;
                string email = UserEntity.GetActive().Email;
                string mobile = UserEntity.GetActive().MobileNo;

                this.userActionsListener.SetTenantFullName(name);
                this.userActionsListener.SetTenantEmailAddress(email);
                this.userActionsListener.SetTenantMobileNumber(mobile);

                txtGSLTenantFullName.Text = name;
                txtGSLTenantEmail.Text = email;

                CheckHintDisplay(GSLLayoutType.FULL_NAME, true);
                CheckHintDisplay(GSLLayoutType.EMAIL_ADDRESS, true);

                SetMobileNumberField(mobile);

                CheckFieldsForButtonState();
            }
        }

        private void OnValidateMobileNumber(bool isValidated)
        {
            CheckFieldsForButtonState();
        }

        private void ValidateField(GSLLayoutType type)
        {
            switch (type)
            {
                case GSLLayoutType.FULL_NAME:
                    string fullName = txtGSLTenantFullName.Text;
                    if (fullName.Trim().IsValid())
                    {
                        if (Utility.IsValidAccountName(fullName.Trim()))
                        {
                            ClearErrors(type);
                        }
                        else
                        {
                            ShowInvalidErrror(type);
                        }
                    }
                    else
                    {
                        ShowEmptyError(type);
                    }
                    break;
                case GSLLayoutType.EMAIL_ADDRESS:
                    string email = txtGSLTenantEmail.Text;
                    if (email.Trim().IsValid())
                    {
                        if (!Patterns.EmailAddress.Matcher(email.Trim()).Matches())
                        {
                            ShowInvalidErrror(type);
                        }
                        else
                        {
                            ClearErrors(type);
                        }
                    }
                    else
                    {
                        ShowEmptyError(type);
                    }
                    break;
                default:
                    break;
            }
            CheckFieldsForButtonState();
        }

        private void CheckFieldsForButtonState()
        {
            string fullName = txtGSLTenantFullName.Text;
            string email = txtGSLTenantEmail.Text;
            bool fullNameIsValid;
            bool emailIsValid;
            bool mobileIsValid = ValidateMobileNumField();

            if (fullName.Trim().IsValid())
            {
                if (Utility.IsValidAccountName(fullName.Trim()))
                {
                    this.ClearErrors(GSLLayoutType.FULL_NAME);
                    fullNameIsValid = true;
                }
                else
                {
                    this.ShowInvalidErrror(GSLLayoutType.FULL_NAME);
                    fullNameIsValid = false;
                }
            }
            else
            {
                this.ShowEmptyError(GSLLayoutType.FULL_NAME);
                fullNameIsValid = false;
            }

            if (email.Trim().IsValid())
            {
                if (!Patterns.EmailAddress.Matcher(email.Trim()).Matches())
                {
                    this.ShowInvalidErrror(GSLLayoutType.EMAIL_ADDRESS);
                    emailIsValid = false;
                }
                else
                {
                    this.ClearErrors(GSLLayoutType.EMAIL_ADDRESS);
                    emailIsValid = true;
                }
            }
            else
            {
                this.ShowEmptyError(GSLLayoutType.EMAIL_ADDRESS);
                emailIsValid = false;
            }

            UpdateButtonState(fullNameIsValid && emailIsValid && mobileIsValid);
        }

        private bool ValidateMobileNumField()
        {
            bool isValid;
            if (mobileNumberInputComponent.IsTextClear())
            {
                mobileNumberInputComponent.RaiseError(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.MOBILE_ERROR));
                isValid = false;
            }
            else
            {
                mobileNumberInputComponent.ClearError();
                isValid = true;
            }
            return isValid;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            this.userActionsListener.OnActivityResult(requestCode, resultCode, data);
        }

        public void SetSelectedCountry(Country country)
        {
            mobileNumberInputComponent.SetSelectedCountry(country);
        }

        [Preserve]
        private void FullNameTextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                ValidateField(GSLLayoutType.FULL_NAME);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [Preserve]
        private void EmailTextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                ValidateField(GSLLayoutType.EMAIL_ADDRESS);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [Preserve]
        private void FullNameTextOnFocusChanged(object sender, FocusChangeEventArgs focusChangedEventArgs)
        {
            if (focusChangedEventArgs.HasFocus)
            {
                CheckHintDisplay(GSLLayoutType.FULL_NAME, true);
            }
            else
            {
                CheckHintDisplay(GSLLayoutType.FULL_NAME, false);
            }
        }

        [Preserve]
        private void EmailTextOnFocusChanged(object sender, FocusChangeEventArgs focusChangedEventArgs)
        {
            if (focusChangedEventArgs.HasFocus)
            {
                CheckHintDisplay(GSLLayoutType.EMAIL_ADDRESS, true);
            }
            else
            {
                CheckHintDisplay(GSLLayoutType.EMAIL_ADDRESS, false);
            }
        }

        private void CheckHintDisplay(GSLLayoutType type, bool onFocus)
        {
            if (type == GSLLayoutType.FULL_NAME)
            {
                if (onFocus)
                {
                    txtGSLTenantFullNameLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.FULL_NAME_HINT).ToUpper();
                }
                else if (!txtGSLTenantFullName.Text.IsValid())
                {
                    txtGSLTenantFullNameLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.FULL_NAME_HINT);
                }
            }
            else if (type == GSLLayoutType.EMAIL_ADDRESS)
            {
                if (onFocus)
                {
                    txtGSLTenantEmailLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.EMAIL_HINT).ToUpper();
                }
                else if (!txtGSLTenantEmail.Text.IsValid())
                {
                    txtGSLTenantEmailLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.EMAIL_HINT);
                }
            }
        }

        [OnClick(Resource.Id.selectorGSLTypeOfRebate)]
        public void Onselector_account_type(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                this.OnSelectRebateType();
            }
        }

        [OnClick(Resource.Id.gslStepOnebtnNext)]
        public void ButtonNextOnClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                if (!this.userActionsListener.GetGSLRebateModel().IsOwner)
                {
                    SaveFields();
                }

                if (this.userActionsListener.CheckRequiredFields())
                {
                    if (this.userActionsListener.GetGSLRebateModel().NeedsIncident)
                    {
                        OnShowGSLRebateStepTwoActivity();
                    }
                    else
                    {
                        OnShowGSLRebateStepThreeActivity();
                    }
                }
                else
                {
                    this.SetIsClicked(false);
                }
            }
        }

        public void UpdateSelectedRebateType(Item item)
        {
            RunOnUiThread(() =>
            {
                if (item != null)
                {
                    selectorGSLTypeOfRebate.Text = item.title ?? string.Empty;

                    int stepTotalNo = item.needsIncident ? 4 : 3;
                    var stepTitleString = string.Format(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_STEP_TITLE), 1, stepTotalNo);
                    gslStepOnePageTitle.Text = stepTitleString;
                }
            });
        }

        public void ShowEmptyError(GSLLayoutType layoutType)
        {
            switch (layoutType)
            {
                case GSLLayoutType.FULL_NAME:
                    {
                        txtGSLTenantFullNameLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
                        TextViewUtils.SetMuseoSans300Typeface(txtGSLTenantFullNameLayout.FindViewById<TextView>(Resource.Id.textinput_error));
                        txtGSLTenantFullNameLayout.Error = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.FULL_NAME_ERROR);
                        var handleBounceError = txtGSLTenantFullNameLayout.FindViewById<TextView>(Resource.Id.textinput_error);
                        handleBounceError.SetPadding(top: 4, left: 0, right: 0, bottom: 0);

                    }
                    break;
                case GSLLayoutType.EMAIL_ADDRESS:
                    {
                        txtGSLTenantEmailLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
                        TextViewUtils.SetMuseoSans300Typeface(txtGSLTenantEmailLayout.FindViewById<TextView>(Resource.Id.textinput_error));
                        txtGSLTenantEmailLayout.Error = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.EMAIL_ERROR);
                        var handleBounceError = txtGSLTenantEmailLayout.FindViewById<TextView>(Resource.Id.textinput_error);
                        handleBounceError.SetPadding(top: 4, left: 0, right: 0, bottom: 0);
                    }
                    break;
                default:
                    break;
            }
        }

        public void ShowInvalidErrror(GSLLayoutType layoutType)
        {
            switch (layoutType)
            {
                case GSLLayoutType.FULL_NAME:
                    {
                        txtGSLTenantFullNameLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
                        TextViewUtils.SetMuseoSans300Typeface(txtGSLTenantFullNameLayout.FindViewById<TextView>(Resource.Id.textinput_error));
                        txtGSLTenantFullNameLayout.Error = Utility.GetLocalizedErrorLabel(LanguageConstants.Error.INVALID_FULLNAME);
                        var handleBounceError = txtGSLTenantFullNameLayout.FindViewById<TextView>(Resource.Id.textinput_error);
                        handleBounceError.SetPadding(top: 4, left: 0, right: 0, bottom: 0);
                    }
                    break;
                case GSLLayoutType.EMAIL_ADDRESS:
                    {
                        txtGSLTenantEmailLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
                        TextViewUtils.SetMuseoSans300Typeface(txtGSLTenantEmailLayout.FindViewById<TextView>(Resource.Id.textinput_error));
                        txtGSLTenantEmailLayout.Error = Utility.GetLocalizedErrorLabel(LanguageConstants.Error.INVALID_EMAIL);
                        var handleBounceError = txtGSLTenantEmailLayout.FindViewById<TextView>(Resource.Id.textinput_error);
                        handleBounceError.SetPadding(top: 4, left: 0, right: 0, bottom: 0);
                    }
                    break;
                default:
                    break;
            }
        }

        public void ClearErrors(GSLLayoutType layoutType)
        {
            switch (layoutType)
            {
                case GSLLayoutType.FULL_NAME:
                    txtGSLTenantFullNameLayout.Error = "";
                    break;
                case GSLLayoutType.EMAIL_ADDRESS:
                    txtGSLTenantEmailLayout.Error = "";
                    break;
                default:
                    break;
            }
        }

        public void UpdateButtonState(bool isEnabled)
        {
            gslStepOnebtnNext.Enabled = isEnabled;
            gslStepOnebtnNext.Background = ContextCompat.GetDrawable(this, isEnabled ? Resource.Drawable.green_button_background :
                Resource.Drawable.silver_chalice_button_background);
        }

        public bool IsMobileNumEmpty()
        {
            return mobileNumberInputComponent.IsTextClear();
        }

        private void SetMobileNumberField(string value)
        {
            if (value.IsValid())
            {
                if (value.Contains("+"))
                {
                    var countryFromPhoneNumber = CountryUtil.Instance.GetCountryFromPhoneNumber(value);

                    if (countryFromPhoneNumber.ToString().IsValid())
                    {
                        mobileNumberInputComponent.SetSelectedCountry(countryFromPhoneNumber);
                        mobileNumberInputComponent.SetMobileNumber(int.Parse(value.Trim()[countryFromPhoneNumber.isd.Length..]));
                    }
                }
                else
                {
                    if (value.Trim().Substring(0, 1) == "6")
                    {
                        mobileNumberInputComponent.SetMobileNumber(int.Parse(value.Trim().Substring(2)));
                    }
                    else
                    {
                        mobileNumberInputComponent.SetMobileNumber(int.Parse(value.Trim()));
                    }
                }
            }
        }

        public void OnSelectRebateType()
        {
            Intent newIntent = new Intent(this, typeof(SelectItemActivity));
            newIntent.PutExtra("LIST_TITLE", Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_REBATE_TYPE_TITLE));
            newIntent.PutExtra("ITEM_LIST", JsonConvert.SerializeObject(this.userActionsListener.GetRebateTypeList()));
            StartActivityForResult(newIntent, EnquiryConstants.SELECT_REBATE_TYPE_REQ_CODE);
            this.SetIsClicked(false);
        }

        private void OnTapCountryCode()
        {
            Intent intent = new Intent(this, typeof(SelectCountryActivity));
            StartActivityForResult(intent, EnquiryConstants.COUNTRY_CODE_SELECT_REQUEST);
        }

        private void SaveFields()
        {
            this.userActionsListener.SetTenantFullName(txtGSLTenantFullName.Text);
            this.userActionsListener.SetTenantEmailAddress(txtGSLTenantEmail.Text);
            this.userActionsListener.SetTenantMobileNumber(mobileNumberInputComponent.GetMobileNumberValueWithISDCode());
        }

        private void OnShowGSLRebateStepTwoActivity()
        {
            this.SetIsClicked(true);
            Intent stepTwoActivity = new Intent(this, typeof(GSLRebateStepTwoActivity));
            stepTwoActivity.PutExtra(GSLRebateConstants.REBATE_MODEL, JsonConvert.SerializeObject(this.userActionsListener.GetGSLRebateModel()));
            StartActivity(stepTwoActivity);
        }

        private void OnShowGSLRebateStepThreeActivity()
        {
            this.SetIsClicked(true);
            Intent stepThreectivity = new Intent(this, typeof(GSLRebateStepThreeActivity));
            stepThreectivity.PutExtra(GSLRebateConstants.REBATE_MODEL, JsonConvert.SerializeObject(this.userActionsListener.GetGSLRebateModel()));
            StartActivity(stepThreectivity);
        }
    }
}
