using System;
using Android.Content;
using Android.Util;
using Android.Widget;

namespace myTNB_Android.Src.CompoundView
{
    public class MobileNumberInputComponent : LinearLayout
    {
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

        public void Init(Context context)
        {
            Inflate(context, Resource.Layout.MobileNumberInputComponentLayout, this);
        }
    }
}
