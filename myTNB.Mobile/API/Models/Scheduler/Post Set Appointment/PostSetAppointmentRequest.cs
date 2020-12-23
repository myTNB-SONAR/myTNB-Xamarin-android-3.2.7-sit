using System;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.Scheduler.PostSetAppointment
{
    public class PostSetAppointmentRequest
    {
        [JsonProperty("setAppointment")]
        public SetAppointment SetAppointment { set; get; }
    }

    public class SetAppointment
    {
        [JsonProperty("ApplicationId")]
        public string ApplicationId { set; get; }
        [JsonProperty("ApplicationType")]
        public string ApplicationType { set; get; }
        [JsonProperty("SrNo")]
        public string SrNo { set; get; }
        [JsonProperty("SrType")]
        public string SrType { set; get; }
        [JsonProperty("BusinessArea")]
        public string BusinessArea { set; get; }
        [JsonProperty("AppointmentDate")]
        public DateTime AppointmentDate { set; get; }
        [JsonProperty("AppointmentStartTime")]
        public DateTime AppointmentStartTime { set; get; }
        [JsonProperty("AppointmentEndTime")]
        public DateTime AppointmentEndTime { set; get; }
    }
}