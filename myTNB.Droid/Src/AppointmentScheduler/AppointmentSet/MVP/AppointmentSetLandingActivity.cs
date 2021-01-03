
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;


using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AppointmentScheduler.AppointmentSetLanding.MVP;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.RewardDetail.MVP;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.WhatsNewDetail.MVP;

namespace myTNB_Android.Src.AppointmentScheduler.AAppointmentSetLanding.MVP
{
    [Activity(Label = "Application Set", Theme = "@style/Theme.AppointmentScheduler")]
    public class AppointmentSetLandingActivity : BaseAppCompatActivity, AppointmentSetLandingContract.IView
    {
        AppointmentSetLandingPresenter mPresenter;
        [BindView(Resource.Id.txtTitleInfo)]
        TextView txtTitleInfo;
        [BindView(Resource.Id.txtMessageInfo)]
        TextView txtMessageInfo;
        [BindView(Resource.Id.appointmentLabel)]
        TextView appointmentLabel;
        [BindView(Resource.Id.appointmentTimeLabel)]
        TextView appointmentTimeLabel;
        [BindView(Resource.Id.appointmentValue)]
        TextView appointmentValue;
        [BindView(Resource.Id.appointmentTimeValue)]
        TextView appointmentTimeValue;
        [BindView(Resource.Id.premiseLabel)]
        TextView premiseLabel;
        [BindView(Resource.Id.premiseaddresstext)]
        TextView premiseaddresstext;
        [BindView(Resource.Id.servicerequest)]
        TextView servicerequest;
        [BindView(Resource.Id.servicerequestLabel)]
        TextView servicerequestLabel;
        [BindView(Resource.Id.btnTrackApplication)]
        Button btnTrackApplication;
        [BindView(Resource.Id.btnAddtoCalendar)]
        Button btnAddtoCalendar;

        
        string srnumber;
        string selecteddate;
        string timeslot;
        string appointment;
        public override int ResourceId()
        {
            return Resource.Layout.AppointmentSetLandingLayout;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mPresenter = new AppointmentSetLandingPresenter(this);
            txtTitleInfo.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "title");

            appointmentTimeLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "timeTitle").ToUpper();
            appointmentLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "dateTitle").ToUpper();
            premiseLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "addressTitle").ToUpper();
            servicerequestLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "srTitle");
            btnTrackApplication.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "viewDetails").ToUpper();
            btnAddtoCalendar.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "addToCalendar");

            TextViewUtils.SetMuseoSans300Typeface(txtMessageInfo, txtTitleInfo, servicerequestLabel, servicerequest, appointmentLabel, appointmentTimeLabel, appointmentValue, appointmentTimeValue, premiseLabel, premiseaddresstext);
            TextViewUtils.SetMuseoSans500Typeface(btnTrackApplication, btnAddtoCalendar);

            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                srnumber = extras.GetString("srnumber");
                selecteddate = extras.GetString("selecteddate");
                timeslot = extras.GetString("timeslot");
                appointment = extras.GetString("appointment");
                appointmentValue.Text = selecteddate;
                appointmentTimeValue.Text = timeslot;
                servicerequest.Text = srnumber;
                if (appointment == "Reschedule")
                {
                    txtMessageInfo.Text = string.Format(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "rescheduleDetails"),selecteddate);
                   
                }
                else
                {
                    txtMessageInfo.Text = string.Format(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "appointmentDetails"),selecteddate);
                }

            }
        }

        public void UpdateUI()
        {
            throw new NotImplementedException();
        }
    }
}