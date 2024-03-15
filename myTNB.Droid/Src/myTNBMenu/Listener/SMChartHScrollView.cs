using Android.Content;
using Android.Util;
using Android.Widget;

namespace myTNB.AndroidApp.Src.myTNBMenu.Listener
{
    public class SMChartHScrollView : HorizontalScrollView
    {

        public SMChartHScrollViewListener hScrollViewListener = null;

        public SMChartHScrollView(Context context) : base(context)
        {

        }

        public SMChartHScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        public SMChartHScrollView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {

        }


        public interface SMChartHScrollViewListener
        {
            void OnHScrollChanged(SMChartHScrollView v, int l, int t, int oldl, int oldt);
        }

        public void setOnHScrollViewListener(SMChartHScrollViewListener scrollViewListener)
        {
            this.hScrollViewListener = scrollViewListener;
        }

        protected override void OnScrollChanged(int l, int t, int oldl, int oldt)
        {
            base.OnScrollChanged(l, t, oldl, oldt);
            if (hScrollViewListener != null)
            {
                hScrollViewListener.OnHScrollChanged(this, l, t, oldl, oldt);
            }
        }
    }
}