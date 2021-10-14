using System.Collections.Generic;

namespace myTNB_Android.Src.Enquiry.GSL.MVP
{
    public class GSLRebateModel
    {
        public bool IsOwner { get; set; }
        public string RebateType { get; set; }
        public GSLRebateTenantModel TenantInfo { get; set; }
        public List<GSLRebateIncidentModel> IncidentList { get; set; }
        public GSLRebateDocumentModel Documents { get; set; }
        public GSLRebateAccountInfoModel AccountInfo { get; set; }
    }

    public class GSLRebateTenantModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
    }

    public class GSLRebateIncidentModel
    {
        public string IncidentDateTime { get; set; }
        public string RestorationDateTime { get; set; }
    }

    public class GSLRebateDocumentModel
    {
        public string TenancyAgreement { get; set; }
        public string OwnerIC { get; set; }
    }

    public class GSLRebateAccountInfoModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
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

    public enum GSLDocumentType
    {
        TENANCY_AGREEMENT,
        OWNER_IC,
        NONE
    }

    public class GSLRebateConstants
    {
        internal static readonly string REBATE_MODEL = "RebateModel";
        internal static readonly string DATE_FORMAT = "dd MMM yyyy";
        internal static readonly string TIME_FORMAT = "h:mm tt";
    }
}