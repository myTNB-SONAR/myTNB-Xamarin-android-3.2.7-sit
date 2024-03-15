using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class UpdateUserStatusActivateRequest
    {
        public string userID;
        public string lang;

        public UpdateUserStatusActivateRequest(string userID, string lang)
        {
            this.userID = userID;
            this.lang = lang;
        }
    }
}
