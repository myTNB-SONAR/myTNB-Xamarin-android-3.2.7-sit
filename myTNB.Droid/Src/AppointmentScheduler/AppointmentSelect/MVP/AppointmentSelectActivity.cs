using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.Core.Widget;
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
        public DateTime selectedDateTime;
        public string selectedTime;
        public string dateSelected;
        public int  monthSelected;
        public int yearSelected;
        public DateTime selectedStartTime;
        public DateTime selectedEndTime;
        Button calenderNext;
        Button btnSubmitAppointment;
        public CustomCalendar customCalendar;
        public RelativeLayout ll = null;
        private Snackbar mNoInternetSnackbar;
        RelativeLayout rootview;

        [BindView(Resource.Id.btnMon)]
        Button btnMon;

        [BindView(Resource.Id.btnTue)]
        Button btnTue;

        [BindView(Resource.Id.btnWed)]
        Button btnWed;

        [BindView(Resource.Id.btnThu)]
        Button btnThu;

        NestedScrollView scrollcontainer;


        [BindView(Resource.Id.timeSlotNoteContainer)]
        LinearLayout timeSlotNoteContainer;

        [BindView(Resource.Id.timeSlotErrorContainer)]
        LinearLayout timeSlotErrorContainer;

        const string PAGE_ID = "ApplicationAppointment";

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
            rootview = FindViewById<RelativeLayout>(Resource.Id.rootView);
            currentMonth.TextSize = TextViewUtils.GetFontSize(16f);
            scrollcontainer = FindViewById<NestedScrollView>(Resource.Id.schedulerNestedScrollView);
            appointmentLabel = FindViewById<TextView>(Resource.Id.appointmentLabel);
            btnSubmitAppointment.Text = Utility.GetLocalizedLabel("ApplicationStatusScheduler", "confirm");
            btnSubmitAppointment.Enabled = false;
            btnSubmitAppointment.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
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
                currentMonth.Text = ScheduleKeys[SelectedKeyIndex];
                GetVisibleNumbers(ScheduleKeys[SelectedKeyIndex], SelectedKeyIndex);
                SetToolBarTitle(Utility.GetLocalizedLabel("ApplicationStatusScheduler", "title"));
                UpdateUI();

                if (schedulerDisplayResponse.ScheduleList.Count == 1)
                {
                    calenderBack.Visibility = ViewStates.Gone;
                    calenderNext.Visibility = ViewStates.Gone;
                }
                else
                {
                    calenderNext.Visibility = ViewStates.Visible;
                }
            }
            SetFonts();

            timeSlotNoteContainer.Visibility = ViewStates.Gone;
            timeSlotErrorContainer.Visibility = ViewStates.Gone;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok && requestCode == Constants.APPLICATION_STATUS_DETAILS_SCHEDULER_REQUEST_CODE)
            {
                SetResult(Result.Ok);
                Finish();
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
            timeSlotNoteContainer.Visibility = ViewStates.Visible;
            timeSlotErrorContainer.Visibility = ViewStates.Gone;
            btnSubmitAppointment.Enabled = false;
            btnSubmitAppointment.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
        }

        private void Calendar_DatetimeScrollValidate(object sender, bool e)
        {
           // scrollcontainer.FullScroll(scrollcontainer.Bottom);
        }

        private void Calendar_DatetimeValidate(object sender, bool e)
        {
            if (e == true)
            {
                CustomCalendar timeAdapter = (CustomCalendar)sender;
                selectedDateTime = timeAdapter.selectedDateTime;
                selectedTime = timeAdapter.selectedTime;
                dateSelected = timeAdapter.selectedDate;
                monthSelected = timeAdapter.selectedMonth; 
                yearSelected = timeAdapter.selectedYear;
                selectedStartTime = timeAdapter.selectedStartTime;
                selectedEndTime = timeAdapter.selectedEndTime;
               
                if (Convert.ToDateTime(applicationDetailDisplay.ApplicationAppointmentDetail.AppointmentDate).Day == timeAdapter.selectedDateTime.Day
                    && Convert.ToDateTime(applicationDetailDisplay.ApplicationAppointmentDetail.AppointmentDate).Month == timeAdapter.selectedDateTime.Month
                    && Convert.ToDateTime(applicationDetailDisplay.ApplicationAppointmentDetail.AppointmentDate).Year == timeAdapter.selectedDateTime.Year
                    && applicationDetailDisplay.ApplicationAppointmentDetail.TimeSlotDisplay == timeAdapter.selectedTime)
                {
                    btnSubmitAppointment.Enabled = false;
                    btnSubmitAppointment.Background = ContextCompat.GetDrawable(this, Resource.Drawable.silver_chalice_button_background);
                    timeSlotError.Text = Utility.GetLocalizedLabel("ApplicationStatusScheduler", "sameDateTimeError");
                    timeSlotErrorContainer.Visibility = ViewStates.Visible;
                    timeSlotError.Visibility = ViewStates.Visible;
                   
                    scrollcontainer.Post(() =>
                    {
                        scrollcontainer.FullScroll(Convert.ToInt32(FocusSearchDirection.Down));
                    });
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
                    , selectedDateTime
                    , selectedStartTime
                    , selectedEndTime);
                HideProgressDialog();
                if (postSetAppointmentResponse.StatusDetail.IsSuccess)
                {
                    Intent intent = new Intent(this, typeof(AppointmentSetLandingActivity));
                    intent.PutExtra("srnumber", applicationDetailDisplay.SRNumber);
                    intent.PutExtra("selecteddate", customCalendar.selectedDateTime.ToString("dd MMM yyyy"));
                    intent.PutExtra("timeslot", customCalendar.selectedTime);
                    intent.PutExtra("appointment", appointment);
                    intent.PutExtra("applicationDetailDisplay", JsonConvert.SerializeObject(applicationDetailDisplay));
                    intent.PutExtra("selectedStartTime", customCalendar.selectedStartTime.ToString());
                    intent.PutExtra("selectedEndTime", customCalendar.selectedEndTime.ToString());
                    StartActivityForResult(intent, Constants.APPLICATION_STATUS_DETAILS_SCHEDULER_REQUEST_CODE);
                }
                else
                {
                    //Todo: @Raja to Fix
                    bool isTowButtons = !string.IsNullOrEmpty(postSetAppointmentResponse.StatusDetail.SecondaryCTATitle)
                        && !string.IsNullOrWhiteSpace(postSetAppointmentResponse.StatusDetail.SecondaryCTATitle);
                    MyTNBAppToolTipBuilder setAppointment = MyTNBAppToolTipBuilder.Create(this, isTowButtons
                            ? MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON
                            : MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                        .SetTitle(postSetAppointmentResponse.StatusDetail.Title)
                        .SetMessage(postSetAppointmentResponse.StatusDetail.Message)
                        .SetCTALabel(postSetAppointmentResponse.StatusDetail.PrimaryCTATitle);
                }
            }
            else
            {
                ShowNoInternetSnackbar();
            }
        }
        public void ShowNoInternetSnackbar()
        {
            if (mNoInternetSnackbar != null && mNoInternetSnackbar.IsShown)
            {
                mNoInternetSnackbar.Dismiss();
            }

            mNoInternetSnackbar = Snackbar.Make(rootview, Utility.GetLocalizedErrorLabel("noDataConnectionMessage"), Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate
            {
                mNoInternetSnackbar.Dismiss();
            }
            );
            View v = mNoInternetSnackbar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);
            mNoInternetSnackbar.Show();
            this.SetIsClicked(false);
        }
        //Todo: @Raja to do logic of hide unhide button
        public void OnClickCalenderBack(object sender, System.EventArgs e)
        {
            if (SelectedKeyIndex > 0)
            {
                SelectedKeyIndex--;
               calenderNext.Visibility = SelectedKeyIndex  < ScheduleKeys.Count ? ViewStates.Visible : ViewStates.Gone;
                calenderBack.Visibility = ViewStates.Gone;
            }
            currentMonth.Text = ScheduleKeys[SelectedKeyIndex];
            GetVisibleNumbers(ScheduleKeys[SelectedKeyIndex], SelectedKeyIndex);
        }

        //Todo: @Raja to do logic of hide unhide button
        public void OnClickCalenderNext(object sender, System.EventArgs e)
        {
          
            if (SelectedKeyIndex + 1 < ScheduleKeys.Count)
            {
                SelectedKeyIndex++;
                currentMonth.Text = ScheduleKeys[SelectedKeyIndex];
                calenderNext.Visibility = SelectedKeyIndex+1 == ScheduleKeys.Count ? ViewStates.Gone : ViewStates.Visible;
                calenderBack.Visibility = SelectedKeyIndex-1 < ScheduleKeys.Count ? ViewStates.Visible : ViewStates.Gone;
                GetVisibleNumbers(ScheduleKeys[SelectedKeyIndex], SelectedKeyIndex);
            }
            else
                {
                    calenderNext.Visibility = ViewStates.Gone;
                }
    
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
                if (ll != null && customCalendar != null)
                {
                    ll.RemoveView(customCalendar);
                }
                  customCalendar = new CustomCalendar(this, schedulerDisplayResponse.MonthYearList[SelectedKeyIndex].Month - 1, selectedKey, "", schedulerDisplayResponse.MonthYearList[SelectedKeyIndex].Year, visibleNumbers, schedulerDisplayResponse,selectedDateTime, dateSelected, monthSelected, yearSelected, selectedTime, selectedStartTime, selectedEndTime);
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

        private void SetFonts()
        {
            timeSlotNote.TextSize = TextViewUtils.GetFontSize(12f);
            timeSlotError.TextSize = TextViewUtils.GetFontSize(12f);
            btnSubmitAppointment.TextSize = TextViewUtils.GetFontSize(16f);
            currentMonth.TextSize = TextViewUtils.GetFontSize(16f);
            appointmentLabel.TextSize = TextViewUtils.GetFontSize(16f);
            TextViewUtils.SetMuseoSans500Typeface(btnSubmitAppointment, currentMonth, appointmentLabel, timeSlotError);
            TextViewUtils.SetMuseoSans300Typeface(timeSlotNote);
        }
    }
}