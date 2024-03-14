using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Com.Airbnb.Lottie;

namespace myTNB.Android.Src.Utils
{
	class LoadingOverlayUtils
	{
        private static MaterialDialog popup;

        private static LottieAnimationView lottieLoadingImage;

        public static void OnRunLoadingAnimation(Android.App.Activity mActivity)
		{
            OnStopLoadingAnimation(mActivity);

            try
            {
                popup = new MaterialDialog.Builder(mActivity)
                        .CustomView(Resource.Layout.GeneralLoadingLayout, false)
                        .Cancelable(false)
                        .CanceledOnTouchOutside(false)
                        .Build();

                View dialogView = popup.Window.DecorView;
                dialogView.SetBackgroundResource(Android.Resource.Color.Transparent);
                WindowManagerLayoutParams wlp = popup.Window.Attributes;
                wlp.Gravity = GravityFlags.Center;
                wlp.Width = ViewGroup.LayoutParams.MatchParent;
                wlp.Height = ViewGroup.LayoutParams.WrapContent;
                popup.Window.Attributes = wlp;

                lottieLoadingImage = popup.FindViewById<LottieAnimationView>(Resource.Id.loading_image);

                try
                {
                    lottieLoadingImage.SetMinAndMaxProgress(0.05f, 0.8f);
                    lottieLoadingImage.Progress = 0.05f;
                    lottieLoadingImage.PlayAnimation();
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                popup.Show();
            }
            catch (Exception e)
            {

                Utility.LoggingNonFatalError(e);
            }
		}

        public static void OnStopLoadingAnimation(Android.App.Activity mActivity)
        {
            try
            {
                if (lottieLoadingImage != null)
                {
                    lottieLoadingImage.CancelAnimation();
                    lottieLoadingImage = null;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                if (popup != null)
                {
                    popup.Dismiss();
                    popup = null;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}