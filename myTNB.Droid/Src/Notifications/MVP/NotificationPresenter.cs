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
using myTNB_Android.Src.MyTNBService.Notification;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.Request;

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
        NotificationApiImpl notificationAPI;
        public NotificationPresenter(NotificationContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            notificationAPI = new NotificationApiImpl();
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

        public async void OnShowNotificationDetails(UserNotificationData userNotification, int position)
        {
            try
            {
                this.mView.ShowProgress();
                //UserNotificationDetailsRequest request = new UserNotificationDetailsRequest(userNotification.NotificationTypeId, userNotification.NotificationType);
                //UserNotificationDetailsResponse response = await notificationAPI.GetNotificationDetailedInfo<UserNotificationDetailsResponse>(request);
                //if (response.Data.ErrorCode == "7200")
                //{
                //    UserNotificationEntity.UpdateIsRead(response.Data.ResponseData.UserNotificationDetail.Id, true);
                //    this.mView.ShowDetails(response.Data.ResponseData.UserNotificationDetail, userNotification, position);
                //    //NotificationTypesEntity entity = NotificationTypesEntity.GetById(userNotification.NotificationTypeId);

                //    //if (entity != null)
                //    //{
                //    //    this.mView.ShowDetails(response.Data.ResponseData.UserNotificationDetail, userNotification, position);
                //    //}
                //}

                this.mView.ShowDetails(GetMockDetails(userNotification.BCRMNotificationTypeId), userNotification, position);
                this.mView.HideProgress();
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
                                var userNotificationResponse = await api.GetUserNotifications(new AppLaunch.Requests.UserNotificationRequest()
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
                var userNotificationResponse = await api.GetUserNotifications(new AppLaunch.Requests.UserNotificationRequest()
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

                //if (notificationData.BCRMNotificationTypeId.Equals("01"))
                //{
                //    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_new_bill));
                //}
                //else if (notificationData.BCRMNotificationTypeId.Equals("02"))
                //{
                //    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_bill_due));
                //}
                //else if (notificationData.BCRMNotificationTypeId.Equals("03"))
                //{
                //    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_dunning_disconnection));
                //}
                //else if (notificationData.BCRMNotificationTypeId.Equals("04"))
                //{
                //    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_disconnection));
                //}
                //else if (notificationData.BCRMNotificationTypeId.Equals("05"))
                //{
                //    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_reconnection));
                //}
                //else if (notificationData.BCRMNotificationTypeId.Equals("06"))
                //{
                //    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_smr));
                //}
                ////else if (notificationData.BCRMNotificationTypeId.Equals("97"))
                ////{
                ////    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.ic_notification_promo));
                ////}
                ////else if (notificationData.BCRMNotificationTypeId.Equals("98"))
                ////{
                ////    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.ic_notification_news));
                ////}
                //else if (notificationData.BCRMNotificationTypeId.Equals("99"))
                //{
                //    viewHolder.notificationIcon.SetImageDrawable(ContextCompat.GetDrawable(notifyContext, Resource.Drawable.notification_settings));
                //}

                listOfNotifications.Clear();
                UserNotificationData data = new UserNotificationData();
                data.BCRMNotificationTypeId = "01";
                data.CreatedDate = "9/1/2019 5:00:00 PM";
                data.IsRead = false;
                data.Title = "New Bill";
                data.Message = "Dear customer, your TNB bill RM5…";
                listOfNotifications.Add(data);

                data = new UserNotificationData();
                data.BCRMNotificationTypeId = "02";
                data.CreatedDate = "9/1/2019 5:00:00 PM";
                data.IsRead = false;
                data.Title = "Bill Due";
                data.Message = "Dear customer, your Sep 2017 bill… ";
                listOfNotifications.Add(data);

                data = new UserNotificationData();
                data.BCRMNotificationTypeId = "03";
                data.CreatedDate = "9/1/2019 5:00:00 PM";
                data.IsRead = false;
                data.Title = "Disconnection Notice";
                data.Message = "Dear customer, enjoy special promo…";
                listOfNotifications.Add(data);

                data = new UserNotificationData();
                data.BCRMNotificationTypeId = "04";
                data.CreatedDate = "9/1/2019 5:00:00 PM";
                data.IsRead = false;
                data.Title = "Disconnection";
                data.Message = "Dear customer, your connection…";
                listOfNotifications.Add(data);

                data = new UserNotificationData();
                data.BCRMNotificationTypeId = "05";
                data.CreatedDate = "9/1/2019 5:00:00 PM";
                data.IsRead = false;
                data.Title = "Reconnection";
                data.Message = "Dear customer, your connection…";
                listOfNotifications.Add(data);

                data = new UserNotificationData();
                data.BCRMNotificationTypeId = "0009";
                data.CreatedDate = "9/1/2019 5:00:00 PM";
                data.IsRead = false;
                data.Title = "Self Meter Reading";
                data.Message = "Dear customer, submit your meter…";
                listOfNotifications.Add(data);

                data = new UserNotificationData();
                data.BCRMNotificationTypeId = "99";
                data.CreatedDate = "9/1/2019 5:00:00 PM";
                data.IsRead = false;
                data.Title = "Maintenance";
                data.Message = "Dear customer, kindly be informed…";
                data.NotificationType = "1000011";
                data.NotificationTypeId = "1001";
                listOfNotifications.Add(data);

                this.mView.ShowNotificationsList(listOfNotifications);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public NotificationDetails.Models.NotificationDetails GetMockDetails(string bcrmType)
        {
            NotificationDetails.Models.NotificationDetails data = new NotificationDetails.Models.NotificationDetails();
            data.BCRMNotificationTypeId = bcrmType;
            data.Title = "Testing of Reseed validation";
            data.Message = "Message for Testing of Reseed validation";
            return data;
        }
    }
}
