using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class CustomUIView : UIView
    {
        private string PageName = string.Empty;
        private string EventName = string.Empty;

        public CustomUIView(CGRect frame)
        {
            Frame = frame;
        }

        public CustomUIView(CGRect frame, string pageName, string eventName)
        {
            Frame = frame;
            PageName = pageName;
            EventName = eventName;
        }

        public override void AddGestureRecognizer(UIGestureRecognizer gestureRecognizer)
        {
            base.AddGestureRecognizer(gestureRecognizer);
            //Todo: Add Firebase Call
        }
    }
}