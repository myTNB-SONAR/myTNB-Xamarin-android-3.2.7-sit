
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

namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    [Activity(Label = "ApplicationFormSMRActivity", Theme = "@style/Theme.Dashboard")]
    public class ApplicationFormSMRActivity : BaseAppCompatActivity
    {
        public override int ResourceId()
        {
            return Resource.Layout.ApplicationFormSMRLayout;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }
    }
}
