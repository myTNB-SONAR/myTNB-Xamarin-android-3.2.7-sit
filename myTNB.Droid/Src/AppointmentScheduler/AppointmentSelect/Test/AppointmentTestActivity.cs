
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace myTNB_Android.Src.AppointmentScheduler.AppointmentSelect.Test
{
    [Activity(Label = "Set an Appointment", Theme = "@style/Theme.AppointmentScheduler")]
    public class AppointmentTestActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AppointmentTestLayout);


            // Create your application here
        }
    }
}
