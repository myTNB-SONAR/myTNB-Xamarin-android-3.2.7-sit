using System;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.ServiceDistruptionRating.Request
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


