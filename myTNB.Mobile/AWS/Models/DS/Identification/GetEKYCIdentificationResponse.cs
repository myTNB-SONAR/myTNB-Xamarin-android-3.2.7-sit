using myTNB.Mobile.AWS.Managers.DS;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.DS.Identification
{
    public class GetEKYCIdentificationResponse : BaseResponse<GetEKYCIdentificationModel>
    {

    }

    public class GetEKYCIdentificationModel
    {
        [JsonProperty("userID")]
        public string UserID { set; get; }
        [JsonProperty("identificationType")]
        public int? IdentificationType { set; get; } = null;
        [JsonProperty("identificationNo")]
        public string IdentificationNo { set; get; }

        public string Status { set; get; }

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

        public string IDTypeName
        {
            get
            {
                return DSUtility.Instance.GetIdentificationTypeDescription(IdentificationType);
            }
        }
    }
}