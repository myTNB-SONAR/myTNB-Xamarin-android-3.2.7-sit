using System;
using CoreGraphics;
using UIKit;

namespace myTNB.DashboardV2
{
    public class DashboardV2BaseController : CustomUIViewController
    {
        public DashboardV2BaseController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            IsGradientRequired = true;
            IsFullGradient = true;
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController.SetNavigationBarHidden(true, animated);
        }

        public override void ConfigureNavigationBar()
        {
            NavigationController.SetNavigationBarHidden(true, true);
            nfloat yLoc = DeviceHelper.GetStatusBarHeight();
            if (NavigationController != null)
            {
                nfloat navHeight = NavigationController.NavigationBar.Frame.Height;
                yLoc += ((navHeight - 24) / 2);
            }
            else { yLoc += 8; }
            UIView viewBack = new UIView(new CGRect(16, yLoc, 24, 24));
            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            { NavigationController.PopViewController(true); }));

            UIImageView imgBack = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle(Constants.IMG_Back)
            };
            viewBack.AddSubview(imgBack);
            View.AddSubview(viewBack);
        }
    }
}
