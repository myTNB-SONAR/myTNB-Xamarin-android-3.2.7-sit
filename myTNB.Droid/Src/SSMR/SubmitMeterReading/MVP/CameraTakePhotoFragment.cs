
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.SSMR.SubmitMeterReading.Listener;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class CameraTakePhotoFragment : Fragment
    {
        FrameLayout cameraContainer;
        Camera cameraInstance;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public Camera SetUpCamera()
        {
            Camera c = null;
            try
            {
                c = Camera.Open(0);
            }
            catch (Exception e)
            {
                Log.Debug("", "Device camera not available now.");
            }

            return c;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.CameraTakePhotoFragmentLayout,container,false);
            cameraContainer = view.FindViewById<FrameLayout>(Resource.Id.camera_preview);
            cameraInstance = SetUpCamera();
            cameraContainer.AddView(new CameraPreview(Activity, cameraInstance));
            return view;
        }
    }
}
