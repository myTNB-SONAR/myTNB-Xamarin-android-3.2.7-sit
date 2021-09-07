
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
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.OverVoltageFeedback.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.AppointmentDetailSet.Activity
{
    [Activity(Label = "AppointmentSetActivity", ScreenOrientation = ScreenOrientation.Portrait
                  , WindowSoftInputMode = SoftInput.AdjustPan
          , Theme = "@style/Theme.FaultyStreetLamps")]
    public class AppointmentSetActivity : BaseAppCompatActivity
    {
        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;

        [BindView(Resource.Id.txtMessageInfo)]
        TextView txtMessageInfo;

        [BindView(Resource.Id.servicerequestLabel)]
        TextView servicerequestLabel;

        [BindView(Resource.Id.appointmentLabel)]
        TextView appointmentLabel;

        [BindView(Resource.Id.premiseLabel)]
        TextView premiseLabel;

        [BindView(Resource.Id.addressLabel)]
        TextView addressLabel;

        [BindView(Resource.Id.buttonBackToHome)]
        TextView buttonBackToHome;

        [BindView(Resource.Id.btnViewSubmitted)]
        TextView btnViewSubmitted;

        [BindView(Resource.Id.servicerequestnumber)]
        TextView _servicerequestnumber;
        string requestnumber;

        [BindView(Resource.Id.appointmentdate)]
        TextView _appointmentdate;
        string appointdate;

        [BindView(Resource.Id.technicianname)]
        TextView _technicianname;
        string techname;

        [BindView(Resource.Id.incidentadd)]
        TextView _incidentadd;
        string incdaddress;

        [BindView(Resource.Id.btnViewSubmitted)]
        TextView _btnViewSubmitted;

        [BindView(Resource.Id.buttonBackToHome)]
        TextView _buttonBackToHome;
        public bool isRescheduleappointment;
        public override int ResourceId()
        {
            return Resource.Layout.AppointmentsetLayout;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetUI();
            // Create your application here
        }

        private void SetUI()
        {
                 
            txtMessageInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "appointmentSetDescription");
            servicerequestLabel.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "serviceReqNum");
            appointmentLabel.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "dateTitle");
            premiseLabel.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "technicianName");
            addressLabel.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "incidentAddress");
            buttonBackToHome.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "backHomeButton");
            btnViewSubmitted.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "viewSubmitted");

            requestnumber = Intent.GetStringExtra("Sernumbr");
            appointdate = Intent.GetStringExtra("ApptDate");
            techname = Intent.GetStringExtra("TechName");
            incdaddress = Intent.GetStringExtra("IncdAdd");
            isRescheduleappointment = OverVoltageFeedbackDetailActivity.isRescheduleappointment;//Intent.GetStringExtra("isRescheduleappointment");
            if (isRescheduleappointment)
            {
                txtTitleInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "appointmentUpdate"); //"Your negotiation request has been submitted";
            }
            else
            {
                txtTitleInfo.Text = Utility.GetLocalizedLabel("SubmitEnquiry", "appointmentSet"); //"Your negotiation request has been submitted";
            }
            _servicerequestnumber.Text = requestnumber;
            _appointmentdate.Text = appointdate;
            _technicianname.Text = techname;
            _incidentadd.Text = incdaddress;
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        [OnClick(Resource.Id.btnViewSubmitted)]
        void OnToViewSubmittedEnquiry(object sender, EventArgs eventArgs)
        {
           // OverVoltageFeedbackDetailActivity.backFromAppointmentFlag = true;
            OnBackPressed();
        }

        [OnClick(Resource.Id.buttonBackToHome)]
        void OnToHome(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(this, typeof(DashboardHomeActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(intent);
        }
    }
}