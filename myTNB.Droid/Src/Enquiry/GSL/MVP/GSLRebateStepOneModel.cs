namespace myTNB_Android.Src.Enquiry.GSL.MVP
{
    public class GSLRebateModel
    {
        public string RebateType { get; set; }
        public GSLRebateTenantModel TenantInfo { get; set; }
    }

    public class GSLRebateTenantModel
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
}