using System;
using System.Collections.Generic;
using System.Diagnostics;
using Firebase.Analytics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class CustomUIButtonV2 : UIButton
    {
        public string PageName { set; private get; }
        public string EventName { set; private get; }
        private string EventFormat = "{0}_{1}";

        public CustomUIButtonV2(bool isWhiteBG = false)
        {
            SetDefaultUIButton(isWhiteBG);
        }

        private void SetDefaultUIButton(bool isWhiteBG = false)
        {
            Layer.CornerRadius = 5.0F;
            Layer.BorderColor = UIColor.White.CGColor;
            Layer.BorderWidth = ScaleUtility.GetScaledWidth(1);
#pragma warning disable XI0003 // Notifies you when using a deprecated, obsolete or unavailable Apple API
            Font = TNBFont.MuseoSans_16_500;
#pragma warning restore XI0003 // Notifies you when using a deprecated, obsolete or unavailable Apple API
            if (isWhiteBG)
            {
                BackgroundColor = UIColor.White;
                SetTitleColor(MyTNBColor.WaterBlue, UIControlState.Normal);
            }
        }

        public override void AddGestureRecognizer(UIGestureRecognizer gestureRecognizer)
        {
            gestureRecognizer.AddTarget(new Action(() =>
            {
                //Handle Firebase Log Event
                Debug.WriteLine("Tapped");
                string pName = PageName.IsValid() ? PageName : "NoPage";
                Dictionary<string, string> mDic =  new Dictionary<string, string>
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