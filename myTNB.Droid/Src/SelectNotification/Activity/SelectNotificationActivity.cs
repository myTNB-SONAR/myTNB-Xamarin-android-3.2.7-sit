using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Base.Activity;
using System;
using System.Runtime;

namespace myTNB.AndroidApp.Src.SelectNotification.Activity
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

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }
    }
}