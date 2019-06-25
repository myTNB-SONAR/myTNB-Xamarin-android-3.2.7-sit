using myTNB_Android.Src.NotificationDetails.Models;
using myTNB_Android.Src.NotificationDetails.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.NotificationDetails.Api
{
    public interface INotificationDetailsApi
    {

        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v5/my_billingssp.asmx/GetNotificationDetailedInfo")]
        //Task<NotificationDetailsResponse> GetNotificationDetailedInfo([Body] NotificationDetailsRequest request, CancellationToken cancellationToken);


        //[Headers("Content-Type:application/json; charset=utf-8")]
        //[Post("/v5/my_billingssp.asmx/DeleteUserNotification")]
        //Task<NotificationDetailsDeleteResponse> DeleteUserNotification([Body] NotificationDetailsDeleteRequest request, CancellationToken cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetNotificationDetailedInfo_V2")]
        Task<NotificationDetailsResponse> GetNotificationDetailedInfoV2([Body] NotificationDetailsRequestV2 request, CancellationToken cancellationToken);


        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/DeleteUserNotification_V2")]
        Task<NotificationDetailsDeleteResponse> DeleteUserNotificationV2([Body] NotificationDetailsDeleteRequestV2 request, CancellationToken cancellationToken);

    }
}