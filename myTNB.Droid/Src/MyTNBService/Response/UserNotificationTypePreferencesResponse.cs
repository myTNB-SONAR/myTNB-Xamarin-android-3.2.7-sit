using System;
using System.Collections.Generic;
using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class UserNotificationTypePreferencesResponse : BaseResponse<List<UserNotificationType>>
    {
        public List<UserNotificationType> GetData()
        {
            return Response.Data;
        }
    }
}
