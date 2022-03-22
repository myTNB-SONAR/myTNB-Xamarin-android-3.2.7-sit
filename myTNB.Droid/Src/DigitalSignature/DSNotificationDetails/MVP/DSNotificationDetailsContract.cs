using System;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Base.MVP;
using Refit;

namespace myTNB_Android.Src.DigitalSignature.DSNotificationDetails.MVP
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

            /// <summary>
            /// Rendering the UI content
            /// </summary>
            void RenderContent();

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
        }
    }
}
