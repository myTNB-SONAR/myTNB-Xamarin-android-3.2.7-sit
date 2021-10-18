using System.Collections.Generic;

namespace myTNB_Android.Src.Enquiry.GSL.MVP
{
    public class GSLRebateModel
    {
        public string ServiceReqNo { get; set; }
        public string StatusCode { get; set; }
        public EnquiryStatusCode EnquiryStatusCode
        {
            get
            {
                return StatusCode switch
                {
                    "CLO1" => EnquiryStatusCode.CL01,
                    "CL02" => EnquiryStatusCode.CL02,
                    "CL03" => EnquiryStatusCode.CL03,
                    "CL04" => EnquiryStatusCode.CL04,
                    "CLO6" => EnquiryStatusCode.CL06,
                    _ => EnquiryStatusCode.NONE,
                };
            }
        }
        public int StatusColor
        {
            get
            {
                return EnquiryStatusCode switch
                {
                    EnquiryStatusCode.CL01 => Resource.Color.createdColorSubmit,
                    EnquiryStatusCode.CL02 => Resource.Color.inProgressColor,
                    EnquiryStatusCode.CL03 => Resource.Color.completedColor,
                    EnquiryStatusCode.CL04 => Resource.Color.completedColor,
                    EnquiryStatusCode.CL06 => Resource.Color.cancelledColor,
                    _ => Resource.Color.createdColorSubmit,
                };
            }
        }
        public string StatusDesc { get; set; }
        public string FeedbackCategoryId { get; set; }
        public string AccountNum { get; set; }
        public bool IsOwner { get; set; }
        public string RebateTypeKey { get; set; }
        public GSLRebateTenantModel TenantInfo { get; set; }
        public List<GSLRebateIncidentModel> IncidentList { get; set; }
        public List<GSLRebateIncidentDisplayModel> IncidentDisplayList { get; set; }
        public GSLRebateDocumentModel Documents { get; set; }
        public GSLRebateAccountInfoModel AccountInfo { get; set; }
        public GSLRebateAccountInfoModel ContactInfo { get; set; }
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

    public class GSLRebateIncidentDisplayModel
    {
        public string IncidentDate { get; set; }
        public string RestorationDate { get; set; }
        public string IncidentTime { get; set; }
        public string RestorationTime { get; set; }
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
        public string Address { get; set; }
    }

    public class GSLInfo
    {
        public string title { get; set; }
        public string description { get; set; }
        public List<GSLInfoContent> expandCollapseList { get; set; }
    }

    public class GSLInfoContent
    {
        public string title { get; set; }
        public string description { get; set; }
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
        internal static readonly string DATE_RESPONSE_PARSE_FORMAT = "yyyy-MM-dd";
        internal static readonly string TIME_RESPONSE_PARSE_FORMAT = "HH:mm:ss";
        internal static readonly string DATETIME_PARSE_FORMAT = "dd'/'MM'/'yyyy HH:mm:ss";
    }
}