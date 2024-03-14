using System;
using System.Collections.Generic;
using myTNB.Android.Src.myTNBMenu.Models;

namespace myTNB.Android.Src.MyTNBService.Response
{
    public class UserNotificationChannelPreferencesResponse : BaseResponse<List<UserNotificationChannel>>
    {
        public List<UserNotificationChannel> GetData()
        {
            return Response.Data;
        }
    }
}
