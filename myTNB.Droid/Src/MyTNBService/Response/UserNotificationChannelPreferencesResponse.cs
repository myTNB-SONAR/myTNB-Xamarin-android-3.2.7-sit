using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.myTNBMenu.Models;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class UserNotificationChannelPreferencesResponse : BaseResponse<List<UserNotificationChannel>>
    {
        public List<UserNotificationChannel> GetData()
        {
            return Response.Data;
        }
    }
}
