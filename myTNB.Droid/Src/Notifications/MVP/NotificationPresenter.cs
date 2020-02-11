﻿using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Text;
using myTNB_Android.Src.AppLaunch.Api;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Database.Model;
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
using myTNB_Android.Src.SummaryDashBoard.Models;

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
                UserNotificationDetailsResponse response = await notificationAPI.GetNotificationDetails<UserNotificationDetailsResponse>(request);
                if (response.Data.ErrorCode == "7200")
                {
                    Utility.SetIsPayDisableNotFromAppLaunch(!response.Data.IsPayEnabled);
                    UserNotificationEntity.UpdateIsRead(response.Data.ResponseData.UserNotificationDetail.Id, true);
                    this.mView.ShowDetails(response.Data.ResponseData.UserNotificationDetail, userNotification, position);
                }
                else
                {
                    this.mView.ShowRetryOptionsCancelledException(null);
                }

                ////MOCK RESPONSE
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

        public void OnShowNotificationFilter()
        {
            this.mView.ShowNotificationFilter();
        }

        public async void QueryOnLoad(string deviceId)
        {
            MyTNBAccountManagement.GetInstance().SetIsNotificationServiceCompleted(false);
            MyTNBAccountManagement.GetInstance().SetIsNotificationServiceMaintenance(false);
            MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(false);
            List<Notifications.Models.UserNotificationData> ToBeDeleteList = new List<Notifications.Models.UserNotificationData>();
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
                                            try
                                            {
                                                if ((userNotification.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_BILL_DUE_ID) || userNotification.BCRMNotificationTypeId.Equals(Constants.BCRM_NOTIFICATION_DISCONNECT_NOTICE_ID)) && !userNotification.IsDeleted && !TextUtils.IsEmpty(userNotification.NotificationTypeId))
                                                {
                                                    CustomerBillingAccount selected = CustomerBillingAccount.FindByAccNum(userNotification.AccountNum);
                                                    if (selected.billingDetails != null)
                                                    {
                                                        SummaryDashBoardDetails cached = JsonConvert.DeserializeObject<SummaryDashBoardDetails>(selected.billingDetails);
                                                        double amtDue = 0.00;
                                                        if (cached.AccType == "2")
                                                        {
                                                            amtDue = double.Parse(cached.AmountDue) * -1;
                                                        }
                                                        else
                                                        {
                                                            amtDue = double.Parse(cached.AmountDue);
                                                        }

                                                        if (amtDue <= 0.00)
                                                        {
                                                            userNotification.IsDeleted = true;
                                                            Notifications.Models.UserNotificationData temp = new Notifications.Models.UserNotificationData();
                                                            temp.Id = userNotification.Id;
                                                            temp.NotificationType = userNotification.NotificationType;
                                                            ToBeDeleteList.Add(temp);
                                                        }
                                                    }
                                                }
                                            }
                                            catch (System.Exception ene)
                                            {
                                                Utility.LoggingNonFatalError(ene);
                                            }

                                            int newRecord = UserNotificationEntity.InsertOrReplace(userNotification);
                                        }
                                        this.mView.ShowView();
                                        this.mView.ClearAdapter();
                                        this.ShowFilteredList();
                                        MyTNBAccountManagement.GetInstance().SetIsNotificationServiceCompleted(true);
                                    }
                                    else
                                    {
                                        this.mView.ShowRefreshView(true, response.Data.RefreshMessage, response.Data.RefreshBtnText);
                                        MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                                    }
                                }
                                else if(response != null && response.Data != null && response.Data.ErrorCode == "8400")
                                {
                                    MyTNBAccountManagement.GetInstance().SetIsNotificationServiceMaintenance(true);
                                    // TODO: Show Maintenance Screen
                                    string contentTxt = "";
                                    if (response != null && response.Data != null)
                                    {
                                        if (!string.IsNullOrEmpty(response.Data.DisplayMessage))
                                        {
                                            contentTxt = response.Data.DisplayMessage;
                                        }
                                    }
                                    this.mView.ShowRefreshView(false, contentTxt, "");
                                }
                                else
                                {
                                    MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                                    this.mView.ShowRefreshView(true, response.Data.RefreshMessage, response.Data.RefreshBtnText);
                                }

                                this.mView.HideQueryProgress();
                            }
                            catch (ApiException apiException)
                            {

                                this.mView.HideQueryProgress();
                                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                                this.mView.ShowRefreshView(true, null, null);
                                Utility.LoggingNonFatalError(apiException);
                            }
                            catch (Exception e)
                            {
                                this.mView.HideQueryProgress();
                                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                                this.mView.ShowRefreshView(true, null, null);
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
                        this.mView.ShowRefreshView(true, null, null);
                    }

                }
                else
                {
                    this.mView.HideQueryProgress();
                    MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                    this.mView.ShowRefreshView(true, null, null);
                }

                if (ToBeDeleteList != null && ToBeDeleteList.Count > 0)
                {
                    _ = OnBatchDeleteNotifications(ToBeDeleteList);
                }
            }
            catch (ApiException apiException)
            {

                this.mView.HideQueryProgress();
                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                this.mView.ShowRefreshView(true, null, null);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {

                this.mView.HideQueryProgress();
                MyTNBAccountManagement.GetInstance().SetIsNotificationServiceFailed(true);
                this.mView.ShowRefreshView(true, null, null);
                Utility.LoggingNonFatalError(e);
            }

        }

        public void Start()
        {
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
                                        if (userNotificationData.ODNBatchSubcategory == "ODNAsBATCH")
                                        {
                                            listOfNotifications.Add(UserNotificationData.Get(entity, notificationTypesEntity.Code));
                                        }
                                        else
                                        {
                                            if (UserEntity.GetActive().Email.Equals(userNotificationData.Email) &&
                                            MyTNBAccountManagement.GetInstance().IsAccountNumberExist(userNotificationData.AccountNum))
                                            {
                                                listOfNotifications.Add(UserNotificationData.Get(entity, notificationTypesEntity.Code));
                                            }
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

                ////MOCK DATA FOR TESTING - DO NOT DELETE
                //listOfNotifications.Clear();
                //UserNotificationData data = new UserNotificationData();
                //data.BCRMNotificationTypeId = Constants.BCRM_NOTIFICATION_PAYMENT_FAILED_ID;
                //data.CreatedDate = "9/1/2019 5:00:00 PM";
                //data.IsRead = false;
                //data.Title = "New Bill";
                //data.Message = "Dear customer, your TNB bill RM5…";
                //listOfNotifications.Add(data);

                //data = new UserNotificationData();
                //data.BCRMNotificationTypeId = Constants.BCRM_NOTIFICATION_PAYMENT_SUCCESS_ID;
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

        private async Task OnBatchDeleteNotifications(List<Notifications.Models.UserNotificationData> accountList)
        {
            try
            {
                if (accountList != null && accountList.Count > 0)
                {
                    NotificationApiImpl notificationAPI = new NotificationApiImpl();
                    UserNotificationDeleteResponse notificationDeleteResponse = await notificationAPI.DeleteUserNotification<UserNotificationDeleteResponse>(new UserNotificationDeleteRequest(accountList));

                    if (notificationDeleteResponse != null && notificationDeleteResponse.Data != null && notificationDeleteResponse.Data.ErrorCode == "7200")
                    {

                    }
                }
            }
            catch (System.OperationCanceledException cancelledException)
            {
                Utility.LoggingNonFatalError(cancelledException);
            }
            catch (ApiException apiException)
            {
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception unknownException)
            {
                Utility.LoggingNonFatalError(unknownException);
            }
        }
    }
}
