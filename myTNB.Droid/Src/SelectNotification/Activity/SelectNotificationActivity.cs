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
using Android.Content.PM;
using CheeseBind;

namespace myTNB_Android.Src.SelectNotification.Activity
{
    [Activity(Label = "@string/select_notification_activity_title"
              , Icon = "@drawable/ic_launcher"
     , ScreenOrientation = ScreenOrientation.Portrait
     , Theme = "@style/Theme.Notification")]
    public class SelectNotificationActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.listView)]
        ListView listView;

        public override int ResourceId()
        {
            return Resource.Layout.SelectNotificationView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }
    }
}