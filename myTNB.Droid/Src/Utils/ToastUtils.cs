using System;
using Android.Views;
using Android.Widget;

namespace myTNB_Android.Src.Utils
{
    public class ToastUtils
    {
        public static void OnDisplayToast(Android.App.Activity mActivity, string message)
        {
            try
            {
                LayoutInflater inflater = LayoutInflater.From(mActivity);
                View layout = inflater.Inflate(Resource.Layout.ToastLayout, null, false);
                TextView messageTextView = layout.FindViewById<TextView>(Resource.Id.toastMessage);
                messageTextView.Text = message ?? string.Empty;
                //Todo: Set Font size for large fonts

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
