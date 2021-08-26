using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.EnergyBudgetRating.Fargment;
using myTNB_Android.Src.Utils;
using System;
using System.Runtime;

namespace myTNB_Android.Src.EnergyBudgetRating.Activity
{
    [Activity(Label = "@string/app_name"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.PreLogin")]
    public class EnergyBudgetRatingActivity : BaseAppCompatActivity
    {

        //private AndroidX.AppCompat.Widget.Toolbar toolbar;
        private FrameLayout frameContainer;
        //private AndroidX.CoordinatorLayout.Widget.CoordinatorLayout coordinatorLayout;

        AndroidX.Fragment.App.Fragment  currentFragment;

        private int SelectStar = 1;
        private string SelectYesOrNo;
        private string deviceID;
        private int selectedRating;
        private string PAGE_ID = "Rating";
        private bool fromNotification = false;
        private bool fromSaveEnergyBudget = false;

        public override int ResourceId()
        {
            return Resource.Layout.rating_activity_view_energybudget;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                //appBarLayout = FindViewById<AppBarLayout>(Resource.Id.appBar);
                //toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
                frameContainer = FindViewById<FrameLayout>(Resource.Id.fragment_container);
                //coordinatorLayout = FindViewById<AndroidX.CoordinatorLayout.Widget.CoordinatorLayout>(Resource.Id.coordinatorLayout);

                deviceID = DeviceIdUtils.DeviceId(this);
                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey("feedbackOne"))
                    {
                        fromNotification = true;
                        SelectYesOrNo = extras.GetString("feedbackOne");
                    }
                    else if (extras.ContainsKey("feedbackTwo"))
                    {
                        fromSaveEnergyBudget = true;
                        SelectStar = extras.GetInt("feedbackTwo");
                    }
                }

                if (fromNotification)
                {
                    OnLoadFeedbackOneFragment();
                }
                else
                {
                    OnLoadFeedbackTwoFragment();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                //FirebaseAnalyticsUtils.SetScreenName(this, "Post-Payment Rating");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnLoadFeedbackOneFragment()
        {
            AndroidX.Fragment.App.Fragment feedbackOne = new FeedbackOne();
            Bundle bundle = new Bundle();
            bundle.PutString("StarFromNotificationDetailPage", SelectYesOrNo);
            feedbackOne.Arguments = bundle;
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.fragment_container, feedbackOne);
            fragmentTransaction.Commit();
            currentFragment = feedbackOne;
        }

        public void OnLoadFeedbackTwoFragment()
        {
            AndroidX.Fragment.App.Fragment feedbackTwo = new FeedbackTwo();
            Bundle bundle = new Bundle();
            bundle.PutInt("StarFromDashboardFragmentPage", SelectStar);
            feedbackTwo.Arguments = bundle;
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.fragment_container, feedbackTwo);
            fragmentTransaction.Commit();
            currentFragment = feedbackTwo;
        }

        public void nextFragment(AndroidX.Fragment.App.Fragment  fragment, Bundle bundle)
        {
            if (currentFragment is FeedbackTwo || currentFragment is FeedbackOne)
            {
                Finish();
            }
        }

        public override void OnBackPressed()
        {
            try
            {
                int count = this.SupportFragmentManager.BackStackEntryCount;
                Log.Debug("OnBackPressed", "fragment stack count :" + count);
                if (currentFragment is FeedbackTwo || currentFragment is FeedbackOne)
                {
                    Finish();
                }
                else
                {
                    this.SupportFragmentManager.PopBackStack();
                }

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
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
        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }
    }
}