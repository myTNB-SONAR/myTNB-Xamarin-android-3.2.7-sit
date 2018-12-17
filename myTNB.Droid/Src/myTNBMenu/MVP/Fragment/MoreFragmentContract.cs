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
using Refit;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class MoreFragmentContract 
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// Show notification
            /// </summary>
            void ShowNotifications();

            /// <summary>
            /// Show users account
            /// </summary>
            void ShowMyAccount();

            /// <summary>
            /// Show notification progress dialog
            /// </summary>
            void ShowNotificationsProgressDialog();

            /// <summary>
            /// HIde notification progress dialog
            /// </summary>
            void HideNotificationsProgressDialog();

            /// <summary>
            /// Show terms and condition
            /// </summary>
            void ShowTermsAndConditions();

            /// <summary>
            /// Show find us
            /// </summary>
            void ShowFindUs();

            /// <summary>
            /// Show call us
            /// </summary>
            /// <param name="entity">WeblinkEntity</param>
            void ShowCallUs(WeblinkEntity entity);

            /// <summary>
            /// Show understand your bill
            /// </summary>
            /// <param name="entity">WeblinkEntity</param>
            void ShowUnderstandYourBill(WeblinkEntity entity);

            /// <summary>
            /// Show FAQ
            /// </summary>
            /// <param name="entity">WeblinkEntity</param>
            void ShowFAQ(WeblinkEntity entity);

            /// <summary>
            /// Show share app
            /// </summary>
            /// <param name="entity">WeblinkEntity</param>
            void ShowShareApp(WeblinkEntity entity);

            /// <summary>
            /// Show rate us
            /// </summary>
            /// <param name="entity">WeblinkEntity</param>
            void ShowRateUs(WeblinkEntity entity);

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

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to navigate to notification
            /// </summary>
            /// <param name="deviceId">string</param>
            void OnNotification(string deviceId);

            /// <summary>
            /// Action to navigate to user account
            /// </summary>
            void OnMyAccount();

            /// <summary>
            /// Action to navigate to terms and condition
            /// </summary>
            void OnTermsAndConditions();

            /// <summary>
            /// Action to navigate to find us
            /// </summary>
            void OnFindUs();

            /// <summary>
            /// Action to navigate to call us (Outages & Breakdown)
            /// </summary>
            void OnCallUs();

            /// <summary>
            /// Action to navigate to call us (Billing Enquiries)
            /// </summary>
            void OnCallUs1();

            /// <summary>
            /// Action to navigate to understand your bill
            /// </summary>
            void OnUnderstandBill();

            /// <summary>
            /// Action to navigate to FAQ
            /// </summary>
            void OnFAQ();

            /// <summary>
            /// Action to navigate to share app
            /// </summary>
            void OnShareApp();

            /// <summary>
            /// Action to navigate to rate us
            /// </summary>
            void OnRateUs();


        }
    }
}