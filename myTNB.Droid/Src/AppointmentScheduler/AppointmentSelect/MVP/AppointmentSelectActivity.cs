
using System;
using Android.App;
using Android.Icu.Util;
using Android.OS;

using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.AppointmentScheduler.AppointmentSelect.MVP
{
    [Activity(Label = "Set anAppointment", Theme = "@style/Theme.AppointmentScheduler")]
    public class AppointmentSelectActivity : BaseActivityCustom, AppointmentSelectContract.IView
    {
        Button calenderBack;
        private TextView currentMonth;
        Button calenderNext;
        Button btnSubmitAppointment;

        [BindView(Resource.Id.btnMon)]
        Button btnMon;

        [BindView(Resource.Id.btnTue)]
        Button btnTue;

        [BindView(Resource.Id.btnWed)]
        Button btnWed;

        [BindView(Resource.Id.btnThu)]
        Button btnThu;

        [BindView(Resource.Id.btnFri)]
        Button btnFri;

        [BindView(Resource.Id.btnSat)]
        Button btnSat;

        [BindView(Resource.Id.btnSun)]
        Button btnSun;

        



        const string PAGE_ID = "ApplicationAppointment";

        private static string[] timeNames = { "9:00 AM - 1:00 PM", "2:00 PM - 6:00 PM"};

        private static int[] monthNames = { 7, 8, 9 };
        private static string[] yearhNames = { "2020"};

        private static int[] visibleNumbers = { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override int ResourceId()
        {
            return Resource.Layout.AppointmentSelectLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void UpdateUI()
        {
            TextViewUtils.SetMuseoSans500Typeface(btnMon);
            TextViewUtils.SetMuseoSans500Typeface(btnTue);
            TextViewUtils.SetMuseoSans500Typeface(btnWed);
            TextViewUtils.SetMuseoSans500Typeface(btnThu);
            TextViewUtils.SetMuseoSans500Typeface(btnFri);
            TextViewUtils.SetMuseoSans500Typeface(btnSat);
            TextViewUtils.SetMuseoSans500Typeface(btnSun);

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            calenderBack = (Button)FindViewById<Button>(Resource.Id.CalenderBack);
            calenderNext = (Button)FindViewById<Button>(Resource.Id.CalenderNext);
            currentMonth = FindViewById<TextView>(Resource.Id.current_month);
            btnSubmitAppointment = (Button)FindViewById<Button>(Resource.Id.btnSubmitAppointment);
            calenderBack.Click += OnClickCalenderBack;

            calenderNext.Click += OnClickCalenderNext;

            
            
            RelativeLayout ll = (RelativeLayout)FindViewById<RelativeLayout>(Resource.Id.CalendarLayout);
            CustomCalendar customCalendar = new CustomCalendar(this,7, "August", 2020, visibleNumbers, timeNames);
            currentMonth.Text = "August" + " " + "2020";
            ll.AddView(customCalendar);

            customCalendar.DatetimeValidate += Calendar_DatetimeValidate;


            //  TODO: ApplicationStatus Multilingual
            SetToolBarTitle("Set an Appointment");
            UpdateUI();
        }

        private void Calendar_DatetimeValidate(object sender, bool e)
        {
            if (e == true)
            {
                btnSubmitAppointment.Visibility = ViewStates.Gone;
            }
        }

        public void OnClickCalenderBack(object sender, System.EventArgs e)
        {
            RelativeLayout ll = (RelativeLayout)FindViewById<RelativeLayout>(Resource.Id.CalendarLayout);
            CustomCalendar customCalendar = new CustomCalendar(this, 6, "July", 2020, visibleNumbers, timeNames);
            currentMonth.Text = "July" + " " + "2020";
            ll.AddView(customCalendar);

        }
        public void OnClickCalenderNext(object sender, System.EventArgs e)
        {
            RelativeLayout ll = (RelativeLayout)FindViewById<RelativeLayout>(Resource.Id.CalendarLayout);
            CustomCalendar customCalendar = new CustomCalendar(this, 8, "September", 2020, visibleNumbers, timeNames);
            currentMonth.Text = "September" + " " + "2020";
            ll.AddView(customCalendar);

        }
    }
}
