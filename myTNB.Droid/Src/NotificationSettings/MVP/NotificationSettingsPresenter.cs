using Android.Text;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.SelectNotification.Models;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.NotificationSettings.MVP
{
    public class NotificationSettingsPresenter : NotificationSettingsContract.IUserActionsListener
    {

        private NotificationSettingsContract.IView mView;

        public NotificationSettingsPresenter(NotificationSettingsContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public async void OnChannelItemClick(NotificationChannelUserPreference item, int position)
        {
            if (item.PreferenceMode == "M")
            {
                return;
            }
            bool isOpted = !item.IsOpted;
            try
            {
                var channelApi = await ServiceApiImpl.Instance.SaveUserNotificationChannelPreference(new SaveUserNotificationChannelPreferenceRequest(item.Id, item.MasterId, isOpted.ToString()));

                if (channelApi.IsSuccessResponse())
                {
                    // TODO : SHOW UPDATE ISOPTED ITEM

                    item.IsOpted = isOpted;
                    if (TextUtils.IsEmpty(item.Id))
                    {
                        item.Id = channelApi.GetData().Id;
                        UserNotificationChannelEntity.UpdateIsOpted(channelApi.GetData().Id, item.Code, isOpted);
                    }
                    else
                    {
                        UserNotificationChannelEntity.UpdateIsOpted(item.Code, isOpted);
                    }
                    this.mView.ShowSuccessUpdatedNotificationChannel(item, position);
                }
                else
                {
                    this.mView.ShowRetryOptionsApiException(null, item, position);
                }
            }
            catch (System.OperationCanceledException e)
            {
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e, item, position);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException, item, position);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e, item, position);
                Utility.LoggingNonFatalError(e);
            }
        }

        public async void OnTypeItemClick(NotificationTypeUserPreference item, int position, string deviceId)
        {
            if (item.PreferenceMode == "M")
            {
                return;
            }
            bool isOpted = !item.IsOpted;
            try
            {
                var typeApi = await ServiceApiImpl.Instance.SaveUserNotificationTypePreference(new SaveUserNotificationTypePreferenceRequest(item.Id, item.MasterId, isOpted.ToString()));


                if (typeApi.IsSuccessResponse())
                {
                    // TODO : SHOW UPDATE ISOPTED ITEM
                    item.IsOpted = isOpted;
                    NotificationTypesEntity masterEntity = NotificationTypesEntity.GetById(item.MasterId);
                    if (masterEntity != null)
                    {
                        if (TextUtils.IsEmpty(item.Id))
                        {
                            item.Id = typeApi.GetData().Id;
                            UserNotificationTypesEntity.UpdateIsOpted(typeApi.GetData().Id, masterEntity.Code, isOpted);
                        }
                        else
                        {
                            UserNotificationTypesEntity.UpdateIsOpted(masterEntity.Code, isOpted);
                        }

                    }

                    this.mView.ShowSuccessUpdatedNotificationType(item, position);
                }
                else
                {
                    this.mView.ShowRetryOptionsApiException(null, item, position);
                }
            }
            catch (System.OperationCanceledException e)
            {
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e, item, position);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException, item, position);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e, item, position);
                Utility.LoggingNonFatalError(e);
            }


        }

        public void OnNotification(string deviceId)
        {
            LoadAllUserPrefNotifications(deviceId);
        }

        private async void LoadAllUserPrefNotifications(string deviceId)
        {
            if (mView.IsActive())
            {
                this.mView.ShowProgressDialog();
            }
            try
            {
                var notificationTypesApi = await ServiceApiImpl.Instance.UserNotificationTypePreferences(new MyTNBService.Request.BaseRequest());

                if (notificationTypesApi.IsSuccessResponse())
                {
                    var notificationChannelApi = await ServiceApiImpl.Instance.UserNotificationChannelPreferences(new MyTNBService.Request.BaseRequest());
                    
                    if (mView.IsActive())
                    {
                        this.mView.HideShowProgressDialog();
                    }

                    if (notificationChannelApi.IsSuccessResponse())
                    {
                        UserNotificationTypesEntity.RemoveActive();
                        UserNotificationChannelEntity.RemoveActive();

                        foreach (UserNotificationType notificationType in notificationTypesApi.GetData())
                        {
                            int newRecord = UserNotificationTypesEntity.InsertOrReplace(notificationType);
                            Console.WriteLine(string.Format("New Type Created {0}", newRecord));
                        }

                        foreach (UserNotificationChannel notificationChannel in notificationChannelApi.GetData())
                        {
                            int newRecord = UserNotificationChannelEntity.InsertOrReplace(notificationChannel);
                            Console.WriteLine(string.Format("New Channel Created {0}", newRecord));
                        }

                        ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                        List<UserNotificationTypesEntity> typesList = UserNotificationTypesEntity.ListAllActive();
                        List<NotificationTypeUserPreference> typeUserPrefList = new List<NotificationTypeUserPreference>();
                        foreach (UserNotificationTypesEntity type in typesList)
                        {
                            if (type.ShowInPreference)
                            {
                                typeUserPrefList.Add(NotificationTypeUserPreference.Get(type));
                            }
                        }
                        this.mView.ShowNotificationTypesList(typeUserPrefList);
                    }
                    else
                    {
                        // SHOW ERROR
                        this.mView.ShowRetryOptionsApiException(null);
                    }
                }
                else
                {
                    // SHOW ERROR
                    this.mView.ShowRetryOptionsApiException(null);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideShowProgressDialog();
                }
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }


        }

        public void Start()
        {
            // LOAD TYPES / CHANNELS NOTIFICATIONS LIST
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                List<UserNotificationChannelEntity> channelsList = UserNotificationChannelEntity.ListAllActive();
                List<UserNotificationTypesEntity> typesList = UserNotificationTypesEntity.ListAllActive();

                List<NotificationChannelUserPreference> channelUserPrefList = new List<NotificationChannelUserPreference>();
                List<NotificationTypeUserPreference> typeUserPrefList = new List<NotificationTypeUserPreference>();


                foreach (UserNotificationChannelEntity channel in channelsList)
                {
                    if (channel.ShowInPreference)
                    {
                        channelUserPrefList.Add(NotificationChannelUserPreference.Get(channel));
                    }
                }

                foreach (UserNotificationTypesEntity type in typesList)
                {
                    if (type.ShowInPreference)
                    {
                        typeUserPrefList.Add(NotificationTypeUserPreference.Get(type));
                    }
                }

                this.mView.ShowNotificationTypesList(typeUserPrefList);
                //this.mView.ShowNotificationChannelList(channelUserPrefList);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }
    }
}