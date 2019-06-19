using Android.Util;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class MoreFragmentPresenter : MoreFragmentContract.IUserActionsListener
    {
        private MoreFragmentContract.IView mView;
        private readonly string TAG = typeof(MoreFragmentPresenter).Name;
        CancellationTokenSource cts;

        public MoreFragmentPresenter(MoreFragmentContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnNotification(string deviceId)
        {
            // TODO : LOAD USER NOTIFICATIONS TYPE / CHANNEL

            LoadAllUserPrefNotifications(deviceId);
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

        public void OnTermsAndConditions()
        {
            this.mView.ShowTermsAndConditions();
        }

        public void Start()
        {
            // NO IMPL
        }

        public void OnMyAccount()
        {
            this.mView.ShowMyAccount();
        }


        public void OnFindUs()
        {
            this.mView.ShowFindUs();
        }

        public void OnCallUs()
        {
            if (WeblinkEntity.HasRecord("TNBCLO"))
            {
                this.mView.ShowCallUs(WeblinkEntity.GetByCode("TNBCLO"));
            }

        }

        public void OnCallUs1()
        {
            if (WeblinkEntity.HasRecord("TNBCLE"))
            {
                this.mView.ShowCallUs(WeblinkEntity.GetByCode("TNBCLE"));
            }
        }

        public void OnUnderstandBill()
        {
            if (WeblinkEntity.HasRecord("BILL"))
            {
                this.mView.ShowUnderstandYourBill(WeblinkEntity.GetByCode("BILL"));
            }
        }

        public void OnFAQ()
        {
            if (WeblinkEntity.HasRecord("FAQ"))
            {
                this.mView.ShowFAQ(WeblinkEntity.GetByCode("FAQ"));
            }
        }

        public void OnShareApp()
        {
            if (WeblinkEntity.HasRecord("DROID"))
            {
                this.mView.ShowShareApp(WeblinkEntity.GetByCode("DROID"));
            }
        }

        public void OnRateUs()
        {
            if (WeblinkEntity.HasRecord("DROID"))
            {
                this.mView.ShowRateUs(WeblinkEntity.GetByCode("DROID"));
            }
        }


    }
}