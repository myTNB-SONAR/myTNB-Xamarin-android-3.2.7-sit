using System;
using Android.Content;
using Android.Util;
using Android.Widget;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.CompoundView
{
    public class MobileNumberInputComponent : LinearLayout
    {
        private Action onTapCountryCodeAction;
        private ImageView imgFlag;
        private TextView countryISDCode;
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
            imgFlag = FindViewById<ImageView>(Resource.Id.countryIcon);
            countryISDCode = FindViewById<TextView>(Resource.Id.coutryCodeValue);
        }

        public void SetOnTapCountryCodeAction(Action onTapAction)
        {
            LinearLayout countryCodeContainer = FindViewById<LinearLayout>(Resource.Id.countryCodeContainer);
            countryCodeContainer.Click += delegate
            {
                onTapAction?.Invoke();
            };
        }

        public void SetSelectedCountry(Country country)
        {
            imgFlag.SetImageResource(CountryUtil.Instance.GetFlagImageResource(Context, country.code));
            countryISDCode.Text = country.isd;
        }
    }
}
