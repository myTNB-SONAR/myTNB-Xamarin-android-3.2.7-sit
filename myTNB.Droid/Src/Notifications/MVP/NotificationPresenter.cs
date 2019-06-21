﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Notifications.Models;
using Android.Text;
using System.Net;
using myTNB_Android.Src.Utils;
using System.Net.Http;
using Refit;
using myTNB_Android.Src.NotificationDetails.Api;
using System.Threading;
using Newtonsoft.Json;
using myTNB_Android.Src.AppLaunch.Api;
using myTNB_Android.Src.AppLaunch.Requests;
using static Android.Widget.CompoundButton;

namespace myTNB_Android.Src.Notifications.MVP
{
    public class NotificationPresenter : NotificationContract.IUserActionsListener
    {
        private NotificationContract.IView mView;
        CancellationTokenSource cts;
        public NotificationPresenter(NotificationContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void DeleteNotificationByPosition(int position)
        {
            //TODO
        }

        public void DeleteAllSelectedNotifications()
        {
            //TODO
        }

        public void EditNotification()
        {
            throw new NotImplementedException();
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try {
            if (requestCode == Constants.NOTIFICATION_DETAILS_REQUEST_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    int position = data.Extras.GetInt(Constants.SELECTED_NOTIFICATION_ITEM_POSITION , -1);
                   
                    this.mView.UpdateIsReadNotificationItem(position , true);

                    bool isDeleted = data.Extras.GetBoolean(Constants.ACTION_IS_DELETE , false);
                    if (isDeleted)
                    {
                        this.mView.UpdateIsDeleteNotificationItem(position , true);
                        this.mView.ShowNotificationRemoved();
                    }
                }
            }
            else if (requestCode == Constants.NOTIFICATION_FILTER_REQUEST_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    this.mView.ClearAdapter();
                    this.ShowFilteredList();
                }
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
            if (mView.IsActive()) {
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
                    UserNotificationEntity.UpdateIsRead(detailNotificationResponse.Data.Data.Id , true);
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
            var api = RestService.For<INotificationApi>(httpClient);
#else
            var api = RestService.For<INotificationApi>(Constants.SERVER_URL.END_POINT);
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


            if (mView.IsActive())
                {
                    this.mView.HideQueryProgress();
                }

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
                            UserEntity loggedUser = UserEntity.GetActive();
                            var userNotificationResponse = await api.GetUserNotifications(new UserNotificationRequest()
                            {
                                ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                                Email = loggedUser.Email,
                                DeviceId = deviceId

                            }, cts.Token);

                            if (!userNotificationResponse.Data.IsError)
                            {
                                foreach (UserNotification userNotification in userNotificationResponse.Data.Data)
                                {
                                    // tODO : SAVE ALL NOTIFICATIONs
                                    UserNotificationEntity.InsertOrReplace(userNotification);
                                }

                            }

                            this.mView.ClearAdapter();
                            this.ShowFilteredList();


                        }

                    }

                }

            }
            catch (ApiException apiException)
            {

            if (mView.IsActive())
                {
                    this.mView.HideQueryProgress();
                }
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {

            if (mView.IsActive())
                {
                    this.mView.HideQueryProgress();
                }
                Utility.LoggingNonFatalError(e);
            }

        }

        public void ReadNotificationByPosition(int position)
        {
            //TODO
        }

        public void ReadAllSelectedNotifications()
        {
            //TODO
        }

        public void Start()
        {
            
            ShowFilteredList();
           
        }

        void ShowFilteredList()
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
                                listOfNotifications.Add(UserNotificationData.Get(entity, notificationTypesEntity.Code));
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