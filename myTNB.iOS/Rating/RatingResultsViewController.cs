using CoreAnimation;
using CoreGraphics;
using Foundation;
using myTNB.Rating;
using System;
using UIKit;

namespace myTNB
{
    public partial class RatingResultsViewController : CustomUIViewController
    {
        public RatingResultsViewController(IntPtr handle) : base(handle) { }
        private UIButton _btnDashBoard;

        public override void ViewDidLoad()
        {
            PageName = RatingConstants.Pagename_RatingResults;
            base.ViewDidLoad();
            SetupSuperViewBackground();
            InitiliazeViews();
            SetEvents();
            NavigationController.NavigationBarHidden = true;
        }

        private void SetupSuperViewBackground()
        {
            UIColor startColor = MyTNBColor.GradientPurpleDarkElement;
            UIColor endColor = MyTNBColor.GradientPurpleLightElement;
            CAGradientLayer gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor },
                Locations = new NSNumber[] { 0, 1 },
                Frame = View.Bounds
            };
            View.Layer.InsertSublayer(gradientLayer, 0);
        }

        private void InitiliazeViews()
        {
            int locY = !DeviceHelper.IsIphoneXUpResolution() ? 36 : 56;
            UIView viewContainer = new UIView((new CGRect(18, locY, View.Frame.Width - 36, 140)))
            {
                BackgroundColor = UIColor.White
            };
            viewContainer.Layer.CornerRadius = 4f;
            View.AddSubview(viewContainer);

            UIImageView imgViewCheck = new UIImageView(new CGRect((viewContainer.Frame.Width - 64) / 2, 16, 64, 64))
            {
                Image = UIImage.FromBundle("Circle-With-Check-Green")
            };
            viewContainer.AddSubview(imgViewCheck);

            UILabel lblFeedback = new UILabel(new CGRect(16, imgViewCheck.Frame.GetMaxY() + 5, viewContainer.Frame.Width - 32, 18))
            {
                Font = MyTNBFont.MuseoSans16,
                TextColor = MyTNBColor.PowerBlue,
                Text = GetI18NValue(RatingConstants.I18N_Thankyou),
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            nfloat newThankyouHeight = lblFeedback.GetLabelHeight(GetScaledHeight(60F));
            lblFeedback.Frame = new CGRect(lblFeedback.Frame.Location, new CGSize(lblFeedback.Frame.Width, newThankyouHeight));

            viewContainer.AddSubview(lblFeedback);

            UILabel lblDetail = new UILabel(new CGRect(16, lblFeedback.Frame.GetMaxY() + 1, viewContainer.Frame.Width - 32, 16))
            {
                Font = MyTNBFont.MuseoSans12,
                TextColor = MyTNBColor.TunaGrey(),
                Text = GetI18NValue(RatingConstants.I18N_Description),
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            nfloat newTitleHeight = lblDetail.GetLabelHeight(GetScaledHeight(60F));
            lblDetail.Frame = new CGRect(lblDetail.Frame.Location, new CGSize(lblDetail.Frame.Width, newTitleHeight));
            viewContainer.AddSubview(lblDetail);

            viewContainer.Frame = new CGRect(viewContainer.Frame.Location, new CGSize(viewContainer.Frame.Width, lblDetail.Frame.GetMaxY() + 16));

            //Back to Dashboard Button
            _btnDashBoard = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - 64, View.Frame.Width - 36, 48),
                Font = MyTNBFont.MuseoSans16,
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _btnDashBoard.SetTitle(GetI18NValue(RatingConstants.I18N_BackToHome), UIControlState.Normal);
            _btnDashBoard.Layer.CornerRadius = 5.0f;
            View.AddSubview(_btnDashBoard);
        }

        private void SetEvents()
        {
            _btnDashBoard.TouchUpInside += (sender, e) =>
            {
                DataManager.DataManager.SharedInstance.SummaryNeedsRefresh = true;
                ViewHelper.DismissControllersAndSelectTab(this, 0, true, true);
            };
        }
    }
}