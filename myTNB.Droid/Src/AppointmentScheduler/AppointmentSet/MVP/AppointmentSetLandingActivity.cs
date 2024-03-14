using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Google.Android.Material.Snackbar;
using Java.Util;
using myTNB;
using myTNB.Mobile;
using myTNB.Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP;
using myTNB.Android.Src.AppointmentScheduler.AppointmentSetLanding.MVP;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.Utils;
using myTNB.Android.Src.Database.Model;
using Newtonsoft.Json;

namespace myTNB.Android.Src.AppointmentScheduler.AAppointmentSetLanding.MVP
{
    [Activity(Label = "Application Set", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.AppointmentScheduler")]
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
        readonly Button btnAddtoCalendar;
        [BindView(Resource.Id.rootview)]
        RelativeLayout rootview;

        private Snackbar mNoInternetSnackbar;

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

            appointmentTimeLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "timeTitle").ToUpper();
            appointmentLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "dateTitle").ToUpper();
            premiseLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "addressTitle").ToUpper();
            servicerequestLabel.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "srTitle").ToUpper();
            btnTrackApplication.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "viewDetails");
            btnAddtoCalendar.Text = Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "addToCalendar");

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

                premiseaddresstext.Text = applicationDetailDisplay.PremisesAddress;
                appointmentValue.Text = selecteddate;
                appointmentTimeValue.Text = timeslot;
                servicerequest.Text = srnumber;
                string infoKey = appointment == "Reschedule" ? "rescheduleDetails" : "appointmentDetails";

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtTitleInfo.Text = Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "title"), FromHtmlOptions.ModeLegacy).ToString();
                    txtMessageInfo.Text = Html.FromHtml(string.Format(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", infoKey)
                        , selecteddate), FromHtmlOptions.ModeLegacy).ToString();
                }
                else
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    txtMessageInfo.Text = Html.FromHtml(string.Format(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", infoKey)
                        , selecteddate)).ToString();
                    txtTitleInfo.Text = Html.FromHtml(Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "title")).ToString();
#pragma warning restore CS0618 // Type or member is obsolete
                }
            }

            SetFonts();
        }

        private async void GetApplicationStatus()
        {
            try
            {
                if (ConnectionUtils.HasInternetConnection(this))
                {
                    ShowProgressDialog();
                    ApplicationDetailDisplay response = await ApplicationStatusManager.Instance.GetApplicationDetail(string.Empty
                        , applicationDetailDisplay.ApplicationDetail.ApplicationId
                        , applicationDetailDisplay.ApplicationTypeCode
                        , UserEntity.GetActive().UserID ?? string.Empty
                        , UserEntity.GetActive().Email ?? string.Empty
                        , applicationDetailDisplay.System);

                    if (response.StatusDetail.IsSuccess)
                    {
                        Intent applicationStatusDetailIntent = new Intent(this, typeof(ApplicationStatusDetailActivity));
                        applicationStatusDetailIntent.PutExtra("applicationStatusResponse", JsonConvert.SerializeObject(response.Content));
                        StartActivity(applicationStatusDetailIntent);
                        SetResult(Result.Ok, new Intent());
                        Finish();
                    }
                    else
                    {
                        ShowApplicationPopupMessage(this, response.StatusDetail);
                    }
                    HideProgressDialog();
                }
                else
                {
                    ShowNoInternetSnackbar();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void ShowApplicationPopupMessage(Activity context, StatusDetail statusDetail)
        {
            MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(statusDetail.Title)
                .SetMessage(statusDetail.Message)
                .SetCTALabel(statusDetail.PrimaryCTATitle)
                .Build();
            whereisMyacc.Show();

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

        public override bool CalendarPemissionRequired()
        {
            return true;
        }

        public void UpdateUI()
        {
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

                ToastUtils.OnDisplayToast(this, Utility.GetLocalizedLabel("ApplicationStatusAppointmentSuccess", "addToCalendarSuccessMessage"));
            }
            catch (Exception e)
            {
                Log.Debug(PAGE_ID, e.Message);
            }
        }

        [OnClick(Resource.Id.btnTrackApplication)]
        internal void OnViewDetails(object sender, EventArgs args)
        {
            GetApplicationStatus();
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

        private void SetFonts()
        {
            TextViewUtils.SetTextSize10(servicerequestLabel, appointmentLabel, appointmentTimeLabel, premiseLabel);
            TextViewUtils.SetTextSize12(txtMessageInfo);
            TextViewUtils.SetTextSize14(servicerequest, appointmentValue, appointmentTimeValue, premiseaddresstext);
            TextViewUtils.SetTextSize16(txtTitleInfo, btnTrackApplication, btnAddtoCalendar);
            TextViewUtils.SetMuseoSans300Typeface(txtMessageInfo, txtTitleInfo, servicerequestLabel
                , servicerequest, appointmentLabel, appointmentTimeLabel, appointmentValue
                , appointmentTimeValue, premiseLabel, premiseaddresstext);
            TextViewUtils.SetMuseoSans500Typeface(btnTrackApplication, btnAddtoCalendar);
        }
    }
}