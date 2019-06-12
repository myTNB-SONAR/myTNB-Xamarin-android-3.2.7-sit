using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class CustomUIViewController : UIViewController
    {
        UIView _viewDelete;
        UILabel lblDeleteDetails;

        public CustomUIViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
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

        public void DisplayToast(string message, Action action)
        {

            if (_viewDelete == null)
            {
                _viewDelete = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 48))
                {
                    BackgroundColor = MyTNBColor.SunGlow
                };
                _viewDelete.Layer.CornerRadius = 2.0f;
                _viewDelete.Hidden = true;

                lblDeleteDetails = new UILabel(new CGRect(16, 16, _viewDelete.Frame.Width - 32, 16))
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = MyTNBFont.MuseoSans12,
                    TextColor = MyTNBColor.TunaGrey(),
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap
                };
                _viewDelete.AddSubview(lblDeleteDetails);
                View.AddSubview(_viewDelete);
            }
            lblDeleteDetails.Text = message ?? string.Empty;
            _viewDelete.Hidden = false;
            _viewDelete.Alpha = 1.0f;
#pragma warning disable XI0001 // Notifies you with advices on how to use Apple APIs
            UIView.Animate(5, 1, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                _viewDelete.Alpha = 0.0f;
            }, () =>
            {
                _viewDelete.Hidden = true;
                action?.Invoke();

            });
#pragma warning restore XI0001 // Notifies you with advices on how to use Apple APIs
        }
    }
}