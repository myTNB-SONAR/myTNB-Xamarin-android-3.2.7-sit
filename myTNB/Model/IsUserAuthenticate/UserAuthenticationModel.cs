using Newtonsoft.Json;

namespace myTNB.Model
{
    public class UserAuthenticationModel
    {
        public string userID { set; get; }
        public string displayName { set; get; }
        public string userName { set; get; }
        public string email { set; get; }
        public string dateCreated { set; get; }
        public string lastLoginDate { set; get; }
        public string isError { set; get; }
        public string message { set; get; }
        public string identificationNo { set; get; }
        public string mobileNo { set; get; }
        public string isPhoneVerified { set; get; }

        [JsonIgnore]

        public bool IsVerifiedPhone
        {
            get
            {
                bool ret = false;

                if (!string.IsNullOrEmpty(isPhoneVerified))
                {
                    if(bool.TryParse(isPhoneVerified, out ret))
                    {
                        return ret;
                    }
                }
                return ret;
            }
        }
    }
}