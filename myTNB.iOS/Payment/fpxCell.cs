using System;
using UIKit;
using CoreGraphics;

namespace myTNB
{
    public partial class fpxCell : UITableViewCell
    {
        public fpxCell (IntPtr handle) : base (handle)
        {
            var imgView = new UIImageView(new CGRect(35, 16, 24, 24));
            imgView.Image = UIImage.FromBundle("IC-Payment-FPX");
            AddSubview(imgView);
            var backgroundView = new UIView();
        }
    }
}