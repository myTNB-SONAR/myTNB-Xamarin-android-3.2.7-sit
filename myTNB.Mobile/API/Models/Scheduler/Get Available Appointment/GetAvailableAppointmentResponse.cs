using System;
using System.Collections.Generic;
using System.Globalization;
using myTNB.Mobile.API.Base;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.Scheduler.GetAvailableAppointment
{
    public class GetAvailableAppointmentResponse : BaseListResponse<GetAvailableAppointmentModel> { }

    public class GetAvailableAppointmentModel
    {
        [JsonProperty("appointmentDate")]
        public DateTime? AppointmentDate { set; get; }
        [JsonProperty("appointmentTimeSlot")]
        public List<AppointmentTimeSlot> AppointmentTimeSlot { set; get; }

        [JsonIgnore]
        public string DayOfWeek
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                return AppointmentDate != null && AppointmentDate.Value != null
                    ? AppointmentDate.Value.ToString("ddd", dateCultureInfo) ?? string.Empty
                    : string.Empty;
            }
        }

        [JsonIgnore]
        public string MonthOrder
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                return AppointmentDate != null && AppointmentDate.Value != null
                    ? AppointmentDate.Value.ToString("MM", dateCultureInfo) ?? string.Empty
                    : string.Empty;;
            }
        }
    }

    public class AppointmentTimeSlot
    {
        [JsonProperty("slotStartTime")]
        public DateTime? SlotStartTime { set; get; }
        [JsonProperty("slotEndTime")]
        public DateTime? SlotEndTime { set; get; }
        [JsonProperty("availableSlotCount")]
        public int? AvailableSlotCount { set; get; }
    }
}