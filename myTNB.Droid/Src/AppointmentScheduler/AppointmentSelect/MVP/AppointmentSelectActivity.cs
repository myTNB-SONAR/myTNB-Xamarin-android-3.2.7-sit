
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
            
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            calenderBack = (Button)FindViewById<Button>(Resource.Id.CalenderBack);
            calenderNext = (Button)FindViewById<Button>(Resource.Id.CalenderNext);
            currentMonth = FindViewById<TextView>(Resource.Id.current_month);

            calenderBack.Click += OnClickCalenderBack;

            calenderNext.Click += OnClickCalenderNext;

            
            
            RelativeLayout ll = (RelativeLayout)FindViewById<RelativeLayout>(Resource.Id.CalendarLayout);
            CustomCalendar customCalendar = new CustomCalendar(this,7, "August", 2020, visibleNumbers, timeNames);
            currentMonth.Text = "August" + " " + "2020";
            ll.AddView(customCalendar);
            if (CustomCalendar.isValidDateTime)
            {

            }

            // ApplicationStatus TODO: Multilingual
            SetToolBarTitle("Set an Appointment");
            UpdateUI();
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
//Doneeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee