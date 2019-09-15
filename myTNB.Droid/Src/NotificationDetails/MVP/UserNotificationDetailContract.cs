using System;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Model;
using Refit;

namespace myTNB_Android.Src.NotificationDetails.MVP
{
    public class UserNotificationDetailContract
    {
        public interface IView
        {
            void ViewBill();
            void PayNow(AccountData mSelectedAccountData);
            void ContactUs(WeblinkEntity entity);
            void ViewUsage();
            void ViewDetails(AccountData mSelectedAccountData, AccountChargeModel accountChargeModel);
            void ShowLoadingScreen();
            void HideLoadingScreen();

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
        }
    }
}
