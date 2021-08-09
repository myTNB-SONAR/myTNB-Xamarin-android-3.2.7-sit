
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;

namespace myTNB_Android.Src.AppointmentDetailSet.Activity
{
    [Activity(Label = "AppointmentSetActivity", ScreenOrientation = ScreenOrientation.Portrait
                  , WindowSoftInputMode = SoftInput.AdjustPan
          , Theme = "@style/Theme.FaultyStreetLamps")]
    public class AppointmentSetActivity : BaseAppCompatActivity
    {
        public override int ResourceId()
        {
            return Resource.Layout.AppointmentsetLayout;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }
    }
}