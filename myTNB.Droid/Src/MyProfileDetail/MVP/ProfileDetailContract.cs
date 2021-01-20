using System;
using System.Collections.Generic;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.MultipleAccountPayment.Models;
using Refit;

namespace myTNB_Android.Src.MyProfileDetail.MVP
{
    public class ProfileDetailContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Shows a snackbar after success resend email verification
            /// </summary>
            void ShowEmailUpdateSuccess(string message);

            /// <summary>
            /// Shows error message
            /// </summary>
            /// <param name="errorMessage"></param>
            void ShowError(string errorMessage);

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

            /// <summary>
            /// Show get code progress dialog
            /// </summary>
            void ShowGetCodeProgressDialog();

            /// <summary>
            /// Hide get code progress dialog
            /// </summary>
            void HideGetCodeProgressDialog();

            /// <summary>
            /// Shows an cancelled exception with an option to retry
            /// </summary>
            /// <param name="operationCanceledException"></param>
            void ShowRetryOptionsCodeCancelledException(System.OperationCanceledException operationCanceledException);

            /// <summary>
            /// Shows an api exception with an option to retry
            /// </summary>
            /// <param name="apiException"></param>
            void ShowRetryOptionsCodeApiException(ApiException apiException);

            /// <summary>
            /// Shows an unknown exception with an option to retry
            /// </summary>
            /// <param name="exception"></param>
            void ShowRetryOptionsCodeUnknownException(Exception exception);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="email"></param>
            void ResendEmailVerify(string apiKeyId, string email);
        }
    }
}
