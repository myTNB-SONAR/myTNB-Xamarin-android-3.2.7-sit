using System.Threading.Tasks;
using myTNB_Android.Src.Base.Request;
using Refit;

namespace myTNB_Android.Src.MyTNBService.InterfaceAPI
{
    public interface INotificationAPI
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetUserNotifications")]
        Task<T> GetUserNotifications<T>([Body] APIBaseRequest request);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetNotificationDetailedInfo")]
        Task<T> GetNotificationDetailedInfo<T>([Body] APIBaseRequest request);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/DeleteUserNotification")]
        Task<T> DeleteUserNotification<T>([Body] APIBaseRequest request);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/ReadUserNotification")]
        Task<T> ReadUserNotification<T>([Body] APIBaseRequest request);
    }
}
