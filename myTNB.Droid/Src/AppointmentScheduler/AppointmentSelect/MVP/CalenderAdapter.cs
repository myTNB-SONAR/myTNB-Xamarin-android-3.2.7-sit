using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Java.Util;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB.Mobile.API.DisplayModel.Scheduler;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.AppointmentScheduler.AppointmentSelect.MVP
{
    public interface IDayClickListener
    {
        void OnDayClick(View view);
    }
    public class CustomCalendar : LinearLayout
    {
        private const string color_grey = "#727171";
        private const string colorLight_grey = "#e4e4e4";
        private const string color_blue = "#1c79ca";
        public DateTime selectedDateTime;
        public string selectedDate;
        public string keySelected;
        public int selectedMonth;
        public int selectedYear;
        public DateTime selectedStartTime;
        public DateTime selectedEndTime;
        public string selectedTime = string.Empty;
        public bool isTimeSelected = false;
        public bool isDateSelected = false;
        public event EventHandler<bool> DatetimeValidate;
        public event EventHandler<bool> DatetimeScrollValidate;
        public event EventHandler<bool> DateChanged;
        public TimeAdapter timeAdapter;
        public static bool isValidDateTime = false;
        public DateTime dateInfo;
        RecyclerView timeLayout;

        private Button selectedDayButton;
        LinearLayout calendarContent;
        private Button[] days;
        LinearLayout weekOneLayout;
        LinearLayout weekTwoLayout;
        LinearLayout weekThreeLayout;
        LinearLayout weekFourLayout;
        LinearLayout weekFiveLayout;
        LinearLayout weekSixLayout;
        private LinearLayout[] weeks;
        private TextView appointmentLabel2;
        private int currentDateDay, chosenDateDay, currentDateMonth, chosenDateMonth
            , currentDateYear, chosenDateYear, pickedDateDay, pickedDateMonth, pickedDateYear;
        int userMonth, userYear;
        private IDayClickListener mListener;
        private Drawable userDrawable;

        private Calendar calendar;

        LayoutParams defaultButtonParams;
        private LayoutParams userButtonParams;

        public CustomCalendar(Context context
            , int calenderMonth
            , string selectedKey
            , string calenderMonthName
            , int calendarYear
            , List<int> visibleNumbers
            , SchedulerDisplay schedulerDisplayResponse
            , DateTime selectedDateTime
            , string dateSelected
            , int monthSelected
            , int yearSelected
            , string selectedTime
            , DateTime selectedStartTime
            , DateTime selectedEndTime) : base(context)
        {
            Initialize(context
                , calenderMonth
                , selectedKey
                , calenderMonthName
                , calendarYear
                , visibleNumbers
                , schedulerDisplayResponse
                , selectedDateTime
                , dateSelected
                , monthSelected
                , yearSelected
                , selectedTime
                , selectedStartTime
                , selectedEndTime);
        }
        public CustomCalendar(Context context
            , int calenderMonth
            , string selectedKey
            , string calenderMonthName
            , int calendarYear
            , List<int> visibleNumbers
            , SchedulerDisplay schedulerDisplayResponse
            , IAttributeSet attrs
            , DateTime selectedDateTime
            , string dateSelected
            , int monthSelected
            , int yearSelected
            , string selectedTime
            , DateTime selectedStartTime
            , DateTime selectedEndTime) : base(context, attrs)
        {
            Initialize(context
                , calenderMonth
                , selectedKey
                , calenderMonthName
                , calendarYear
                , visibleNumbers
                , schedulerDisplayResponse
                , selectedDateTime
                , dateSelected
                , monthSelected
                , yearSelected
                , selectedTime
                , selectedStartTime
                , selectedEndTime);
        }

        public CustomCalendar(Context context
            , int calenderMonth
            , string selectedKey
            , string calenderMonthName
            , int calendarYear
            , List<int> visibleNumbers
            , SchedulerDisplay schedulerDisplayResponse
            , IAttributeSet attrs
            , int defStyleAttr
            , DateTime selectedDateTime
            , string dateSelected
            , int monthSelected
            , int yearSelected
            , string selectedTime
            , DateTime selectedStartTime
            , DateTime selectedEndTime)
            : base(context, attrs, defStyleAttr)
        {
            Initialize(context
                , calenderMonth
                , selectedKey
                , calenderMonthName
                , calendarYear
                , visibleNumbers
                , schedulerDisplayResponse
                , selectedDateTime
                , dateSelected
                , monthSelected
                , yearSelected
                , selectedTime
                , selectedStartTime
                , selectedEndTime);
        }

        private void Initialize(Context context
            , int calenderMonth
            , string selectedKey
            , string calenderMonthName
            , int calendarYear
            , List<int> visibleNumbers
            , SchedulerDisplay schedulerDisplayResponse
            , DateTime selectedDateTime
            , string dateSelected
            , int monthSelected
            , int yearSelected
            , string selectedTime
            , DateTime selectedStartTime
            , DateTime selectedEndTime)
        {
            this.selectedStartTime = selectedStartTime;
            this.selectedEndTime = selectedEndTime;
            this.selectedDateTime = selectedDateTime;
            DisplayMetrics metrics = Resources.DisplayMetrics;
            SetUserCurrentMonthYear(calenderMonth, calendarYear);
            var inflater = LayoutInflater.FromContext(context);
            View view = inflater.Inflate(Resource.Layout.AppointmentCalendarLayout, this, true);
            calendar = null;
            calendar = Calendar.GetInstance(Java.Util.TimeZone.Default);

            calendar.Set(CalendarField.DayOfMonth, 1);
            calendar.Set(CalendarField.Month, calenderMonth);
            calendar.Set(CalendarField.Year, calendarYear);

            calendarContent = FindViewById<LinearLayout>(Resource.Id.calendarContent);
            weekTwoLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_2);
            weekOneLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_1);
            weekTwoLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_2);
            weekThreeLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_3);
            weekFourLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_4);
            weekFiveLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_5);
            weekSixLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_6);

            appointmentLabel2 = FindViewById<TextView>(Resource.Id.appointmentLabel2);
            appointmentLabel2.Text = Utility.GetLocalizedLabel("ApplicationStatusScheduler", "timeSectionTitle");
            appointmentLabel2.TextSize = TextViewUtils.GetFontSize(16f);
            appointmentLabel2.Visibility = ViewStates.Gone;

            TextViewUtils.SetMuseoSans500Typeface(appointmentLabel2);

            currentDateDay = chosenDateDay = calendar.Get(CalendarField.DayOfMonth);

            if (userMonth != 0 && userYear != 0)
            {
                currentDateMonth = chosenDateMonth = userMonth;
                currentDateYear = chosenDateYear = userYear;
            }
            else
            {
                currentDateMonth = chosenDateMonth = calendar.Get(CalendarField.Month);
                currentDateYear = chosenDateYear = calendar.Get(CalendarField.Year);
            }

            InitializeDaysWeeks();
            if (userButtonParams != null)
            {
                defaultButtonParams = userButtonParams;
            }
            else
            {
                defaultButtonParams = GetdaysLayoutParams();
            }
            AddDaysinCalendar(defaultButtonParams, context, metrics);

            InitCalendarWithDate(context
                , chosenDateYear
                , chosenDateMonth
                , chosenDateDay
                , calenderMonthName
                , calendarYear.ToString()
                , visibleNumbers
                , schedulerDisplayResponse
                , selectedDateTime
                , dateSelected
                , monthSelected
                , yearSelected
                , selectedTime);

            // Time
            timeLayout = (RecyclerView)FindViewById<RecyclerView>(Resource.Id.TimeRecyclerView);
            GridLayoutManager gridLayoutManager = new GridLayoutManager(context, 2);
            timeLayout.SetLayoutManager(gridLayoutManager);
            isValidDateTime = isValidDateTime && isDateSelected;
            selectedDate = dateSelected;
            selectedMonth = monthSelected;
            selectedYear = yearSelected;
            keySelected = selectedKey;

            if (selectedDate != null && selectedDate != string.Empty)
            {
                var monthYear = schedulerDisplayResponse.MonthYearList.Where(x => x.Month == selectedMonth && x.Year == selectedYear).FirstOrDefault();
                var monthindex = schedulerDisplayResponse.ScheduleList[monthYear.MonthYearDisplay].Where(x => x.Day == selectedDate.ToString()).FirstOrDefault();

                timeAdapter = new TimeAdapter(monthindex.TimeSlotDisplay, Convert.ToInt32(selectedDate), isDateSelected, selectedTime, isTimeSelected, chosenDateMonth, selectedMonth - 1, chosenDateYear, selectedYear);
                timeLayout.SetAdapter(timeAdapter);

                timeAdapter.TimeClickEvent += Adapter_TimeClickEvent;
                isValidDateTime = isValidDateTime && isDateSelected;
            }
        }

        private void InitializeDaysWeeks()
        {
            weeks = new LinearLayout[6];
            days = new Button[6 * 7];

            weeks[0] = weekOneLayout;
            weeks[1] = weekTwoLayout;
            weeks[2] = weekThreeLayout;
            weeks[3] = weekFourLayout;
            weeks[4] = weekFiveLayout;
            weeks[5] = weekSixLayout;
        }

        private void InitCalendarWithDate(Context context
            , int year
            , int month
            , int day
            , string calenderMonthName
            , string yearhNames
            , List<int> visibleNumbers
            , SchedulerDisplay schedulerDisplayResponse
            , DateTime selectedDateTime
            , string dateSelected
            , int monthSelected
            , int yearSelected
            , string selectedTime)
        {
            calendar.Set(year, month, 1);

            int daysInCurrentMonth = calendar.GetActualMaximum(CalendarField.DayOfMonth);

            chosenDateYear = year;
            chosenDateMonth = month;
            chosenDateDay = day;

            int firstDayOfCurrentMonth = calendar.Get(CalendarField.DayOfWeek) - 2;

            calendar.Set(year, month, daysInCurrentMonth);

            int dayNumber = 1;
            int daysLeftInFirstWeek = 0;
            int indexOfDayAfterLastDayOfMonth = 0;

            daysLeftInFirstWeek = firstDayOfCurrentMonth;
            indexOfDayAfterLastDayOfMonth = daysLeftInFirstWeek + daysInCurrentMonth;
            for (int i = firstDayOfCurrentMonth; i < firstDayOfCurrentMonth + daysInCurrentMonth; ++i)
            {

                if (visibleNumbers.Where(x => x == dayNumber).FirstOrDefault() > 0)
                {
                    days[i].SetTextColor(Color.ParseColor(color_grey));
                    days[i].Click += (sender, e) =>
                    {
                        OnDayClick(sender as View, context, schedulerDisplayResponse);
                    };
                }
                else
                {
                    days[i].SetTextColor(Color.LightGray);

                }
                days[i].SetBackgroundColor(Color.Transparent);


                int[] dateArr = new int[3];
                dateArr[0] = dayNumber;
                dateArr[1] = chosenDateMonth;
                dateArr[2] = chosenDateYear;
                days[i].Tag = dateArr;
                days[i].Text = dayNumber.ToString();
                if (days[i].Text == dateSelected && chosenDateMonth == monthSelected - 1 && chosenDateYear == yearSelected)
                {
                    days[i].SetBackgroundResource(Resource.Drawable.daySelector);
                    days[i].SetTextColor(Color.White);
                    selectedDayButton = days[i];
                }

                days[i].TextSize = TextViewUtils.GetFontSize(14f);
                TextViewUtils.SetMuseoSans500Typeface(days[i]);
                ++dayNumber;
            }
            for(int j =28; j < days.Count(); j++)
            {
                if(days[j].Text == string.Empty)
                {
                    days[j].Visibility = ViewStates.Gone;
                }
            }
            if (month > 0)
            {
                calendar.Set(year, month - 1, 1);
            }
            else
            {
                calendar.Set(year - 1, 11, 1);
            }
            if (selectedTime != string.Empty && dateSelected != string.Empty && chosenDateMonth == monthSelected - 1 && chosenDateYear == yearSelected)
            {
                var monthYear = schedulerDisplayResponse.MonthYearList.Where(x => x.Month == chosenDateMonth + 1 && x.Year == chosenDateYear).FirstOrDefault();
                var monthindex = schedulerDisplayResponse.ScheduleList[monthYear.MonthYearDisplay].Where(x => x.Day == dateSelected.ToString()).FirstOrDefault();

                timeLayout = (RecyclerView)FindViewById<RecyclerView>(Resource.Id.TimeRecyclerView);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(context, 2);
                timeLayout.SetLayoutManager(gridLayoutManager);

                timeAdapter = new TimeAdapter(monthindex.TimeSlotDisplay, pickedDateDay, isDateSelected, selectedTime, isTimeSelected, chosenDateMonth, selectedMonth - 1, chosenDateYear, selectedYear);
                timeLayout.SetAdapter(timeAdapter);
                timeAdapter.TimeClickEvent += Adapter_TimeClickEvent;
                isValidDateTime = isValidDateTime && isDateSelected;
                selectedStartTime = timeAdapter.selectedStartTime;
                selectedEndTime = timeAdapter.selectedEndTime;
                appointmentLabel2.Visibility = ViewStates.Visible;
            }
            if (dateSelected != null && dateSelected != string.Empty)
            {
                appointmentLabel2.Visibility = ViewStates.Visible;
            }

            calendar.Set(chosenDateYear, chosenDateMonth, chosenDateDay);
        }

        public void OnDayClick(View view, Context context, SchedulerDisplay schedulerDisplayResponse)
        {
            isDateSelected = true;
           
            if (selectedDayButton != null)
            {
                selectedDayButton.SetBackgroundColor(Color.Transparent);
                selectedDayButton.SetTextColor(Color.ParseColor(color_grey));
            }

            selectedDayButton = (Button)view;
            if (selectedDayButton.Tag != null)
            {
                int[] dateArray = (int[])selectedDayButton.Tag;

                pickedDateDay = dateArray[0];
                pickedDateMonth = dateArray[1] + 1;
                pickedDateYear = dateArray[2];

                var selectedKeyMonth = schedulerDisplayResponse.ScheduleList.Where(x => x.Key == keySelected).FirstOrDefault();
                selectedDateTime = Convert.ToDateTime(selectedKeyMonth.Value.Where(x => x.Day == pickedDateDay.ToString()).FirstOrDefault().AppointmentDate);

                selectedDate = selectedDateTime.Day.ToString();
                selectedMonth = selectedDateTime.Month;
                selectedYear = selectedDateTime.Year;
                DateChanged(this, true);
            }

            selectedDayButton.SetBackgroundResource(Resource.Drawable.daySelector);
            selectedDayButton.SetTextColor(Color.White);

            if (pickedDateDay != 0)
            {
                var monthYear = schedulerDisplayResponse.MonthYearList.Where(x => x.Month == pickedDateMonth && x.Year == pickedDateYear).FirstOrDefault();
                var monthindex = schedulerDisplayResponse.ScheduleList[monthYear.MonthYearDisplay].Where(x => x.Day == pickedDateDay.ToString()).FirstOrDefault();

                timeLayout = (RecyclerView)FindViewById<RecyclerView>(Resource.Id.TimeRecyclerView);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(context, 2);
                timeLayout.SetLayoutManager(gridLayoutManager);
                selectedTime = string.Empty;
                timeAdapter = new TimeAdapter(monthindex.TimeSlotDisplay, pickedDateDay, isDateSelected, selectedTime, isTimeSelected, chosenDateMonth, selectedMonth-1, chosenDateYear, selectedYear);
                timeLayout.SetAdapter(timeAdapter);
               
                timeAdapter.TimeClickEvent += Adapter_TimeClickEvent;
                isValidDateTime = isValidDateTime && isDateSelected;
            }
            appointmentLabel2.Visibility = ViewStates.Visible;
        }

        private void Adapter_TimeClickEvent(object sender, bool e)
        {
            if (chosenDateMonth == selectedMonth - 1 && chosenDateYear == selectedYear)
            {
                timeAdapter = (TimeAdapter)sender;
                selectedTime = timeAdapter.timeSelected;
                isTimeSelected = timeAdapter.isTimeSelected;
                selectedStartTime = timeAdapter.selectedStartTime;
                selectedEndTime = timeAdapter.selectedEndTime;
                DatetimeValidate(this, true);
                DatetimeScrollValidate(this, true);
            }
        }

        private void AddDaysinCalendar(LayoutParams buttonParams, Context context, DisplayMetrics metrics)
        {
            int engDaysArrayCounter = 0;

            for (int weekNumber = 0; weekNumber < 6; ++weekNumber)
            {
                for (int dayInWeek = 0; dayInWeek < 7; ++dayInWeek)
                {
                    Button day = new Button(context);
                    day.SetTextColor(Color.ParseColor(colorLight_grey));
                    day.SetBackgroundColor(Color.Transparent);
                    day.LayoutParameters = buttonParams;


                    //buttonParams.SetMargins(11, 8, 11, 8);
                    day.LayoutParameters = buttonParams;

                    day.SetTextSize(ComplexUnitType.Dip, (int)metrics.Density * 5);
                    day.SetSingleLine();
                    days[engDaysArrayCounter] = day;
                    weeks[weekNumber].AddView(day);

                    ++engDaysArrayCounter;
                }
            }
        }

        private LayoutParams GetdaysLayoutParams()
        {
            LayoutParams buttonParams = new LayoutParams(
                ViewGroup.LayoutParams.WrapContent
                , ViewGroup.LayoutParams.WrapContent)
            {
                Weight = 1
            };
            return buttonParams;
        }

        public void SetUserDaysLayoutParams(LayoutParams userButtonParams)
        {
            this.userButtonParams = userButtonParams;
        }

        public void SetUserCurrentMonthYear(int userMonth, int userYear)
        {
            this.userMonth = userMonth;
            this.userYear = userYear;
        }

        public void SetDayBackground(Drawable userDrawable)
        {
            this.userDrawable = userDrawable;
        }

        public void SetCallBack(IDayClickListener mListener)
        {
            this.mListener = mListener;
        }
    }
}