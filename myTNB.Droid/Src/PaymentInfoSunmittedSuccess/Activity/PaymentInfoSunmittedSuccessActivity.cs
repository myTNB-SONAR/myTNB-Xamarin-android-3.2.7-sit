
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
using myTNB.Android.Src.AppLaunch.Activity;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.myTNBMenu.Activity;
using myTNB.Android.Src.MyTNBService.Request;
using myTNB.Android.Src.MyTNBService.ServiceImpl;
using myTNB.Android.Src.OverVoltageFeedback;
using myTNB.Android.Src.OverVoltageFeedback.Activity;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.PaymentInfoSunmittedSuccess.Activity
{
    [Activity(Label = "PaymentInfoSunmittedSuccessActivity", ScreenOrientation = ScreenOrientation.Portrait
                  , WindowSoftInputMode = SoftInput.AdjustPan
          , Theme = "@style/Theme.FaultyStreetLamps")]
    public class PaymentInfoSunmittedSuccessActivity : BaseAppCompatActivity
    {
        //System.Globalization.CultureInfo info = System.Globalization.CultureInfo.GetCultureInfo("en - IN");
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

            TextViewUtils.SetMuseoSans300Typeface(txtContentInfo, txtFeedbackIdTitle, txtFeedbackIdContent, totalclaimamt);
            TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo, buttonBackToHome, btnViewSubmitted, txttotalclaimamt);
            TextViewUtils.SetTextSize10(txtFeedbackIdTitle);
            TextViewUtils.SetTextSize12(txtContentInfo);
            TextViewUtils.SetTextSize14(txtFeedbackIdContent);
            TextViewUtils.SetTextSize15(txttotalclaimamt);
            TextViewUtils.SetTextSize16(txtTitleInfo, btnViewSubmitted, buttonBackToHome);
            TextViewUtils.SetTextSize20(totalclaimamt);

            txtFeedbackIdContent.Text = Sernumbr;
            txtTitleInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "thankYouForProvidingYourDetails");
            txtContentInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "thankYouForProvidingYourDetailsDescription");
            txttotalclaimamt.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "totalClaimAmount");
            //txtTitleInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "submitICFeedback_OverVoltageClainSuccessPageThankyouRequest");
            //txtContentInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "submitICFeedback_OverVoltageClainSuccessPageThankyouRequestContent");
            //totalclaimamt.Text = Convert.ToDouble(TotalAmt).ToString("N2", info);//TotalAmt;
            var Amt = Convert.ToDouble(TotalAmt);
            totalclaimamt.Text = string.Format("{0:#,0.00##}", Amt);
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
            intent.PutExtra("TITLE", Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimTitle"));
            StartActivity(intent);
            //OnBackPressed();
        }
    }
}