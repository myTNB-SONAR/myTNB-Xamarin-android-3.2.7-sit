﻿using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using System;

namespace myTNB_Android.Src.Utils.Custom.ProgressDialog
{
    public class LoadingOverlay : Android.App.Dialog
    {
        private TextView txtLoadingText;
        private ImageView imgLoadingImage;
        private AnimationDrawable animationDrawable;

        public LoadingOverlay(Context context, int themeResId) : base(context, themeResId)
        {
            try
            {
                WindowManagerLayoutParams lp = new WindowManagerLayoutParams();
                lp.CopyFrom(Window.Attributes);
                lp.Gravity = GravityFlags.CenterHorizontal | GravityFlags.CenterVertical;
                Window.Attributes = lp;
                Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                SetCancelable(false);
                SetOnCancelListener(null);

                View layout = LayoutInflater.From(context).Inflate(Resource.Layout.LoadingOverlayView, null, false);
                txtLoadingText = (TextView)layout.FindViewById(Resource.Id.textLoadingText);
                TextViewUtils.SetMuseoSans300Typeface(txtLoadingText);
                imgLoadingImage = (ImageView)layout.FindViewById(Resource.Id.imgLoadingLogo);
                imgLoadingImage.SetPadding(0, 0, 0, 10);
                imgLoadingImage.SetBackgroundResource(Resource.Drawable.LoadingViewAnim);
                AddContentView(layout, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void Show()
        {
            base.Show();
            try
            {
                if (imgLoadingImage != null)
                {
                    imgLoadingImage.Post(() =>
                    {
                        animationDrawable = (AnimationDrawable)imgLoadingImage.Background;
                        if (animationDrawable != null && !animationDrawable.IsRunning)
                        {
                            animationDrawable.Start();
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void Dismiss()
        {
            base.Dismiss();
            try
            {
                if (animationDrawable != null)
                {
                    imgLoadingImage.Background = null;
                    //animationDrawable.Dispose();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


    }
}