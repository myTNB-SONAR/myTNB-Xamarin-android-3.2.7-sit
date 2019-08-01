
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
    [Activity(Label = "Submit Meter Reading", Theme = "@style/Theme.Dashboard", MainLauncher = true)]
    public class SubmitMeterReadingActivity : BaseToolbarAppCompatActivity
    {
        public override int ResourceId()
        {
            return Resource.Layout.SubmitMeterReadingLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            LinearLayout linearLayout = FindViewById(Resource.Id.kwCard) as LinearLayout;
            EditText et = linearLayout.FindViewById(Resource.Id.new_reading_1) as EditText;
            et.Text = "9";
        }
    }
}
