using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Utils;
using System;
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

            TextViewUtils.SetMuseoSans300Typeface(txtContentInfo);
            TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo, btnBackToDashboard, btnTryAgain);
            txtTitleInfo.TextSize = TextViewUtils.GetFontSize(16f);
            txtContentInfo.TextSize = TextViewUtils.GetFontSize(14f);
            if (!UserEntity.IsCurrentlyActive())
            {
                btnBackToDashboard.Visibility = ViewStates.Gone;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Submit Feedback Failed");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        [OnClick(Resource.Id.btnTryAgain)]
        void TryAgain(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Finish();
            }
        }

        [OnClick(Resource.Id.btnBackToDashboard)]
        void BackToDashboard(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                var dashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
                dashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(dashboardIntent);
            }
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