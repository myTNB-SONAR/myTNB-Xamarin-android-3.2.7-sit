using Foundation;
using System;
using UIKit;
using CoreAnimation;
using CoreGraphics;


namespace myTNB
{
    public partial class GetAccessSuccessViewController : UIViewController
    {
        public GetAccessSuccessViewController(IntPtr handle) : base(handle)
        {
        }

        UIButton btnBackToDashboard;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetupSuperViewBackground();
            InitializedSubviews();
            SetEvents();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.NavigationBar.Hidden = true;
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

        internal void InitializedSubviews()
        {
            var viewContainer = new UIView((new CGRect(18, 36, View.Frame.Width - 36, 279)));
            viewContainer.Layer.CornerRadius = 4.0f;
            viewContainer.BackgroundColor = UIColor.White;

            UIImageView imgViewClose = new UIImageView(UIImage.FromBundle("Delete"))
            {
                Frame = new CGRect(View.Frame.Width - 59, 52, 25, 25),
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            UIImageView imgViewSuccess = new UIImageView(UIImage.FromBundle("Circle-With-Check-Green"))
            {
                Frame = new CGRect(View.Frame.Width / 2 - 25, 83, 50, 50),
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            UILabel lblAccessRequested = new UILabel
            {
                Frame = new CGRect(18, 143, View.Frame.Width - 36, 18),
                AttributedText = new NSAttributedString(
                             "GetAccess_RequestedToOwnerMessage".Translate(),
                    font: MyTNBFont.MuseoSans16,
                             foregroundColor: MyTNBColor.PowerBlue,
                             strokeWidth: 0
                            ),
                TextAlignment = UITextAlignment.Center,
            };

            UIView viewLine = new UIView((new CGRect(32, 176, View.Frame.Width - 64, 1)));
            viewLine.BackgroundColor = MyTNBColor.PlatinumGrey;

            UILabel lblDescription = new UILabel
            {
                Frame = new CGRect(42, 193, View.Frame.Width - 84, 18),
                AttributedText = new NSAttributedString(
                    DataManager.DataManager.SharedInstance.SelectedAccount.accDesc,
                    font: MyTNBFont.MuseoSans14,
                 foregroundColor: MyTNBColor.TunaGrey(),
                 strokeWidth: 0
                ),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextAlignment = UITextAlignment.Left,
            };

            UILabel lblAccountNo = new UILabel
            {
                Frame = new CGRect(42, 212, View.Frame.Width - 84, 16),
                AttributedText = new NSAttributedString(
                    DataManager.DataManager.SharedInstance.SelectedAccount.accNum,
                    font: MyTNBFont.MuseoSans12,
                 foregroundColor: MyTNBColor.TunaGrey(),
                 strokeWidth: 0
                ),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextAlignment = UITextAlignment.Left,
            };

            UILabel lblAddress = new UILabel
            {
                Frame = new CGRect(42, 244, View.Frame.Width - 84, 48),
                AttributedText = new NSAttributedString(
                    DataManager.DataManager.SharedInstance.BillingAccountDetails.addStreet,
                    font: MyTNBFont.MuseoSans12,
                 foregroundColor: MyTNBColor.TunaGrey(),
                 strokeWidth: 0
                ),
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextAlignment = UITextAlignment.Left,
            };

            btnBackToDashboard = new UIButton(UIButtonType.Custom);
            btnBackToDashboard.Frame = new CGRect(18, View.Frame.Height - 72, View.Frame.Width - 36, 48);
            btnBackToDashboard.SetTitle("Common_BackToDashboard".Translate(), UIControlState.Normal);
            btnBackToDashboard.Font = MyTNBFont.MuseoSans16;
            btnBackToDashboard.Layer.CornerRadius = 4.0f;
            btnBackToDashboard.BackgroundColor = MyTNBColor.FreshGreen;

            View.AddSubview(viewContainer);
            View.AddSubview(imgViewClose);
            View.AddSubview(imgViewSuccess);
            View.AddSubview(lblAccessRequested);
            View.AddSubview(viewLine);

            View.AddSubview(lblDescription);
            View.AddSubview(lblAccountNo);
            View.AddSubview(lblAddress);

            View.AddSubview(btnBackToDashboard);

            UITapGestureRecognizer tap = new UITapGestureRecognizer(() =>
            {
                this.DismissViewController(false, null);
            });
            imgViewClose.AddGestureRecognizer(tap);
        }

        internal void SetEvents()
        {
            btnBackToDashboard.TouchUpInside += (sender, e) =>
            {
                this.DismissViewController(false, null);
            };
        }
    }
}