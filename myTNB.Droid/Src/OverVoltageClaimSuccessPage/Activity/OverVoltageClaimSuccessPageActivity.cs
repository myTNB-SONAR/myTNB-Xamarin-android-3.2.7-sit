
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

namespace myTNB_Android.Src.OverVoltageClaimSuccessPage.Activity
{
    [Activity(Label = "OverVoltageClaimSuccessPage"
          , ScreenOrientation = ScreenOrientation.Portrait
                  , WindowSoftInputMode = SoftInput.AdjustPan
          , Theme = "@style/Theme.FaultyStreetLamps")]
    public class OverVoltageClaimSuccessPageActivity : BaseAppCompatActivity
    {

        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtContentInfo)]
        TextView txtContentInfo;

        [BindView(Resource.Id.txtFeedbackIdContent)]
        TextView txtFeedbackIdContent;

        [BindView(Resource.Id.txtFeedbackIdTitle)]
        TextView txtFeedbackIdTitle;

        [BindView(Resource.Id.buttonBackToHome)]
        Button buttonBackToHome;

        [BindView(Resource.Id.btnViewSubmitted)]
        Button btnViewSubmitted;


        string SerialNumber;
        public bool AppointmentFlag = false;
        public bool EnuiryFlag = false;

        public override int ResourceId()
        {
            return Resource.Layout.OverVoltageClaimSuccessPage;
        }


        public override void OnBackPressed()
        {
            try
            {

                if (AppointmentFlag == true)
                {
                    base.OnBackPressed();
                }
                else if (EnuiryFlag == true)
                {
                    base.OnBackPressed();
                }
                else
                {
                    Intent intent = new Intent(this, typeof(DashboardHomeActivity));
                    // generalEnquiry.PutExtra(Constants.ACCOUNT_NUMBER, txtAccountNo.Text.ToString().Trim());           
                    intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(intent);
                }
              
            }
            catch (Exception ex)
            {

            }        

        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                AppointmentFlag = Convert.ToBoolean(Intent.GetStringExtra("AppointmentFlag"));
                EnuiryFlag = Convert.ToBoolean(Intent.GetStringExtra("EnuiryFlag"));
                if (AppointmentFlag == true)
                {
                    SetDataForCancleAppointment();
                }
                else if (EnuiryFlag == true)
                {
                    SetDataForCancleEnquiry();
                }
                else
                {
                    SerialNumber = Intent.GetStringExtra("SerialNumber");
                    txtFeedbackIdContent.Text = SerialNumber;
                    txtTitleInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "submitICFeedback_OverVoltageClainSuccessPageThankyouRequest");
                    txtContentInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "submitICFeedback_OverVoltageClainSuccessPageThankyouRequestContent");
                    txtFeedbackIdTitle.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "serviceNoTitle");
                    buttonBackToHome.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "backHomeButton");
                    btnViewSubmitted.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "viewSubmittedEnquiry");                  
                }
                // string SerialNumber =  Intent.GetStringExtra("SerialNumber");
                
            }
            catch (Exception ex)
            {

            }

        }

        private void SetDataForCancleEnquiry()
        {
            SerialNumber = Intent.GetStringExtra("Sernumbr");
            txtFeedbackIdContent.Text = SerialNumber;
            txtTitleInfo.Text = "Your enquiry has been cancelled.";
            txtContentInfo.Text = "Your enquiry for over voltage claim request has been successfully cancelled.";
            txtFeedbackIdTitle.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "serviceNoTitle");
            buttonBackToHome.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "backHomeButton");
            btnViewSubmitted.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "viewSubmittedEnquiry");
        }

        private void SetDataForCancleAppointment()
        {
            SerialNumber = Intent.GetStringExtra("Sernumbr");
            txtFeedbackIdContent.Text = SerialNumber;
            txtTitleInfo.Text = "Your appointment has been cancelled.";
            txtContentInfo.Text = "Your appointment for over voltage claim request has been successfully cancelled.";
            txtFeedbackIdTitle.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "serviceNoTitle");
            buttonBackToHome.Text = "Set New Appointment";
            btnViewSubmitted.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "viewSubmittedEnquiry");
        }

        private void SetUI()
        {
            //buttonBackToHome.Click += ButtonBackToHome_Click;
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
            if (AppointmentFlag == true)
            {
                OnBackPressed();
            }
            else
            {
                GetClaimID();
            }
        }

        private async void GetClaimID()
        {
            try
            {
                var claimDetailResponce = await ServiceApiImpl.Instance.SubmittedFeedbackClaimIdDetail(new SubmittedFeedbeckClaimIdDetailRequestModel(SerialNumber));

                if (claimDetailResponce.d.data != null)
                {
                    var ClaimId = claimDetailResponce.d.data.ClaimId;
                    var othersIntent = new Intent(this, typeof(OverVoltageFeedbackDetailActivity));
                    othersIntent.PutExtra("TITLE", Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimTitle"));
                    othersIntent.PutExtra("ClaimId", ClaimId);
                    StartActivity(othersIntent);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
