using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.SelectNotification.Models;
using System.Net;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Database.Model;
using Refit;
using System.Net.Http;
using myTNB_Android.Src.NotificationSettings.Api;
using System.Threading;
using Android.Text;

namespace myTNB_Android.Src.NotificationSettings.MVP
{
    public class NotificationSettingsPresenter : NotificationSettingsContract.IUserActionsListener
    {

        private NotificationSettingsContract.IView mView;
        CancellationTokenSource cts;

        public NotificationSettingsPresenter(NotificationSettingsContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public async void OnChannelItemClick(NotificationChannelUserPreference item , int position )
        {
            if (item.PreferenceMode == "M")
            {
                return;
            }
            cts = new CancellationTokenSource();
            bool isOpted = !item.IsOpted;
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IUserPreference>(httpClient);
#else
            var api = RestService.For<IUserPreference>(Constants.SERVER_URL.END_POINT);
#endif
            UserEntity userEntity = UserEntity.GetActive();
            try
            {
                var channelApi = await api.SaveUserNotificationChannelPreference(new Requests.SaveUserNotificationChannelPreferenceRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    Id = item.Id,
                    Email = userEntity.Email,
                    ChannelTypeId = item.MasterId,
                    IsOpted = isOpted
                }, cts.Token);

                if (!channelApi.Data.IsError)
                {
                    // TODO : SHOW UPDATE ISOPTED ITEM

                    item.IsOpted = isOpted;
                    if (TextUtils.IsEmpty(item.Id))
                    {
                        item.Id = channelApi.Data.Data.Id;
                        UserNotificationChannelEntity.UpdateIsOpted(channelApi.Data.Data.Id, item.Code, isOpted);
                    }
                    else
                    {
                        UserNotificationChannelEntity.UpdateIsOpted(item.Code, isOpted);
                    }
                    this.mView.ShowSuccessUpdatedNotificationChannel(item , position);
                }
                else
                {
                    this.mView.ShowRetryOptionsApiException(null , item , position);
                }
            }
            catch (System.OperationCanceledException e)
            {
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e, item, position);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException, item, position);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e, item, position);
            }
        }

        public async void OnTypeItemClick(NotificationTypeUserPreference item, int position, string deviceId)
        {
            if (item.PreferenceMode == "M")
            {
                return;
            }
            cts = new CancellationTokenSource();
            bool isOpted = !item.IsOpted;
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IUserPreference>(httpClient);
#else
            var api = RestService.For<IUserPreference>(Constants.SERVER_URL.END_POINT);
#endif
            UserEntity userEntity = UserEntity.GetActive();
            try
            {
                var typeApi = await api.SaveUserNotificationTypePreference(new Requests.SaveUserNotificationTypePreferenceRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    Id = item.Id,
                    Email = userEntity.Email,
                    DeviceId = deviceId,
                    NotificationTypeId = item.MasterId,
                    IsOpted = isOpted
                } , cts.Token);

                if (!typeApi.Data.IsError)
                {
                    // TODO : SHOW UPDATE ISOPTED ITEM
                    item.IsOpted = isOpted;
                    NotificationTypesEntity masterEntity = NotificationTypesEntity.GetById(item.MasterId);
                    if (masterEntity != null)
                    {
                        if (TextUtils.IsEmpty(item.Id))
                        {
                            item.Id = typeApi.Data.Data.Id;
                            UserNotificationTypesEntity.UpdateIsOpted(typeApi.Data.Data.Id, masterEntity.Code, isOpted);
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
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException, item, position);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e, item, position);
            }


        }

        public void Start()
        {
            // LOAD TYPES / CHANNELS NOTIFICATIONS LIST

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
            this.mView.ShowNotificationChannelList(channelUserPrefList);
        }
    }
}