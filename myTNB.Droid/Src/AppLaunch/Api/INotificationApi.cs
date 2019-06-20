using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.AppLaunch.Api
{
    public interface INotificationApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetAppNotificationChannels")]
        Task<NotificationChannelsResponse> GetAppNotificationChannels([Body] NotificationRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetAppNotificationTypes")]
        Task<NotificationTypesResponse> GetAppNotificationTypes([Body] NotificationRequest request, CancellationToken token);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetUserNotifications")]
        Task<UserNotificationResponse> GetUserNotifications([Body] UserNotificationRequest request, CancellationToken token);
    }
}