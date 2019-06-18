using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class CustomUIViewController : UIViewController
    {
        UIView _viewToast, _viewToastOverlay;
        UILabel _lblToastDetails;
        bool _isAnimating;

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

        public void DisplayToast(string message)
        {
            if (_viewToast == null)
            {
                _viewToast = new UIView(new CGRect(18, 32, View.Frame.Width - 36, 48))
                {
                    BackgroundColor = MyTNBColor.SunGlow,
                    Hidden = true,
                    ClipsToBounds = true
                };
                _viewToast.Layer.CornerRadius = 2.0f;

                _viewToastOverlay = new UIView(_viewToast.Frame)
                {
                    UserInteractionEnabled = true,
                    MultipleTouchEnabled = true,
                    Hidden = true
                };

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
                AddSwipeGestureForToast();
                View.AddSubviews(new UIView[] { _viewToast, _viewToastOverlay });
            }
            _lblToastDetails.Text = message ?? string.Empty;

            CGSize size = LabelHelper.GetLabelSize(_lblToastDetails
                , _lblToastDetails.Frame.Width, _lblToastDetails.Frame.Height);
            _lblToastDetails.Frame = new CGRect(_lblToastDetails.Frame.X
                , _lblToastDetails.Frame.Y, _lblToastDetails.Frame.Width, size.Height);
            _viewToast.Frame = new CGRect(_viewToast.Frame.X
                , 0, _viewToast.Frame.Width, 0);
            _viewToastOverlay.Frame = new CGRect(_viewToast.Frame.X
                , 32, _viewToast.Frame.Width, size.Height + 32);

            _viewToast.Hidden = false;
            _viewToastOverlay.Hidden = false;
            if (!_isAnimating)
            {
#pragma warning disable XI0001 // Notifies you with advices on how to use Apple APIs
                UIView.Animate(0.3, 0.3, UIViewAnimationOptions.CurveEaseOut, () =>
                 {
                     _isAnimating = true;
                     _viewToast.Frame = new CGRect(_viewToast.Frame.X
                    , 32, _viewToast.Frame.Width, size.Height + 32);
                 }, () =>
                 {
                     DismissToast(2.0F);
                 });
#pragma warning restore XI0001 // Notifies you with advices on how to use Apple APIs
            }
        }

        public void DisplayCustomAlert(string title, string message, Action handler = null, string actionTitle = null)
        {
            ToastHelper.DisplayCustomAlert(title, message, handler, actionTitle);
        }

        public void DisplayCustomAlert(string title, string message, Dictionary<string, Action> ctaButtons)
        {
            ToastHelper.DisplayCustomAlert(title, message, ctaButtons);
        }



        #region Private Methods

        void AddSwipeGestureForToast()
        {
            if (_viewToastOverlay != null)
            {
                _viewToastOverlay.AddGestureRecognizer(new UISwipeGestureRecognizer((obj) =>
                {
                    Debug.WriteLine("SWIPED Up");
                    DismissToast(0.1F);
                })
                {
                    Direction = UISwipeGestureRecognizerDirection.Up
                });
                _viewToastOverlay.AddGestureRecognizer(new UITapGestureRecognizer((obj) =>
                {
                    Debug.WriteLine("Tap");
                    DismissToast(0.1F);
                }));
            }
        }

        void DismissToast(float delay)
        {
#pragma warning disable XI0001 // Notifies you with advices on how to use Apple APIs
            UIView.Animate(0.3, delay, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                _viewToast.Frame = new CGRect(_viewToast.Frame.X
               , 0, _viewToast.Frame.Width, 0);
            }, () =>
            {
                _isAnimating = false;
                _viewToast.Hidden = true;
                _viewToastOverlay.Hidden = true;
                _viewToast.Layer.RemoveAllAnimations();
            });
#pragma warning restore XI0001 // Notifies you with advices on how to use Apple APIs
        }

        #endregion
    }
}
