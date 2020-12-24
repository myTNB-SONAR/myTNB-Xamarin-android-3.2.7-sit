
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Icu.Util;
using Android.OS;

using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.Mobile;
using myTNB.Mobile.API.DisplayModel.Scheduler;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

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

        private static string[] timeNames = { "9:00 AM - 1:00 PM", "2:00 PM - 6:00 PM" };
        private static int[] visibleNumbers = {  };
        private GetApplicationStatusDisplay applicationDetailDisplay;
        private SchedulerDisplay schedulerDisplayResponse;

        internal List<string> ScheduleKeys;
        internal int SelectedKeyIndex = 0;

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
           /* TextViewUtils.SetMuseoSans500Typeface(btnMon);
            TextViewUtils.SetMuseoSans500Typeface(btnTue);
            TextViewUtils.SetMuseoSans500Typeface(btnWed);
            TextViewUtils.SetMuseoSans500Typeface(btnThu);
            TextViewUtils.SetMuseoSans500Typeface(btnFri);
            TextViewUtils.SetMuseoSans500Typeface(btnSat);
            TextViewUtils.SetMuseoSans500Typeface(btnSun);*/

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
            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                applicationDetailDisplay = JsonConvert.DeserializeObject<GetApplicationStatusDisplay>(extras.GetString("applicationDetailDisplay"));
                schedulerDisplayResponse = JsonConvert.DeserializeObject<SchedulerDisplay>(extras.GetString("newAppointmentResponse"));

            }

            if (schedulerDisplayResponse != null && schedulerDisplayResponse.ScheduleList != null)
            {


                SetKeys();

                SelectedKeyIndex++;
            
            currentMonth.Text = ScheduleKeys[SelectedKeyIndex];
            GetVisibleNumbers(ScheduleKeys[SelectedKeyIndex], SelectedKeyIndex);

            


                //  TODO: ApplicationStatus Multilingual
                SetToolBarTitle("Set an Appointment");

                UpdateUI();
            }

        }
        private void SetKeys()
        {
            ScheduleKeys = schedulerDisplayResponse.ScheduleList.Keys.ToList();
            SelectedKeyIndex = 0;
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
            if (SelectedKeyIndex > 0)
            {
                SelectedKeyIndex--;
            }
            currentMonth.Text = ScheduleKeys[SelectedKeyIndex];
            GetVisibleNumbers(ScheduleKeys[SelectedKeyIndex], SelectedKeyIndex);
        }
        public void OnClickCalenderNext(object sender, System.EventArgs e)
        {
            if (SelectedKeyIndex + 1 < ScheduleKeys.Count)
            {
                SelectedKeyIndex++;
            }
            currentMonth.Text = ScheduleKeys[SelectedKeyIndex];
            GetVisibleNumbers(ScheduleKeys[SelectedKeyIndex], SelectedKeyIndex);

        }
        public void GetVisibleNumbers(string selectedKey, int SelectedKeyIndex)
        {
            if (schedulerDisplayResponse != null && schedulerDisplayResponse.MonthYearList != null && schedulerDisplayResponse.ScheduleList != null)
            {
                var selectedMonth = schedulerDisplayResponse.ScheduleList.Where(x => x.Key == selectedKey).FirstOrDefault();
                for (int i = 0; i < selectedMonth.Value.Count(); i++)
                {
                    if (selectedMonth.Value[i].IsAvailable)
                    {
                        visibleNumbers[i] = Convert.ToInt32(selectedMonth.Value[i].Day);
                    }
                }

                RelativeLayout ll = (RelativeLayout)FindViewById<RelativeLayout>(Resource.Id.CalendarLayout);
                CustomCalendar customCalendar = new CustomCalendar(this, schedulerDisplayResponse.MonthYearList[SelectedKeyIndex].Month, "", schedulerDisplayResponse.MonthYearList[SelectedKeyIndex].Year, visibleNumbers, timeNames);

                ll.AddView(customCalendar);
                customCalendar.DatetimeValidate += Calendar_DatetimeValidate;
            }
        }
    }
}
