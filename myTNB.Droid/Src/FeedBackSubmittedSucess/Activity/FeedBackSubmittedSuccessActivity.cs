
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AppLaunch.Activity;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.OverVoltageFeedback;
using myTNB_Android.Src.OverVoltageFeedback.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.FeedBackSubmittedSucess.Activity
{
    [Activity(Label = "FeedBackSubmittedSuccessActivity", ScreenOrientation = ScreenOrientation.Portrait
                  , WindowSoftInputMode = SoftInput.AdjustPan
          , Theme = "@style/Theme.FaultyStreetLamps")]
    public class FeedBackSubmittedSuccessActivity : BaseAppCompatActivity
    {

        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtContentInfo)]
        TextView txtContentInfo;

        [BindView(Resource.Id.buttonBackToHome)]
        Button buttonBackToHome;

        [BindView(Resource.Id.btnViewSubmitted)]
        Button btnViewSubmitted;

        string ClaimId;
        public override int ResourceId()
        {
            return Resource.Layout.FeedBackSubmittedSuccessView;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetUI();
            // Create your application here
        }
        private void SetUI()
        {
            TextViewUtils.SetMuseoSans300Typeface(txtContentInfo);
            TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo, buttonBackToHome, btnViewSubmitted);
            TextViewUtils.SetTextSize12(txtContentInfo);
            TextViewUtils.SetTextSize16(txtTitleInfo, btnViewSubmitted, buttonBackToHome);

            txtTitleInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "thankYouForYourSubmission");
            txtContentInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "yourRatingWillHelpUsServeYouBetter");
            buttonBackToHome.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "backHomeButton");
            btnViewSubmitted.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "viewSubmitted");
            ClaimId = Intent.GetStringExtra("ClaimId");
        }

        [OnClick(Resource.Id.buttonBackToHome)]
        void OnToHome(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(this, typeof(DashboardHomeActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(intent);
        }

        [OnClick(Resource.Id.btnViewSubmitted)]
        void GoToClimDetail(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(this, typeof(OverVoltageFeedbackDetailActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            intent.PutExtra("IsfromFeedBackSubmittedSucces", "True");
            intent.PutExtra("ClaimId", ClaimId);
            intent.PutExtra("TITLE", Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimTitle"));
            StartActivity(intent);
        }
    }
}
