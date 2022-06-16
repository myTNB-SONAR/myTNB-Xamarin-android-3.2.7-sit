namespace myTNB_Android.Src.DigitalSignature.IdentityVerification.MVP
{
    public class DSIdentityVerificationModel { }

    public class DSIdTypeSelectorModel
    {
        public string key { get; set; }
        public string description { get; set; }
    }

    public class DSDynamicLinkParamsModel
    {
        public string? UserID { get; set; }
        public bool IsContractorApplied { get; set; } = false;
        public string? AppRef { get; set; }
    }
}
