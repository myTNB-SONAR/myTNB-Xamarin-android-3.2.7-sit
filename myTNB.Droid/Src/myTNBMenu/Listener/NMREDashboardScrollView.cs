using Android.Content;
using Android.Support.V4.Widget;
using Android.Util;

namespace myTNB_Android.Src.myTNBMenu.Listener
{
    public class NMREDashboardScrollView : NestedScrollView
    {
        public NMREDashboardScrollViewListener scrollViewListener = null;

        public NMREDashboardScrollView(Context context) : base(context)
        {
        }

        public NMREDashboardScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public NMREDashboardScrollView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {

        }


        public interface NMREDashboardScrollViewListener
        {
            void OnScrollChanged(NMREDashboardScrollView v, int l, int t, int oldl, int oldt);
        }



        public void setOnScrollViewListener(NMREDashboardScrollViewListener scrollViewListener)
        {
            this.scrollViewListener = scrollViewListener;
        }


        protected override void OnScrollChanged(int l, int t, int oldl, int oldt)
        {

            base.OnScrollChanged(l, t, oldl, oldt);
            if (scrollViewListener != null)
            {
                scrollViewListener.OnScrollChanged(this, l, t, oldl, oldt);
            }
        }

    }
}