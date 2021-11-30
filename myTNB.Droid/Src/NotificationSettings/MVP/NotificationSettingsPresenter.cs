using Android.Text;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
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

        public void Start()
        {
            // LOAD TYPES / CHANNELS NOTIFICATIONS LIST
            try
            {
                List<CustomerBillingAccount> smartmeterAccounts = CustomerBillingAccount.SMeterBudgetAccountList();        //smart meter ca                                                                                                                           //energy budget

                ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
                List<UserNotificationChannelEntity> channelsList = UserNotificationChannelEntity.ListAllActive();
                List<UserNotificationTypesEntity> typesList = UserNotificationTypesEntity.ListAllActive();

                List<NotificationChannelUserPreference> channelUserPrefList = new List<NotificationChannelUserPreference>();
                List<NotificationTypeUserPreference> typeUserPrefList = new List<NotificationTypeUserPreference>();
                List<NotificationTypeUserPreference> filtertypeUserPrefList = new List<NotificationTypeUserPreference>();

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
                        if (type.MasterId == "1000020")
                        {
                            if (MyTNBAccountManagement.GetInstance().IsEBUserVerify() && !MyTNBAccountManagement.GetInstance().COMCLandNEM())
                            {
                                typeUserPrefList.Add(NotificationTypeUserPreference.Get(type));
                            }
                        }
                        else if (type.MasterId == "1000028")
                        {
                            if (MyTNBAccountManagement.GetInstance().IsSDUserVerify())
                            {
                                typeUserPrefList.Add(NotificationTypeUserPreference.Get(type));
                            }
                        }
                        else
                        {
                            typeUserPrefList.Add(NotificationTypeUserPreference.Get(type));
                        }
                    }
                }


                this.mView.ShowNotificationTypesList(typeUserPrefList);
                this.mView.ShowNotificationChannelList(channelUserPrefList);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }
    }
}