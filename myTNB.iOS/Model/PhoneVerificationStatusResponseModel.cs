using Newtonsoft.Json;

namespace myTNB.Model
{
    public class PhoneVerificationStatusResponseModel
    {
        public PhoneVerificationStatusModel d { set; get; }
    }

    public class PhoneVerificationStatusModel : BaseModelV2
    {
        public PhoneVerificationStatusDataModel data { set; get; }
    }

    public class PhoneVerificationStatusDataModel
    {

        public string PhoneNumber
        {
            get;
            set;
        }

        public string IsPhoneVerified
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsVerified
        {
            get
            {
                bool res = true;

                if (bool.TryParse(IsPhoneVerified, out bool parsed))
                {
                    res = parsed;
                }

                return res;
            }

        }
    }
}
