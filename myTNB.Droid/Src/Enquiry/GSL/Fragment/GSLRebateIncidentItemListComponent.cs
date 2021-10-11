using System;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Widget;
using Google.Android.Material.TextField;
using Java.Util;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Enquiry.GSL.MVP;
using myTNB_Android.Src.Utils;
using Calendar = Java.Util.Calendar;

namespace myTNB_Android.Src.Enquiry.GSL.Fragment
{
    public class GSLRebateIncidentItemListComponent : LinearLayout, DatePickerDialog.IOnDateSetListener, TimePickerDialog.IOnTimeSetListener
    {
        TextView gslIncidentItemTitle;
        TextView incidentTitle;
        TextInputLayout incidentDateLayout, incidentTimeLayout, restorationDateLayout, restorationTimeLayout;
        EditText txtIncidentDate, txtIncidentTime, txtRestorationDate, txtRestorationTime;

        private readonly BaseAppCompatActivity mActivity;
        private readonly Context mContext;
        private GSLIncidentDateTimePicker activePicker;
        private Action<GSLIncidentDateTimePicker, DateTime> selectedDateTime;

        private DateTime incidentDate;
        private DateTime restorationDate;

        public GSLRebateIncidentItemListComponent(Context context, BaseAppCompatActivity activity) : base(context)
        {
            mContext = context;
            mActivity = activity;
            Init(mContext);
        }

        public GSLRebateIncidentItemListComponent(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mContext = context;
            Init(mContext);
        }

        public GSLRebateIncidentItemListComponent(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            mContext = context;
            Init(mContext);
        }

        public void Init(Context context)
        {
            Inflate(context, Resource.Layout.GSLRebateIncidentItemListView, this);
            gslIncidentItemTitle = FindViewById<TextView>(Resource.Id.gslIncidentItemTitle);
            incidentTitle = FindViewById<TextView>(Resource.Id.incidentTitle);

            incidentDateLayout = FindViewById<TextInputLayout>(Resource.Id.incidentDateLayout);
            incidentTimeLayout = FindViewById<TextInputLayout>(Resource.Id.incidentTimeLayout);
            restorationDateLayout = FindViewById<TextInputLayout>(Resource.Id.restorationDateLayout);
            restorationTimeLayout = FindViewById<TextInputLayout>(Resource.Id.restorationTimeLayout);

            txtIncidentDate = FindViewById<EditText>(Resource.Id.txtIncidentDate);
            txtIncidentTime = FindViewById<EditText>(Resource.Id.txtIncidentTime);
            txtRestorationDate = FindViewById<EditText>(Resource.Id.txtRestorationDate);
            txtRestorationTime = FindViewById<EditText>(Resource.Id.txtRestorationTime);

            SetUpViews();
        }

        private void SetUpViews()
        {
            TextViewUtils.SetMuseoSans500Typeface(gslIncidentItemTitle);
            TextViewUtils.SetTextSize16(gslIncidentItemTitle);

            TextViewUtils.SetMuseoSans300Typeface(incidentTitle);
            TextViewUtils.SetTextSize12(incidentTitle);

            incidentDateLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
            incidentDateLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
            incidentDateLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.INCIDENT_DATE_HINT);

            txtIncidentDate.Focusable = false;
            txtIncidentTime.Focusable = false;
            txtRestorationDate.Focusable = false;

            txtIncidentDate.Click += delegate
            {
                OnClickDateInput(GSLIncidentDateTimePicker.INCIDENT_DATE);
            };

            txtIncidentTime.Click += delegate
            {
                OnClickTimeInput(GSLIncidentDateTimePicker.INCIDENT_TIME);
            };

            txtRestorationDate.Click += delegate
            {
                OnClickDateInput(GSLIncidentDateTimePicker.RESTORATION_DATE);
            };
        }

        private void OnClickDateInput(GSLIncidentDateTimePicker picker)
        {
            activePicker = picker;
            ShowDatePicker();
        }

        private void OnClickTimeInput(GSLIncidentDateTimePicker picker)
        {
            activePicker = picker;
            ShowTimePicker();
        }

        private void ShowDatePicker()
        {
            try
            {
                Calendar calendar = Calendar.GetInstance(Locale.Default);
                var dateTimeNow = DateTime.Now;
                DatePickerDialog datePickerDialog = new DatePickerDialog(this.mActivity, this, dateTimeNow.Year, dateTimeNow.Month - 1, dateTimeNow.Day);
                datePickerDialog.DatePicker.MaxDate = calendar.TimeInMillis;
                datePickerDialog.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ShowTimePicker()
        {
            try
            {
                Calendar calendar = Calendar.GetInstance(Locale.Default);
                int hour = calendar.Get(CalendarField.HourOfDay);
                int minute = calendar.Get(CalendarField.Minute);
                bool is24HourView = true;
                TimePickerDialog timePickerDialog = new TimePickerDialog(this.mActivity, this, hour, minute, is24HourView);
                timePickerDialog.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            DateTime selectedDate = new DateTime(year, month + 1, dayOfMonth);
            CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(LanguageUtil.GetAppLanguage());
            System.Console.WriteLine("OnDateSet");
            switch (activePicker)
            {
                case GSLIncidentDateTimePicker.INCIDENT_DATE:
                    System.Console.WriteLine("INCIDENT_DATE");
                    incidentDate = selectedDate;
                    txtIncidentDate.Text = selectedDate.ToString(GSLRebateConstants.DATE_FORMAT, dateCultureInfo);
                    System.Console.WriteLine("incidentDate*** " + incidentDate);
                    break;
                case GSLIncidentDateTimePicker.INCIDENT_TIME:

                    break;
                case GSLIncidentDateTimePicker.RESTORATION_DATE:
                    restorationDate = selectedDate;
                    txtRestorationDate.Text = selectedDate.ToString(GSLRebateConstants.DATE_FORMAT, dateCultureInfo);
                    break;
                case GSLIncidentDateTimePicker.RESTORATION_TIME:

                    break;
                default:
                    break;
            }
            selectedDateTime?.Invoke(activePicker, selectedDate);
        }

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            switch (activePicker)
            {
                case GSLIncidentDateTimePicker.INCIDENT_TIME:
                    TimeSpan selectedTime = new TimeSpan(hourOfDay, minute, 0);
                    incidentDate = incidentDate.Add(selectedTime);
                    System.Console.WriteLine("incidentDate******* " + incidentDate);
                    break;
                case GSLIncidentDateTimePicker.RESTORATION_TIME:

                    break;
                default:
                    break;
            }
        }

        public void SetSelectedDateTimeAction(Action<GSLIncidentDateTimePicker, DateTime> dateTime)
        {
            selectedDateTime = dateTime;
        }
    }
}