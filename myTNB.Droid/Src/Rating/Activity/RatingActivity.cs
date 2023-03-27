using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Android.Material.AppBar;
using myTNB.Mobile;
using myTNB.Mobile.SessionCache;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.MyHome;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Rating.Fargment;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Deeplink;
using Newtonsoft.Json;
using System;
using System.Runtime;

namespace myTNB_Android.Src.Rating.Activity
{
    [Activity(Label = "Rating"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.PaymentSuccessExperienceRating")]
    public class RatingActivity : BaseActivityCustom
    {

        private AndroidX.AppCompat.Widget.Toolbar toolbar;
        private AppBarLayout appBarLayout;
        private FrameLayout frameContainer;
        private AndroidX.CoordinatorLayout.Widget.CoordinatorLayout coordinatorLayout;

        AndroidX.Fragment.App.Fragment  currentFragment;

        private string quesIdCategory = "1";
        private string merchantTransID;
        private string deviceID;
        private int selectedRating;
        private string PAGE_ID = "Rating";

        private DetailCTAType _ctaType;

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
                    AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams lp = new AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams(AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams.MatchParent, AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams.MatchParent);
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
                    AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams lp = new AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams(AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams.MatchParent, AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams.MatchParent);
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
                appBarLayout = FindViewById<AppBarLayout>(Resource.Id.appBar);
                toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
                frameContainer = FindViewById<FrameLayout>(Resource.Id.fragment_container);
                coordinatorLayout = FindViewById<AndroidX.CoordinatorLayout.Widget.CoordinatorLayout>(Resource.Id.coordinatorLayout);

                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

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
                    if (extras.ContainsKey(MyHomeConstants.CTA_TYPE))
                    {
                        _ctaType = JsonConvert.DeserializeObject<DetailCTAType>(extras.GetString(MyHomeConstants.CTA_TYPE));
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
            AndroidX.Fragment.App.Fragment  submitRatingFragment = new SubmitRatingFragment();
            Bundle bundle = new Bundle();
            bundle.PutString(Constants.QUESTION_ID_CATEGORY, quesIdCategory);
            bundle.PutInt(Constants.SELECTED_RATING, selectedRating);
            bundle.PutString(Constants.MERCHANT_TRANS_ID, merchantTransID);
            bundle.PutString(Constants.DEVICE_ID_PARAM, deviceID);
            bundle.PutString(MyHomeConstants.CTA_TYPE, JsonConvert.SerializeObject(_ctaType));
            submitRatingFragment.Arguments = bundle;
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.fragment_container, submitRatingFragment);
            fragmentTransaction.Commit();
            currentFragment = submitRatingFragment;
        }

        public void nextFragment(AndroidX.Fragment.App.Fragment  fragment, Bundle bundle)
        {
            if (fragment is SubmitRatingFragment)
            {
                var thankYouFragment = new ThankYouFragment();
                thankYouFragment.Arguments = bundle;
                var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                fragmentTransaction.Add(Resource.Id.fragment_container, thankYouFragment);
                fragmentTransaction.AddToBackStack(null);
                fragmentTransaction.Commit();
                currentFragment = thankYouFragment;
            }
        }

        public void OnNavigateToApplicationDetails()
        {
            Intent finishRateIntent = new Intent();
            finishRateIntent.PutExtra(Constants.APPLICATION_STATUS_DETAIL_RELOAD, true);
            finishRateIntent.PutExtra(Constants.APPLICATION_STATUS_DETAIL_RATED_TOAST_MESSAGE, RatingCache.Instance.GetRatingToastMessage());
            RatingCache.Instance.Clear();
            SetResult(Result.Ok, finishRateIntent);
            Finish();
        }

        public override void OnBackPressed()
        {
            try
            {
                int count = this.SupportFragmentManager.BackStackEntryCount;
                Log.Debug("OnBackPressed", "fragment stack count :" + count);
                if (currentFragment is ThankYouFragment || currentFragment is SubmitRatingFragment)
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

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}