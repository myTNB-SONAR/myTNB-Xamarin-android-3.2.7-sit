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
using myTNB;
using myTNB.Mobile.API.Managers.Rating;
using System.Linq;
using Android.App;
namespace myTNB_Android.Src.ApplicationStatusRating.Activity
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
        private Button btnSubmit;
        public GetCustomerRatingMasterResponse getCustomerRatingMasterResponse;
        private AndroidX.CoordinatorLayout.Widget.CoordinatorLayout coordinatorLayout;
        GetApplicationStatusDisplay applicationDetailDisplay;
        [OnClick(Resource.Id.btnSubmit)]
        void OnSubmit(object sender, EventArgs eventArgs)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] Rating");

            Intent rateUslIntent = new Intent(this, typeof(RateUsActivity));
            rateUslIntent.PutExtra("selectedRating", selectedRating.ToString());
            rateUslIntent.PutExtra("customerRatingMasterResponse", JsonConvert.SerializeObject(getCustomerRatingMasterResponse));
            rateUslIntent.PutExtra("applicationDetailDisplay", JsonConvert.SerializeObject(applicationDetailDisplay));
            StartActivity(rateUslIntent);

            SetResult(Result.Ok, new Intent());
            Finish();
        }
        public async void GetCustomerRatingAsync()
        {
            try
            {
                ShowProgressDialog();
                getCustomerRatingMasterResponse = await RatingManager.Instance.GetCustomerRatingMaster();
                if (!getCustomerRatingMasterResponse.StatusDetail.IsSuccess)
                {
                    ShowApplicaitonPopupMessage(this, getCustomerRatingMasterResponse.StatusDetail);
                }
                else
                {
                    var sequence = getCustomerRatingMasterResponse.Content.QuestionAnswerSets.Where(x => x.Sequence == 1).FirstOrDefault();
                    txtContentInfo.Text = sequence.QuestionDetail.QuestionDescription["0"];
                }
                FirebaseAnalyticsUtils.LogClickEvent(this, "Rate Buttom Clicked");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
            HideProgressDialog();
        }


        public async void ShowApplicaitonPopupMessage(Android.App.Activity context, StatusDetail statusDetail)
        {
            MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(statusDetail.Title)
                .SetMessage(statusDetail.Message)
                .SetCTALabel(statusDetail.PrimaryCTATitle)
                .Build();
            whereisMyacc.Show();

        }

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
       /* public async void ShowApplicaitonPopupMessage(Activity context, StatusDetail statusDetail)
        {
            MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(statusDetail.Title)
                .SetMessage(statusDetail.Message)
                .SetCTALabel(statusDetail.PrimaryCTATitle)
                .Build();
            whereisMyacc.Show();

        }*/
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
                btnSubmit = FindViewById<Button>(Resource.Id.btnSubmit);
                btnSubmit.Enabled = false;
                btnSubmit.Text = Utility.GetLocalizedLabel("ApplicationStatusRating", "submit");
                btnSubmit.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);

                GetCustomerRatingMasterResponse customerRatingMasterResponse;
                SetToolBarTitle(Utility.GetLocalizedLabel("ApplicationStatusRating", "title"));
                GetCustomerRatingAsync();
                TextViewUtils.SetMuseoSans500Typeface(txtContentInfo);
                // OnLoadMainFragment();
                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    applicationDetailDisplay = JsonConvert.DeserializeObject<GetApplicationStatusDisplay>(extras.GetString("applicationDetailDisplay"));
                }
                ratingBar.RatingBarChange += (o, e) =>
                {
                    ratingBar.Rating = e.Rating;
                    selectedRating = ((int)e.Rating);
                    if (selectedRating != 0)
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
    }
}
