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
        private Action<GSLIncidentDateTimePicker, DateTime, int> selectedDateTimeAction;
        private Action<GSLIncidentDateTimePicker, int> resetDateTimeAction;

        private DateTime incidentDate;
        private DateTime restorationDate;
        private int itemIndex;

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

            TextViewUtils.SetMuseoSans300Typeface(incidentDateLayout, incidentTimeLayout, restorationDateLayout, restorationTimeLayout);
            TextViewUtils.SetMuseoSans300Typeface(txtIncidentDate, txtIncidentTime, txtRestorationDate, txtRestorationTime);
            TextViewUtils.SetTextSize16(txtIncidentDate, txtIncidentTime, txtRestorationDate, txtRestorationTime);

            incidentDateLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
            incidentTimeLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
            restorationDateLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
            restorationTimeLayout.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);

            incidentDateLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
            incidentTimeLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
            restorationDateLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);
            restorationTimeLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayout_TextAppearance_Large : Resource.Style.TextInputLayout_TextAppearance_Small);

            incidentDateLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.INCIDENT_DATE_HINT).ToUpper();
            incidentTimeLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.INCIDENT_TIME_HINT).ToUpper();
            restorationDateLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.RESTORATION_DATE_HINT).ToUpper();
            restorationTimeLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.RESTORATION_TIME_HINT).ToUpper();

            gslIncidentItemTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_INCIDENT_INFO_TITLE);
            incidentTitle.Text = string.Format(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_INCIDENT_ITEM_TITLE), this.itemIndex + 1);

            txtIncidentDate.Focusable = false;
            txtIncidentTime.Focusable = false;
            txtRestorationDate.Focusable = false;
            txtRestorationTime.Focusable = false;

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

            txtRestorationTime.Click += delegate
            {
                OnClickTimeInput(GSLIncidentDateTimePicker.RESTORATION_TIME);
            };
        }

        private void OnClickDateInput(GSLIncidentDateTimePicker picker)
        {
            if (picker == GSLIncidentDateTimePicker.RESTORATION_DATE)
            {
                if (txtIncidentDate.Text.IsValid() && txtIncidentTime.Text.IsValid())
                {
                    activePicker = picker;
                    ShowDatePicker();
                }
            }
            else
            {
                activePicker = picker;
                ShowDatePicker();
            }
        }

        private void OnClickTimeInput(GSLIncidentDateTimePicker picker)
        {
            if (picker == GSLIncidentDateTimePicker.INCIDENT_TIME)
            {
                if (txtIncidentDate.Text.IsValid())
                {
                    activePicker = picker;
                    ShowTimePicker();
                }
            }
            else if (picker == GSLIncidentDateTimePicker.RESTORATION_TIME)
            {
                if (txtIncidentDate.Text.IsValid() &&
                    txtIncidentTime.Text.IsValid() &&
                    txtRestorationDate.Text.IsValid())
                {
                    activePicker = picker;
                    ShowTimePicker();
                }
            }
        }

        private void ShowDatePicker()
        {
            try
            {
                Calendar calendar = Calendar.GetInstance(Locale.Default);
                var dateTimeNow = DateTime.Now;

                DatePickerDialog datePickerDialog = new DatePickerDialog(this.mActivity, AlertDialog.ThemeHoloLight, this, dateTimeNow.Year, dateTimeNow.Month - 1, dateTimeNow.Day);

                if (activePicker == GSLIncidentDateTimePicker.RESTORATION_DATE)
                {
                    Calendar minCalendar = Calendar.GetInstance(Locale.Default);
                    minCalendar.Set(CalendarField.Year, incidentDate.Year);
                    minCalendar.Set(CalendarField.Month, incidentDate.Month - 1);
                    minCalendar.Set(CalendarField.DayOfMonth, incidentDate.Day);

                    datePickerDialog.DatePicker.MinDate = minCalendar.TimeInMillis;
                }
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
                TimePickerDialog timePickerDialog = new TimePickerDialog(this.mActivity, AlertDialog.ThemeHoloLight, this, hour, minute, false);
                timePickerDialog.Show();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetItemIndex(int index)
        {
            itemIndex = index;
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            try
            {
                DateTime selectedDate = new DateTime(year, month + 1, dayOfMonth);
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(LanguageUtil.GetAppLanguage());
                switch (activePicker)
                {
                    case GSLIncidentDateTimePicker.INCIDENT_DATE:
                        incidentDate = selectedDate;
                        txtIncidentDate.Text = incidentDate.ToString(GSLRebateConstants.DATE_FORMAT, dateCultureInfo);
                        txtIncidentTime.Text = string.Empty;
                        ResetRestorationDateTime();
                        break;
                    case GSLIncidentDateTimePicker.RESTORATION_DATE:
                        restorationDate = selectedDate;
                        txtRestorationDate.Text = restorationDate.ToString(GSLRebateConstants.DATE_FORMAT, dateCultureInfo);
                        txtRestorationTime.Text = string.Empty;
                        break;
                    default:
                        break;
                }
                selectedDateTimeAction?.Invoke(activePicker, selectedDate, itemIndex);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            try
            {
                TimeSpan selectedTime = new TimeSpan(hourOfDay, minute, 0);
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(LanguageUtil.GetAppLanguage());
                switch (activePicker)
                {
                    case GSLIncidentDateTimePicker.INCIDENT_TIME:
                        {
                            var dateTimeNow = DateTime.Now;
                            if (incidentDate.Year == dateTimeNow.Year
                                && incidentDate.Month == dateTimeNow.Month
                                && incidentDate.Day == dateTimeNow.Day)
                            {
                                TimeSpan maxTime = new TimeSpan(dateTimeNow.Hour, dateTimeNow.Minute, 0);
                                int compare = selectedTime.CompareTo(maxTime);
                                if (compare > 0)
                                {
                                    selectedTime = maxTime;
                                }
                            }
                            incidentDate = incidentDate.Date + selectedTime;
                            txtIncidentTime.Text = incidentDate.ToString(GSLRebateConstants.TIME_FORMAT, dateCultureInfo);
                            selectedDateTimeAction?.Invoke(activePicker, incidentDate, itemIndex);
                            ResetRestorationDateTime();
                        }
                        break;
                    case GSLIncidentDateTimePicker.RESTORATION_TIME:
                        {
                            var dateTimeNow = DateTime.Now;
                            if (restorationDate.Year == dateTimeNow.Year
                               && restorationDate.Month == dateTimeNow.Month
                               && restorationDate.Day == dateTimeNow.Day)
                            {
                                TimeSpan minTime = new TimeSpan(incidentDate.Hour, incidentDate.Minute, 0);
                                TimeSpan maxTime = new TimeSpan(dateTimeNow.Hour, dateTimeNow.Minute, 0);
                                int compareMin = selectedTime.CompareTo(minTime);
                                int compareMax = selectedTime.CompareTo(maxTime);
                                if (compareMin < 0)
                                {
                                    selectedTime = minTime;
                                }
                                else if (compareMax > 0)
                                {
                                    selectedTime = maxTime;
                                }
                            }
                            else
                            {
                                TimeSpan minTime = new TimeSpan(incidentDate.Hour, incidentDate.Minute, 0);
                                if (hourOfDay < minTime.Hours || (hourOfDay == minTime.Hours && minute < incidentDate.Minute))
                                {
                                    selectedTime = minTime;
                                }
                            }
                            restorationDate = restorationDate.Date + selectedTime;
                            txtRestorationTime.Text = restorationDate.ToString(GSLRebateConstants.TIME_FORMAT, dateCultureInfo);
                            selectedDateTimeAction?.Invoke(activePicker, restorationDate, itemIndex);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetSelectedDateTimeAction(Action<GSLIncidentDateTimePicker, DateTime, int> action)
        {
            selectedDateTimeAction = action;
        }

        public void SetResetDateTimeValueAction(Action<GSLIncidentDateTimePicker, int> action)
        {
            resetDateTimeAction = action;
        }

        private void ResetRestorationDateTime()
        {
            var dateTimeNow = DateTime.Now;
            if (incidentDate.Year == dateTimeNow.Year
                && incidentDate.Month == dateTimeNow.Month
                && incidentDate.Day == dateTimeNow.Day)
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(LanguageUtil.GetAppLanguage());
                restorationDate = incidentDate;
                txtRestorationDate.Text = restorationDate.ToString(GSLRebateConstants.DATE_FORMAT, dateCultureInfo);
                txtRestorationTime.Text = string.Empty;
                selectedDateTimeAction?.Invoke(activePicker, restorationDate, itemIndex);
            }
            else
            {
                restorationDate = new DateTime();
                txtRestorationDate.Text = string.Empty;
                txtRestorationTime.Text = string.Empty;
                resetDateTimeAction?.Invoke(GSLIncidentDateTimePicker.RESTORATION_DATE, itemIndex);
            }
        }
    }
}