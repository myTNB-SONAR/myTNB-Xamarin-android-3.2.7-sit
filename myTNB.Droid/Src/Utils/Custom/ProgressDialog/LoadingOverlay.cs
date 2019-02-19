using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace myTNB_Android.Src.Utils.Custom.ProgressDialog
{
    public class LoadingOverlay : Android.App.Dialog
    {
        private TextView txtLoadingText;
        private ImageView imgLoadingImage;
        private AnimationDrawable animationDrawable;

        public LoadingOverlay(Context context, int themeResId) : base(context, themeResId)
        {
            WindowManagerLayoutParams lp = new WindowManagerLayoutParams();
            lp.CopyFrom(Window.Attributes);
            lp.Gravity = GravityFlags.CenterHorizontal | GravityFlags.CenterVertical;
            Window.Attributes = lp;
            Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            SetCancelable(false);
            SetOnCancelListener(null);

            View layout = LayoutInflater.From(context).Inflate(Resource.Layout.LoadingOverlayView, null, false);
            txtLoadingText = (TextView) layout.FindViewById(Resource.Id.textLoadingText);
            TextViewUtils.SetMuseoSans300Typeface(txtLoadingText);
            imgLoadingImage = (ImageView)layout.FindViewById(Resource.Id.imgLoadingLogo);
            imgLoadingImage.SetPadding(0, 0, 0, 10);
            AddContentView(layout, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        }

        public override void Show()
        {
            base.Show();
            if (imgLoadingImage != null)
            {
                imgLoadingImage.Post(() =>
                {
                    animationDrawable = (AnimationDrawable)imgLoadingImage.Background;
                    if (!animationDrawable.IsRunning)
                    {
                        animationDrawable.Start();
                    }
                });
            }
        }

        public override void Dismiss()
        {
            base.Dismiss();
            if(animationDrawable != null){
                imgLoadingImage.Background = null;
                animationDrawable.Dispose();
            }
        }
    }
}