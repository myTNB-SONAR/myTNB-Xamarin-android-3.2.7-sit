using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Utils;
using System;
using myTNB.Mobile;
using Newtonsoft.Json;
using myTNB.Mobile.API.Models.Rating.GetCustomerRatingMaster;
using System.Linq;
using Android.Runtime;

namespace myTNB.AndroidApp.Src.ApplicationStatusRating.Activity
{
    [Activity(Label = "Rate"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.PaymentSuccessExperienceRating")]
    public class ApplicationRatingActivity : BaseActivityCustom
    {
        private TextView txtContentInfo;
        public RatingBar ratingBar;
        private FrameLayout frameContainer;
        private int selectedRating;

        public GetCustomerRatingMasterResponse customerRatingMasterResponse;
        private AndroidX.CoordinatorLayout.Widget.CoordinatorLayout coordinatorLayout;
        GetApplicationStatusDisplay applicationDetailDisplay;

        AndroidX.Fragment.App.Fragment currentFragment;

        private string PAGE_ID = "Rate";

        public override int ResourceId()
        {
            return Resource.Layout.ApplicationStauts_rating_view;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }
        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
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
                ratingBar = FindViewById<RatingBar>(Resource.Id.applicationRatingBar);
                txtContentInfo = FindViewById<TextView>(Resource.Id.txtContentInfo);
                SetToolBarTitle(Utility.GetLocalizedLabel("ApplicationStatusRating", "title"));

                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

                TextViewUtils.SetMuseoSans500Typeface(txtContentInfo);
                TextViewUtils.SetTextSize16(txtContentInfo);

                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey("applicationDetailDisplay"))
                    {
                        applicationDetailDisplay = JsonConvert.DeserializeObject<GetApplicationStatusDisplay>(extras.GetString("applicationDetailDisplay"));
                    }

                    if (extras.ContainsKey("customerRatingMasterResponse"))
                    {
                        customerRatingMasterResponse = JsonConvert.DeserializeObject<GetCustomerRatingMasterResponse>(extras.GetString("customerRatingMasterResponse"));
                    }

                    var sequence = customerRatingMasterResponse.Content.QuestionAnswerSets.Where(x => x.Sequence == 1).FirstOrDefault();
                    txtContentInfo.Text = sequence != null ? sequence.QuestionDetail.QuestionDescription["0"] : string.Empty;
                }
                ratingBar.RatingBarChange += (o, e) =>
                {
                    ShowProgressDialog();
                    ratingBar.Rating = e.Rating;
                    selectedRating = ((int)e.Rating);
                    if (selectedRating != 0)
                    {
                        System.Diagnostics.Debug.WriteLine("[DEBUG] Rating");
                        Intent rateUslIntent = new Intent(this, typeof(RateUsActivity));
                        rateUslIntent.PutExtra("selectedRating", selectedRating.ToString());
                        rateUslIntent.PutExtra("customerRatingMasterResponse", JsonConvert.SerializeObject(customerRatingMasterResponse));
                        rateUslIntent.PutExtra("applicationDetailDisplay", JsonConvert.SerializeObject(applicationDetailDisplay));
                        StartActivityForResult(rateUslIntent, Constants.APPLICATION_STATUS_RATING_REQUEST_CODE);
                    }
                    HideProgressDialog();
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
                FirebaseAnalyticsUtils.SetScreenName(this, "Application Rate");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok && requestCode == Constants.APPLICATION_STATUS_RATING_REQUEST_CODE)
            {
                SetResult(Result.Ok, data);
                Finish();
            }
            else if (resultCode == Result.Canceled)
            {
                SetResult(Result.Canceled);
                Finish();
            }
        }
    }
}