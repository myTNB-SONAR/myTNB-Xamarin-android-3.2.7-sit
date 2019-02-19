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
using myTNB_Android.Src.Rating.Fargment;
using Android.Util;
using myTNB_Android.Src.Rating.Model;
using myTNB_Android.Src.Utils;

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

        public void HideToolBar()
        {
            if (appBarLayout != null)
            {
                appBarLayout.Visibility = ViewStates.Gone;
                Android.Support.Design.Widget.CoordinatorLayout.LayoutParams lp = new Android.Support.Design.Widget.CoordinatorLayout.LayoutParams(Android.Support.Design.Widget.CoordinatorLayout.LayoutParams.MatchParent, Android.Support.Design.Widget.CoordinatorLayout.LayoutParams.MatchParent);
                lp.SetMargins(0, 0, 0, 0);

                frameContainer.LayoutParameters = lp;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
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
    }
}