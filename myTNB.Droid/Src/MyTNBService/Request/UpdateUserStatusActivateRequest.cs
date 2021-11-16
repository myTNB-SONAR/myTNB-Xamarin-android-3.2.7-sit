using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class UpdateUserStatusActivateRequest : BaseRequestV4
    {
        public string userID;
        //public string lang;

        public UpdateUserStatusActivateRequest(string userID)
        {
            this.userID = userID;
            //this.lang = lang;
        }
    }
}
