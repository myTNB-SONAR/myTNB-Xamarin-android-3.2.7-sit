using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Text;
using myTNB_Android.Src.AppLaunch.Api;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.NotificationDetails.Api;
using myTNB_Android.Src.Notifications.Models;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;
using myTNB_Android.Src.AppLaunch.Api;
using myTNB_Android.Src.AppLaunch.Requests;
using static Android.Widget.CompoundButton;
using myTNB_Android.Src.NotificationDetails.Requests;
using System.Threading.Tasks;
using myTNB_Android.Src.Notifications.Api;

namespace myTNB_Android.Src.Notifications.MVP
{
    enum API_ACTION
    {
        DELETE,
        READ
    }
    public class NotificationPresenter : NotificationContract.IUserActionsListener
    {
        private NotificationContract.IView mView;
        NotificationContract.IApiNotification notificationApi;
        CancellationTokenSource cts;
        List<UserNotificationData> selectedNotificationList;
        public NotificationPresenter(NotificationContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        private async Task InvokeNotificationApi(API_ACTION apiAction)
        {
            NotificationApiResponse notificationApiResponse = null;
            selectedNotificationList = new List<UserNotificationData>();
            foreach (UserNotificationData notification in this.mView.GetNotificationList())
            {
                if (notification.IsSelected)
                {
                    selectedNotificationList.Add(notification);
                }
            }
            try
            {
                this.mView.ShowProgress();
                switch (apiAction)
                {
                    case API_ACTION.DELETE:
                        notificationApiResponse = await notificationApi.DeleteUserNotification(this.mView.GetDeviceId(), selectedNotificationList);
                        if (!notificationApiResponse.Data.IsError)
                        {
                            foreach (UserNotificationData userNotificationData in selectedNotificationList)
                            {
                                UserNotificationEntity.RemoveById(userNotificationData.Id);
                            }
                            this.mView.UpdateDeleteNotifications();
                        }
                        break;
                    case API_ACTION.READ:
                        notificationApiResponse = await notificationApi.ReadUserNotification(this.mView.GetDeviceId(), selectedNotificationList);
                        if (!notificationApiResponse.Data.IsError)
                        {
                            foreach(UserNotificationData userNotificationData in selectedNotificationList)
                            {
                                UserNotificationEntity.UpdateIsRead(userNotificationData.Id, true);
                            }
                            this.mView.UpdateReadNotifications();
                        }
                        break;
                }
                if (notificationApiResponse.Data.IsError)
                {
                    this.mView.ShowFailedErrorMessage(notificationApiResponse.Data.Message);
                    this.mView.OnFailedNotificationAction();
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                this.mView.OnFailedNotificationAction();
                //Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                this.mView.OnFailedNotificationAction();
                //Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                this.mView.OnFailedNotificationAction();
                //Utility.LoggingNonFatalError(e);
            }
        }

        public void DeleteAllSelectedNotifications()
        {
            _ = InvokeNotificationApi(API_ACTION.DELETE);
        }

        public void ReadAllSelectedNotifications()
        {
            _ = InvokeNotificationApi(API_ACTION.READ);
        }

        public void EditNotification()
        {
            throw new NotImplementedException();
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if (resultCode == Result.Ok)
                {
                    this.mView.ClearAdapter();
                    this.ShowFilteredList();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void OnSelectedNotificationItem(UserNotificationData userNotification, int position)
        {
            cts = new CancellationTokenSource();
            if (mView.IsActive())
            {
                this.mView.ShowProgress();
            }
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var detailNotificationApi = RestService.For<INotificationDetailsApi>(httpClient);
#else
            var detailNotificationApi = RestService.For<INotificationDetailsApi>(Constants.SERVER_URL.END_POINT);
#endif
            try
            {
                var detailNotificationResponse = await detailNotificationApi.GetNotificationDetailedInfoV2(new NotificationDetails.Requests.NotificationDetailsRequestV2()
                {
                    ApiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                    NotificationId = userNotification.Id,
                    NotificationType = userNotification.NotificationType,
                    Email = UserEntity.GetActive().Email,
                    DeviceId = this.mView.GetDeviceId(),
                    SSPUserId = UserEntity.GetActive().UserID
                }, cts.Token);

                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }

                if (!detailNotificationResponse.Data.IsError)
                {
                    UserNotificationEntity.UpdateIsRead(detailNotificationResponse.Data.Data.Id, true);
                    NotificationTypesEntity entity = NotificationTypesEntity.GetById(userNotification.NotificationTypeId);

                    if (entity != null)
                    {
                        this.mView.ShowDetails(detailNotificationResponse.Data.Data, userNotification, position);
                    }

                }

            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgress();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnShowNotificationFilter()
        {
            this.mView.ShowNotificationFilter();
        }

        public async void QueryOnLoad(string deviceId)
        {
            cts = new CancellationTokenSource();
            if (mView.IsActive())
            {
                this.mView.ShowQueryProgress();
            }
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<AppLaunch.Api.INotificationApi>(httpClient);
#else
            var api = RestService.For<AppLaunch.Api.INotificationApi>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                var appNotificationChannelsResponse = await api.GetAppNotificationChannels(new NotificationRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID
                }, cts.Token);

                var appNotificationTypesResponse = await api.GetAppNotificationTypes(new NotificationRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID
                }, cts.Token);

                if (!appNotificationChannelsResponse.Data.IsError)
                {
                    if (!appNotificationTypesResponse.Data.IsError)
                    {


                        foreach (NotificationChannels notificationChannel in appNotificationChannelsResponse.Data.Data)
                        {
                            NotificationChannelEntity.InsertOrReplace(notificationChannel);

                        }

                        foreach (NotificationTypes notificationTypes in appNotificationTypesResponse.Data.Data)
                        {
                            NotificationTypesEntity.InsertOrReplace(notificationTypes);

                        }

                        if (UserEntity.IsCurrentlyActive())
                        {
                            try
                            {
                                UserEntity loggedUser = UserEntity.GetActive();
                                var userNotificationResponse = await api.GetUserNotifications(new UserNotificationRequest()
                                {
                                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                                    Email = loggedUser.Email,
                                    DeviceId = deviceId

                                }, cts.Token);

                                if (mView.IsActive())
                                {
                                    this.mView.HideQueryProgress();
                                }

                                if (userNotificationResponse != null && userNotificationResponse.Data != null && userNotificationResponse.Data.Status.ToUpper() == Constants.REFRESH_MODE)
                                {
                                    this.mView.ShowRefreshView(userNotificationResponse.Data.RefreshMessage, userNotificationResponse.Data.RefreshBtnText);
                                }
                                else if (userNotificationResponse != null && userNotificationResponse.Data != null && !userNotificationResponse.Data.IsError)
                                {
                                    if (userNotificationResponse.Data.Data.Count() > 0)
                                    {
                                        try
                                        {
                                            UserNotificationEntity.RemoveAll();
                                        }
                                        catch (System.Exception ne)
                                        {
                                            Utility.LoggingNonFatalError(ne);
                                        }
                                    }
                                    foreach (UserNotification userNotification in userNotificationResponse.Data.Data)
                                    {
                                        // tODO : SAVE ALL NOTIFICATIONs
                                        UserNotificationEntity.InsertOrReplace(userNotification);
                                    }
                                    this.mView.ShowView();
                                    this.mView.ClearAdapter();
                                    this.ShowFilteredList();
                                }
                                else
                                {
                                    this.mView.ShowRefreshView(null, null);
                                }
                            }
                            catch (ApiException apiException)
                            {

                                if (mView.IsActive())
                                {
                                    this.mView.HideQueryProgress();
                                }
                                this.mView.ShowRefreshView(null, null);
                                Utility.LoggingNonFatalError(apiException);
                            }
                            catch (Exception e)
                            {
                                if (mView.IsActive())
                                {
                                    this.mView.HideQueryProgress();
                                }
                                this.mView.ShowRefreshView(null, null);
                                Utility.LoggingNonFatalError(e);
                            }
                        }
                        else
                        {
                            if (mView.IsActive())
                            {
                                this.mView.HideQueryProgress();
                            }
                        }

                    }
                    else
                    {
                        if (mView.IsActive())
                        {
                            this.mView.HideQueryProgress();
                        }
                        this.mView.ShowRefreshView(null, null);
                    }

                }
                else
                {
                    if (mView.IsActive())
                    {
                        this.mView.HideQueryProgress();
                    }
                    this.mView.ShowRefreshView(null, null);
                }
            }
            catch (ApiException apiException)
            {

                if (mView.IsActive())
                {
                    this.mView.HideQueryProgress();
                }
                this.mView.ShowRefreshView(null, null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {

                if (mView.IsActive())
                {
                    this.mView.HideQueryProgress();
                }
                this.mView.ShowRefreshView(null, null);
                Utility.LoggingNonFatalError(e);
            }

        }

        public async void QueryNotifications(string deviceId)
        {
            cts = new CancellationTokenSource();
            if (mView.IsActive())
            {
                this.mView.ShowQueryProgress();
            }
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<AppLaunch.Api.INotificationApi>(httpClient);
#else
            var api = RestService.For<AppLaunch.Api.INotificationApi>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                UserEntity loggedUser = UserEntity.GetActive();
                var userNotificationResponse = await api.GetUserNotifications(new UserNotificationRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    Email = loggedUser.Email,
                    DeviceId = deviceId

                }, cts.Token);

                if (mView.IsActive())
                {
                    this.mView.HideQueryProgress();
                }

                if (userNotificationResponse != null && userNotificationResponse.Data != null && userNotificationResponse.Data.Status.ToUpper() == Constants.REFRESH_MODE)
                {
                    this.mView.ShowRefreshView(userNotificationResponse.Data.RefreshMessage, userNotificationResponse.Data.RefreshBtnText);
                }
                else if (userNotificationResponse != null && userNotificationResponse.Data != null && !userNotificationResponse.Data.IsError)
                {
                    if (userNotificationResponse.Data.Data.Count() > 0)
                    {
                        try
                        {
                            UserNotificationEntity.RemoveAll();
                        }
                        catch (System.Exception ne)
                        {
                            Utility.LoggingNonFatalError(ne);
                        }
                    }
                    foreach (UserNotification userNotification in userNotificationResponse.Data.Data)
                    {
                        // tODO : SAVE ALL NOTIFICATIONs
                        UserNotificationEntity.InsertOrReplace(userNotification);
                    }
                    this.mView.ShowView();
                    this.mView.ClearAdapter();
                    this.ShowFilteredList();
                }
                else
                {
                    this.mView.ShowRefreshView(null, null);
                }
            }
            catch (ApiException apiException)
            {

                if (mView.IsActive())
                {
                    this.mView.HideQueryProgress();
                }
                this.mView.ShowRefreshView(null, null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {

                if (mView.IsActive())
                {
                    this.mView.HideQueryProgress();
                }
                this.mView.ShowRefreshView(null, null);
                Utility.LoggingNonFatalError(e);
            }

        }

        public void Start()
        {
            notificationApi = new NotificationApiCall();
            ShowFilteredList();

        }

        public void ShowFilteredList()
        {
            try
            {
                NotificationFilterEntity filter = new NotificationFilterEntity();
                filter = NotificationFilterEntity.GetActive();
                List<UserNotificationEntity> entities = new List<UserNotificationEntity>();
                entities = UserNotificationEntity.ListAllActive();

                if (filter != null)
                {
                    if (!string.IsNullOrEmpty(filter.Id))
                    {
                        if (!filter.Id.Equals(Constants.ZERO_INDEX_FILTER))
                        {
                            entities = UserNotificationEntity.ListFiltered(filter.Id);
                        }
                    }
                    if (!string.IsNullOrEmpty(filter.Title))
                    {
                        this.mView.ShowNotificationFilterName(filter.Title);
                    }
                }

                List<UserNotificationData> listOfNotifications = new List<UserNotificationData>();
                if (entities != null && entities.Count() > 0)
                {
                    foreach (UserNotificationEntity entity in entities)
                    {
                        if (!TextUtils.IsEmpty(entity.NotificationTypeId))
                        {
                            NotificationTypesEntity notificationTypesEntity = new NotificationTypesEntity();
                            notificationTypesEntity = NotificationTypesEntity.GetById(entity.NotificationTypeId);
                            if (!TextUtils.IsEmpty(notificationTypesEntity.Code))
                            {
                                UserNotificationData userNotificationData = UserNotificationData.Get(entity, notificationTypesEntity.Code);
                                if (!userNotificationData.IsDeleted)
                                {
                                    listOfNotifications.Add(UserNotificationData.Get(entity, notificationTypesEntity.Code));
                                }
                            }
                        }

                    }
                }

                this.mView.ShowNotificationsList(listOfNotifications);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
