using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class CustomUITableViewCell : UITableViewCell
    {
        public Func<string, string> GetI18NValue;
        public nfloat _cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        public nfloat _cellHeight = UIApplication.SharedApplication.KeyWindow.Frame.Height;

        public CustomUITableViewCell(IntPtr handle) : base(handle)
        { BackgroundColor = UIColor.Clear; }

        public nfloat BaseMarginWidth16
        {
            get
            {
                return ScaleUtility.BaseMarginWidth16;
            }
        }

        public nfloat GetScaledHeight(nfloat value)
        {
            return ScaleUtility.GetScaledHeight(value);
        }

        public nfloat GetScaledWidth(nfloat value)
        {
            return ScaleUtility.GetScaledWidth(value);
        }

        public nfloat GetYLocationFromFrame(CGRect frame, nfloat value)
        {
            return ScaleUtility.GetYLocationFromFrame(frame, value);
        }
    }
}
