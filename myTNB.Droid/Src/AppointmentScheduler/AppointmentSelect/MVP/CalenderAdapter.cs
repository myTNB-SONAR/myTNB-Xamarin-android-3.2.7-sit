using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;


using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB.Mobile.API.DisplayModel.Scheduler;

namespace myTNB_Android.Src.AppointmentScheduler.AppointmentSelect.MVP
{
    public interface IDayClickListener
    {
        void onDayClick(View view);
    }
    public class CustomCalendar : LinearLayout
    {
        private static string color_calendar_number = "#424A56";
  
        private static string color_grey = "#727171";
        private static string colorLight_grey = "#e4e4e4";
        public DateTime selectedDate;
        public DateTime selectedStartTime;
        public DateTime selectedEndTime;
        public string selectedTime = string.Empty;
        public bool isDateSelected = false;
        public event EventHandler<bool> DatetimeValidate;


        public static bool isValidDateTime = false;

        RecyclerView timeLayout;

        private TextView selectedTimeTextView;
     

        private TextView currentDate;
       
        private Button selectedDayButton;
        private Button[] days;
        LinearLayout weekOneLayout;
        LinearLayout weekTwoLayout;
        LinearLayout weekThreeLayout;
        LinearLayout weekFourLayout;
        LinearLayout weekFiveLayout;
        LinearLayout weekSixLayout;
        private LinearLayout[] weeks;

        private int currentDateDay, chosenDateDay, currentDateMonth,
                chosenDateMonth, currentDateYear, chosenDateYear,
                pickedDateDay, pickedDateMonth, pickedDateYear;
        int userMonth, userYear;
        private IDayClickListener mListener;
        private Drawable userDrawable;

        private Calendar calendar;
        private DateTime dateTime;

        LayoutParams defaultButtonParams;
        private LayoutParams userButtonParams;

        public CustomCalendar(Context context, int calenderMonth, string calenderMonthName, int calendarYear, List<int> visibleNumbers, SchedulerDisplay schedulerDisplayResponse) : base(context)
        {
            Initialize(context, calenderMonth, calenderMonthName, calendarYear, visibleNumbers, schedulerDisplayResponse);
        }
        public CustomCalendar(Context context, int calenderMonth, string calenderMonthName, int calendarYear, List<int> visibleNumbers, SchedulerDisplay schedulerDisplayResponse, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context, calenderMonth, calenderMonthName, calendarYear, visibleNumbers, schedulerDisplayResponse);
        }

        public CustomCalendar(Context context, int calenderMonth, string calenderMonthName, int calendarYear, List<int> visibleNumbers, SchedulerDisplay schedulerDisplayResponse, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            Initialize(context, calenderMonth, calenderMonthName, calendarYear, visibleNumbers, schedulerDisplayResponse);
        }

        

        private void Initialize(Context context, int calenderMonth, string calenderMonthName, int calendarYear, List<int> visibleNumbers, SchedulerDisplay schedulerDisplayResponse)
        {
            DisplayMetrics metrics = Resources.DisplayMetrics;
            setUserCurrentMonthYear(calenderMonth, calendarYear);
            var inflater = LayoutInflater.FromContext(context);
            View view = inflater.Inflate(Resource.Layout.AppointmentCalendarLayout, this, true);
            calendar = null;
            calendar = Calendar.GetInstance(Java.Util.Locale.English);
            calendar.Set(CalendarField.DayOfMonth, 1);
            calendar.Set(CalendarField.Month, calenderMonth);
            calendar.Set(CalendarField.Year, calendarYear);

            weekOneLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_1);
            weekTwoLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_2);
            weekThreeLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_3);
            weekFourLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_4);
            weekFiveLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_5);
            weekSixLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_6);
            
           

            var dayof = calendar.FirstDayOfWeek;
            //currentDateDay = chosenDateDay = calendar.Get(CalendarField.DayOfMonth);
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

            
            

            initializeDaysWeeks();
            if (userButtonParams != null)
            {
                defaultButtonParams = userButtonParams;
            }
            else
            {
                defaultButtonParams = getdaysLayoutParams();
            }
            addDaysinCalendar(defaultButtonParams, context, metrics);

            InitCalendarWithDate(context, chosenDateYear, chosenDateMonth, chosenDateDay, calenderMonthName, calendarYear.ToString(), visibleNumbers, schedulerDisplayResponse);


            // TIme
            timeLayout = (RecyclerView)FindViewById<RecyclerView>(Resource.Id.TimeRecyclerView);
            GridLayoutManager gridLayoutManager = new GridLayoutManager(context, 2);
            timeLayout.SetLayoutManager(gridLayoutManager);

            //TimeAdapter timeAdapter = new TimeAdapter(timeNames, pickedDateDay, isDateSelected);
            //timeLayout.SetAdapter(timeAdapter); 

            if(isValidDateTime && isDateSelected)
            {
                isValidDateTime = true;
            }
            else
            {
                isValidDateTime = false;
            }

            //for (int i = 0; i < timeNames.Length; i++)
            //{
            //    TextView dynamicTextView = new TextView(context);
            //    dynamicTextView.Text = timeNames[i];
            //    dynamicTextView.SetForegroundGravity(GravityFlags.Center);
            //    dynamicTextView.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.blue)));
            //    dynamicTextView.SetBackgroundColor(Color.Transparent);

            //    dynamicTextView.SetTextSize(ComplexUnitType.Dip, 14f);
            //    dynamicTextView.SetSingleLine();

            //    dynamicTextView.Click += (sender, e) =>
            //    {

            //        onTimeClick(sender as View, context);
            //    };

            //    timeLayout.AddView(dynamicTextView);
            //}
        }
        //public void onTimeClick(View view, Context context)
        //{
        //    selectedTimeTextView = (TextView)view;
        //    //selectedTimeTextView.SetBackgroundResource(Resource.Drawable.AppointmentTimeSelector);
        //    //selectedTimeTextView.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.white)));


        //    selectedTimeTextView.SetBackgroundColor(Color.Transparent);
        //    selectedTimeTextView.SetTextColor(Color.ParseColor(color_calendar_number));
        //    selectedTimeTextView.SetBackgroundResource(Resource.Drawable.AppointmentTimeSelector);
        //    selectedTimeTextView.SetForegroundGravity(GravityFlags.Center);
        //    selectedTimeTextView.SetTextColor(Color.White);
        //}

        private void initializeDaysWeeks()
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


        private void InitCalendarWithDate(Context context, int year, int month, int day, string calenderMonthName, string yearhNames, List<int> visibleNumbers, SchedulerDisplay schedulerDisplayResponse)
        {
            //calendar = null;
               // calendar = Calendar.GetInstance(Java.Util.Locale.English);

            calendar.Set(year, month, day);

            int daysInCurrentMonth = calendar.GetActualMaximum(CalendarField.DayOfMonth);

          

            chosenDateYear = year;
            chosenDateMonth = month;
            chosenDateDay = day;

            calendar.Set(year, month, 1);
            int firstDayOfCurrentMonth = 7- calendar.Get(CalendarField.DayOfWeek);

            calendar.Set(year, month, daysInCurrentMonth); 

            int dayNumber = 1;
            int daysLeftInFirstWeek = 0;
            int indexOfDayAfterLastDayOfMonth = 0;

           // if (firstDayOfCurrentMonth != 1)
           // {
                daysLeftInFirstWeek = firstDayOfCurrentMonth;
                indexOfDayAfterLastDayOfMonth = daysLeftInFirstWeek + daysInCurrentMonth;
                for (int i = firstDayOfCurrentMonth; i < firstDayOfCurrentMonth + daysInCurrentMonth; ++i)
                {
                    if (currentDateMonth == chosenDateMonth
                            && currentDateYear == chosenDateYear
                            && dayNumber == currentDateDay)
                    {
                       

                    }
                    else
                    {
                        if (visibleNumbers.Where(x => x == dayNumber).FirstOrDefault() > 0)
                        {
                            days[i].SetTextColor(Color.Gray);
                            days[i].Click += (sender, e) =>
                            {
                               
                                onDayClick(sender as View,context, schedulerDisplayResponse);
                            };
                        }
                        else
                        {
                            days[i].SetTextColor(Color.LightGray);

                        }
                        days[i].SetBackgroundColor(Color.Transparent);
                    }

                    int[] dateArr = new int[3];
                    dateArr[0] = dayNumber;
                    dateArr[1] = chosenDateMonth;
                    dateArr[2] = chosenDateYear;
                    days[i].Tag = dateArr;
                    days[i].Text = dayNumber.ToString();



                    ++dayNumber;
                //  }
                // }
                /* else
                 {
                     daysLeftInFirstWeek = 8;
                     indexOfDayAfterLastDayOfMonth = daysLeftInFirstWeek + daysInCurrentMonth;
                     for (int i = 8; i < 8 + daysInCurrentMonth; ++i)
                     {
                         if (currentDateMonth == chosenDateMonth
                                 && currentDateYear == chosenDateYear)
                         {

                             if (dayNumber < currentDateDay)
                             {

                             }
                             else
                             {


                                 if (dayNumber == 15 || dayNumber == 16 || dayNumber == 22 || dayNumber == 23)
                                 {

                                 }

                                 if (dayNumber == 14 || dayNumber == 31 || dayNumber == 30)
                                 {

                                 }
                             }

                         }




                         int[] dateArr = new int[3];
                         dateArr[0] = dayNumber;
                         dateArr[1] = chosenDateMonth;
                         dateArr[2] = chosenDateYear;
                         days[i].Tag = dateArr;
                         days[i].Text = dayNumber.ToString();
                         days[i].Click += (sender, e) =>
                         {

                             onDayClick(sender as View, context, schedulerDisplayResponse);
                         };

                         ++dayNumber;
                     }*/
            }

            if (month > 0)
                calendar.Set(year, month - 1, 1);
            else
                calendar.Set(year - 1, 11, 1);

       

            calendar.Set(chosenDateYear, chosenDateMonth, chosenDateDay);
        }

        public void onDayClick(View view,Context context, SchedulerDisplay schedulerDisplayResponse)
        {
            isDateSelected = true;

            if (selectedDayButton != null)
            {
                if (chosenDateYear == currentDateYear
                        && chosenDateMonth == currentDateMonth
                        && pickedDateDay == currentDateDay)
                {
                    
                }
                else
                {
                    selectedDayButton.SetBackgroundColor(Color.Transparent);
                    if (selectedDayButton.CurrentTextColor != Color.Red)
                    {
                        
                        selectedDayButton.SetTextColor(Color.ParseColor(color_calendar_number));
                    }
                }
            }

            selectedDayButton = (Button)view;
            if (selectedDayButton.Tag != null)
            {
                int[] dateArray = (int[])selectedDayButton.Tag;

                pickedDateDay = dateArray[0];
                pickedDateMonth = dateArray[1];
                pickedDateYear = dateArray[2];

                string iDate = (pickedDateDay.ToString().Length < 2 ? "0" + pickedDateDay.ToString() : pickedDateDay.ToString()) +"/"+ (pickedDateMonth.ToString().Length < 2 ? "0" + pickedDateMonth.ToString() : pickedDateMonth.ToString()) + "/" + pickedDateYear;
              
            

               System.Globalization.CultureInfo currCult = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
                DateTime date = DateTime.ParseExact(iDate, "dd/mm/yyyy", currCult);

                selectedDate = date;
            }

            if (pickedDateYear == currentDateYear
                    && pickedDateMonth == currentDateMonth
                    && pickedDateDay == currentDateDay)
            {

               
            }
            else
            {
                
                selectedDayButton.SetBackgroundResource(Resource.Drawable.daySelector);
                selectedDayButton.SetTextColor(Color.White);
            }
            if (pickedDateDay != 0)
            {
               
                var monthYear = schedulerDisplayResponse.MonthYearList.Where(x => x.Month == pickedDateMonth && x.Year == pickedDateYear).FirstOrDefault();
                var monthindex = schedulerDisplayResponse.ScheduleList[monthYear.MonthYearDisplay].Where(x => x.Day == pickedDateDay.ToString()).FirstOrDefault();
               
                //string[] timeNames = schedulerDisplayResponse.ScheduleList.Values;
               
                timeLayout = (RecyclerView)FindViewById<RecyclerView>(Resource.Id.TimeRecyclerView);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(context, 2);
                timeLayout.SetLayoutManager(gridLayoutManager);

                TimeAdapter timeAdapter = new TimeAdapter(monthindex.TimeSlotDisplay, pickedDateDay, isDateSelected);
                timeLayout.SetAdapter(timeAdapter);

                // adapter listener

                timeAdapter.TimeClickEvent += Adapter_TimeClickEvent;

                if (isValidDateTime && isDateSelected)
                {
                    isValidDateTime = true;
                }
                else
                {
                    isValidDateTime = false;
                }
            }
        }

        private void Adapter_TimeClickEvent(object sender, bool e)
        {
            TimeAdapter timeAdapter = (TimeAdapter)sender;
            selectedTime = timeAdapter.selectedTime;
           
            selectedStartTime = timeAdapter.selectedStartTime;
            selectedEndTime = timeAdapter.selectedEndTime;
            DatetimeValidate(this, true);
        }

       


            private void addDaysinCalendar(LayoutParams buttonParams, Context context,
                                       DisplayMetrics metrics)
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
                    day.SetTextSize(ComplexUnitType.Dip, (int)metrics.Density * 5);
                    day.SetSingleLine();

                    days[engDaysArrayCounter] = day;
                    weeks[weekNumber].AddView(day);

                    ++engDaysArrayCounter;
                }
            }
        }


        private LayoutParams getdaysLayoutParams()
        {
            LinearLayout.LayoutParams buttonParams = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent);
            buttonParams.Weight = 1;
            return buttonParams;
        }

        public void setUserDaysLayoutParams(LinearLayout.LayoutParams userButtonParams)
        {
            this.userButtonParams = userButtonParams;
        }
        public void setUserCurrentMonthYear(int userMonth, int userYear)
        {
            this.userMonth = userMonth;
            this.userYear = userYear;
        }
        public void setDayBackground(Drawable userDrawable)
        {
            this.userDrawable = userDrawable;
        }

        public void setCallBack(IDayClickListener mListener)
        {
            this.mListener = mListener;
        }
    }

   
}
