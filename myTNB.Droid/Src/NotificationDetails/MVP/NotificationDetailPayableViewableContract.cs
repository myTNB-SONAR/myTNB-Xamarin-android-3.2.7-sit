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
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using Refit;

namespace myTNB_Android.Src.NotificationDetails.MVP
{
    public class NotificationDetailPayableViewableContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            void ShowPayment(AccountData selectedAccount);

            void ShowDetails(AccountData selectedAccount);


            void ShowRetrievalProgress();

            void HideRetrievalProgress();

            void ShowToolbarTitle(string title);


            /// <summary>
            /// 
            /// </summary>
            /// <param name="operationCanceledException"></param>
            void ShowRetryOptionsCancelledException(System.OperationCanceledException operationCanceledException);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="apiException"></param>
            void ShowRetryOptionsApiException(ApiException apiException);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="exception"></param>
            void ShowRetryOptionsUnknownException(Exception exception);

            ///<summary>
            /// Show promotion
            /// </summary>
            void ShowPromotion(NotificationDetails.Models.NotificationDetails notificationDetails);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            void OnPayment(NotificationDetails.Models.NotificationDetails notificationDetails);

            void OnViewDetails(NotificationDetails.Models.NotificationDetails notificationDetails);

            void OnViewPromotion(NotificationDetails.Models.NotificationDetails notificationDetails);

        }
    }
}