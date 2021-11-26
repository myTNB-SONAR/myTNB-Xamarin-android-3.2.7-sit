
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
        string ClaimId;
        public bool AppointmentFlag = false;
        public bool EnuiryFlag = false;
        public bool AgreeFlag = false;
        public bool DisAgreeFlag = false;
        public static bool comeFromsubmitClaimPage = false;
        public static bool FormOverVoltageFeedbackDetailActivity = false;
        public override int ResourceId()
        {
            return Resource.Layout.OverVoltageClaimSuccessPage;
        }

        public override void OnBackPressed()
        {
            try
            {
                if (AppointmentFlag == true | EnuiryFlag == true | AgreeFlag == true | DisAgreeFlag == true)
                {
                    base.OnBackPressed();
                }                
                else
                {
                    Intent intent = new Intent(this, typeof(DashboardHomeActivity));                          
                    intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    StartActivity(intent);
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
                AppointmentFlag = Convert.ToBoolean(Intent.GetStringExtra("AppointmentFlag"));
                EnuiryFlag = Convert.ToBoolean(Intent.GetStringExtra("EnuiryFlag"));
                AgreeFlag = Convert.ToBoolean(Intent.GetStringExtra("AgreeFlag"));
                DisAgreeFlag = Convert.ToBoolean(Intent.GetStringExtra("DisAgreeFlag"));
                ClaimId = Intent.GetStringExtra("ClaimId");

                TextViewUtils.SetMuseoSans300Typeface(txtContentInfo, txtFeedbackIdTitle, txtFeedbackIdContent);
                TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo, buttonBackToHome, btnViewSubmitted);
                TextViewUtils.SetTextSize10(txtFeedbackIdTitle);
                TextViewUtils.SetTextSize12(txtContentInfo);
                TextViewUtils.SetTextSize14(txtFeedbackIdContent);
                TextViewUtils.SetTextSize16(txtTitleInfo, btnViewSubmitted, buttonBackToHome);

                if (AppointmentFlag == true)
                {
                    SetDataForCancleAppointment();
                }
                else if (EnuiryFlag == true)
                {
                    SetDataForCancleEnquiry();
                }
                else if (AgreeFlag == true)
                {
                    SetDataForAgreementSuccess();
                }
                else if (DisAgreeFlag == true)
                {
                    SetDataForNegotationScreen();
                }
                else
                {
                    SerialNumber = Intent.GetStringExtra("SerialNumber");
                    txtFeedbackIdContent.Text = SerialNumber;
                    txtTitleInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "thankYouForYourRequest");
                    txtContentInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "thankYouForYourRequestDescription");
                    txtFeedbackIdTitle.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "serviceNoTitle");
                    buttonBackToHome.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "backHomeButton");
                    btnViewSubmitted.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "viewSubmittedEnquiry");                  
                }              
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void SetDataForNegotationScreen()
        {
            SerialNumber = Intent.GetStringExtra("Sernumbr");
            txtFeedbackIdContent.Text = SerialNumber;
            txtTitleInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "thankYouForYourDisagree"); //"Your negotiation request has been submitted";
            txtContentInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "thankYouForYourDisagreeDescription"); //"We'll get back to you soon. In the meantime,stay updated by traccking your submitted enquiry status.";
            txtFeedbackIdTitle.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "serviceNoTitle");
            buttonBackToHome.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "backHomeButton");
            btnViewSubmitted.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "viewSubmittedEnquiry");
        }

        private void SetDataForAgreementSuccess()
        {
            SerialNumber = Intent.GetStringExtra("Sernumbr");
            txtFeedbackIdContent.Text = SerialNumber;
            txtTitleInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "thankYouForYourAgree"); //"Thank you for your submisison.";
            txtContentInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "thankYouForYourAgreeDescription");//"To proceed with payment, please submit the required payment information on the submitted enquiry page.";
            txtFeedbackIdTitle.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "serviceNoTitle");
            buttonBackToHome.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "backHomeButton");
            btnViewSubmitted.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "proccedToPaymentInformation"); //"Proceed to Payment Infomation";
        }

        private void SetDataForCancleEnquiry()
        {
            SerialNumber = Intent.GetStringExtra("Sernumbr");
            txtFeedbackIdContent.Text = SerialNumber;
            txtTitleInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "thankYouForYourEnquiry"); //"Your enquiry has been cancelled.";
            txtContentInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "thankYouForYourEnquiryDescription"); //"Your enquiry for over voltage claim request has been successfully cancelled.";
            txtFeedbackIdTitle.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "serviceNoTitle");
            buttonBackToHome.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "backHomeButton");
            btnViewSubmitted.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "viewSubmittedEnquiry");
        }

        private void SetDataForCancleAppointment()
        {
            SerialNumber = Intent.GetStringExtra("Sernumbr");
            txtFeedbackIdContent.Text = SerialNumber;
            txtTitleInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "thankYouForYourAppointment"); //"Your appointment has been cancelled.";
            txtContentInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "thankYouForYourAppointmentDescription"); //"Your appointment for over voltage claim request has been successfully cancelled.";
            txtFeedbackIdTitle.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "serviceNoTitle");
            buttonBackToHome.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "setAppointmentTitle");
            btnViewSubmitted.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "viewSubmittedEnquiry");
        }       
        
        [OnClick(Resource.Id.buttonBackToHome)]
        void OnToHome(object sender, EventArgs eventArgs)
        {
            if (AppointmentFlag)
            {
                Intent intent = new Intent(this, typeof(OverVoltageFeedbackDetailActivity));
                intent.PutExtra("setAppointmentFlag", "True");
                intent.PutExtra("ClaimId", ClaimId);
                intent.PutExtra("TITLE", Utility.GetLocalizedLabel("SubmitEnquiry", "setAppointmentTitle"));
                intent.PutExtra("IsfromSetAppointmentSucces", "True");                
                intent.SetFlags(ActivityFlags.ClearTop);
                StartActivity(intent);
            }
            else if(EnuiryFlag == true | DisAgreeFlag == true | comeFromsubmitClaimPage == true | AgreeFlag == true)
            {
                //Back to home
                Intent intent = new Intent(this, typeof(DashboardHomeActivity));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(intent);
            }
            else
            {
                //Back to home
                Intent intent = new Intent(this, typeof(DashboardHomeActivity));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(intent);
            }

        }

        [OnClick(Resource.Id.btnViewSubmitted)]
        void GoToClimDetail(object sender, EventArgs eventArgs)
        {
            if (EnuiryFlag == true | DisAgreeFlag == true)
            {
                base.OnBackPressed();
            }
            else if (AgreeFlag)
            {
                OverVoltageFeedbackDetailActivity.proccedToPaymentFlag=true;
                base.OnBackPressed();
            }           
            else if(comeFromsubmitClaimPage || FormOverVoltageFeedbackDetailActivity)
            {
                GetClaimID();             
            }
            else if(AppointmentFlag)
            {
                base.OnBackPressed();
            }
        }

        private async void GetClaimID()
        {
            try
            {
                var claimDetailResponce = await ServiceApiImpl.Instance.OvervoltageClaimDetail(new SubmittedFeedbeckClaimIdDetailRequestModel(SerialNumber));

                if (claimDetailResponce.d.data != null)
                {
                    var ClaimId = claimDetailResponce.d.data.ClaimId;
                    var othersIntent = new Intent(this, typeof(OverVoltageFeedbackDetailActivity));
                    othersIntent.PutExtra("TITLE", Utility.GetLocalizedLabel("SubmitEnquiry", "overVoltageClaimTitle"));
                    othersIntent.PutExtra("ClaimId", ClaimId);
                    StartActivity(othersIntent);
                    comeFromsubmitClaimPage = false;
                    FormOverVoltageFeedbackDetailActivity = false;
                }                
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
