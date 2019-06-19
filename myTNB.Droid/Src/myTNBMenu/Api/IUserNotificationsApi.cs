using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

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