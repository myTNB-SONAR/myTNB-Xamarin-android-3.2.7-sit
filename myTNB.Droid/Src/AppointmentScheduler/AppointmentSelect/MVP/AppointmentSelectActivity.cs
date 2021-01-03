
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Icu.Util;
using Android.OS;

using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB.Mobile;
using myTNB.Mobile.API.DisplayModel.Scheduler;
using myTNB.Mobile.API.Managers.Scheduler;
using myTNB.Mobile.API.Models.Scheduler.PostSetAppointment;
using myTNB_Android.Src.AppointmentScheduler.AAppointmentSetLanding.MVP;
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
        private TextView timeSlotError;
        private TextView timeSlotNote;
        private TextView appointmentLabel;
        
        Button calenderNext;
        Button btnSubmitAppointment;
        public CustomCalendar customCalendar;
        ScrollView scrollcontainer;
        private Snackbar mNoInternetSnackbar;
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
        private static List<int> visibleNumbers = new List<int>();
        private GetApplicationStatusDisplay applicationDetailDisplay;
        private SchedulerDisplay schedulerDisplayResponse;
        private string appointment;
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
         

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            calenderBack = (Button)FindViewById<Button>(Resource.Id.CalenderBack);
            calenderNext = (Button)FindViewById<Button>(Resource.Id.CalenderNext);
            currentMonth = FindViewById<TextView>(Resource.Id.current_month);
            timeSlotError = FindViewById<TextView>(Resource.Id.timeSlotError);
            timeSlotNote = FindViewById<TextView>(Resource.Id.timeSlotNote);
            btnSubmitAppointment = (Button)FindViewById<Button>(Resource.Id.btnSubmitAppointment);
            appointmentLabel = FindViewById<TextView>(Resource.Id.appointmentLabel);
            scrollcontainer = FindViewById<ScrollView>(Resource.Id.scrollcontainer);
            btnSubmitAppointment.TextSize = TextViewUtils.GetFontSize(16f);

            btnSubmitAppointment.Click += OnClickSubmitAppointment;
            calenderBack.Click += OnClickCalenderBack;
            
            calenderNext.Click += OnClickCalenderNext;

            timeSlotNote.Text = Utility.GetLocalizedLabel("ApplicationStatusScheduler", "note");
            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                applicationDetailDisplay = JsonConvert.DeserializeObject<GetApplicationStatusDisplay>(extras.GetString("applicationDetailDisplay"));
                schedulerDisplayResponse = JsonConvert.DeserializeObject<SchedulerDisplay>(extras.GetString("newAppointmentResponse"));
                appointment = extras.GetString("appointment");
                appointmentLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusScheduler", "dateSectionTitle");
            }

            if (schedulerDisplayResponse != null && schedulerDisplayResponse.ScheduleList != null)
            {


                SetKeys();

                //SelectedKeyIndex++;
            
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
        private void CalendarDateChanged(object sender, bool e)
        {
            timeSlotError.Visibility = ViewStates.Gone;
            btnSubmitAppointment.Enabled = false;
        }
        private void Calendar_DatetimeScrollValidate(object sender, bool e)
        {
            scrollcontainer.ScrollTo(1000, scrollcontainer.Bottom);
        }
        private void Calendar_DatetimeValidate(object sender, bool e)
        {
            if (e == true)
            {

                CustomCalendar timeAdapter = (CustomCalendar)sender;

                if (applicationDetailDisplay.ApplicationAppointmentDetail.TimeSlotDisplay == timeAdapter.selectedTime)
                {
                    btnSubmitAppointment.Enabled = false;
                    btnSubmitAppointment.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
                    timeSlotError.Text = Utility.GetLocalizedLabel("ApplicationStatusScheduler", "sameDateTimeError");
                    timeSlotError.Visibility = ViewStates.Visible;
                    scrollcontainer.ScrollTo(0, scrollcontainer.Bottom);
                   

                }
                else
                {
                    btnSubmitAppointment.Enabled = true;
                    btnSubmitAppointment.Background = ContextCompat.GetDrawable(this, Resource.Drawable.green_button_background);
                    timeSlotError.Visibility = ViewStates.Gone;
                }
            }
        }
        
        public void OnClickSubmitAppointment(object sender, System.EventArgs e)
        {
            SubmitAppointmentAsync();
        }
        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        public async System.Threading.Tasks.Task SubmitAppointmentAsync()
        {
           
            if (ConnectionUtils.HasInternetConnection(this))
            {
                ShowProgressDialog();
                PostSetAppointmentResponse postSetAppointmentResponse = await ScheduleManager.Instance.SetAppointment(applicationDetailDisplay.ApplicationDetail.ApplicationId
                          , applicationDetailDisplay.ApplicationTypeCode
                          , applicationDetailDisplay.SRNumber
                          , applicationDetailDisplay.SRType
                          , applicationDetailDisplay.ApplicationAppointmentDetail.BusinessArea
                          , customCalendar.selectedDate
                          , customCalendar.selectedStartTime
                          , customCalendar.selectedEndTime);
                HideProgressDialog();
                if (postSetAppointmentResponse.StatusDetail.IsSuccess)
                {
                    Intent appointmentSetLandingIntent = new Intent(this, typeof(AppointmentSetLandingActivity));
                    appointmentSetLandingIntent.PutExtra("srnumber", applicationDetailDisplay.SRNumber);
                    appointmentSetLandingIntent.PutExtra("selecteddate", customCalendar.selectedDate.ToString("dd MMM yyyy"));
                    appointmentSetLandingIntent.PutExtra("timeslot", customCalendar.selectedTime);
                    appointmentSetLandingIntent.PutExtra("appointment", appointment);
                    appointmentSetLandingIntent.PutExtra("applicationDetailDisplay", JsonConvert.SerializeObject(applicationDetailDisplay));
<<<<<<< HEAD
=======
                    appointmentSetLandingIntent.PutExtra("selectedStartTime", customCalendar.selectedStartTime.ToString());
                    appointmentSetLandingIntent.PutExtra("selectedEndTime", customCalendar.selectedEndTime.ToString());

>>>>>>> 7808bebb978734908fb4aedcc1de147d178787df
                    StartActivity(appointmentSetLandingIntent);
                    SetResult(Result.Ok, new Intent());
                    Finish();
                }
                else
                {
                    bool isTowButtons = !string.IsNullOrEmpty(postSetAppointmentResponse.StatusDetail.SecondaryCTATitle)
                        && !string.IsNullOrWhiteSpace(postSetAppointmentResponse.StatusDetail.SecondaryCTATitle);
                    MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, isTowButtons
                            ? MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON
                            : MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                        .SetTitle(postSetAppointmentResponse.StatusDetail.Title)
                        .SetMessage(postSetAppointmentResponse.StatusDetail.Message)
                        .SetCTALabel(postSetAppointmentResponse.StatusDetail.PrimaryCTATitle);

                   
                }
            }
            else
            {
                //ShowNoInternetSnackbar();
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
            visibleNumbers = new List<int>();
            if (schedulerDisplayResponse != null && schedulerDisplayResponse.MonthYearList != null && schedulerDisplayResponse.ScheduleList != null)
            {
                var selectedMonth = schedulerDisplayResponse.ScheduleList.Where(x => x.Key == selectedKey).FirstOrDefault();
                for (int i = 0; i < selectedMonth.Value.Count(); i++)
                {
                    if (selectedMonth.Value[i].IsAvailable)
                    {
                        visibleNumbers.Add(Convert.ToInt32(selectedMonth.Value[i].Day));
                    }
                }

                RelativeLayout ll = null;
                customCalendar = new CustomCalendar(this, schedulerDisplayResponse.MonthYearList[SelectedKeyIndex].Month, "", schedulerDisplayResponse.MonthYearList[SelectedKeyIndex].Year, visibleNumbers, schedulerDisplayResponse);
                ll = (RelativeLayout)FindViewById<RelativeLayout>(Resource.Id.CalendarLayout);
                ll.AddView(customCalendar);
                ll.Visibility = ViewStates.Gone;
                ll.Visibility = ViewStates.Visible;
                ll.RefreshDrawableState();
                customCalendar.DatetimeValidate += Calendar_DatetimeValidate;
                customCalendar.DatetimeScrollValidate += Calendar_DatetimeScrollValidate;
                customCalendar.DateChanged += CalendarDateChanged;
            }
        }
    }
}
