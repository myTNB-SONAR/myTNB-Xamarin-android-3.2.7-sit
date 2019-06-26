using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace myTNB_Android.Src.Notifications.Api
{
    public interface INotificationApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetNotificationDetailedInfo_V2")]
        Task<NotificationApiResponse> DeleteUserNotification([Body] NotificationDeleteRequest request, CancellationTokenSource cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetNotificationDetailedInfo_V2")]
        Task<NotificationApiResponse> ReadUserNotification([Body] NotificationDeleteRequest request, CancellationTokenSource cancellationToken);
    }
}
