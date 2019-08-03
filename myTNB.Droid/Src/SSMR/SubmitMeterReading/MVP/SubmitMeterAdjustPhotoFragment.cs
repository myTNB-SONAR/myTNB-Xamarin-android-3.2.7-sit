
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class SubmitMeterAdjustPhotoFragment : Fragment
    {
        Bitmap myBitmap;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            ImageView capturedImage = view.FindViewById<ImageView>(Resource.Id.adjust_photo_preview);
            capturedImage.SetImageBitmap(myBitmap);
        }

        public static SubmitMeterAdjustPhotoFragment NewIntance()
        {
            return new SubmitMeterAdjustPhotoFragment();
        }

        public void SetCapturedImage(Bitmap capturedBitmap)
        {
            myBitmap = capturedBitmap;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return inflater.Inflate(Resource.Layout.SubmitMeterAdjustPhotoFragmentLayout, container, false);
        }
    }
}
