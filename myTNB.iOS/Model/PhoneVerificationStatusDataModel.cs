using System;
using Newtonsoft.Json;

namespace myTNB.Model
{
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
