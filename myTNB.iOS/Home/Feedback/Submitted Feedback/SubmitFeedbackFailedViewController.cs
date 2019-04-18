using Foundation;
using System;
using UIKit;
using CoreAnimation;
using CoreGraphics;


namespace myTNB
{
    public partial class SubmitFeedbackFailedViewController : UIViewController
    {
        public SubmitFeedbackFailedViewController(IntPtr handle) : base(handle)
        {
        }

        UIButton _btnDashBoard;
        UIButton _btnTryAgain;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupSuperViewBackground();
            InitilizedViews();
            SetEvents();
            this.NavigationController.NavigationBarHidden = true;
        }

        internal void SetupSuperViewBackground()
        {
            var startColor = myTNBColor.GradientPurpleDarkElement();
            var endColor = myTNBColor.GradientPurpleLightElement();
            CAGradientLayer gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor },
                Locations = new NSNumber[] { 0, 1 },
                Frame = View.Bounds
            };
            View.Layer.InsertSublayer(gradientLayer, 0);
        }

        internal void InitilizedViews()
        {
            UIView viewContainer = new UIView((new CGRect(18, 36, View.Frame.Width - 36, 203)))
            {
                BackgroundColor = UIColor.White
            };
            viewContainer.Layer.CornerRadius = 4f;
            View.AddSubview(viewContainer);

            UIImageView imgViewCheck = new UIImageView(new CGRect((viewContainer.Frame.Width - 64) / 2, 16, 64, 64))
            {
                Image = UIImage.FromBundle("Red-Cross")
            };
            viewContainer.AddSubview(imgViewCheck);

            UILabel lblFeedback = new UILabel(new CGRect(0, 80, viewContainer.Frame.Width, 18))
            {
                Font = myTNBFont.MuseoSans16(),
                TextColor = myTNBColor.PowerBlue(),
                Text = "Feedback_Unsuccessful".Translate(),
                TextAlignment = UITextAlignment.Center
            };
            viewContainer.AddSubview(lblFeedback);

            UIView viewLine = new UIView((new CGRect(14, 114, viewContainer.Frame.Width - 28, 1)))
            {
                BackgroundColor = myTNBColor.LightGrayBG()
            };
            viewContainer.AddSubview(viewLine);

            UILabel lblDetail = new UILabel(new CGRect(0, 131, viewContainer.Frame.Width, 16))
            {
                Font = myTNBFont.MuseoSans12(),
                TextColor = myTNBColor.TunaGrey(),
                Text = "Feedback_UnsuccessfulMessage".Translate(),
                TextAlignment = UITextAlignment.Center
            };
            viewContainer.AddSubview(lblDetail);

            UILabel lblSubDetail = new UILabel(new CGRect(0, 150, viewContainer.Frame.Width, 16))
            {
                Font = myTNBFont.MuseoSans12(),
                TextColor = myTNBColor.TunaGrey(),
                Text = "Error_DefaultMessage".Translate(),
                TextAlignment = UITextAlignment.Center
            };
            viewContainer.AddSubview(lblSubDetail);

            //Back to Dashboard Button
            _btnDashBoard = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - 118, View.Frame.Width - 36, 48)
            };
            _btnDashBoard.SetTitle("Common_BackToDashboard".Translate(), UIControlState.Normal);
            _btnDashBoard.Layer.CornerRadius = 5.0f;
            _btnDashBoard.Layer.BorderWidth = 1.0f;
            _btnDashBoard.Layer.BorderColor = UIColor.White.CGColor;
            _btnDashBoard.BackgroundColor = UIColor.Clear;
            View.AddSubview(_btnDashBoard);

            //Try Again Button
            _btnTryAgain = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - 64, View.Frame.Width - 36, 48)
            };
            _btnTryAgain.SetTitle("Common_TryAgain".Translate(), UIControlState.Normal);
            _btnTryAgain.Font = myTNBFont.MuseoSans16();
            _btnTryAgain.Layer.CornerRadius = 5.0f;
            _btnTryAgain.BackgroundColor = myTNBColor.FreshGreen();
            View.AddSubview(_btnTryAgain);

        }

        internal void SetEvents()
        {
            _btnDashBoard.TouchUpInside += (sender, e) =>
            {
                DismissViewController(true, null);
            };

            _btnTryAgain.TouchUpInside += (sender, e) =>
            {
                this.NavigationController.NavigationBarHidden = false;
                NavigationController.PopViewController(false);
            };
        }
    }
}