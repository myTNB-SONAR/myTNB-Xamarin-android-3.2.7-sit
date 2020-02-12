using System;
using Airbnb.Lottie;
using CoreGraphics;
using UIKit;
using static myTNB.TNBGlobal;

namespace myTNB
{
    public class LoadingOverlay : UIView
    {
        public LoadingOverlay(CGRect frame) : base(frame)
        {
            // configurable bits
            BackgroundColor = UIColor.Clear;
            AutoresizingMask = UIViewAutoresizing.All;

            UIView container = new UIView(this.Bounds)
            {
                BackgroundColor = MyTNBColor.Black75
            };
            AddSubview(container);

            this.Tag = TNBGlobal.Tags.LoadingOverlay;

            nfloat animationWidth = ScaleUtility.GetScaledWidth(48F);
            nfloat animationHeight = ScaleUtility.GetScaledHeight(48F);

            LOTAnimationView animation = LOTAnimationView.AnimationNamed("TNB_Logo");
            animation.Frame = new CGRect(ScaleUtility.GetXLocationToCenterObject(animationWidth, container), ScaleUtility.GetYLocationToCenterObject(animationHeight, container),
                                         animationWidth, animationHeight);
            animation.ContentMode = UIViewContentMode.ScaleAspectFit;
            animation.LoopAnimation = true;
            animation.Play();
            container.AddSubview(animation);
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
                foreach (UIView view in currentWindow.Subviews)
                {
                    if (view.Tag == TNBGlobal.Tags.LoadingOverlay)
                    {
                        view.RemoveFromSuperview();
                        break;
                    }
                }
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