
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

namespace myTNB_Android.Src.AppointmentDetailSet.Activity
{
    [Activity(Label = "AppointmentSetActivity", ScreenOrientation = ScreenOrientation.Portrait
                  , WindowSoftInputMode = SoftInput.AdjustPan
          , Theme = "@style/Theme.FaultyStreetLamps")]
    public class AppointmentSetActivity : BaseAppCompatActivity
    {
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
            requestnumber = Intent.GetStringExtra("Sernumbr");
            appointdate = Intent.GetStringExtra("ApptDate");
            techname = Intent.GetStringExtra("TechName");
            incdaddress = Intent.GetStringExtra("IncdAdd");

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