using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V4.View;
using Android.Text;
using Android.Util;
using Android.Widget;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.Utils;
using static Android.Resource;

namespace myTNB_Android.Src.CompoundView
{
    public class MobileNumberInputComponent : LinearLayout
    {
        private ImageView imgFlag;
        private TextView countryCodeHeaderTitle, countryISDCode, countryCodeError;
        private EditText editTextMobileNumber;
        //private TextInputLayout TextInputLayoutMobileNo;
        private Action<bool> validateAction;
        private Country selectedCountry;
        public static bool isSelectionTapped;
        private Action onSelectCountryISDCodeAction;
        private bool isFromCountryCode = false;
        private bool isEmptyNoAfterDelete = false;

        private const int MAX_NUMBER_INPUT = 15;
        private const string PLUS_CHARACTER = "+";


        /// <summary>
        /// Make sure SetValidationAction called last before added to view
        /// 
        /// </summary>
        /// <param name="context"></param>
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
            countryCodeError = FindViewById<TextView>(Resource.Id.countryCodeError);
            editTextMobileNumber = FindViewById<EditText>(Resource.Id.txtMobileNo);
            //  TextInputLayoutMobileNo = FindViewById<TextInputLayout>(Resource.Id.TextInputLayoutMobileNo);
            isSelectionTapped = false;

            LinearLayout countryISDCodeContainer = FindViewById<LinearLayout>(Resource.Id.countryISDCodeContainer);
            countryISDCodeContainer.Click += CountryISDCodeContainer_Click;

            TextViewUtils.SetMuseoSans300Typeface(countryCodeHeaderTitle, countryISDCode, editTextMobileNumber, countryCodeError);
            editTextMobileNumber.TextChanged += OnMobileNumberChangeValue;

            editTextMobileNumber.FocusChange += EditTextMobileNumber_FocusChange;
            TextViewUtils.SetTextSize10(countryCodeHeaderTitle);
            TextViewUtils.SetTextSize12(countryCodeError);
            TextViewUtils.SetTextSize16(countryISDCode, editTextMobileNumber);
        }

        private void EditTextMobileNumber_FocusChange(object sender, FocusChangeEventArgs e)
        {

            if (editTextMobileNumber.IsFocused)
            {
                ViewCompat.SetBackgroundTintList(editTextMobileNumber, ColorStateList.ValueOf(Android.Graphics.Color.ParseColor("#1c79ca")));  //blue color
                isEmptyNoAfterDelete = true; //when user delete or add key detect
            }
            else
            {
                ViewCompat.SetBackgroundTintList(editTextMobileNumber, ColorStateList.ValueOf(Android.Graphics.Color.ParseColor("#a6a6a6")));  //silver chalice
            }
        }

        private void OnMobileNumberChangeValue(object sender, TextChangedEventArgs e)
        {
            if (editTextMobileNumber.Text.StartsWith("0", StringComparison.Ordinal))
            {
                string validatedMobileNumber = editTextMobileNumber.Text.Substring(1);
                editTextMobileNumber.Text = validatedMobileNumber;
            }

            isFromCountryCode = false;  // any changes will set from country code false

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
            isFromCountryCode = true;
            ClearError();
        }

        public bool GetIsFromCountryCode()
        {
            bool holdIsFromCountryCode = isFromCountryCode;
            isFromCountryCode = false;
            return holdIsFromCountryCode;
        }

        public bool IsTextClear()
        {
            return string.IsNullOrEmpty(editTextMobileNumber.Text);
        }

        public string GetMobileNumberValueWithISDCode()
        {
            return selectedCountry.isd + editTextMobileNumber.Text.Trim();
        }

        public bool GetMobileNumberReset()
        {
            return isEmptyNoAfterDelete;
        }

        public string GetMobileNumberValue()
        {
            return editTextMobileNumber.Text.Trim();
        }

        public void SetMobileNumber(int mobileNo)
        {
            editTextMobileNumber.Text = mobileNo.ToString();
        }

        public string GetISDOnly()
        {
            return selectedCountry.isd;
        }

        public void ClearMobileNumber()
        {

            editTextMobileNumber.Text = "";

        }
        public void RaiseError(string errText)
        {
            countryCodeError.Visibility = Android.Views.ViewStates.Visible;
            countryCodeHeaderTitle.SetTextColor(Android.Graphics.Color.ParseColor("#e44b21"));//red color
            ViewCompat.SetBackgroundTintList(editTextMobileNumber, ColorStateList.ValueOf(Android.Graphics.Color.ParseColor("#e44b21"))); //red color
            countryCodeError.Text = errText;
        }
        public void NewRaiseError()
        {
            //countryCodeError.Visibility = Android.Views.ViewStates.Visible;
            //countryCodeHeaderTitle.SetTextColor(Android.Graphics.Color.ParseColor("#e44b21"));//red color
            ViewCompat.SetBackgroundTintList(editTextMobileNumber, ColorStateList.ValueOf(Android.Graphics.Color.ParseColor("#e44b21"))); //red color
        }
        public void ClearError()
        {
            if (editTextMobileNumber.IsFocused)
            {
                ViewCompat.SetBackgroundTintList(editTextMobileNumber, ColorStateList.ValueOf(Android.Graphics.Color.ParseColor("#1c79ca"))); //blue focus
            }
            else
            {
                ViewCompat.SetBackgroundTintList(editTextMobileNumber, ColorStateList.ValueOf(Android.Graphics.Color.ParseColor("#a6a6a6"))); //silver chalice
            }

            countryCodeHeaderTitle.SetTextColor(Android.Graphics.Color.ParseColor("#a6a6a6"));//title to grey color
            countryCodeError.Visibility = Android.Views.ViewStates.Gone;
            countryCodeError.Text = "";
        }

        private void SetMaxNumberInputValue()
        {
            try
            {
                int plusPosition = selectedCountry.isd.IndexOf(PLUS_CHARACTER, StringComparison.Ordinal);
                string removedPlus = selectedCountry.isd.Remove(plusPosition, 1);
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
