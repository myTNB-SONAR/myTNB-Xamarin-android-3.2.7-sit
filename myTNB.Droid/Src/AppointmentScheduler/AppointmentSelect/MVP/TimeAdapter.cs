using System;
using System.Collections.Generic;
using Android.Graphics;

using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB.Mobile.API.DisplayModel.Scheduler;

namespace myTNB_Android.Src.AppointmentScheduler.AppointmentSelect.MVP
{
    public class TimeAdapter : RecyclerView.Adapter
    {
        private string[] timeNames;
        private List<string> timeSolts = new List<string>();
        private int pickedDateDay;
        private bool isDateSelected = false;
        private bool isTimeSelected = false;
        private TextView selectedTimeTextView;
        public string selectedDate = string.Empty;
        public string selectedTime = string.Empty;
        private TextView previousSelectedTime;
        private static string color_calendar_number = "#424A56";
        private static string colorLight_grey = "#e4e4e4";
        private static string color_blue = "#1c79ca";

        public event EventHandler<bool> TimeClickEvent;

        public TimeAdapter(List<AppointmentTimeSlotDisplay> timeSlotDisplay, int pickedDateDay, bool isDateSelected) 
        {
            foreach(var item in timeSlotDisplay)
            {
                timeSolts.Add(item.TimeSlotDisplay);
            }
           
            this.timeNames = timeSolts.ToArray();
            this.pickedDateDay = pickedDateDay;
            this.isDateSelected = isDateSelected;
        }
      

        public override int ItemCount => this.timeNames.Length;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            TimeView vh = holder as TimeView;


            vh.textViewTime.Text = timeNames[position];
            if (!isTimeSelected)
            {
                if (isDateSelected)
                {
                    vh.textViewTime.SetTextColor(Color.ParseColor(color_blue));
                    vh.textViewTime.Click += (sender, e) =>
                        {

                            onTimeClick(sender as View);
                        };
                }
                else
                {
                    vh.textViewTime.SetTextColor(Color.ParseColor(colorLight_grey));
                }
            }
        }
        public void onTimeClick(View view)
        {
            if (selectedTimeTextView != null)
            {

                selectedTimeTextView.SetBackgroundColor(Color.Transparent);
                if (selectedTimeTextView.CurrentTextColor != Color.Red)
                {

                    selectedTimeTextView.SetTextColor(Color.ParseColor(color_blue));
                }

            }

            selectedTimeTextView = (TextView)view;

            isTimeSelected = true;

            //selectedTimeTextView.SetBackgroundResource(Resource.Drawable.AppointmentTimeSelector);
            //selectedTimeTextView.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.white)));

            //selectedTimeTextView.SetTextColor(Color.ParseColor(color_calendar_number));


            //selectedTimeTextView.SetBackgroundColor(Color.Transparent);
            selectedTimeTextView.Gravity = GravityFlags.Center;
            selectedTimeTextView.SetBackgroundResource(Resource.Drawable.AppointmentTimeSelector);
            selectedTimeTextView.SetTextColor(Color.White);

            selectedDate = selectedDate;
            selectedTime = selectedTimeTextView.Text;
            TimeClickEvent(this,true);
            //CustomCalendar.isValidDateTime = true;

            //NotifyDataSetChanged();
            //selectedTimeTextView.pare
            //RelativeLayout.LayoutParams param = selectedTimeTextView.LayoutParameters as RelativeLayout.LayoutParams;
            //param.AddRule(LayoutRules.CenterInParent);

            //RelativeLayout.LayoutParams layoutparams = (new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent));

            //layoutparams.AddRule(LayoutRules.CenterInParent);

            //RelativeLayout.LayoutParams layoutparams = (new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent));

            //layoutparams.AddRule(LayoutRules.CenterInParent);



        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.AppointmentTimeSelectionLayout,parent,false);
            return new TimeView(view); 
        }

        public class TimeView : RecyclerView.ViewHolder
        {
            public TextView textViewTime;
           
            public TimeView(View itemView) : base(itemView)
            {
                textViewTime = (TextView)ItemView.FindViewById<TextView>(Resource.Id.timeList);
                
          



            }
        }

    }
}
