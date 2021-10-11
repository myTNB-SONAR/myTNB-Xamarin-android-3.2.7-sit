using System.Collections.Generic;

namespace myTNB_Android.Src.Enquiry.GSL.MVP
{
    public class GSLRebateModel
    {
        public string RebateType { get; set; }
        public GSLRebateTenantModel TenantInfo { get; set; }
        public List<GSLRebateIncidentModel> IncidentList { get; set; }
    }

    public class GSLRebateTenantModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
    }

    public class GSLRebateIncidentModel
    {
        public string IncidentDate { get; set; }
        public string IncidentTime { get; set; }
        public string RestorationDate { get; set; }
        public string RestorationTime { get; set; }
    }

    public enum GSLLayoutType
    {
        FULL_NAME,
        EMAIL_ADDRESS,
        MOBILE_NUMBER,
        NONE
    };

    public enum GSLIncidentDateTimePicker
    {
        INCIDENT_DATE,
        INCIDENT_TIME,
        RESTORATION_DATE,
        RESTORATION_TIME,
        NONE
    }

    public class GSLRebateConstants
    {
        internal static readonly string REBATE_MODEL = "RebateModel";
        internal static readonly string DATE_FORMAT = "dd MMM yyyy";
        internal static readonly string TIME_FORMAT = "h:mm tt";
    }
}