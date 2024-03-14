using Android.Animation;
using Android.Content;

using Android.Util;
using Android.Views;
using AndroidX.Core.Widget;

namespace myTNB.Android.Src.myTNBMenu.Listener
{
    public class NMRESMDashboardScrollView : NestedScrollView
    {
        public NMRESMDashboardScrollViewListener scrollViewListener = null;

        private bool scrollable = true;

        private static int MAX_Y_OVERSCROLL_DISTANCE = 200;

        private Context mContext;

        private int mMaxYOverscrollDistance;

        public NMRESMDashboardScrollView(Context context) : base(context)
        {
            mContext = context;
            init();
        }

        public NMRESMDashboardScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mContext = context;
            init();
        }

        public NMRESMDashboardScrollView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            mContext = context;
            init();
        }


        public interface NMRESMDashboardScrollViewListener
        {
            void OnScrollChanged(NMRESMDashboardScrollView v, int l, int t, int oldl, int oldt);
        }



        public void setOnScrollViewListener(NMRESMDashboardScrollViewListener scrollViewListener)
        {
            this.scrollViewListener = scrollViewListener;
        }

        private void init()
        {
            DisplayMetrics metrics = mContext.Resources.DisplayMetrics;
            float density = metrics.Density;

            mMaxYOverscrollDistance = (int)(density * MAX_Y_OVERSCROLL_DISTANCE);
        }


        protected override void OnScrollChanged(int l, int t, int oldl, int oldt)
        {

            base.OnScrollChanged(l, t, oldl, oldt);
            if (scrollViewListener != null)
            {
                scrollViewListener.OnScrollChanged(this, l, t, oldl, oldt);
            }
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            return scrollable && base.OnTouchEvent(ev);
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return scrollable && base.OnInterceptTouchEvent(ev);
        }

        public void SetScrollingEnabled(bool enabled)
        {
            scrollable = enabled;
        }

        protected override bool OverScrollBy(int deltaX, int deltaY, int scrollX, int scrollY, int scrollRangeX, int scrollRangeY, int maxOverScrollX, int maxOverScrollY, bool isTouchEvent)
        {
            return base.OverScrollBy(deltaX, deltaY, scrollX, scrollY, scrollRangeX, scrollRangeY, maxOverScrollX, mMaxYOverscrollDistance, isTouchEvent);
        }

    }
}