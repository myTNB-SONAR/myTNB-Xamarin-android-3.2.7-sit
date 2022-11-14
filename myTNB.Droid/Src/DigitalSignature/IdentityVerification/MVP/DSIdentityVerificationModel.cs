using myTNB.Mobile.AWS.Managers.DS;
using myTNB_Android.Src.Utils;
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
        public int IdentificationType { get; set; }
        public string IdentificationNo { get; set; }
        public string? Status { get; set; }
        public string ApplicationModuleID { get; set; }
        public string IdentificationTypeDescription
        {
            get
            {
                return DSUtility.Instance.GetIdentificationTypeDescription(IdentificationType);
            }
        }

        public bool IsCompletedOnOtherDevice
        {
            get
            {
                return Status.IsValid()
                    && Status.ToUpper() == "PENDING";
            }
        }

        public bool IsVerified
        {
            get
            {
                return Status.IsValid()
                    && Status.ToUpper() == "VERIFIED";
            }
        }
    }
}
