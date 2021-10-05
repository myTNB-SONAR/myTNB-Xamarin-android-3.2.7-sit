using System;
namespace myTNB_Android.Src.OverVoltageFeedback.Model
{
    public class DTOWebView
    {
        public string srNumber { get; set; }
        public string appointmentDate { get; set; }
        public string technicianName { get; set; }
        public string incidentAddress { get; set; }
        public string title { get; set; }
        public string claimId { get; set; }
        public string currentScreen { get; set; }
        public string nextScreen { get; set; }
        public string totalAmount { get; set; }
        public string message { get; set; }
        public string crStatus { get; set; }
        public string crStatusCode { get; set; }
    }
}
