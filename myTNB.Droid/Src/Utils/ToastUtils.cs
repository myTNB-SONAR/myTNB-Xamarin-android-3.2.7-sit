using System;
using Android.Text;
using Android.Views;
using Android.Widget;

namespace myTNB.AndroidApp.Src.Utils
{
    public class ToastUtils
    {
        public static void OnDisplayToast(Android.App.Activity mActivity, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message))
                {
                    return;
                }
                //Toast.MakeText(mActivity, message, ToastLength.Long).Show();
                LayoutInflater inflater = LayoutInflater.From(mActivity);
                View layout = inflater.Inflate(Resource.Layout.ToastLayout, null, false);
                TextView messageTextView = layout.FindViewById<TextView>(Resource.Id.toastMessage);
                messageTextView.Text = message ?? string.Empty;
                TextViewUtils.SetTextSize12(messageTextView);
                TextViewUtils.SetMuseoSans300Typeface(messageTextView);

                Toast toast = new Toast(mActivity)
                {
                    Duration = ToastLength.Long,
                    View = layout
                };

                toast.SetGravity(GravityFlags.Top | GravityFlags.FillHorizontal, 0, 0);
                toast.Show();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] OnDisplayToast Error: " + e.Message);
            }
        }
    }
}