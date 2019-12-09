using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using Android.Util;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MakePayment.Api;
using myTNB_Android.Src.MakePayment.Models;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
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

        public void Start()
        {
            LoadAccount();
        }

        private async void LoadAccount()
        {
            cts = new CancellationTokenSource();

            if (mView.IsActive())
            {
                this.mView.ShowNotificationsProgressDialog();
            }

            UserEntity userEntity = UserEntity.GetActive();
#if DEBUG || STUB
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<GetRegisteredCardsApi>(httpClient);
#else
            var api = RestService.For<GetRegisteredCardsApi>(Constants.SERVER_URL.END_POINT);
#endif
            try
            {
                var cardsApiResponse = await api.GetRegisteredCards(new MakePayment.Models.GetRegisteredCardsRequest(
                        Constants.APP_CONFIG.API_KEY_ID,
                        userEntity.Email
                    ));

                if (mView.IsActive())
                {
                    this.mView.HideNotificationsProgressDialog();
                }

                if (!cardsApiResponse.Data.IsError)
                {
                    foreach (CreditCard card in cardsApiResponse.Data.creditCard)
                    {
                        cardList.Add(CreditCardData.Copy(card));
                    }

                    this.mView.ShowUserData(userEntity, cardsApiResponse.Data.creditCard.Count);

                    if (cardList.Count > 0)
                    {
                        //this.mView.EnableManageCards();
                    }
                    else
                    {
                        //this.mView.DisableManageCards();
                    }
                }
                else
                {
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

        private async void LoadAllUserPrefNotifications(string deviceId)
        {
            if (mView.IsActive())
            {
                this.mView.ShowNotificationsProgressDialog();
            }
            cts = new CancellationTokenSource();
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var api = RestService.For<IUserNotificationsApi>(httpClient);

#else
            var api = RestService.For<IUserNotificationsApi>(Constants.SERVER_URL.END_POINT);

#endif

            try
            {
                UserEntity userEntity = UserEntity.GetActive();
                var notificationTypesApi = await api.GetNotificationType(new Requests.UserNotificationTypeRequest()
                {
                    ApiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                    Email = userEntity.Email,
                    DeviceId = deviceId

                }, cts.Token);

                if (!notificationTypesApi.Data.IsError)
                {
                    var notificationChannelApi = await api.GetNotificationChannel(new Requests.UserNotificationChannelRequest()
                    {
                        ApiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                        Email = userEntity.Email,

                    }, cts.Token);

                    if (mView.IsActive())
                    {
                        this.mView.HideNotificationsProgressDialog();
                    }

                    if (!notificationChannelApi.Data.IsError)
                    {
                        UserNotificationTypesEntity.RemoveActive();
                        UserNotificationChannelEntity.RemoveActive();

                        foreach (UserNotificationType notificationType in notificationTypesApi.Data.Data)
                        {
                            int newRecord = UserNotificationTypesEntity.InsertOrReplace(notificationType);
                            Console.WriteLine(string.Format("New Type Created {0}", newRecord));
                        }

                        foreach (UserNotificationChannel notificationChannel in notificationChannelApi.Data.Data)
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
    }
}
