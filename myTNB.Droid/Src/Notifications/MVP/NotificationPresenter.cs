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
using myTNB_Android.Src.Base;
using myTNB_Android.Src.MyTNBService.ServiceImpl;

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
            UserNotificationDeleteResponse notificationDeleteResponse = null;
            UserNotificationReadResponse notificationReadResponse = null;

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
                        notificationDeleteResponse = await notificationAPI.DeleteUserNotification<UserNotificationDeleteResponse>(new UserNotificationDeleteRequest(selectedNotificationList));
                        if (notificationDeleteResponse.Data.ErrorCode == "7200")
                        {
                            foreach (UserNotificationData userNotificationData in selectedNotificationList)
                            {
                                UserNotificationEntity.RemoveById(userNotificationData.Id);
                            }
                            this.mView.UpdateDeleteNotifications();
                        }
                        else
                        {
                            if (mView.IsActive())
                            {
                                this.mView.HideProgress();
                            }
                            this.mView.ShowFailedErrorMessage(notificationDeleteResponse.Data.ErrorMessage);
                            this.mView.OnFailedNotificationAction();
                        }
                        break;
                    case API_ACTION.READ:
                        notificationReadResponse = await notificationAPI.ReadUserNotification<UserNotificationReadResponse>(new UserNotificationReadRequest(selectedNotificationList));
                        if (notificationReadResponse.Data.ErrorCode == "7200")
                        {
                            foreach(UserNotificationData userNotificationData in selectedNotificationList)
                            {
                                UserNotificationEntity.UpdateIsRead(userNotificationData.Id, true);
                            }
                            this.mView.UpdateReadNotifications();
                        }
                        else
                        {
                            if (mView.IsActive())
                            {
                                this.mView.HideProgress();
                            }
                            this.mView.ShowFailedErrorMessage(notificationReadResponse.Data.ErrorMessage);
                            this.mView.OnFailedNotificationAction();
                        }
                        break;
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
                UserNotificationDetailsRequest request = new UserNotificationDetailsRequest(userNotification.Id, userNotification.NotificationType);
                UserNotificationDetailsResponse response = await notificationAPI.GetNotificationDetailedInfo<UserNotificationDetailsResponse>(request);
                if (response.Data.ErrorCode == "7200")
                {
                    UserNotificationEntity.UpdateIsRead(response.Data.ResponseData.UserNotificationDetail.Id, true);
                    this.mView.ShowDetails(response.Data.ResponseData.UserNotificationDetail, userNotification, position);
                }
                else
                {
                    this.mView.ShowRetryOptionsCancelledException(null);
                }

                //this.mView.ShowDetails(GetMockDetails(userNotification.BCRMNotificationTypeId), userNotification, position);
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
            MyTNBAccountManagement.GetInstance().SetIsNotificationServiceCompleted(false);
            MyTNBAccountManagement.GetInstance().SetIsNotificationServiceMaintenance(false);
            MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(false);
            cts = new CancellationTokenSource();
            this.mView.ShowQueryProgress();

            try
            {
                //var appNotificationChannelsResponse = await api.GetAppNotificationChannels(new NotificationRequest()
                //{
                //    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID
                //}, cts.Token);

                var appNotificationChannelsResponse = await ServiceApiImpl.Instance.AppNotificationChannels(new BaseRequest());

                //var appNotificationTypesResponse = await api.GetAppNotificationTypes(new NotificationRequest()
                //{
                //    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID
                //}, cts.Token);

                var appNotificationTypesResponse = await ServiceApiImpl.Instance.AppNotificationTypes(new BaseRequest());

                if (appNotificationChannelsResponse != null && appNotificationChannelsResponse.Response != null && appNotificationChannelsResponse.Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                {
                    if (appNotificationTypesResponse != null && appNotificationTypesResponse.Response != null && appNotificationTypesResponse.Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
                    {


                        foreach (AppNotificationChannelsResponse.ResponseData notificationChannel in appNotificationChannelsResponse.GetData())
                        {
                            NotificationChannels channel = new NotificationChannels()
                            {
                                Id = notificationChannel.Id,
                                Title = notificationChannel.Title,
                                Code = notificationChannel.Code,
                                PreferenceMode = notificationChannel.PreferenceMode,
                                Type = notificationChannel.Type,
                                CreatedDate = notificationChannel.CreatedDate,
                                MasterId = notificationChannel.MasterId,
                                IsOpted = notificationChannel.IsOpted == "true" ? true : false,
                                ShowInPreference = notificationChannel.ShowInPreference == "true" ? true : false,
                                ShowInFilterList = notificationChannel.ShowInFilterList == "true" ? true : false
                            };
                            NotificationChannelEntity.InsertOrReplace(channel);

                        }

                        foreach (AppNotificationTypesResponse.ResponseData notificationTypes in appNotificationTypesResponse.GetData())
                        {
                            NotificationTypes type = new NotificationTypes()
                            {
                                Id = notificationTypes.Id,
                                Title = notificationTypes.Title,
                                Code = notificationTypes.Code,
                                PreferenceMode = notificationTypes.PreferenceMode,
                                Type = notificationTypes.Type,
                                CreatedDate = notificationTypes.CreatedDate,
                                MasterId = notificationTypes.MasterId,
                                IsOpted = notificationTypes.IsOpted == "true" ? true : false,
                                ShowInPreference = notificationTypes.ShowInPreference == "true" ? true : false,
                                ShowInFilterList = notificationTypes.ShowInFilterList == "true" ? true : false
                            };

                            NotificationTypesEntity.InsertOrReplace(type);
                        }

                        if (UserEntity.IsCurrentlyActive())
                        {
                            try
                            {
                                MyTNBService.Response.UserNotificationResponse response = await notificationAPI.GetUserNotifications<MyTNBService.Response.UserNotificationResponse>(new Base.Request.APIBaseRequest());
                                if (response != null && response.Data != null && response.Data.ErrorCode == "7200")
                                {
                                    if (response.Data.ResponseData != null && response.Data.ResponseData.UserNotificationList != null)
                                    {
                                        try
                                        {
                                            UserNotificationEntity.RemoveAll();
                                        }
                                        catch (System.Exception ne)
                                        {
                                            Utility.LoggingNonFatalError(ne);
                                        }

                                        foreach (UserNotification userNotification in response.Data.ResponseData.UserNotificationList)
                                        {
                                            int newRecord = UserNotificationEntity.InsertOrReplace(userNotification);
                                        }
                                        this.mView.ShowView();
                                        this.mView.ClearAdapter();
                                        this.ShowFilteredList();
                                        MyTNBAccountManagement.GetInstance().SetIsNotificationServiceCompleted(true);
                                    }
                                    else
                                    {
                                        this.mView.ShowRefreshView(response.Data.RefreshMessage, response.Data.RefreshBtnText);
                                        MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                                    }
                                }
                                else if(response != null && response.Data != null && response.Data.ErrorCode == "8400")
                                {
                                    MyTNBAccountManagement.GetInstance().SetIsNotificationServiceMaintenance(true);
                                    // TODO: Show Maintenance Screen
                                }
                                else
                                {
                                    MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                                    this.mView.ShowRefreshView(response.Data.RefreshMessage, response.Data.RefreshBtnText);
                                }

                                this.mView.HideQueryProgress();
                            }
                            catch (ApiException apiException)
                            {

                                this.mView.HideQueryProgress();
                                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                                this.mView.ShowRefreshView(null, null);
                                Utility.LoggingNonFatalError(apiException);
                            }
                            catch (Exception e)
                            {
                                this.mView.HideQueryProgress();
                                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                                this.mView.ShowRefreshView(null, null);
                                Utility.LoggingNonFatalError(e);
                            }
                        }
                        else
                        {
                            this.mView.HideQueryProgress();
                        }

                    }
                    else
                    {
                        this.mView.HideQueryProgress();
                        MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                        this.mView.ShowRefreshView(null, null);
                    }

                }
                else
                {
                    this.mView.HideQueryProgress();
                    MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                    this.mView.ShowRefreshView(null, null);
                }
            }
            catch (ApiException apiException)
            {

                this.mView.HideQueryProgress();
                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                this.mView.ShowRefreshView(null, null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {

                this.mView.HideQueryProgress();
                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                this.mView.ShowRefreshView(null, null);
                Utility.LoggingNonFatalError(e);
            }

        }

//        public async void QueryNotifications(string deviceId)
//        {
//            cts = new CancellationTokenSource();
//            if (mView.IsActive())
//            {
//                this.mView.ShowQueryProgress();
//            }
//#if DEBUG
//            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
//            var api = RestService.For<AppLaunch.Api.INotificationApi>(httpClient);
//#else
//            var api = RestService.For<AppLaunch.Api.INotificationApi>(Constants.SERVER_URL.END_POINT);
//#endif

//            try
//            {
//                MyTNBService.Response.UserNotificationResponse response = await notificationAPI.GetUserNotifications<MyTNBService.Response.UserNotificationResponse>(new Base.Request.APIBaseRequest());
//                if (response != null && response.Data != null && response.Data.ErrorCode == "7200")
//                {

//                    if (response.Data.ResponseData != null && response.Data.ResponseData.UserNotificationList != null &&
//                        response.Data.ResponseData.UserNotificationList.Count > 0)
//                    {
//                        foreach (UserNotification userNotification in response.Data.ResponseData.UserNotificationList)
//                        {
//                            // tODO : SAVE ALL NOTIFICATIONs
//                            int newRecord = UserNotificationEntity.InsertOrReplace(userNotification);
//                        }
//                    }
//                }

//                UserEntity loggedUser = UserEntity.GetActive();
//                var userNotificationResponse = await api.GetUserNotifications(new AppLaunch.Requests.UserNotificationRequest()
//                {
//                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
//                    Email = loggedUser.Email,
//                    DeviceId = deviceId

//                }, cts.Token);

//                if (mView.IsActive())
//                {
//                    this.mView.HideQueryProgress();
//                }

//                if (userNotificationResponse != null && userNotificationResponse.Data != null && userNotificationResponse.Data.Status.ToUpper() == Constants.REFRESH_MODE)
//                {
//                    this.mView.ShowRefreshView(userNotificationResponse.Data.RefreshMessage, userNotificationResponse.Data.RefreshBtnText);
//                }
//                else if (userNotificationResponse != null && userNotificationResponse.Data != null && !userNotificationResponse.Data.IsError)
//                {
//                    if (userNotificationResponse.Data.Data.Count() > 0)
//                    {
//                        try
//                        {
//                            UserNotificationEntity.RemoveAll();
//                        }
//                        catch (System.Exception ne)
//                        {
//                            Utility.LoggingNonFatalError(ne);
//                        }
//                    }
//                    foreach (UserNotification userNotification in userNotificationResponse.Data.Data)
//                    {
//                        // tODO : SAVE ALL NOTIFICATIONs
//                        UserNotificationEntity.InsertOrReplace(userNotification);
//                    }
//                    this.mView.ShowView();
//                    this.mView.ClearAdapter();
//                    this.ShowFilteredList();
//                }
//                else
//                {
//                    this.mView.ShowRefreshView(null, null);
//                }
//            }
//            catch (ApiException apiException)
//            {

//                if (mView.IsActive())
//                {
//                    this.mView.HideQueryProgress();
//                }
//                this.mView.ShowRefreshView(null, null);
//                Utility.LoggingNonFatalError(apiException);
//            }
//            catch (Exception e)
//            {

//                if (mView.IsActive())
//                {
//                    this.mView.HideQueryProgress();
//                }
//                this.mView.ShowRefreshView(null, null);
//                Utility.LoggingNonFatalError(e);
//            }

//        }

        public void Start()
        {
            notificationApi = new NotificationApiCall();
            ShowFilteredList();

        }

        public void InitialSetFilterName()
        {
            try
            {
                NotificationFilterEntity filter = new NotificationFilterEntity();
                filter = NotificationFilterEntity.GetActive();
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty(filter.Title))
                    {
                        this.mView.ShowNotificationFilterName(filter.Title);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
                                    if (userNotificationData.NotificationType != "ODN")
                                    {
                                        if (UserEntity.GetActive().Email.Equals(userNotificationData.Email) &&
                                            MyTNBAccountManagement.GetInstance().IsAccountNumberExist(userNotificationData.AccountNum))
                                        {
                                            listOfNotifications.Add(UserNotificationData.Get(entity, notificationTypesEntity.Code));
                                        }
                                    }
                                    else
                                    {
                                        listOfNotifications.Add(UserNotificationData.Get(entity, notificationTypesEntity.Code));
                                    }
                                }
                            }
                        }

                    }
                }

                ////MOCK DATA
                //listOfNotifications.Clear();
                //UserNotificationData data = new UserNotificationData();
                //data.BCRMNotificationTypeId = "01";
                //data.CreatedDate = "9/1/2019 5:00:00 PM";
                //data.IsRead = false;
                //data.Title = "New Bill";
                //data.Message = "Dear customer, your TNB bill RM5…";
                //listOfNotifications.Add(data);

                //data = new UserNotificationData();
                //data.BCRMNotificationTypeId = "02";
                //data.CreatedDate = "9/1/2019 5:00:00 PM";
                //data.IsRead = false;
                //data.Title = "Bill Due";
                //data.Message = "Dear customer, your Sep 2017 bill… ";
                //listOfNotifications.Add(data);

                //data = new UserNotificationData();
                //data.BCRMNotificationTypeId = "03";
                //data.CreatedDate = "9/1/2019 5:00:00 PM";
                //data.IsRead = false;
                //data.Title = "Disconnection Notice";
                //data.Message = "Dear customer, enjoy special promo…";
                //listOfNotifications.Add(data);

                //data = new UserNotificationData();
                //data.BCRMNotificationTypeId = "04";
                //data.CreatedDate = "9/1/2019 5:00:00 PM";
                //data.IsRead = false;
                //data.Title = "Disconnection";
                //data.Message = "Dear customer, your connection…";
                //listOfNotifications.Add(data);

                //data = new UserNotificationData();
                //data.BCRMNotificationTypeId = "05";
                //data.CreatedDate = "9/1/2019 5:00:00 PM";
                //data.IsRead = false;
                //data.Title = "Reconnection";
                //data.Message = "Dear customer, your connection…";
                //listOfNotifications.Add(data);

                //data = new UserNotificationData();
                //data.BCRMNotificationTypeId = "99";
                //data.CreatedDate = "9/1/2019 5:00:00 PM";
                //data.IsRead = false;
                //data.Title = "Maintenance";
                //data.Message = "Dear customer, kindly be informed…";
                //data.NotificationType = "1000011";
                //data.NotificationTypeId = "1001";
                //listOfNotifications.Add(data);

                //data = new UserNotificationData();
                //data.BCRMNotificationTypeId = "0009";
                //data.CreatedDate = "9/1/2019 5:00:00 PM";
                //data.IsRead = false;
                //data.Title = "Self Meter Reading Open";
                //data.Message = "Dear customer, submit your meter…";
                //listOfNotifications.Add(data);

                //data = new UserNotificationData();
                //data.BCRMNotificationTypeId = "0010";
                //data.CreatedDate = "9/1/2019 5:00:00 PM";
                //data.IsRead = false;
                //data.Title = "Self Meter Reading Remind";
                //data.Message = "Dear customer, submit your meter…";
                //listOfNotifications.Add(data);

                //data = new UserNotificationData();
                //data.BCRMNotificationTypeId = "0011";
                //data.CreatedDate = "9/1/2019 5:00:00 PM";
                //data.IsRead = false;
                //data.Title = "Self Meter Reading Disabled";
                //data.Message = "Dear customer, submit your meter…";
                //listOfNotifications.Add(data);

                //data = new UserNotificationData();
                //data.BCRMNotificationTypeId = "50";
                //data.CreatedDate = "9/1/2019 5:00:00 PM";
                //data.IsRead = false;
                //data.Title = "Self Meter Reading Application Success";
                //data.Message = "Dear customer, submit your meter…";
                //listOfNotifications.Add(data);

                //data = new UserNotificationData();
                //data.BCRMNotificationTypeId = "51";
                //data.CreatedDate = "9/1/2019 5:00:00 PM";
                //data.IsRead = false;
                //data.Title = "Self Meter Reading Application Failed";
                //data.Message = "Dear customer, submit your meter…";
                //listOfNotifications.Add(data);

                //data = new UserNotificationData();
                //data.BCRMNotificationTypeId = "52";
                //data.CreatedDate = "9/1/2019 5:00:00 PM";
                //data.IsRead = false;
                //data.Title = "Self Meter Reading Disabled Success";
                //data.Message = "Dear customer, submit your meter…";
                //listOfNotifications.Add(data);

                //data = new UserNotificationData();
                //data.BCRMNotificationTypeId = "53";
                //data.CreatedDate = "9/1/2019 5:00:00 PM";
                //data.IsRead = false;
                //data.Title = "Self Meter Reading Disabled Failed";
                //data.Message = "Dear customer, submit your meter…";
                //listOfNotifications.Add(data);

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
            data.AccountNum = "220914778610";
            data.BCRMNotificationTypeId = bcrmType;
            data.Title = "Testing of Reseed validation";
            data.Message = "Your bill is {0}. Got a minute? Make a quick and easy payment on the myTNB app now. <br/><br/>Account: #accountName#";
            return data;
        }
    }
}
