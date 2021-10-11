using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.TextField;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Common;
using myTNB_Android.Src.Common.Activity;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Enquiry.GSL.MVP;
using myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Enquiry.GSL.Activity
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

        [BindView(Resource.Id.gslStepOnebtnNext)]
        Button gslStepOnebtnNext;

        private MobileNumberInputComponent mobileNumberInputComponent;

        private GSLRebateStepOneContract.IUserActionsListener userActionsListener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _ = new GSLRebateStepOnePresenter(this);
            this.userActionsListener?.OnInitialize();
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

            var stepTitleString = string.Format(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_STEP_TITLE), 1, 2);
            gslStepOnePageTitle.Text = stepTitleString;

            txtGSLTenantFullNameLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
            txtGSLTenantEmailLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);

            txtGSLTenantFullNameLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
            txtGSLTenantEmailLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);

            txtGSLTenantFullName.TextChanged += FullNameTextChanged;
            txtGSLTenantFullName.AddTextChangedListener(new InputFilterFormField(txtGSLTenantFullName, txtGSLTenantFullNameLayout));

            txtGSLTenantEmail.TextChanged += EmailTextChanged;
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

        public void SetPresenter(GSLRebateStepOneContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
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
                    if (!txtGSLTenantFullName.Text.IsValid())
                    {
                        ShowEmptyError(GSLLayoutType.FULL_NAME);
                    }
                    else
                    {
                        ClearErrors(GSLLayoutType.FULL_NAME);
                    }
                    break;
                case GSLLayoutType.EMAIL_ADDRESS:
                    if (!txtGSLTenantEmail.Text.IsValid())
                    {
                        ShowEmptyError(GSLLayoutType.EMAIL_ADDRESS);
                    }
                    else
                    {
                        ClearErrors(GSLLayoutType.EMAIL_ADDRESS);
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
            var mobileNotEmpty = !mobileNumberInputComponent.IsTextClear();

            UpdateButtonState(fullName.IsValid() && email.IsValid() && mobileNotEmpty);
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
                SaveFields();

                if (this.userActionsListener.CheckRequiredFields())
                {
                    OnShowGSLRebateStepTwoActivity();
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
                    }
                    break;
                case GSLLayoutType.EMAIL_ADDRESS:
                    {
                        txtGSLTenantEmailLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
                        TextViewUtils.SetMuseoSans300Typeface(txtGSLTenantEmailLayout.FindViewById<TextView>(Resource.Id.textinput_error));
                        txtGSLTenantEmailLayout.Error = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.EMAIL_ERROR);
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
    }
}
