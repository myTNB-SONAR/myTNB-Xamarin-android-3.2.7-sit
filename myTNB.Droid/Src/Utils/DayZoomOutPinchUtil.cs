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
using Android.Widget;

namespace myTNB_Android.Src.Utils
{
    class DayZoomOutPinchUtil
    {

        public static MaterialDialog OnBuildZoomOutPinchTooltip(Android.App.Activity mActivity)
        {
            MaterialDialog popup;

            popup = new MaterialDialog.Builder(mActivity)
                    .CustomView(Resource.Layout.CustomDialogViewDailyUsage, false)
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


            TextView txtTitle = popup.FindViewById<TextView>(Resource.Id.day_zoomout_pinch_txtTitle);
            TextView txtMessage = popup.FindViewById<TextView>(Resource.Id.day_zoomout_pinch_txtMsg);
            Button txtBtnFirst = popup.FindViewById<Button>(Resource.Id.day_zoomout_pinch_button);

            TextViewUtils.SetMuseoSans500Typeface(txtTitle, txtBtnFirst);
            TextViewUtils.SetMuseoSans300Typeface(txtMessage);

            txtBtnFirst.Click += delegate
            {
                popup.Dismiss();
            };

            return popup;
        }
    }
}