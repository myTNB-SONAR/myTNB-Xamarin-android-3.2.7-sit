using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.Core.Content;
using Google.Android.Material.TextField;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Enquiry.Component
{
    public class EnquiryAccountDetailsComponent : LinearLayout
    {
        TextInputLayout txtEnquiryAcctDetailsFullNameLayout, txtEnquiryAcctDetailsEmailLayout;
        EditText txtEnquiryAcctDetailsFullName, txtEnquiryAcctDetailsEmail;
        LinearLayout enquiryAcctDetailsLMobileNumContainer;

        private MobileNumberInputComponent mobileNumberInputComponent;

        private readonly BaseAppCompatActivity mActivity;
        private readonly Context mContext;

        private Action OnTapCountryCodeAction;
        private Action<bool> CheckRequiredFieldsAction;

        public EnquiryAccountDetailsComponent(Context context, BaseAppCompatActivity activity) : base(context)
        {
            mContext = context;
            mActivity = activity;
            Init(mContext);
        }

        public EnquiryAccountDetailsComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mContext = context;
            Init(mContext);
        }

        public EnquiryAccountDetailsComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            mContext = context;
            Init(mContext);
        }

        public void Init(Context context)
        {
            Inflate(context, Resource.Layout.EnquiryAccountDetailsComponentView, this);
            txtEnquiryAcctDetailsFullNameLayout = FindViewById<TextInputLayout>(Resource.Id.txtEnquiryAcctDetailsFullNameLayout);
            txtEnquiryAcctDetailsEmailLayout = FindViewById<TextInputLayout>(Resource.Id.txtEnquiryAcctDetailsEmailLayout);

            txtEnquiryAcctDetailsFullName = FindViewById<EditText>(Resource.Id.txtEnquiryAcctDetailsFullName);
            txtEnquiryAcctDetailsEmail = FindViewById<EditText>(Resource.Id.txtEnquiryAcctDetailsEmail);

            enquiryAcctDetailsLMobileNumContainer = FindViewById<LinearLayout>(Resource.Id.enquiryAcctDetailsLMobileNumContainer);

            SetUpViews();
        }

        private void SetUpViews()
        {
            TextViewUtils.SetMuseoSans300Typeface(txtEnquiryAcctDetailsFullNameLayout, txtEnquiryAcctDetailsEmailLayout);
            TextViewUtils.SetMuseoSans300Typeface(txtEnquiryAcctDetailsFullName, txtEnquiryAcctDetailsEmail);

            TextViewUtils.SetTextSize16(txtEnquiryAcctDetailsFullName, txtEnquiryAcctDetailsEmail);

            txtEnquiryAcctDetailsFullNameLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
            txtEnquiryAcctDetailsEmailLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);

            txtEnquiryAcctDetailsFullNameLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
            txtEnquiryAcctDetailsEmailLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);

            txtEnquiryAcctDetailsFullName.TextChanged += FullNameTextChanged;
            txtEnquiryAcctDetailsFullName.AddTextChangedListener(new InputFilterFormField(txtEnquiryAcctDetailsFullName, txtEnquiryAcctDetailsFullNameLayout));

            txtEnquiryAcctDetailsEmail.TextChanged += EmailTextChanged;
            txtEnquiryAcctDetailsEmail.AddTextChangedListener(new InputFilterFormField(txtEnquiryAcctDetailsEmail, txtEnquiryAcctDetailsEmailLayout));

            Drawable fullNameIcon = ContextCompat.GetDrawable(this.mActivity, Resource.Drawable.placeholder_name);
            txtEnquiryAcctDetailsFullName.SetCompoundDrawablesWithIntrinsicBounds(fullNameIcon, null, null, null);

            Drawable emailIcon = ContextCompat.GetDrawable(this.mActivity, Resource.Drawable.placeholder_email);
            txtEnquiryAcctDetailsEmail.SetCompoundDrawablesWithIntrinsicBounds(emailIcon, null, null, null);

            txtEnquiryAcctDetailsFullNameLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.FULL_NAME_HINT);
            txtEnquiryAcctDetailsEmailLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.EMAIL_HINT);

            enquiryAcctDetailsLMobileNumContainer.RemoveAllViews();
            mobileNumberInputComponent = new MobileNumberInputComponent(this.mContext);
            mobileNumberInputComponent.SetOnTapCountryCodeAction(OnTapCountryCode);
            mobileNumberInputComponent.SetMobileNumberLabel(Utility.GetLocalizedCommonLabel(LanguageConstants.Common.MOBILE_NO));
            mobileNumberInputComponent.SetSelectedCountry(CountryUtil.Instance.GetDefaultCountry());
            mobileNumberInputComponent.SetValidationAction(OnValidateMobileNumber);
            enquiryAcctDetailsLMobileNumContainer.AddView(mobileNumberInputComponent);
        }

        public void SetFullNameField(string value)
        {
            txtEnquiryAcctDetailsFullName.Text = value;
        }

        public void SetEmailAddressField(string value)
        {
            txtEnquiryAcctDetailsEmail.Text = value;
        }

        public void SetMobileNumberField(string value)
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

        private void OnTapCountryCode()
        {
            this.OnTapCountryCodeAction?.Invoke();
        }

        private void OnValidateMobileNumber(bool isValidated)
        {
            this.ValidateFieldWithType(EnquiryAccountDetailType.MOBILE_NUMBER);
        }

        [Preserve]
        private void FullNameTextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            this.ValidateFieldWithType(EnquiryAccountDetailType.FULL_NAME);
        }

        [Preserve]
        private void EmailTextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            this.ValidateFieldWithType(EnquiryAccountDetailType.EMAIL_ADDRESS);
        }

        public void SetOnTapCountryCodeAction(Action action)
        {
            this.OnTapCountryCodeAction = action;
        }

        public void SetCheckRequiredFieldsAction(Action<bool> action)
        {
            this.CheckRequiredFieldsAction = action;
        }

        private void ValidateFieldWithType(EnquiryAccountDetailType type)
        {
            if (type == EnquiryAccountDetailType.FULL_NAME)
            {
                if (!txtEnquiryAcctDetailsFullName.Text.IsValid())
                {
                    ShowEmptyError(type);
                }
                else
                {
                    ClearErrors(type);
                }
            }
            else if (type == EnquiryAccountDetailType.EMAIL_ADDRESS)
            {
                if (!txtEnquiryAcctDetailsEmail.Text.IsValid())
                {
                    ShowEmptyError(type);
                }
                else
                {
                    ClearErrors(type);
                }
            }
            else if (type == EnquiryAccountDetailType.MOBILE_NUMBER)
            {
                if (mobileNumberInputComponent.IsTextClear())
                {
                    mobileNumberInputComponent.RaiseError(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.MOBILE_ERROR));
                }
                else
                {
                    mobileNumberInputComponent.ClearError();
                }
            }
            this.CheckRequiredFieldsAction?.Invoke(FieldsAreValid());
        }

        public void ValidateAllFields()
        {
            if (!txtEnquiryAcctDetailsFullName.Text.IsValid())
            {
                ShowEmptyError(EnquiryAccountDetailType.FULL_NAME);
            }
            else
            {
                ClearErrors(EnquiryAccountDetailType.FULL_NAME);
            }

            if (!txtEnquiryAcctDetailsEmail.Text.IsValid())
            {
                ShowEmptyError(EnquiryAccountDetailType.EMAIL_ADDRESS);
            }
            else
            {
                ClearErrors(EnquiryAccountDetailType.EMAIL_ADDRESS);
            }

            if (mobileNumberInputComponent.IsTextClear())
            {
                mobileNumberInputComponent.RaiseError(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.MOBILE_ERROR));
            }
            else
            {
                mobileNumberInputComponent.ClearError();
            }
            this.CheckRequiredFieldsAction?.Invoke(FieldsAreValid());
        }

        private void ShowEmptyError(EnquiryAccountDetailType type)
        {
            if (type == EnquiryAccountDetailType.FULL_NAME)
            {
                txtEnquiryAcctDetailsFullNameLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
                TextViewUtils.SetMuseoSans300Typeface(txtEnquiryAcctDetailsFullNameLayout.FindViewById<TextView>(Resource.Id.textinput_error));
                txtEnquiryAcctDetailsFullNameLayout.Error = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.FULL_NAME_ERROR);
            }
            else if (type == EnquiryAccountDetailType.EMAIL_ADDRESS)
            {
                txtEnquiryAcctDetailsEmailLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);
                TextViewUtils.SetMuseoSans300Typeface(txtEnquiryAcctDetailsEmailLayout.FindViewById<TextView>(Resource.Id.textinput_error));
                txtEnquiryAcctDetailsEmailLayout.Error = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.EMAIL_ERROR);
            }
        }

        private void ClearErrors(EnquiryAccountDetailType type)
        {
            if (type == EnquiryAccountDetailType.FULL_NAME)
            {
                txtEnquiryAcctDetailsFullNameLayout.Error = string.Empty;
            }
            else if (type == EnquiryAccountDetailType.EMAIL_ADDRESS)
            {
                txtEnquiryAcctDetailsEmailLayout.Error = string.Empty;
            }
        }

        public void SetSelectedCountry(Country country)
        {
            mobileNumberInputComponent.SetSelectedCountry(country);
        }

        public string GetFullNameValue()
        {
            return txtEnquiryAcctDetailsFullName.Text;
        }

        public string GetEmailValue()
        {
            return txtEnquiryAcctDetailsEmail.Text;
        }

        public string GetMobileNumValue()
        {
            return mobileNumberInputComponent.GetMobileNumberValueWithISDCode();
        }

        private bool IsMobileNumEmpty()
        {
            return mobileNumberInputComponent.IsTextClear();
        }

        public bool FieldsAreValid()
        {
            return txtEnquiryAcctDetailsFullName.Text.IsValid() && txtEnquiryAcctDetailsEmail.Text.IsValid() && !IsMobileNumEmpty();
        }
    }
}
