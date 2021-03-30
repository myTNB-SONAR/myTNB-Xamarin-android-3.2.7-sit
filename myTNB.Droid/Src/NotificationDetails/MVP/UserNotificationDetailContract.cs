using System;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Model;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.SSMRMeterHistory.MVP;
using Refit;

namespace myTNB_Android.Src.NotificationDetails.MVP
{
    public class UserNotificationDetailContract
    {
        public interface IView
        {
            void PayNow(AccountData mSelectedAccountData);
            void ContactUs(WeblinkEntity entity);
            void ViewUsage(AccountData mSelectedAccountData);
            void ViewDetails(AccountData mSelectedAccountData);
            void SubmitMeterReading(AccountData mSelectedAccountData, SMRActivityInfoResponse SMRAccountActivityInfoResponse);
            void EnableSelfMeterReading(AccountData mSelectedAccountData);
            void ViewBillHistory(AccountData mSelectedAccountData);
            void ViewManageAccess(AccountData mSelectedAccountData);
            void ShowPaymentReceipt(GetPaymentReceiptResponse response);
            void ShowSelectBill(AccountData mSelectedAccountData);
            void ShowPaymentReceiptError();
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

            /// <summary>
            /// Show notification list as deleted
            /// </summary>
            void ShowNotificationListAsDeleted();
        }
    }
}
