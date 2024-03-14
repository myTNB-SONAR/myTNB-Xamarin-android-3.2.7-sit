using System;
using System.Collections.Generic;
using myTNB.Android.Src.myTNBMenu.Models;

namespace myTNB.Android.Src.MyTNBService.Response
{
    public class UserNotificationTypePreferencesResponse : BaseResponse<List<UserNotificationType>>
    {
        public List<UserNotificationType> GetData()
        {
            return Response.Data;
        }
    }
}
