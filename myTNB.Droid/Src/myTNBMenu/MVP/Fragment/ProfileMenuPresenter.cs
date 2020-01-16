﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using Android.Util;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MakePayment.Api;
using myTNB_Android.Src.MakePayment.Models;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class ProfileMenuPresenter : ProfileMenuContract.IUserActionsListener
    {
        private ProfileMenuContract.IView mView;
        private readonly string TAG = typeof(ProfileMenuPresenter).Name;
        private List<CreditCardData> cardList = new List<CreditCardData>();
        CancellationTokenSource cts;

        public ProfileMenuPresenter(ProfileMenuContract.IView mView)
        {
            this.mView = mView;
        }

        public void OnNotification(string deviceId)
        {
            LoadAllUserPrefNotifications(deviceId);
        }

        public void OnManageCards()
        {
            this.mView.ShowManageCards(cardList);
        }

        public void Start()
        {
            LoadAccount();
        }

        private async void LoadAccount()
        {
            if (mView.IsActive())
            {
                this.mView.ShowNotificationsProgressDialog();
            }

            UserEntity userEntity = UserEntity.GetActive();
            try
            {
                var cardsApiResponse = await ServiceApiImpl.Instance.GetRegisteredCards(new RegisteredCardsRequest(true));

                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }

                if (cardsApiResponse.IsSuccessResponse())
                {
                    foreach (CreditCard card in cardsApiResponse.GetData())
                    {
                        cardList.Add(CreditCardData.Copy(card));
                    }

                    this.mView.ShowUserData(userEntity, cardList.Count);

                    if (cardList.Count > 0)
                    {
                        this.mView.EnableManageCards();
                    }
                    else
                    {
                        this.mView.DisableManageCards();
                    }
                }
                else
                {
                    this.mView.DisableManageCards();
                    this.mView.ShowUserData(userEntity, 0);
                }

            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.DisableManageCards();
                this.mView.ShowUserData(userEntity, 0);
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.DisableManageCards();
                this.mView.ShowUserData(userEntity, 0);
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.DisableManageCards();
                this.mView.ShowUserData(userEntity, 0);
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }

        }

        private async void LoadAllUserPrefNotifications(string deviceId)
        {
            if (mView.IsActive())
            {
                this.mView.ShowNotificationsProgressDialog();
            }

            try
            {
                var notificationTypesApi = await ServiceApiImpl.Instance.UserNotificationTypePreferences(new MyTNBService.Request.BaseRequest());

                if (notificationTypesApi.IsSuccessResponse())
                {
                    var notificationChannelApi = await ServiceApiImpl.Instance.UserNotificationChannelPreferences(new MyTNBService.Request.BaseRequest());

                    if (mView.IsActive())
                    {
                        this.mView.HideNotificationsProgressDialog();
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

                        this.mView.ShowNotifications();
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
                Log.Debug(TAG, "Cancelled Exception");
                // ADD OPERATION CANCELLED HERE
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                Log.Debug(TAG, "Stack " + e.StackTrace);
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }


        }

        public async void OnLogout(string deviceId)
        {
            if (mView.IsActive())
            {
                this.mView.ShowNotificationsProgressDialog();
            }

            UserEntity userEntity = UserEntity.GetActive();
            try
            {
                if (userEntity != null)
                {
                    var logoutResponse = await ServiceApiImpl.Instance.LogoutUser(new LogoutUserRequest());

                    if (mView.IsActive())
                    {
                        this.mView.HideNotificationsProgressDialog();
                    }

                    if (logoutResponse.IsSuccessResponse())
                    {
                        UserEntity.RemoveActive();
                        UserRegister.RemoveActive();
                        CustomerBillingAccount.RemoveActive();
                        NotificationFilterEntity.RemoveAll();
                        UserNotificationEntity.RemoveAll();
                        SubmittedFeedbackEntity.Remove();
                        SMUsageHistoryEntity.RemoveAll();
                        UsageHistoryEntity.RemoveAll();
                        BillHistoryEntity.RemoveAll();
                        PaymentHistoryEntity.RemoveAll();
                        REPaymentHistoryEntity.RemoveAll();
                        AccountDataEntity.RemoveAll();
                        SummaryDashBoardAccountEntity.RemoveAll();
                        SelectBillsEntity.RemoveAll();
                        LanguageUtil.SetIsLanguageChanged(false);
                        this.mView.ShowLogout();
                    }
                    else
                    {
                        this.mView.ShowLogoutErrorMessage(logoutResponse.Response.DisplayMessage);
                    }
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }

        }

        public void UpdateCardList(CreditCardData creditCard)
        {
            cardList.Remove(cardList.Single(item => item.Id.Equals(creditCard.Id)));
            this.mView.ShowRemovedCardSuccess(creditCard, cardList.Count);
            if (cardList.Count > 0)
            {
                this.mView.EnableManageCards();
            }
            else
            {
                this.mView.DisableManageCards();
            }
        }
    }
}
