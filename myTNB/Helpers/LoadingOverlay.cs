using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class LoadingOverlay : UIView
    {
        public LoadingOverlay(CGRect frame) : base(frame)
        {
            // configurable bits
            BackgroundColor = UIColor.Black;
            Alpha = 0.75f;
            AutoresizingMask = UIViewAutoresizing.All;

            nfloat labelHeight = 22;
            nfloat labelWidth = Frame.Width - 20;

            // derive the center x and y
            nfloat centerX = Frame.Width / 2;
            nfloat centerY = Frame.Height / 2;

            // create the activity spinner, center it horizontall and put it 5 points above center x
            UIActivityIndicatorView activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            activitySpinner.Frame = new CGRect(
                centerX - (activitySpinner.Frame.Width / 2),
                centerY - activitySpinner.Frame.Height - 20,
                activitySpinner.Frame.Width,
                activitySpinner.Frame.Height);
            activitySpinner.AutoresizingMask = UIViewAutoresizing.All;
            activitySpinner.Color = UIColor.White;
            AddSubview(activitySpinner);
            activitySpinner.StartAnimating();

            // create and configure the "Loading Data" label
            UILabel loadingLabel = new UILabel(new CGRect(
                centerX - (labelWidth / 2),
                centerY + 20,
                labelWidth,
                labelHeight
                ));
            loadingLabel.BackgroundColor = UIColor.Clear;
            loadingLabel.TextColor = UIColor.White;
            loadingLabel.Text = "Loading...";
            loadingLabel.TextAlignment = UITextAlignment.Center;
            loadingLabel.AutoresizingMask = UIViewAutoresizing.All;
            AddSubview(loadingLabel);
        }

        /// <summary>
        /// Fades out the control and then removes it from the super view
        /// </summary>
        public void Hide()
        {
            UIView.Animate(
                0.5, // duration
                () => { Alpha = 0; },
                () => { RemoveFromSuperview(); }
            );
        }
    }
    /// <summary>
    /// Activity indicator.
    /// </summary>
    public static class ActivityIndicator
    {
        static LoadingOverlay loadingOverlay;
        /// <summary>
        /// Shows the activity Indicator.
        /// </summary>
        public static void Show()
        {
            loadingOverlay = new LoadingOverlay(UIScreen.MainScreen.Bounds);

            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;

            if (!loadingOverlay.IsDescendantOfView(currentWindow))
            {
                currentWindow.AddSubview(loadingOverlay);
            }
        }
        /// <summary>
        /// Hides the activity Indicator.
        /// </summary>
        public static void Hide()
        {
            if (loadingOverlay != null)
            {
                loadingOverlay.Hide();
            }
        }
    }
}