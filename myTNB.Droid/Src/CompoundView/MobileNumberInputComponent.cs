using System;
using Android.Content;
using Android.Text;
using Android.Util;
using Android.Widget;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.CompoundView
{
    public class MobileNumberInputComponent : LinearLayout
    {
        private ImageView imgFlag;
        private TextView countryCodeHeaderTitle, countryISDCode;
        private EditText editTextMobileNumber;
        private Action<bool> validateAction;
        private Country selectedCountry;
        public static bool isSelectionTapped;
        private Action onSelectCountryISDCodeAction;
        private const int MAX_NUMBER_INPUT = 15;
        private const string PLUS_CHARACTER = "+";
        public MobileNumberInputComponent(Context context) : base(context)
        {
            Init(context);
        }

        public MobileNumberInputComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(context);
        }

        public MobileNumberInputComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init(context);
        }

        public MobileNumberInputComponent(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init(context);
        }

        private void Init(Context context)
        {
            Inflate(context, Resource.Layout.MobileNumberInputComponentLayout, this);
            countryCodeHeaderTitle = FindViewById<TextView>(Resource.Id.countryCodeHeaderTitle);
            imgFlag = FindViewById<ImageView>(Resource.Id.countryIcon);
            countryISDCode = FindViewById<TextView>(Resource.Id.coutryCodeValue);
            editTextMobileNumber = FindViewById<EditText>(Resource.Id.txtMobileNo);
            isSelectionTapped = false;

            LinearLayout countryISDCodeContainer = FindViewById<LinearLayout>(Resource.Id.countryISDCodeContainer);
            countryISDCodeContainer.Click += CountryISDCodeContainer_Click;

            TextViewUtils.SetMuseoSans300Typeface(countryCodeHeaderTitle,countryISDCode,editTextMobileNumber);
            editTextMobileNumber.TextChanged += delegate {
                OnMobileNumberChangeValue();
            };
        }

        private void OnMobileNumberChangeValue()
        {
            if (editTextMobileNumber.Text.StartsWith("0", StringComparison.Ordinal))
            {
                string validatedMobileNumber = editTextMobileNumber.Text.Substring(1);
                editTextMobileNumber.Text = validatedMobileNumber;
            }

            validateAction?.Invoke(!string.IsNullOrEmpty(editTextMobileNumber.Text));
        }

        public void SetOnTapCountryCodeAction(Action onTapAction)
        {
            onSelectCountryISDCodeAction = onTapAction;
        }

        private void CountryISDCodeContainer_Click(object sender, EventArgs e)
        {
            if (!isSelectionTapped)
            {
                isSelectionTapped = true;
                onSelectCountryISDCodeAction?.Invoke();
            }
        }

        public void SetValidationAction(Action<bool> action)
        {
            validateAction = action;
        }

        public void SetMobileNumberLabel(string label)
        {
            countryCodeHeaderTitle.Text = label.ToUpper();
        }

        public void SetSelectedCountry(Country country)
        {
            imgFlag.SetImageResource(CountryUtil.Instance.GetFlagImageResource(Context, country.code));
            countryISDCode.Text = country.isd;
            selectedCountry = country;
            ClearMobileNumber();
            SetMaxNumberInputValue();
        }

        public string GetMobileNumberValueWithISDCode()
        {
            return selectedCountry.isd + editTextMobileNumber.Text.Trim();
        }

        public string GetMobileNumberValue()
        {
            return editTextMobileNumber.Text.Trim();
        }

        public void ClearMobileNumber()
        {
            editTextMobileNumber.Text = "";
        }

        private void SetMaxNumberInputValue()
        {
            try
            {
                int plusPosition = selectedCountry.isd.IndexOf(PLUS_CHARACTER, StringComparison.Ordinal);
                string removedPlus = selectedCountry.isd.Remove(plusPosition,1);
                int count = removedPlus.Length;
                int maxNumberInputValue = MAX_NUMBER_INPUT - count;
                editTextMobileNumber.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(maxNumberInputValue) });
            }
            catch (Exception e)
            {
                editTextMobileNumber.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(MAX_NUMBER_INPUT) });
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
