using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.ViewPager.Widget;

namespace myTNB_Android.Src.DigitalBillRendering.ManageBillDelivery.Activity.MVP
{
    public class CustomViewPager : ViewPager
    {
        public CustomViewPager(Context context) : base(context)
        {
        }

        public CustomViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int height = 0;
            for (int i = 0; i < ChildCount; i++)
            {
                View? child = GetChildAt(i);
                child.Measure(widthMeasureSpec, MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
                int h = child.MeasuredHeight;
                if (h > height)
                {
                    height = h;
                }
            }
            heightMeasureSpec = MeasureSpec.MakeMeasureSpec(height, MeasureSpecMode.Exactly);
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }
    }
}