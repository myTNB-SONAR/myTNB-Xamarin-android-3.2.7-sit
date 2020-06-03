using System;
using Airbnb.Lottie;
using CoreGraphics;
using UIKit;

namespace myTNB.Home.Components
{
    public class ActivityIndicatorComponent
    {
        private readonly UIView View;
        private UIView _viewActivityIndicator;

        public ActivityIndicatorComponent(UIView view)
        {
            View = view;
            CreateActivityIndicator();
        }

        void CreateActivityIndicator()
        {
            _viewActivityIndicator = new UIView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height))
            {
                BackgroundColor = UIColor.Black,
                Alpha = 0.75F,
                Hidden = true
            };

            nfloat centerX = _viewActivityIndicator.Frame.Width / 2;
            nfloat centerY = _viewActivityIndicator.Frame.Height / 2;

#if true
            nfloat animationWidth = 48;
            LOTAnimationView animation = LOTAnimationView.AnimationNamed("TNB_Logo");
            animation.Frame = new CGRect(centerX - animationWidth / 2, centerY - animationWidth / 2
                , animationWidth, animationWidth);
            animation.ContentMode = UIViewContentMode.ScaleAspectFit;
            animation.LoopAnimation = true;
            animation.Play();
            _viewActivityIndicator.AddSubview(animation);
#else
            UIActivityIndicatorView activityIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            activityIndicator.Frame = new CGRect(
                centerX - (activityIndicator.Frame.Width / 2)
                , centerY - activityIndicator.Frame.Height - 5
                , activityIndicator.Frame.Width
                , activityIndicator.Frame.Height
            );
            activityIndicator.AutoresizingMask = UIViewAutoresizing.All;
            activityIndicator.Color = UIColor.White;
            activityIndicator.StartAnimating();
            _viewActivityIndicator.AddSubview(activityIndicator);

            nfloat labelHeight = 22;
            nfloat labelWidth = _viewActivityIndicator.Frame.Width - 20;

            UILabel lblLoading = new UILabel();
            lblLoading.Frame = new CGRect(
                centerX - (labelWidth / 2)
                , centerY + 5
                , labelWidth
                , labelHeight
            );
            lblLoading.BackgroundColor = UIColor.Clear;
            lblLoading.TextColor = UIColor.White;
            lblLoading.Text = Texts.InfoLoading;
            lblLoading.Font = myTNBFont.MuseoSans14_300;
            lblLoading.TextAlignment = UITextAlignment.Center;
            lblLoading.AutoresizingMask = UIViewAutoresizing.All;
            _viewActivityIndicator.AddSubview(lblLoading);
#endif

            //_viewActivityIndicator.AddSubviews(new UIView[] { activityIndicator, lblLoading });

            View.AddSubview(_viewActivityIndicator);
        }

        public void Show()
        {
            View.BringSubviewToFront(_viewActivityIndicator);
            _viewActivityIndicator.Hidden = false;
        }

        public void Hide()
        {
            _viewActivityIndicator.Hidden = true;
        }

        public UIView GetView
        {
            get
            {
                return _viewActivityIndicator;
            }
            set
            {
                _viewActivityIndicator = value;
            }
        }
    }
}