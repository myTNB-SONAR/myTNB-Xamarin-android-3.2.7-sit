using myTNB_Android.Src.NotificationSettings.Models;
using myTNB_Android.Src.NotificationSettings.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.NotificationSettings.Api
{
    public interface IUserPreference
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/SaveUserNotificationChannelPreference")]
        Task<SaveUserPreferenceResponse> SaveUserNotificationChannelPreference([Body] SaveUserNotificationChannelPreferenceRequest request, CancellationToken cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/SaveUserNotificationTypePreference")]
        Task<SaveUserPreferenceResponse> SaveUserNotificationTypePreference([Body] SaveUserNotificationTypePreferenceRequest request, CancellationToken cancellationToken);

    }
}