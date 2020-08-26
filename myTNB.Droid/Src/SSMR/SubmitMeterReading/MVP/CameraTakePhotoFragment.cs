
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using myTNB_Android.Src.SSMR.SubmitMeterReading.Listener;
using Camera = Android.Hardware.Camera;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class CameraTakePhotoFragment : Fragment
    {
        FrameLayout cameraContainer;
        static Camera cameraInstance;
        ImageView cameraButton;
        static ImageView previewImage;
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
                c = Camera.Open();
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
            cameraButton = view.FindViewById<ImageView>(Resource.Id.imageTakePhoto);
            previewImage = view.FindViewById<ImageView>(Resource.Id.imageGallery);
            cameraButton.Click += delegate
            {
                cameraInstance.TakePicture(null,new RawCallback(),null);
            };
            cameraInstance = SetUpCamera();
            cameraContainer.AddView(new CameraPreview(Activity, cameraInstance));
            return view;
        }

        public class RawCallback : Java.Lang.Object, Camera.IPictureCallback
        {
            public void OnPictureTaken(byte[] data, Camera camera)
            {
                //if (data != null)
                //{
                //    Bitmap bitmap = BitmapFactory.DecodeByteArray(data, 0, data.Length);

                //    if (bitmap != null)
                //    {
                //        previewImage.SetImageBitmap(bitmap);
                //    }
                //}

                

                try
                {
                    Thread.Sleep(20);
                    
                }
                catch (Exception e)
                {
                    //e.print;
                }

                camera.StartPreview();
            }
        }
    }
}
