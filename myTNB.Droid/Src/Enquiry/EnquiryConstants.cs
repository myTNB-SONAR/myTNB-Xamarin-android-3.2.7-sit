namespace myTNB.Android.Src.Enquiry
{
    public class EnquiryConstants
    {
        internal static readonly int COUNTRY_CODE_SELECT_REQUEST = 1;
        internal static readonly int SELECT_REBATE_TYPE_REQ_CODE = 2;

        internal static readonly string UPLOAD_DOCUMENT_DATE_FORMAT = "yyyyMMdd";

        internal static readonly string FEEDBACK_CATEGORY_ID = "FeedbackCategoryId";
        internal static readonly string GSL_FEEDBACK_CATEGORY_ID = "9";
        internal static readonly string GSL_INFO_SELECTOR = "GSL";
        internal static readonly string GSL_INFO = "gslInfo";
    }

    public enum EnquiryAccountDetailType
    {
        FULL_NAME,
        EMAIL_ADDRESS,
        MOBILE_NUMBER,
        NONE
    };

    public enum EnquiryStatusCode
    {
        CL01,
        CL02,
        CL03,
        CL04,
        CL06,
        CL07,
        CL08,
        CL09,
        CL10,
        CL11,
        CL12,
        CL13,
        CL14,
        CL15,
        CL16,
        CL17,
        CL18,
        CL19,
        NONE
    };

    public enum EnquiryGSLStatusCode
    {
        GS01,
        GS02,
        GS03,
        GS04,
        GS05,
        GS06,
        GS07,
        GS08,
        GL01,
        GL02,
        GL03,
        GL04,
        GL05,
        NONE
    };
}
