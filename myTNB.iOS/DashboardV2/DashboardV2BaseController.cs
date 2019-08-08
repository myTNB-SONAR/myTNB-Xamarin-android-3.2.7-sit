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

        internal UILabel NavTitle;

        public override void ViewDidLoad()
        {
            IsGradientRequired = true;
            IsFullGradient = true;
            base.ViewDidLoad();
            if (TabBarController != null && TabBarController.TabBar != null)
            { TabBarController.TabBar.Layer.ZPosition = -1; }
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
            if (TabBarController != null && TabBarController.TabBar != null)
            { TabBarController.TabBar.Layer.ZPosition = 0; }
        }

        public override void ConfigureNavigationBar()
        {
            NavigationController.SetNavigationBarHidden(true, true);
            nfloat yLoc = GetScaledHeight(28);
            UIView viewBack = new UIView(new CGRect(GetScaledWidth(16), yLoc, GetScaledWidth(24), GetScaledHeight(24)));
            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            { NavigationController.PopViewController(true); }));

            UIImageView imgBack = new UIImageView(new CGRect(new CGPoint(0, 0), viewBack.Frame.Size))
            {
                Image = UIImage.FromBundle(Constants.IMG_Back)
            };
            viewBack.AddSubview(imgBack);

            NavTitle = new UILabel(new CGRect(viewBack.Frame.GetMaxX(), yLoc, ViewWidth - (viewBack.Frame.GetMaxX() * 2), GetScaledHeight(24)))
            {
                TextAlignment = UITextAlignment.Center,
                Font = MyTNBFont.MuseoSans16_500V2,
                TextColor = UIColor.White,
                Text = "Usage"
            };
            View.AddSubviews(new UIView[] { viewBack, NavTitle });
        }
    }
}
