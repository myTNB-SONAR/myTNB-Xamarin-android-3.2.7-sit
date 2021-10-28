using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.AppLaunch.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Enquiry;
using myTNB_Android.Src.Enquiry.GSL.Activity;
using myTNB_Android.Src.FeedbackDetails.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.PreLogin.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Runtime;
using System.Threading.Tasks;

namespace myTNB_Android.Src.SubmitEnquirySuccess.Activity
{
    [Activity(
    ScreenOrientation = ScreenOrientation.Portrait
  , Theme = "@style/Theme.BillRelated")]
    public class SubmitEnquirySuccessActivity : BaseAppCompatActivity
    {
        private string date;
        private string feedbackId;
        private string feedbackCategoryId;

        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtContentInfo)]
        TextView txtContentInfo;

        [BindView(Resource.Id.txtFeedbackIdTitle)]
        TextView txtFeedbackIdTitle;

        [BindView(Resource.Id.txtFeedbackIdContent)]
        TextView txtFeedbackIdContent;


        [BindView(Resource.Id.buttonBackToHome)]
        Button buttonBackToHome;


        [BindView(Resource.Id.btnViewSubmitted)]
        Button btnViewSubmitted;

        string isAboutMyBill = "false";
        private ISharedPreferences mSharedPref;
        public override int ResourceId()
        {
            return Resource.Layout.SubmitEnquirySuccessView;
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            try
            {
                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    //date = extras.GetString(Constants.RESPONSE_FEEDBACK_DATE);
                    feedbackId = extras.GetString(Constants.RESPONSE_FEEDBACK_ID);
                    isAboutMyBill = Intent.GetStringExtra("ABOUTMYBILL");
                    feedbackCategoryId = Intent.GetStringExtra(EnquiryConstants.FEEDBACK_CATEGORY_ID);
                }

                TextViewUtils.SetMuseoSans300Typeface(txtContentInfo, txtFeedbackIdTitle, txtFeedbackIdContent);
                TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo, buttonBackToHome, btnViewSubmitted);
                TextViewUtils.SetTextSize10(txtFeedbackIdTitle);
                TextViewUtils.SetTextSize12(txtContentInfo);
                TextViewUtils.SetTextSize14(txtFeedbackIdContent);
                TextViewUtils.SetTextSize16(txtTitleInfo, btnViewSubmitted, buttonBackToHome);

                mSharedPref = PreferenceManager.GetDefaultSharedPreferences(this);

                txtFeedbackIdContent.Text = feedbackId;
                SetStaticLabels();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void SetStaticLabels()
        {
            string successTitle = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, feedbackCategoryId.Equals(EnquiryConstants.GSL_FEEDBACK_CATEGORY_ID) ?
                LanguageConstants.SubmitEnquiry.GSL_SUCCESS_TITLE : LanguageConstants.SubmitEnquiry.SUCCESS_TITLE);
            string successDesc = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, feedbackCategoryId.Equals(EnquiryConstants.GSL_FEEDBACK_CATEGORY_ID) ?
                LanguageConstants.SubmitEnquiry.GSL_SUCCESS_DESC : LanguageConstants.SubmitEnquiry.SUCCESS_DESC);

            txtTitleInfo.Text = successTitle;
            txtContentInfo.Text = successDesc;
            txtFeedbackIdTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.SUCCESS_SERVICE_NO_TITLE);
            if (UserEntity.IsCurrentlyActive())
            {
                buttonBackToHome.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.SUCCESS_BACK_TO_HOME);
            }
            else
            {
                buttonBackToHome.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.SUCCESS_BACK_TO_LOGIN);
            }

            btnViewSubmitted.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.SUCCESS_VIEW_SUBMITTED_ENQUIRY);
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                SetStaticLabels();
                FirebaseAnalyticsUtils.SetScreenName(this, "Submit Enquiry Success");
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

        [OnClick(Resource.Id.btnViewSubmitted)]
        async void OnViewSubmitted(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);

                ShowProgressDialog();
                SubmittedFeedbackDetails submittedFeedbackDetails = await FeedbackSaveSharedPreference(feedbackId);

                if (feedbackCategoryId != null && feedbackCategoryId.Equals(EnquiryConstants.GSL_FEEDBACK_CATEGORY_ID))
                {
                    Intent gslDetailsIntent = new Intent(this, typeof(GSLRebateSubmittedDetailsActivity));
                    StartActivityForResult(gslDetailsIntent, Constants.REQUEST_FEEDBACK_SUCCESS_VIEW);
                }
                else
                {
                    var successIntent = new Intent(this, typeof(FeedbackDetailsBillRelatedActivity));

                    successIntent.PutExtra("NEWSCREEN", "true");
                    if (isAboutMyBill == "true")
                    {
                        successIntent.PutExtra("TITLE", Utility.GetLocalizedLabel("SubmitEnquiry", "aboutMyBillTitle"));
                        successIntent.PutExtra("ABOUTMYBILL", "true");
                    }
                    StartActivityForResult(successIntent, Constants.REQUEST_FEEDBACK_SUCCESS_VIEW);
                }
            }
        }

        public async Task<SubmittedFeedbackDetails> FeedbackSaveSharedPreference(string FeedbackId)
        {
            var detailsResponse = await ServiceApiImpl.Instance.SubmittedFeedbackWithContactDetails(new SubmittedFeedbackDetailsRequest(FeedbackId));
            UserSessions.SaveSelectedFeedback(mSharedPref, JsonConvert.SerializeObject(detailsResponse.GetData()));
            return detailsResponse.GetData();
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

        private Snackbar mApiExcecptionSnackBar;
        public void ShowRetryOptionsApiException(ApiException apiException, string message)
        {
            if (mApiExcecptionSnackBar != null && mApiExcecptionSnackBar.IsShown)
            {
                mApiExcecptionSnackBar.Dismiss();
            }

            mApiExcecptionSnackBar = Snackbar.Make(rootView, !string.IsNullOrEmpty(message) ? message : Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {
                mApiExcecptionSnackBar.Dismiss();
            }
            );
            View v = mApiExcecptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mApiExcecptionSnackBar.Show();
            this.SetIsClicked(false);
        }

        private Snackbar mUknownExceptionSnackBar;
        public void ShowRetryOptionsUnknownException(Exception exception, string message)
        {
            if (mUknownExceptionSnackBar != null && mUknownExceptionSnackBar.IsShown)
            {
                mUknownExceptionSnackBar.Dismiss();

            }

            mUknownExceptionSnackBar = Snackbar.Make(rootView, !string.IsNullOrEmpty(message) ? message : Utility.GetLocalizedErrorLabel("defaultErrorMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {

                mUknownExceptionSnackBar.Dismiss();
            }
            );
            View v = mUknownExceptionSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mUknownExceptionSnackBar.Show();
            this.SetIsClicked(false);
        }

        public override void OnBackPressed()
        {


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

        [OnClick(Resource.Id.buttonBackToHome)]
        void OnToHome(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);

                if (UserEntity.IsCurrentlyActive())
                {
                    Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
                    //MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                    HomeMenuUtils.ResetAll();
                    DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(DashboardIntent);
                    //Finish();
                }
                else
                {
                    /// copy logout style
                    LaunchViewActivity.MAKE_INITIAL_CALL = true;
                    Intent PreLoginIntent = new Intent(this, typeof(PreLoginActivity));
                    PreLoginIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(PreLoginIntent);
                }
            }
        }
    }
}