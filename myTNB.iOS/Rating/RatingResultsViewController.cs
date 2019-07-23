using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace myTNB
{
    public partial class RatingResultsViewController : UIViewController
    {
        public RatingResultsViewController(IntPtr handle) : base(handle)
        {
        }

        UIButton _btnDashBoard;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupSuperViewBackground();
            InitiliazeViews();
            SetEvents();
            this.NavigationController.NavigationBarHidden = true;
        }

        internal void SetupSuperViewBackground()
        {
            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            var gradientLayer = new CAGradientLayer();
            gradientLayer.Colors = new[] { startColor.CGColor, endColor.CGColor };
            gradientLayer.Locations = new NSNumber[] { 0, 1 };
            gradientLayer.Frame = View.Bounds;
            View.Layer.InsertSublayer(gradientLayer, 0);
        }

        internal void InitiliazeViews()
        {
            var locY = !DeviceHelper.IsIphoneXUpResolution() ? 36 : 56;
            var viewContainer = new UIView((new CGRect(18, locY, View.Frame.Width - 36, 140)));
            viewContainer.BackgroundColor = UIColor.White;
            viewContainer.Layer.CornerRadius = 4f;
            View.AddSubview(viewContainer);

            UIImageView imgViewCheck = new UIImageView(new CGRect((viewContainer.Frame.Width - 64) / 2, 16, 64, 64));
            imgViewCheck.Image = UIImage.FromBundle("Circle-With-Check-Green");
            viewContainer.AddSubview(imgViewCheck);

            var lblFeedback = new UILabel(new CGRect(0, imgViewCheck.Frame.GetMaxY() + 5, viewContainer.Frame.Width, 18));
            lblFeedback.Font = MyTNBFont.MuseoSans16;
            lblFeedback.TextColor = MyTNBColor.PowerBlue;
            lblFeedback.Text = "Rating_ThankYou".Translate();
            lblFeedback.TextAlignment = UITextAlignment.Center;
            viewContainer.AddSubview(lblFeedback);

            var lblDetail = new UILabel(new CGRect(0, lblFeedback.Frame.GetMaxY() + 1, viewContainer.Frame.Width, 16));
            lblDetail.Font = MyTNBFont.MuseoSans12;
            lblDetail.TextColor = MyTNBColor.TunaGrey();
            lblDetail.Text = "Rating_Description".Translate();
            lblDetail.TextAlignment = UITextAlignment.Center;
            viewContainer.AddSubview(lblDetail);

            //var lblSubDetail = new UILabel(new CGRect(0, 150, viewContainer.Frame.Width, 16));
            //lblSubDetail.Font = myTNBFont.MuseoSans12;
            //lblSubDetail.TextColor = myTNBColor.TunaGrey();
            //lblSubDetail.Text = "Please try again later.";
            //lblSubDetail.TextAlignment = UITextAlignment.Center;
            //viewContainer.AddSubview(lblSubDetail);

            //Back to Dashboard Button
            _btnDashBoard = new UIButton(UIButtonType.Custom);
            _btnDashBoard.Frame = new CGRect(18, View.Frame.Height - 64, View.Frame.Width - 36, 48);
            _btnDashBoard.SetTitle("Common_BackToDashboard".Translate(), UIControlState.Normal);
            _btnDashBoard.Font = MyTNBFont.MuseoSans16;
            _btnDashBoard.Layer.CornerRadius = 5.0f;
            _btnDashBoard.BackgroundColor = MyTNBColor.FreshGreen;
            View.AddSubview(_btnDashBoard);
        }

        internal void SetEvents()
        {
            _btnDashBoard.TouchUpInside += (sender, e) =>
            {
                DataManager.DataManager.SharedInstance.SummaryNeedsRefresh = true;
                ViewHelper.DismissControllersAndSelectTab(this, 0, true, true);
            };
        }
    }
}