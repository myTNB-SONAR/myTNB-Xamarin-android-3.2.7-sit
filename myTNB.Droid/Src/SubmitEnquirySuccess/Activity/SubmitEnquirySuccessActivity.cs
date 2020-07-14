using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FeedbackDetails.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.SubmittedNewEnquiry.Activity;
using myTNB_Android.Src.SubmittedNewEnquiry.MVP;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        private SubmittedFeedback submittedFeedback;
        private List<SubmittedFeedback> submittedFeedbackList;
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
                }

                  TextViewUtils.SetMuseoSans300Typeface(txtContentInfo, txtFeedbackIdTitle, txtFeedbackIdContent);
                  TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo, buttonBackToHome, btnViewSubmitted);

                mSharedPref = PreferenceManager.GetDefaultSharedPreferences(this);


                txtFeedbackIdContent.Text = feedbackId;
                SetStaticLabels();

                string dateTime = "NA";

                //if (!string.IsNullOrEmpty(date))
                //{
                //    try
                //    {
                //        dateTime = date;
                //        DateTime dateTimeParse = DateTime.ParseExact(dateTime, "dd'/'MM'/'yyyy HH:mm:ss",
                //                CultureInfo.InvariantCulture, DateTimeStyles.None);
                //        if (LanguageUtil.GetAppLanguage().ToUpper() == "MS")
                //        {
                //            CultureInfo currCult = CultureInfo.CreateSpecificCulture("ms-MY");
                //            dateTime = dateTimeParse.ToString("dd MMM yyyy, h:mm tt", currCult);
                //        }
                //        else
                //        {
                //            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                //            dateTime = dateTimeParse.ToString("dd MMM yyyy, h:mm tt", currCult);
                //        }
                //    }
                //    catch (System.Exception e)
                //    {
                //        dateTime = "NA";
                //        Utility.LoggingNonFatalError(e);
                //    }
                //}

                //txtTransactionScheduleContent.Text = dateTime;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

          

        }

        private void SetStaticLabels()
        {
            //txtTitleInfo.Text = Utility.GetLocalizedLabel("Status", "feedbackSuccessTitle");
            //txtContentInfo.Text = Utility.GetLocalizedLabel("Status", "feedbackSuccessMessage");
            //txtFeedbackIdTitle.Text = Utility.GetLocalizedLabel("Status", "feedbackReferenceTitle");
            //buttonBackToHome.Text = Utility.GetLocalizedLabel("Status", "backToFeedback");
            //btnViewSubmitted.Text = Utility.GetLocalizedLabel("Status", "backToFeedback");
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
                var successIntent = new Intent(this, typeof(FeedbackDetailsBillRelatedActivity));
                successIntent.PutExtra("TITLE", submittedFeedbackDetails.FeedbackTypeName);
                successIntent.PutExtra("NEWSCREEN", "true");
                StartActivityForResult(successIntent, Constants.REQUEST_FEEDBACK_SUCCESS_VIEW);
            }
        }


        public async Task<SubmittedFeedbackDetails> FeedbackSaveSharedPreference(string FeedbackId)
        {
            var detailsResponse = await ServiceApiImpl.Instance.SubmittedFeedbackDetails(new SubmittedFeedbackDetailsRequest(FeedbackId));
            UserSessions.SaveSelectedFeedback(mSharedPref, JsonConvert.SerializeObject(detailsResponse.GetData()));
            return  detailsResponse.GetData();
        }




      //public async  Task<List<SubmittedFeedback>> FeedbackList()
      //  {
      //      List<SubmittedFeedback> submittedFeedbackList = new List<SubmittedFeedback>();

      //      try
      //      { 
      //          var submittedFeedbackResponse = await ServiceApiImpl.Instance.SubmittedFeedbackList(new SubmittedFeedbackListRequest());
              

      //          if (submittedFeedbackResponse.IsSuccessResponse())
      //          {
                 
      //              foreach (SubmittedFeedbackListResponse.ResponseData responseData in submittedFeedbackResponse.GetData())
      //              {
      //                  SubmittedFeedback sf = new SubmittedFeedback();
      //                  sf.FeedbackId = responseData.ServiceReqNo;
      //                  sf.FeedbackCategoryId = responseData.FeedbackCategoryId;
      //                  sf.DateCreated = responseData.DateCreated;
      //                  sf.FeedbackMessage = responseData.FeedbackMessage;
      //                  sf.FeedbackCategoryName = responseData.FeedbackCategoryName;
      //                  sf.FeedbackNameInListView = responseData.FeedbackNameInListView;
      //                  submittedFeedbackList.Add(sf);
      //                  SubmittedFeedbackEntity.InsertOrReplace(sf);
      //              }

                 
      //            //  return submittedFeedbackList;

      //          }
      //          else
      //          {
      //            //  return null;
      //          }

      //          return submittedFeedbackList;
      //      }
      //      catch (System.OperationCanceledException e)
      //      {

      //          HideProgressDialog();
      //          // ADD OPERATION CANCELLED HERE
      //          Utility.LoggingNonFatalError(e);
      //          return submittedFeedbackList;
      //      }
      //      catch (ApiException apiException)
      //      {

      //          HideProgressDialog();

      //          // ADD HTTP CONNECTION EXCEPTION HERE
      //          ShowRetryOptionsApiException(apiException, "");
      //          Utility.LoggingNonFatalError(apiException);
      //          return submittedFeedbackList;
      //      }
      //      catch (Exception e)
      //      {

      //          HideProgressDialog();
      //          // ADD UNKNOWN EXCEPTION HERE
      //          ShowRetryOptionsUnknownException(e, "");
      //          Utility.LoggingNonFatalError(e);
      //          return submittedFeedbackList;
      //      }




      //  }


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
                Intent DashboardIntent = new Intent(this, typeof(DashboardHomeActivity));
                //MyTNBAccountManagement.GetInstance().RemoveCustomerBillingDetails();
                HomeMenuUtils.ResetAll();
                DashboardIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(DashboardIntent);
                //Finish();
            }
        }
    }
}