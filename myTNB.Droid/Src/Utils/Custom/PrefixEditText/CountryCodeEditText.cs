using System;
using Android.Content;
using Android.Graphics;
using Android.Util;

namespace myTNB_Android.Src.Utils.Custom.PrefixEditText
{
    public class CountryCodeEditText : Android.Widget.EditText
    {
        float mOriginalLeftPadding = -1;

        public CountryCodeEditText(Context context) : base(context)
        {
            
        }

        public CountryCodeEditText(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            
        }

        public CountryCodeEditText(Context context, IAttributeSet attrs,
                                   int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
         
        }




        protected override void OnMeasure(int widthMeasureSpec,
                                int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            calculatePrefix();
        }

        private void calculatePrefix()
        {
            if (mOriginalLeftPadding == -1)
            {
                String prefix = (String)Tag;
                float[] widths = new float[prefix.Length];
                Paint.GetTextWidths(prefix, widths);
                float textWidth = 0;
                foreach (float w in widths)
                {
                    textWidth += w;
                }
                mOriginalLeftPadding = CompoundPaddingLeft;
                SetPadding((int)(textWidth + mOriginalLeftPadding),
                           PaddingRight, PaddingTop, PaddingBottom);
            }
        }


        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            String prefix = (String)Tag;
            canvas.DrawText(prefix, mOriginalLeftPadding, GetLineBounds(0, null), Paint);
        }
    }
}
