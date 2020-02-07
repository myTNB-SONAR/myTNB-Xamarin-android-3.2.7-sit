using System;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.MyTNBService.InterfaceAPI;
using Refit;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.MyTNBService.Notification
{
    public class NotificationApiImpl
    {
        INotificationAPI api = null;
        HttpClient httpClient = null;

        public NotificationApiImpl()
        {
#if DEBUG || DEVELOP || SIT
            httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            api = RestService.For<INotificationAPI>(httpClient);
#else
            api = RestService.For<INotificationAPI>(Constants.SERVER_URL.END_POINT);
#endif
        }
        public Task<T> DeleteUserNotification<T>([Body] APIBaseRequest request)
        {
            return api.DeleteUserNotification<T>(request, CancellationTokenSourceWrapper.GetToken());
        }

        public Task<T> GetNotificationDetails<T>([Body] APIBaseRequest request)
        {
            return api.GetNotificationDetails<T>(request, CancellationTokenSourceWrapper.GetToken());
        }

        public Task<T> GetUserNotifications<T>([Body] APIBaseRequest request)
        {
            return api.GetUserNotifications<T>(request, CancellationTokenSourceWrapper.GetToken());
        }

        public Task<T> ReadUserNotification<T>([Body] APIBaseRequest request)
        {
            return api.ReadUserNotification<T>(request, CancellationTokenSourceWrapper.GetToken());
        }
    }
}
