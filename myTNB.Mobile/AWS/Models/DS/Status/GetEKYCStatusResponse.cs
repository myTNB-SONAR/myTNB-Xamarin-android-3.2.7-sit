using myTNB.Mobile.Extensions;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.DS.Status
{
    public class GetEKYCStatusResponse : BaseResponse<GetEKYCStatusModel>
    {

    }

    public class GetEKYCStatusModel
    {
        [JsonProperty("status")]
        public string Status { set; get; }

        [JsonIgnore]
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