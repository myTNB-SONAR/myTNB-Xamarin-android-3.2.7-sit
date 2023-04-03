using System;
using System.Collections.Generic;
using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.MultipleAccountPayment.Models;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class ProfileMenuContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show notification
            /// </summary>
            void ShowNotifications();

            /// <summary>
            /// Show notification progress dialog
            /// </summary>
            void ShowNotificationsProgressDialog();

            /// <summary>
            /// HIde notification progress dialog
            /// </summary>
            void HideNotificationsProgressDialog();

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
            /// Shows user data
            /// </summary>
            /// <param name="user">UserEntity</param>
            /// <param name="numOfCards">integer</param>
            void ShowUserData(UserEntity user, int numOfCards);

            /// <summary>
            /// Shows manage credit cards / debit cards screen
            /// </summary>
            void ShowManageCards(List<CreditCardData> cardsList);

            //void ShowRegisterCard(CreditCard creditCardData);


            /// <summary>
            /// Shows Logout Screen
            /// </summary>
            void ShowLogout();

            /// <summary>
            /// Show logout error message from api response
            /// </summary>
            /// <param name="message">string</param>
            void ShowLogoutErrorMessage(string message);

            /// <summary>
            /// Enable manage cards button
            /// </summary>
            //void EnableManageCards();

            /// <summary>
            /// Disable manage cards button
            /// </summary>
            //void DisableManageCards();

            /// <summary>
            /// Show removed card success
            /// </summary>
            /// <param name="creditCard">CreditCardData</param>
            /// <param name="numOfCards">integer</param>
            void ShowRemovedCardSuccess(CreditCardData creditCard, int numOfCards);

            void ShowCCErrorSnakebar();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to navigate to notification
            /// </summary>
            /// <param name="deviceId">string</param>
            void OnNotification(string deviceId);

            /// <summary>
            /// Action to logout
            /// </summary>
            void OnLogout();
        }
    }
}
