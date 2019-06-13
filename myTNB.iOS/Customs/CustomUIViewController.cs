using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class CustomUIViewController : UIViewController
    {
        UIView _viewToast;
        UILabel _lblToastDetails;

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

        public void DisplayToast(string message, Action action = null)
        {
            if (_viewToast == null)
            {
                _viewToast = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 48))
                {
                    BackgroundColor = MyTNBColor.SunGlow
                };
                _viewToast.Layer.CornerRadius = 2.0f;
                _viewToast.Hidden = true;

                _lblToastDetails = new UILabel(new CGRect(16, 16, _viewToast.Frame.Width - 32
                    , View.Frame.Height - ((_viewToast.Frame.X * 2) + 32)))
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = MyTNBFont.MuseoSans12,
                    TextColor = MyTNBColor.TunaGrey(),
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap
                };

                _viewToast.AddSubview(_lblToastDetails);
                View.AddSubview(_viewToast);
            }
            _lblToastDetails.Text = message ?? string.Empty;

            CGSize size = LabelHelper.GetLabelSize(_lblToastDetails
                , _lblToastDetails.Frame.Width, _lblToastDetails.Frame.Height);
            _lblToastDetails.Frame = new CGRect(_lblToastDetails.Frame.X
                , _lblToastDetails.Frame.Y, _lblToastDetails.Frame.Width, size.Height);
            _viewToast.Frame = new CGRect(_viewToast.Frame.X
                , _viewToast.Frame.Y, _viewToast.Frame.Width, size.Height + 32);

            _viewToast.Hidden = false;
            _viewToast.Alpha = 1.0f;
#pragma warning disable XI0001 // Notifies you with advices on how to use Apple APIs
            UIView.Animate(5, 1, UIViewAnimationOptions.CurveEaseOut, () =>
             {
                 _viewToast.Alpha = 0.0f;
             }, () =>
             {
                 _viewToast.Hidden = true;
                 action?.Invoke();
             });
#pragma warning restore XI0001 // Notifies you with advices on how to use Apple APIs
        }
    }
}