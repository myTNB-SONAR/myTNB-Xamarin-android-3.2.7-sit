using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using myTNB.Mobile.API.Models.Scheduler.GetAvailableAppointment;
using myTNB.Mobile.Extensions;

namespace myTNB.Mobile.API.DisplayModel.Scheduler
{
    public class SchedulerDisplay
    {
        public StatusDetail StatusDetail { set; get; }
        /// <summary>
        /// Key = Month Order
        /// </summary>
        public Dictionary<string, List<SchedulerDisplayModel>> ScheduleList { set; get; }
    }

    public class SchedulerDisplayModel
    {
        internal string MonthDisplay
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                return AppointmentDate != null && AppointmentDate.Value != null
                    ? AppointmentDate.Value.ToString("MMM", dateCultureInfo) ?? string.Empty
                    : string.Empty;
            }
        }
        internal string YearDisplay
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                return AppointmentDate != null && AppointmentDate.Value != null
                    ? AppointmentDate.Value.ToString("yyyy", dateCultureInfo) ?? string.Empty
                    : string.Empty;
            }
        }
        internal string MonthOrder
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                return AppointmentDate != null && AppointmentDate.Value != null
                    ? AppointmentDate.Value.ToString("MM", dateCultureInfo) ?? string.Empty
                    : string.Empty;
            }
        }
        /// <summary>
        /// Used to display MMM yyyy in Calendar Header
        /// </summary>
        public string DateTitleDisplay
        {
            get
            {
                return string.Format("{0} {1}"
                    , MonthDisplay
                    , YearDisplay);
            }
        }
        public DateTime? AppointmentDate { set; get; }

        public string DayOfWeek
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(LanguageManager.Language.EN.ToString());
                return AppointmentDate != null && AppointmentDate.Value != null
                    ? AppointmentDate.Value.ToString("ddd", dateCultureInfo) ?? string.Empty
                    : string.Empty;
            }
        }

        /// <summary>
        /// Display Day
        /// </summary>
        public string Day
        {
            get
            {
                return AppointmentDate != null && AppointmentDate.Value != null
                    ? AppointmentDate.Value.Day.ToString()
                    : string.Empty;
            }
        }

        /// <summary>
        /// Ysed to Display Time Slots
        /// </summary>
        public List<AppointmentTimeSlotDisplay> TimeSlotDisplay { set; get; }

        /// <summary>
        /// Determines if the date slot is aActive or Not
        /// </summary>
        public bool IsAvailable { set; get; }
    }

    public class AppointmentTimeSlotDisplay : AppointmentTimeSlot
    {
        public string TimeSlotDisplay
        {
            get
            {
                try
                {
                    CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                    string start = string.Empty;
                    string end = string.Empty;
                    string display = string.Empty;
                    if (SlotStartTime != null && SlotStartTime.Value != null)
                    {
                        start = SlotStartTime.Value.ToString("hh:mm tt", dateCultureInfo);
                    }
                    if (SlotEndTime != null && SlotEndTime.Value != null)
                    {
                        end = SlotEndTime.Value.ToString("hh:mm tt", dateCultureInfo);
                    }

                    if (start.IsValid() && end.IsValid())
                    {
                        display = string.Format("{0} - {1}", start, end);
                    }
                    else if (start.IsValid())
                    {
                        display = start;
                    }
                    else if (end.IsValid())
                    {
                        display = end;
                    }
                    return display;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[DEBUG] TimeSlotDisplay Error: " + e.Message);
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Determines if the time slot is aActive or Not
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return AvailableSlotCount != null && AvailableSlotCount.Value > 0;
            }
        }
    }
}