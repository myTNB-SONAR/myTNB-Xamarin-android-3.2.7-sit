namespace myTNB_Android.Src.Enquiry
{
    public class EnquiryConstants
    {
        internal static readonly int COUNTRY_CODE_SELECT_REQUEST = 1;
        internal static readonly int SELECT_REBATE_TYPE_REQ_CODE = 2;

        internal static readonly string UPLOAD_DOCUMENT_DATE_FORMAT = "yyyyMMdd";

        internal static readonly string FEEDBACK_CATEGORY_ID = "FeedbackCategoryId";
        internal static readonly string GSL_FEEDBACK_CATEGORY_ID = "9";
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
        NONE
    };
}
