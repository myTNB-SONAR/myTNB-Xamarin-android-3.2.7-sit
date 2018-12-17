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
using Android.Content.PM;
using myTNB_Android.Src.Base.Activity;
using CheeseBind;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Database.Model;
using System.Runtime;

namespace myTNB_Android.Src.FeedbackFail.Activity
{
    [Activity(
       ScreenOrientation = ScreenOrientation.Portrait
     , Theme = "@style/Theme.BillRelated")]
    public class FeedbackFailActivity : BaseAppCompatActivity
    {

        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtContentInfo)]
        TextView txtContentInfo;

        [BindView(Resource.Id.btnBackToDashboard)]
        Button btnBackToDashboard;

        [BindView(Resource.Id.btnTryAgain)]
        Button btnTryAgain;

        public override int ResourceId()
        {
            return Resource.Layout.FeedbackFailView;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            TextViewUtils.SetMuseoSans300Typeface(txtContentInfo );
            TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo , btnBackToDashboard , btnTryAgain);

            if (!UserEntity.IsCurrentlyActive())
            {
                btnBackToDashboard.Visibility = ViewStates.Gone;
            }
        }


        [OnClick(Resource.Id.btnTryAgain)]
        void TryAgain(object sender, EventArgs eventArgs)
        {
            Finish();
        }

        [OnClick(Resource.Id.btnBackToDashboard)]
        void BackToDashboard(object sender, EventArgs eventArgs)
        {
            var dashboardIntent = new Intent(this , typeof(DashboardActivity));
            dashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(dashboardIntent);
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