
using System;
using Android.Content;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using System.Collections.Generic;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.NotificationDetails.Models;
using myTNB_Android.Src.SSMRTerminate.Api;
using Refit;
using Newtonsoft.Json;

namespace myTNB_Android.Src.DigitalSignature.DSNotificationDetails.MVP
{
    public class DSNotificationDetailsPresenter 
    {
        DSNotificationDetailsContract.IView view;
        private ISharedPreferences mSharedPref;
        SSMRTerminateImpl terminationApi;

        public DSNotificationDetailsPresenter(DSNotificationDetailsContract.IView view, ISharedPreferences mSharedPref)
        {
            this.view = view;
            this.mSharedPref = mSharedPref;
            terminationApi = new SSMRTerminateImpl();
        }

        public void OnInitialize()
        {
            this.view?.SetUpViews();
            OnStart();
        }

        public void OnStart()
        {
            this.view.RenderContent();
        }

        public async void DeleteNotificationDetail(NotificationDetails.Models.NotificationDetails notificationDetails)
        {
            this.view.ShowLoadingScreen();
            try
            {
                List<Notifications.Models.UserNotificationData> selectedNotificationList = new List<Notifications.Models.UserNotificationData>();
                Notifications.Models.UserNotificationData data = new Notifications.Models.UserNotificationData();
                data.Id = notificationDetails.Id;
                data.NotificationType = notificationDetails.NotificationType;
                selectedNotificationList.Add(data);
                UserNotificationDeleteResponse notificationDeleteResponse = await ServiceApiImpl.Instance.DeleteUserNotification(new UserNotificationDeleteRequest(selectedNotificationList));
                if (notificationDeleteResponse.IsSuccessResponse())
                {
                    UserNotificationEntity.UpdateIsDeleted(notificationDetails.Id, true);
                    this.view.ShowNotificationListAsDeleted();
                }
                else
                {
                    this.view.ShowRetryOptionsCancelledException(null);
                }

            }
            catch (System.OperationCanceledException e)
            {
                // ADD OPERATION CANCELLED HERE
                this.view.ShowRetryOptionsCancelledException(e);
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                // ADD HTTP CONNECTION EXCEPTION HERE
                this.view.ShowRetryOptionsApiException(apiException);
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                // ADD UNKNOWN EXCEPTION HERE
                this.view.ShowRetryOptionsUnknownException(e);
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
