using System;
using System.Diagnostics;
using CoreGraphics;
using Firebase.Analytics;
using UIKit;

namespace myTNB
{
    public class CustomUIView : UIView
    {
        public string PageName { set; private get; } = string.Empty;
        public string EventName { set; private get; } = string.Empty;
        private string EventFormat = "{0}-{1}";

        public CustomUIView() { }

        public CustomUIView(CGRect frame)
        {
            Frame = frame;
        }

        public override void AddGestureRecognizer(UIGestureRecognizer gestureRecognizer)
        {
            gestureRecognizer.AddTarget(new Action(() =>
            {
                //Handle Firebase Log Event
                Debug.WriteLine("Tapped");
                string pName = PageName.IsValid() ? PageName : "NoPage";
                Analytics.LogEvent(string.Format(EventFormat, pName, EventName), null);
            }));
            base.AddGestureRecognizer(gestureRecognizer);
        }
    }
}