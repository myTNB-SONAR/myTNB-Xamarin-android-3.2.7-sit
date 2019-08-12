using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using Firebase.Analytics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class CustomUIView : UIView
    {
        private string PageName = string.Empty;
        private string EventName = string.Empty;

        public CustomUIView() { }

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
            gestureRecognizer.AddTarget(new Action(() =>
            {
                //Handle Firebase Log Event
                Debug.WriteLine("Tapped");
            }));
            base.AddGestureRecognizer(gestureRecognizer);
        }
    }
}