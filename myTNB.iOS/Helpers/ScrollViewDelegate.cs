using System.Diagnostics;
using UIKit;

namespace myTNB
{
    public class ScrollViewDelegate : UIScrollViewDelegate
    {
        public ScrollViewDelegate(UIScrollView scrollView)
        {
        }
        /// <summary>
        /// On drag started
        /// </summary>
        /// <param name="scrollView">Scroll view.</param>
        public override void DraggingStarted(UIScrollView scrollView)
        {
            Debug.WriteLine("DraggingStarted");
        }
    }
}