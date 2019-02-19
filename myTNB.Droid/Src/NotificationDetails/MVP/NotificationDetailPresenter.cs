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
using System.Net;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.NotificationDetails.Api;
using Refit;
using System.Net.Http;
using System.Threading;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.NotificationDetails.MVP
{
    public class NotificationDetailPresenter : NotificationDetailContract.IUserActionsListener
    {

        private NotificationDetailContract.IView mView;
        CancellationTokenSource cts;

        public NotificationDetailPresenter(NotificationDetailContract.IView mView )
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void OnPay()
        {
            throw new NotImplementedException();
        }

        public async void OnRemoveNotification(NotificationDetails.Models.NotificationDetails notificationDetails)
        {
            if (mView.IsActive()) {
            this.mView.ShowRemovingProgress();
            }
            cts = new CancellationTokenSource();
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
#if DEBUG 
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            var notificationApi = RestService.For<INotificationDetailsApi>(httpClient);
#else
            var notificationApi = RestService.For<INotificationDetailsApi>(Constants.SERVER_URL.END_POINT);
#endif

            try
            {
                var notificationDeleteResponse = await notificationApi.DeleteUserNotificationV2(new Requests.NotificationDetailsDeleteRequestV2()
                {
                    ApiKeyID = Constants.APP_CONFIG.API_KEY_ID,
                    NotificationId = notificationDetails.Id,
                    NotificationType = notificationDetails.NotificationType,
                    Email = UserEntity.GetActive().Email,
                    DeviceId = this.mView.GetDeviceId(),
                    SSPUserId = UserEntity.GetActive().UserID
                } , cts.Token);

                if (mView.IsActive())
                {
                    this.mView.HideRemovingProgress();
                }

                if (!notificationDeleteResponse.Data.IsError)
                {
                    UserNotificationEntity.UpdateIsDeleted(notificationDetails.Id , true);
                    this.mView.ShowNotificationListAsDeleted();
                }
                else
                {
                    this.mView.ShowRetryOptionsApiException(null);
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideRemovingProgress();
                }
                // ADD OPERATION CANCELLED HERE
                this.mView.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideRemovingProgress();
                }
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.mView.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideRemovingProgress();
                }
                // ADD UNKNOWN EXCEPTION HERE
                this.mView.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }

        }

        public void OnViewDetails()
        {
            
        }

        public void Start()
        {
            // NO IMPL
            this.mView.ShowAccountNumber();
        }

    }
}