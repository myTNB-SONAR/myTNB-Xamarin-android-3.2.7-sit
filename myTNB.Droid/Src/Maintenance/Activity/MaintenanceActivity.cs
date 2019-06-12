using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Maintenance.Activity
{
    [Activity(Label = "@string/app_name"
       , NoHistory = true
              , Icon = "@drawable/ic_launcher"
       , LaunchMode = LaunchMode.SingleInstance
       , ScreenOrientation = ScreenOrientation.Portrait
              , Theme = "@style/Theme.Dashboard")]
    public class MaintenanceActivity : BaseAppCompatActivity
    {
        [BindView(Resource.Id.maintenance_heading)]
        TextView txtHeading;

        [BindView(Resource.Id.maintenance_content)]
        TextView txtContent;

        [BindView(Resource.Id.maintenance_image)]
        ImageView imgMaintenance;

        public override int ResourceId()
        {
            return Resource.Layout.MaintenanceView;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Intent != null && Intent.Extras != null && Intent.Extras.ContainsKey(Constants.MAINTENANCE_TITLE_KEY) && Intent.Extras.ContainsKey(Constants.MAINTENANCE_MESSAGE_KEY))
            {
                try
                {
                    string title = Intent.Extras.GetString(Constants.MAINTENANCE_TITLE_KEY);
                    string message = Intent.Extras.GetString(Constants.MAINTENANCE_MESSAGE_KEY);
                    txtHeading.Text = title;
                    txtContent.Text = message;

                    TextViewUtils.SetMuseoSans300Typeface(txtContent);
                    TextViewUtils.SetMuseoSans500Typeface(txtHeading);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
        }
    }
}