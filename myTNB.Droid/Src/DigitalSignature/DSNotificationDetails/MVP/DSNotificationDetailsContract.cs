﻿using System;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.Base.MVP;
using Refit;
using myTNB.AndroidApp.Src.NotificationDetails.Models;

namespace myTNB.AndroidApp.Src.DigitalSignature.DSNotificationDetails.MVP
{
    public class DSNotificationDetailsContract
    {
        public interface IView
        {
            /// <summary>
            /// Setting Up Layout
            /// </summary>
            void SetUpViews();

            void ReturnToDashboard();
            void HideLoadingScreen();

            void ShowLoadingScreen();

            /// <summary>
            /// Show notification list as deleted
            /// </summary>
            void ShowNotificationListAsDeleted();

            /// <summary>
            /// Shows a cancelled exception with an option to retry
            /// </summary>
            /// <param name="operationCanceledException">the returned exception</param>
            void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException);

            /// <summary>
            /// Shows an api exception with an option to retry
            /// </summary>
            /// <param name="apiException">the returned exception</param>
            void ShowRetryOptionsApiException(ApiException apiException);

            /// <summary>
            /// Shows an unknown exception with an option to retry
            /// </summary>
            /// <param name="exception">the returned exception</param>
            void ShowRetryOptionsUnknownException(Exception exception);

            void SetUpVerifyNowView();

            void SetUpDynamicView();

            void NavigateToIdentityVerification();

            void NavigateToExternalBrowser(string url);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            NotificationDetailModel GetNotificationDetailModel();

            void DeleteNotificationDetail(NotificationDetails.Models.NotificationDetails notificationDetails);

            void EvaluateDetail(NotificationDetails.Models.NotificationDetails notificationDetails);
        }
    }
}
