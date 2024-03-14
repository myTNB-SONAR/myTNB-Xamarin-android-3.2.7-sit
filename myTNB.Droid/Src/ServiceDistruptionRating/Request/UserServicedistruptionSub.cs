using System;
using myTNB.Android.Src.MyTNBService.Request;
using Newtonsoft.Json;

namespace myTNB.Android.Src.ServiceDistruptionRating.Request
{
    public class UserServicedistruptionSub : BaseRequest
    {
        public string email;
        public string sdEventId;

        public UserServicedistruptionSub(string Email, string SdEventId)
        {
            email = Email;
            sdEventId = SdEventId;
        }
    }
}


