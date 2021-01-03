using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AppointmentScheduler.AppointmentSetLanding.MVP;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using myTNB.Mobile;
using Newtonsoft.Json;
using Java.Util;
using Android.Util;
using Android.Provider;
using Android.Database;

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

        private string srnumber;
        private string selecteddate;
        private string timeslot;
        private string appointment;
        private GetApplicationStatusDisplay applicationDetailDisplay;
        private DateTime startTime;
        private DateTime endTime;

        private const string PAGE_ID = "AppointmentSuccess";

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

            TextViewUtils.SetMuseoSans300Typeface(txtMessageInfo, txtTitleInfo, servicerequestLabel
                , servicerequest, appointmentLabel, appointmentTimeLabel, appointmentValue
                , appointmentTimeValue, premiseLabel, premiseaddresstext);
            TextViewUtils.SetMuseoSans500Typeface(btnTrackApplication, btnAddtoCalendar);

            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                srnumber = extras.GetString("srnumber");
                selecteddate = extras.GetString("selecteddate");
                timeslot = extras.GetString("timeslot");
                appointment = extras.GetString("appointment");
                applicationDetailDisplay = JsonConvert.DeserializeObject<GetApplicationStatusDisplay>(extras.GetString("applicationDetailDisplay"));
                startTime = DateTime.Parse(extras.GetString("selectedStartTime"));
                endTime = DateTime.Parse(extras.GetString("selectedEndTime"));

                appointmentValue.Text = selecteddate;
                appointmentTimeValue.Text = timeslot;
                servicerequest.Text = srnumber;
                if (appointment == "Reschedule")
                {
                    txtMessageInfo.Text = string.Format(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "rescheduleDetails"), selecteddate);
                }
                else
                {
                    txtMessageInfo.Text = string.Format(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "appointmentDetails"), selecteddate);
                }
            }
        }

        public override bool CalendarPemissionRequired()
        {
            return true;
        }

        public void UpdateUI()
        {
            throw new NotImplementedException();
        }

        //Mark: Events
        [OnClick(Resource.Id.btnAddtoCalendar)]
        [Obsolete]
        internal void OnAddToCalendar(object sender, EventArgs args)
        {
            try
            {
                Android.Net.Uri eventsUri = CalendarContract.Events.ContentUri;
                var calendarID = 1;
                string[] eventsProjection = {
                    CalendarContract.Events.InterfaceConsts.Id,
                    CalendarContract.Events.InterfaceConsts.Title,
                    CalendarContract.Events.InterfaceConsts.Dtstart,
                    CalendarContract.Events.InterfaceConsts.Dtend
                };
                CursorLoader loader = new CursorLoader(this, eventsUri, eventsProjection, string.Format("calendar_id={0}", calendarID), null, "dtstart ASC");
                ICursor cursor = (ICursor)loader.LoadInBackground();
                while (cursor.MoveToNext())
                {
                    string title = cursor.GetString(1);
                    long eventId = cursor.GetLong(cursor.GetColumnIndex("_id"));

                    if (title == string.Format(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "calendarTitle"), srnumber))
                    {
                        ContentResolver.Delete(ContentUris.WithAppendedId(eventsUri, eventId), null, null);
                    }
                }
                cursor.Close();

                // Create Event code
                ContentValues eventValues = new ContentValues();
                eventValues.Put(CalendarContract.Events.InterfaceConsts.CalendarId, calendarID);
                eventValues.Put(CalendarContract.Events.InterfaceConsts.Title
                    , string.Format(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "calendarTitle")
                    , srnumber));
                eventValues.Put(CalendarContract.Events.InterfaceConsts.Description
                    , string.Format(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "calendarNote")
                    , applicationDetailDisplay.ApplicationTypeReference
                    , selecteddate));
                eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtstart, GetDateTimeMS(startTime));
                eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtend, GetDateTimeMS(endTime));

                // GitHub issue #9 : Event start and end times need timezone support.
                // https://github.com/xamarin/monodroid-samples/issues/9
                eventValues.Put(CalendarContract.Events.InterfaceConsts.EventTimezone, "UTC");
                eventValues.Put(CalendarContract.Events.InterfaceConsts.EventEndTimezone, "UTC");

                Android.Net.Uri uri = ContentResolver.Insert(CalendarContract.Events.ContentUri, eventValues);
                MyTNBAppToolTipBuilder addToCalendarSuccess = MyTNBAppToolTipBuilder.Create(this
                    , MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                    .SetTitle(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "addToCalendarSuccessTitle"))
                    .SetMessage(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "addToCalendarSuccessMessage"))
                    .SetCTALabel(Utility.GetLocalizedCommonLabel("ok"));
                addToCalendarSuccess.Build();
                addToCalendarSuccess.Show();

            }
            catch (Exception e)
            {
                Log.Debug(PAGE_ID, e.Message);
            }
        }

        [OnClick(Resource.Id.btnTrackApplication)]
        internal void OnViewDetails(object sender, EventArgs args)
        {

        }

        private long GetDateTimeMS(DateTime datetime)
        {
            int year = datetime.Year;
            int month = datetime.Month;
            int day = datetime.Day;
            int hour = datetime.Hour;
            int min = datetime.Minute;

            Calendar caendar = Calendar.GetInstance(Java.Util.TimeZone.Default);
            caendar.Set(CalendarField.DayOfMonth, day);
            caendar.Set(CalendarField.HourOfDay, hour);
            caendar.Set(CalendarField.Minute, min);
            caendar.Set(CalendarField.Month, month - 1);
            caendar.Set(CalendarField.Year, year);
            return caendar.TimeInMillis;
        }
    }
}