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
                Dictionary<string, string> mDic = new Dictionary<string, string>
                {
                    { pName, "Tapped" }
                };
                List<NSString> keys = new List<NSString>();
                List<NSString> values = new List<NSString>();
                foreach (var item in mDic)
                {
                    keys.Add(new NSString(item.Key));
                    values.Add(new NSString(item.Value));
                }
                Analytics.LogEvent(string.Format(EventFormat, pName, EventName), NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values.ToArray(), keys.ToArray(), keys.Count));
            }));
            base.AddGestureRecognizer(gestureRecognizer);
        }
    }
}