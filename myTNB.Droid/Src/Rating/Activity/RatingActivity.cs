using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Rating.Fargment;
using myTNB_Android.Src.Utils;
using System;
using System.Runtime;

namespace myTNB_Android.Src.Rating.Activity
{
    [Activity(Label = "Rating"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.PaymentSuccessExperienceRating")]
    public class RatingActivity : BaseToolbarAppCompatActivity
    {

        private Android.Support.V7.Widget.Toolbar toolbar;
        private Android.Support.Design.Widget.AppBarLayout appBarLayout;
        private FrameLayout frameContainer;
        private Android.Support.Design.Widget.CoordinatorLayout coordinatorLayout;

        Android.App.Fragment currentFragment;

        private string quesIdCategory = "1";
        private string merchantTransID;
        private string deviceID;
        private int selectedRating;

        public override int ResourceId()
        {
            return Resource.Layout.rating_activity_view;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void ShowToolBar()
        {
            try
            {
                if (appBarLayout != null)
                {
                    TypedValue tv = new TypedValue();
                    int actionBarHeight = 0;
                    if (Theme.ResolveAttribute(Android.Resource.Attribute.ActionBarSize, tv, true))
                    {
                        actionBarHeight = TypedValue.ComplexToDimensionPixelSize(tv.Data, Resources.DisplayMetrics);
                    }

                    appBarLayout.Visibility = ViewStates.Visible;
                    Android.Support.Design.Widget.CoordinatorLayout.LayoutParams lp = new Android.Support.Design.Widget.CoordinatorLayout.LayoutParams(Android.Support.Design.Widget.CoordinatorLayout.LayoutParams.MatchParent, Android.Support.Design.Widget.CoordinatorLayout.LayoutParams.MatchParent);
                    lp.SetMargins(0, actionBarHeight, 0, 0);

                    frameContainer.LayoutParameters = lp;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideToolBar()
        {
            try
            {
                if (appBarLayout != null)
                {
                    appBarLayout.Visibility = ViewStates.Gone;
                    Android.Support.Design.Widget.CoordinatorLayout.LayoutParams lp = new Android.Support.Design.Widget.CoordinatorLayout.LayoutParams(Android.Support.Design.Widget.CoordinatorLayout.LayoutParams.MatchParent, Android.Support.Design.Widget.CoordinatorLayout.LayoutParams.MatchParent);
                    lp.SetMargins(0, 0, 0, 0);

                    frameContainer.LayoutParameters = lp;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                appBarLayout = FindViewById<Android.Support.Design.Widget.AppBarLayout>(Resource.Id.appBar);
                toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                frameContainer = FindViewById<FrameLayout>(Resource.Id.fragment_container);
                coordinatorLayout = FindViewById<Android.Support.Design.Widget.CoordinatorLayout>(Resource.Id.coordinatorLayout);

                deviceID = DeviceIdUtils.DeviceId(this);
                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.QUESTION_ID_CATEGORY))
                    {
                        quesIdCategory = extras.GetInt(Constants.QUESTION_ID_CATEGORY).ToString();
                    }
                    if (extras.ContainsKey(Constants.MERCHANT_TRANS_ID))
                    {
                        merchantTransID = extras.GetString(Constants.MERCHANT_TRANS_ID);
                    }
                    if (extras.ContainsKey(Constants.SELECTED_RATING))
                    {
                        selectedRating = extras.GetInt(Constants.SELECTED_RATING, 1);
                    }

                }

                OnLoadMainFragment();
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
                FirebaseAnalyticsUtils.SetScreenName(this, "Post-Payment Rating");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnLoadMainFragment()
        {
            Android.App.Fragment submitRatingFragment = new SubmitRatingFragment();
            Bundle bundle = new Bundle();
            bundle.PutString(Constants.QUESTION_ID_CATEGORY, quesIdCategory);
            bundle.PutInt(Constants.SELECTED_RATING, selectedRating);
            bundle.PutString(Constants.MERCHANT_TRANS_ID, merchantTransID);
            bundle.PutString(Constants.DEVICE_ID_PARAM, deviceID);
            submitRatingFragment.Arguments = bundle;
            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.fragment_container, submitRatingFragment);
            fragmentTransaction.Commit();
            currentFragment = submitRatingFragment;
        }

        public void nextFragment(Android.App.Fragment fragment, Bundle bundle)
        {
            if (fragment is SubmitRatingFragment)
            {
                var thankYouFragment = new ThankYouFragment();
                thankYouFragment.Arguments = bundle;
                var fragmentTransaction = FragmentManager.BeginTransaction();
                fragmentTransaction.Add(Resource.Id.fragment_container, thankYouFragment);
                fragmentTransaction.AddToBackStack(null);
                fragmentTransaction.Commit();
                currentFragment = thankYouFragment;
            }
        }

        public override void OnBackPressed()
        {
            try
            {
                int count = this.FragmentManager.BackStackEntryCount;
                Log.Debug("OnBackPressed", "fragment stack count :" + count);
                if (currentFragment is ThankYouFragment || currentFragment is SubmitRatingFragment)
                {
                    Finish();
                }
                else
                {
                    this.FragmentManager.PopBackStack();
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
    }
}