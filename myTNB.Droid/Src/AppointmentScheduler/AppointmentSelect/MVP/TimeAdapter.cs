using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB.Mobile.API.DisplayModel.Scheduler;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.AppointmentScheduler.AppointmentSelect.MVP
{
    public class TimeAdapter : RecyclerView.Adapter
    {
        private string[] timeNames;
        private int timeNamesCount;
        private List<string> timeSolts = new List<string>();
        private int pickedDateDay;
        private bool isDateSelected = false;
        public bool isTimeSelected = false;
        public string timeSelected;
        public TextView selectedTimeTextView;
        public DateTime selectedDate;
        public DateTime selectedStartTime;
        public DateTime selectedEndTime;
        public string selectedTime = string.Empty;
        private const string colorLight_grey = "#e4e4e4";
        private const string color_blue = "#1c79ca";
        public List<AppointmentTimeSlotDisplay> timeSlotDisplay = new List<AppointmentTimeSlotDisplay>();
        public event EventHandler<bool> TimeClickEvent;
        public RelativeLayout.LayoutParams buttonTimeParams;

        public TimeAdapter(List<AppointmentTimeSlotDisplay> timeSlotDisplay
            , int pickedDateDay
            , bool isDateSelected
            , string selectedTime
            , bool isTimeSelected)
        {
            if (timeSlotDisplay != null)
            {
                foreach (var item in timeSlotDisplay)
                {
                    timeSolts.Add(item.TimeSlotDisplay);
                }
                this.timeSlotDisplay = timeSlotDisplay;
                this.timeNames = timeSolts.ToArray();
                this.pickedDateDay = pickedDateDay;
                this.isDateSelected = isDateSelected;
                this.timeSelected = selectedTime;
                this.isTimeSelected = isTimeSelected;
            }
            this.timeNamesCount = this.timeNames != null ? this.timeNames.Length : 0;
        }

        public override int ItemCount => this.timeNamesCount;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            TimeView vh = holder as TimeView;
            vh.textViewTime.Text = timeNames[position];
            if (!isTimeSelected)
            {
                if (isDateSelected)
                {
                    vh.textViewTime.SetTextColor(Color.ParseColor(color_blue));

                }
                else
                {
                    vh.textViewTime.SetTextColor(Color.ParseColor(colorLight_grey));
                }
            }
            if (!isDateSelected)
            {
                if (timeSelected != string.Empty && timeSelected == vh.textViewTime.Text)
                {
                    if (selectedTimeTextView != null)
                    {
                        selectedTimeTextView.Text = timeSelected;
                    }
                    vh.textViewTime.SetBackgroundResource(Resource.Drawable.AppointmentTimeSelector);
                    vh.textViewTime.SetTextColor(Color.White);
                    var timeSlotDates = this.timeSlotDisplay.Where(x => x.TimeSlotDisplay == timeSelected).FirstOrDefault();
                    selectedStartTime = Convert.ToDateTime(timeSlotDates.SlotStartTime);
                    selectedEndTime = Convert.ToDateTime(timeSlotDates.SlotEndTime);
                    selectedTimeTextView = vh.textViewTime;
                }
                else
                {
                    vh.textViewTime.SetTextColor(Color.ParseColor(color_blue));
                }
            }
            vh.textViewTime.Click += (sender, e) =>
            {
                OnTimeClick(sender as View);
            };

        }

        public void OnTimeClick(View view)
        {
            if (selectedTimeTextView != null)
            {
                selectedTimeTextView.SetBackgroundColor(Color.Transparent);
                selectedTimeTextView.SetTextColor(Color.ParseColor(color_blue));
            }

            selectedTimeTextView = (TextView)view;
            isTimeSelected = true;
            selectedTimeTextView.Gravity = GravityFlags.Center;
            selectedTimeTextView.SetBackgroundResource(Resource.Drawable.AppointmentTimeSelector);
            selectedTimeTextView.SetTextColor(Color.White);
            selectedDate = selectedDate;
            selectedTime = selectedTimeTextView.Text;
            var timeSlotDates = this.timeSlotDisplay.Where(x => x.TimeSlotDisplay == selectedTime).FirstOrDefault();
            selectedStartTime = Convert.ToDateTime(timeSlotDates.SlotStartTime);
            selectedEndTime = Convert.ToDateTime(timeSlotDates.SlotEndTime);
            TimeClickEvent(this, true);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.AppointmentTimeSelectionLayout, parent, false);
            buttonTimeParams = new RelativeLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent
                , ViewGroup.LayoutParams.WrapContent);

            return new TimeView(view);
        }

        public class TimeView : RecyclerView.ViewHolder
        {
            public TextView textViewTime;
            public RelativeLayout relativeLayoutTime;

            public TimeView(View itemView) : base(itemView)
            {
                textViewTime = (TextView)ItemView.FindViewById<TextView>(Resource.Id.timeList);
                relativeLayoutTime = (RelativeLayout)ItemView.FindViewById<RelativeLayout>(Resource.Id.relativeLayoutTime);
                TextViewUtils.SetMuseoSans500Typeface(textViewTime);
                textViewTime.TextSize = TextViewUtils.GetFontSize(12f);
            }
        }
    }
}