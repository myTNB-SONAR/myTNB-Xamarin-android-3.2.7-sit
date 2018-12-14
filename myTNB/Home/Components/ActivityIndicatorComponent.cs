using System;
using CoreGraphics;
using UIKit;

namespace myTNB.Home.Components
{
    public class ActivityIndicatorComponent
    {
        UIView View;
        UIView _viewActivityIndicator;

        public ActivityIndicatorComponent(UIView view)
        {
            View = view;
            CreateActivityIndicator();
        }

        void CreateActivityIndicator()
        {
            _viewActivityIndicator = new UIView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
            _viewActivityIndicator.BackgroundColor = UIColor.Black;
            _viewActivityIndicator.Alpha = 0.75F;
            _viewActivityIndicator.Hidden = true;

            nfloat centerX = _viewActivityIndicator.Frame.Width / 2;
            nfloat centerY = _viewActivityIndicator.Frame.Height / 2;

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
            lblLoading.Text = "Loading...";
            lblLoading.TextAlignment = UITextAlignment.Center;
            lblLoading.AutoresizingMask = UIViewAutoresizing.All;

            _viewActivityIndicator.AddSubviews(new UIView[] { activityIndicator, lblLoading });

            View.AddSubview(_viewActivityIndicator);
        }

        public void Show()
        {
            _viewActivityIndicator.Hidden = false;
        }

        public void Hide()
        {
            _viewActivityIndicator.Hidden = true;
        }
    }
}