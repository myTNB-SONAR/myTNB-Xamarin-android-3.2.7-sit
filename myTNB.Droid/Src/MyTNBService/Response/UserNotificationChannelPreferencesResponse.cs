using System;
using System.Collections.Generic;
using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class UserNotificationChannelPreferencesResponse : BaseResponse<List<UserNotificationChannel>>
    {
        public List<UserNotificationChannel> GetData()
        {
            return Response.Data;
        }
    }
}
