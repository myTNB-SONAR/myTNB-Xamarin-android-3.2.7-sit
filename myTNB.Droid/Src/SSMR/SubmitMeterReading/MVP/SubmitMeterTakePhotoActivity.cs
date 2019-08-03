
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

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    [Activity(Label = "Take Photo", Theme = "@style/Theme.Dashboard", MainLauncher = true)]
    public class SubmitMeterTakePhotoActivity : BaseToolbarAppCompatActivity
    {
        public override int ResourceId()
        {
            return Resource.Layout.SubmitMeterTakePhotoLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override bool CameraPermissionRequired()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            //FragmentManager.BeginTransaction()
            //  .Replace(Resource.Id.photoContainer, new CameraTakePhotoFragment())
            //  .Commit();
            if (savedInstanceState == null)
            {
                FragmentManager.BeginTransaction().Replace(Resource.Id.photoContainer, SubmitMeterTakePhotoFragment.NewInstance()).Commit();
            }
        }
    }
}
