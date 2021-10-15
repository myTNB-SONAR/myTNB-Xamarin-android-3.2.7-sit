namespace myTNB_Android.Src.Enquiry
{
    public class EnquiryConstants
    {
        internal static readonly int COUNTRY_CODE_SELECT_REQUEST = 1;
        internal static readonly int SELECT_REBATE_TYPE_REQ_CODE = 2;

        internal static readonly string UPLOAD_DOCUMENT_DATE_FORMAT = "yyyyMMdd";

        internal static readonly int GSL_CATEGORY_ID = 1;
    }

    public enum EnquiryAccountDetailType
    {
        FULL_NAME,
        EMAIL_ADDRESS,
        MOBILE_NUMBER,
        NONE
    };
}
