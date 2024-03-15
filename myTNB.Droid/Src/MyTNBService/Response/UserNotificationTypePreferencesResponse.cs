using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.myTNBMenu.Models;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class UserNotificationTypePreferencesResponse : BaseResponse<List<UserNotificationType>>
    {
        public List<UserNotificationType> GetData()
        {
            return Response.Data;
        }
    }
}
