
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

namespace myTNB_Android.Src.PaymentInfoSunmittedSuccess.Activity
{
    [Activity(Label = "PaymentInfoSunmittedSuccessActivity", ScreenOrientation = ScreenOrientation.Portrait
                  , WindowSoftInputMode = SoftInput.AdjustPan
          , Theme = "@style/Theme.FaultyStreetLamps")]
    public class PaymentInfoSunmittedSuccessActivity : BaseAppCompatActivity
    {
        System.Globalization.CultureInfo info = System.Globalization.CultureInfo.GetCultureInfo("en - IN");
        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtContentInfo)]
        TextView txtContentInfo;

        [BindView(Resource.Id.txtFeedbackIdContent)]
        TextView txtFeedbackIdContent;

        [BindView(Resource.Id.txtFeedbackIdTitle)]
        TextView txtFeedbackIdTitle;

        [BindView(Resource.Id.txttotalclaimamt)]
        TextView txttotalclaimamt;

        [BindView(Resource.Id.totalclaimamt)]
        TextView totalclaimamt;

        [BindView(Resource.Id.buttonBackToHome)]
        Button buttonBackToHome;

        [BindView(Resource.Id.btnViewSubmitted)]
        Button btnViewSubmitted;

        string Sernumbr, TotalAmt, ClaimId;
        public override int ResourceId()
        {
            return Resource.Layout.PaymentInfoSunmittedSuccessView;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetUI();
            // Create your application here
        }
        private void SetUI()
        {
            Sernumbr = Intent.GetStringExtra("Sernumbr");
            TotalAmt = Intent.GetStringExtra("TotalAmt");
            ClaimId = Intent.GetStringExtra("ClaimId");

            txtFeedbackIdContent.Text = Sernumbr;
            //txtTitleInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "submitICFeedback_OverVoltageClainSuccessPageThankyouRequest");
            //txtContentInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "submitICFeedback_OverVoltageClainSuccessPageThankyouRequestContent");
            totalclaimamt.Text = Convert.ToDouble(TotalAmt).ToString("N2", info);//TotalAmt;
            txtFeedbackIdTitle.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "serviceNoTitle");
            buttonBackToHome.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "backHomeButton");
            btnViewSubmitted.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "viewSubmittedEnquiry");

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
            intent.PutExtra("IsfromPaymentInfoSubmittedSucces", "True");
            intent.PutExtra("ClaimId", ClaimId);
            intent.PutExtra("TITLE", "Overvoltage Claim");
            StartActivity(intent);
            //OnBackPressed();
        }
    }
}