using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using System;
using myTNB.Mobile;
using Newtonsoft.Json;
using myTNB.Mobile.API.Models.Rating.GetCustomerRatingMaster;
using System.Linq;
using Android.Runtime;
using CheeseBind;
using myTNB.Mobile.Constants;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Rating.Activity;
using myTNB_Android.Src.Rating.Model;
using myTNB_Android.Src.MyHome;

namespace myTNB_Android.Src.ApplicationStatusRating.Activity
{
    [Activity(Label = "Rate"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.PaymentSuccessExperienceRating")]
    public class ApplicationRatingActivity : BaseActivityCustom
    {
        [BindView(Resource.Id.ratingDescContainer)]
        LinearLayout ratingDescContainer;

        [BindView(Resource.Id.txtRatingDescription)]
        TextView txtRatingDescription;

        private TextView txtContentInfo;
        public RatingBar ratingBar;
        private FrameLayout frameContainer;
        private int selectedRating;

        public GetCustomerRatingMasterResponse customerRatingMasterResponse;
        public GetRateUsQuestionResponse _submitApplicationRateResponse;
        private AndroidX.CoordinatorLayout.Widget.CoordinatorLayout coordinatorLayout;
        GetApplicationStatusDisplay applicationDetailDisplay;

        AndroidX.Fragment.App.Fragment currentFragment;

        private string PAGE_ID = "Rate";

        private DetailCTAType _ctaType;

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

                SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
                SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

                TextViewUtils.SetMuseoSans500Typeface(txtContentInfo);
                TextViewUtils.SetMuseoSans300Typeface(txtRatingDescription);

                TextViewUtils.SetTextSize16(txtContentInfo);
                TextViewUtils.SetTextSize14(txtRatingDescription);

                txtRatingDescription.Text = Utility.GetLocalizedLabel("ApplicationStatusRating", myHome.Rating.I18N_NCSubmittedDesc);

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

                    if (extras.ContainsKey("submitApplicationRateResponse"))
                    {
                        _submitApplicationRateResponse = JsonConvert.DeserializeObject<GetRateUsQuestionResponse>(extras.GetString("submitApplicationRateResponse"));
                    }

                    if (extras.ContainsKey(MyHomeConstants.CTA_TYPE))
                    {
                        _ctaType = JsonConvert.DeserializeObject<DetailCTAType>(extras.GetString(MyHomeConstants.CTA_TYPE));
                        SetToolbarText();
                        ratingDescContainer.Visibility = _ctaType == DetailCTAType.SubmitApplicationRating ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Gone;
                    }

                    if (_ctaType == DetailCTAType.SubmitApplicationRating && _submitApplicationRateResponse!= null)
                    {
                        var question = _submitApplicationRateResponse.GetData().FirstOrDefault();
                        txtContentInfo.Text = question != null ? question.Question : string.Empty;
                    }
                    else
                    {
                        var sequence = customerRatingMasterResponse.Content.QuestionAnswerSets.Where(x => x.Sequence == 1).FirstOrDefault();
                        txtContentInfo.Text = sequence != null ? sequence.QuestionDetail.QuestionDescription["0"] : string.Empty;
                    }
                }
                ratingBar.RatingBarChange += (o, e) =>
                {
                    ShowProgressDialog();
                    ratingBar.Rating = e.Rating;
                    selectedRating = ((int)e.Rating);
                    if (selectedRating != 0)
                    {
                        if (_ctaType == DetailCTAType.SubmitApplicationRating)
                        {
                            Intent payment_activity = new Intent(this, typeof(RatingActivity));
                            payment_activity.PutExtra(Constants.QUESTION_ID_CATEGORY, ((int)QuestionCategoryID.Payment));
                            payment_activity.PutExtra(Constants.SELECTED_RATING, selectedRating);
                            payment_activity.PutExtra(Constants.MERCHANT_TRANS_ID, applicationDetailDisplay.ApplicationDetail.ReferenceNo);
                            payment_activity.PutExtra(MyHomeConstants.CTA_TYPE, JsonConvert.SerializeObject(_ctaType));
                            StartActivityForResult(payment_activity, Constants.APPLICATION_STATUS_SUBMIT_APPLICATION_RATING_REQUEST_CODE);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("[DEBUG] Rating");
                            Intent rateUslIntent = new Intent(this, typeof(RateUsActivity));
                            rateUslIntent.PutExtra("selectedRating", selectedRating.ToString());
                            rateUslIntent.PutExtra("customerRatingMasterResponse", JsonConvert.SerializeObject(customerRatingMasterResponse));
                            rateUslIntent.PutExtra("applicationDetailDisplay", JsonConvert.SerializeObject(applicationDetailDisplay));
                            StartActivityForResult(rateUslIntent, Constants.APPLICATION_STATUS_RATING_REQUEST_CODE);
                        }
                    }
                    HideProgressDialog();
                };
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void SetToolbarText()
        {
            if (_ctaType == DetailCTAType.SubmitApplicationRating)
            {
                SetToolBarTitle(Utility.GetLocalizedLabel("ApplicationStatusRating", myHome.Rating.I18N_NCSubmittedRatingTitle));
            }
            else
            {
                SetToolBarTitle(Utility.GetLocalizedLabel("ApplicationStatusRating", "title"));
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
            else if (resultCode == Result.Ok && requestCode == Constants.APPLICATION_STATUS_SUBMIT_APPLICATION_RATING_REQUEST_CODE)
            {
                SetResult(Result.Ok, data);
                Finish();
            }
        }
    }
}