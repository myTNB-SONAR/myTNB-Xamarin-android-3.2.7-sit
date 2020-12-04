using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Android.Material.AppBar;
using myTNB_Android.Src.Base.Activity;

using myTNB_Android.Src.Utils;
using System;
using System.Runtime;
using myTNB.Mobile;
using Newtonsoft.Json;
using myTNB.Mobile.API.Models.Rating.GetCustomerRatingMaster;
using AndroidX.Core.Content;
using CheeseBind;

namespace myTNB_Android.Src.ApplicationStatusRating.Activity
{
    [Activity(Label = "Rate"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.PaymentSuccessExperienceRating")]
    public class RateUsActivity : BaseActivityCustom
    {

        private AndroidX.AppCompat.Widget.Toolbar toolbar;
        private AppBarLayout appBarLayout;
        private TextView txtContentInfo;
        public RatingBar ratingBar;
        private FrameLayout frameContainer;
        private Button btnSubmit;
        private AndroidX.CoordinatorLayout.Widget.CoordinatorLayout coordinatorLayout;
        [OnClick(Resource.Id.btnSubmit)]
        void OnPayBill(object sender, EventArgs eventArgs)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] OnPayBill");
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
              /*  Intent payment_activity = new Intent(this, typeof(PaymentActivity));
                payment_activity.PutExtra("ISAPPLICATIONPAYMENT", true);
                payment_activity.PutExtra("APPLICATIONPAYMENTDETAIL", JsonConvert.SerializeObject(applicationDetailDisplay.applicationPaymentDetail));
                payment_activity.PutExtra("TOTAL", applicationDetailDisplay.PaymentDisplay.TotalPayableAmountDisplay);
                StartActivityForResult(payment_activity, PaymentActivity.SELECT_PAYMENT_ACTIVITY_CODE);
              */
                try
                {
                    FirebaseAnalyticsUtils.LogClickEvent(this, "Billing Payment Buttom Clicked");
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
            }
        }

        AndroidX.Fragment.App.Fragment  currentFragment;

        private string quesIdCategory = "1";
        private string merchantTransID;
        private string deviceID;
        private int selectedRating;
        private string PAGE_ID = "Rating";

        public override int ResourceId()
        {
            return Resource.Layout.ApplicationStauts_rating_view;
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
                txtContentInfo = FindViewById<TextView>(Resource.Id.txtContentInfo);
                coordinatorLayout = FindViewById<AndroidX.CoordinatorLayout.Widget.CoordinatorLayout>(Resource.Id.coordinatorLayout);
                ratingBar = FindViewById<RatingBar>(Resource.Id.applicationRatingBar);
                btnSubmit = FindViewById<Button>(Resource.Id.btnSubmit);
                btnSubmit.Visibility = ViewStates.Invisible;
                deviceID = DeviceIdUtils.DeviceId(this);
                GetCustomerRatingMasterResponse customerRatingMasterResponse;

               // OnLoadMainFragment();
                Bundle extras = Intent.Extras;
                if (extras != null && Intent.Extras.ContainsKey("selectedRating"))
                {
                    ratingBar.Rating = Convert.ToUInt32(Intent.Extras.GetString("selectedRating"));
                }
                ratingBar.RatingBarChange += (o, e) =>
                {
                    ratingBar.Rating = e.Rating;
                    int Rating = ((int)e.Rating);
                    if (Rating != 0)
                    {
                      
                        btnSubmit.Enabled = true;
                        btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
                    }
                    else
                    {
                        btnSubmit.Enabled = false;
                        btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
                       
                    }
                };
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

       

       

        public override void OnBackPressed()
        {
            try
            {
               /* int count = this.SupportFragmentManager.BackStackEntryCount;
                Log.Debug("OnBackPressed", "fragment stack count :" + count);
                if (currentFragment is ThankYouFragment || currentFragment is SubmitRatingFragment)
                {
                    Finish();
                }
                else
                {
                    this.SupportFragmentManager.PopBackStack();
                }
               */

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