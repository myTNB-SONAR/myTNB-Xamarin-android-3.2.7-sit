using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class SSMROnboardingController : BasePageViewRootController
    {
        public SSMROnboardingController(UIViewController controller) : base(controller) { }
        public override void OnViewDidLoad()
        {
            nfloat yLoc = 28.0F;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                yLoc = 50.0F;
            }
            UIView viewBack = new UIView(new CGRect(16, yLoc, 24, 24));
            viewBack.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                that.DismissViewController(true, null);
            }));

            UIImageView imgBack = new UIImageView(new CGRect(0, 0, 24, 24))
            {
                Image = UIImage.FromBundle("Back-White")
            };
            viewBack.AddSubview(imgBack);

            UIView viewDontShow = new UIView(new CGRect(16,0, that.View.Frame.Width/2, 16));

            UIView viewSkip = new UIView(new CGRect());

            UIButton btnStart = new UIButton(new CGRect());

            that.View.AddSubviews(new UIView[] { viewBack, viewDontShow, viewSkip, btnStart });
        }

        public override void OnViewDidLayoutSubViews()
        {
            SetupSuperViewBackground();
        }

        private void SetupSuperViewBackground()
        {
            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            var gradientLayer = new CAGradientLayer();
            gradientLayer.Colors = new[] { startColor.CGColor, endColor.CGColor };
            gradientLayer.Locations = new NSNumber[] { 0, 1 };
            gradientLayer.Frame = that.View.Bounds;
            that.View.Layer.InsertSublayer(gradientLayer, 0);
        }
    }
}
