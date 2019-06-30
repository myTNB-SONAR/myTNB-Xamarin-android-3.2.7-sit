using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace myTNB_Android.Src.Notifications.Api
{
    public interface INotificationApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/DeleteUserNotification_V3")]
        Task<NotificationApiResponse> DeleteUserNotification([Body] NotificationDeleteRequest request, CancellationTokenSource cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/ReadUserNotification")]
        Task<NotificationApiResponse> ReadUserNotification([Body] NotificationReadRequest request, CancellationTokenSource cancellationToken);
    }
}
