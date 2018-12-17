using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Login.Requests;
using Refit;
using System.Threading.Tasks;
using System.Threading;
using myTNB_Android.Src.myTNBMenu.Requests;
using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.myTNBMenu.Api
{
    public interface IUserNotificationsApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetUserNotificationTypePreferences")]
        Task<UserNotificationTypeResponse> GetNotificationType([Body] UserNotificationTypeRequest userRequest, CancellationToken cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetUserNotificationChannelPreferences")]
        Task<UserNotificationChannelResponse> GetNotificationChannel([Body] UserNotificationChannelRequest userRequest, CancellationToken cancellationToken);
    }
}