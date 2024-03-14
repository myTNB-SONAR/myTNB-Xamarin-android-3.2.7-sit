using Android.Content;
using Android.Util;
using Android.Widget;

namespace myTNB.Android.Src.myTNBMenu.Listener
{
    public class SMDashboardScrollView : ScrollView
    {
        public SMDashboardScrollViewListener scrollViewListener = null;

        public SMDashboardScrollView(Context context) : base(context)
        {
        }

        public SMDashboardScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public SMDashboardScrollView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {

        }


        public interface SMDashboardScrollViewListener
        {
            void OnScrollChanged(SMDashboardScrollView v, int l, int t, int oldl, int oldt);
        }



        public void setOnScrollViewListener(SMDashboardScrollViewListener scrollViewListener)
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