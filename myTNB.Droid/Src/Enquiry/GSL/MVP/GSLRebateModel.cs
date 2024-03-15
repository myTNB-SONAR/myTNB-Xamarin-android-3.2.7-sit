using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.Enquiry.GSL.MVP
{
    public class GSLRebateModel
    {
        public string ServiceReqNo { get; set; }
        public string StatusCode { get; set; }
        public EnquiryGSLStatusCode GSLStatusCode
        {
            get
            {
                return StatusCode switch
                {
                    "GS01" => EnquiryGSLStatusCode.GS01,
                    "GL01" => EnquiryGSLStatusCode.GL01,
                    "GS08" => EnquiryGSLStatusCode.GS08,
                    "GL05" => EnquiryGSLStatusCode.GL05,
                    "GS06" => EnquiryGSLStatusCode.GS06,
                    "GL03" => EnquiryGSLStatusCode.GL03,
                    "GS07" => EnquiryGSLStatusCode.GS07,
                    "GL04" => EnquiryGSLStatusCode.GL04,
                    "GS05" => EnquiryGSLStatusCode.GS05,
                    "GS02" => EnquiryGSLStatusCode.GS02,
                    "GL02" => EnquiryGSLStatusCode.GL02,
                    "GS03" => EnquiryGSLStatusCode.GS03,
                    "GS04" => EnquiryGSLStatusCode.GS04,
                    _ => EnquiryGSLStatusCode.NONE,
                };
            }
        }
        public int StatusColor
        {
            get
            {
                return GSLStatusCode switch
                {
                    EnquiryGSLStatusCode.GS01 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GS03 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GS04 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GS06 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GL01 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GL03 => Resource.Color.createdColorSubmit,
                    EnquiryGSLStatusCode.GS02 => Resource.Color.inProgressColor,
                    EnquiryGSLStatusCode.GL02 => Resource.Color.inProgressColor,
                    EnquiryGSLStatusCode.GL05 => Resource.Color.completedColor,
                    EnquiryGSLStatusCode.GS08 => Resource.Color.completedColor,
                    EnquiryGSLStatusCode.GS05 => Resource.Color.cancelledColor,
                    EnquiryGSLStatusCode.GS07 => Resource.Color.cancelledColor,
                    EnquiryGSLStatusCode.GL04 => Resource.Color.cancelledColor,
                    _ => Resource.Color.createdColorSubmit,
                };
            }
        }
        public string StatusDesc { get; set; }
        public string FeedbackCategoryId { get; set; }
        public string AccountNum { get; set; }
        public bool IsOwner { get; set; }
        public string RebateTypeKey { get; set; }
        public bool NeedsIncident { get; set; }
        public GSLRebateTenantModel TenantInfo { get; set; }
        public List<GSLRebateIncidentModel> IncidentList { get; set; }
        public List<GSLRebateIncidentDisplayModel> IncidentDisplayList { get; set; }
        public GSLRebateDocumentModel Documents { get; set; }
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