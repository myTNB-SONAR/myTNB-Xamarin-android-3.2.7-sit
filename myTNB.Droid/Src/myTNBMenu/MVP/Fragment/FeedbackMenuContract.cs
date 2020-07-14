using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Database.Model;
using Refit;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.MVP.Fragment
{
    public class FeedbackMenuContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            //void ShowCallUi();

            /// <summary>
            /// Show progress dialog
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hide progress dialog
            /// </summary>
            void HideProgressDialog();

            /// <summary>
            /// Show billing related
            /// </summary>
            void ShowBillingPayment();

            /// <summary>
            /// Show faulty street lamps
            /// </summary>
            void ShowFaultyStreetLamps();

            /// <summary>
            /// Show others
            /// </summary>
            void ShowOthers();

            /// <summary>
            /// Shows at start the initial feedback category
            /// </summary>
            /// <param name="feedbackCategory"></param>
            void ShowFeedbackMenu(List<FeedbackCategoryEntity> feedbackCategory);

            /// <summary>
            /// Show submitted feedback
            /// </summary>
            void ShowSubmittedFeedback();

            /// <summary>
            /// Show submitted feedback count NOT USED
            /// </summary>
            /// <param name="count">integer</param>
            void ShowSubmittedFeedbackCount(int count);

            /// <summary>
            /// Returns unique device id
            /// </summary>
            /// <returns></returns>
            String GetDeviceId();

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

            void ShowSubmitNewEnquiry();

            void ShowSubmittedFeedbackNew();
        }

        public interface IUserActionsListener : IBasePresenter
        {
            //void OnCallUi();
            /// <summary>
            /// Action to navigate to BillRelated
            /// </summary>
            void OnBillingPayment();

            /// <summary>
            /// Action navigate to New Submit
            /// </summary>
            void OnSubmitNewEnquiry();

            /// <summary>
            /// Action to navigate to FaultyStreetLamps
            /// </summary>
            void OnFaultyStreetLamps();

            /// <summary>
            /// Action to navigate to Others 
            /// </summary>
            void OnOthers();

            /// <summary>
            /// Action to navigate to submitted feedback
            /// </summary>
            void OnSubmittedFeedback();

            /// <summary>
            /// Action to retry getting feedback
            /// </summary>
            void OnRetry();

            /// <summary>
            /// Action on resume
            /// </summary>
            void OnResume();

            void onSubmittedFeedbackNew();


        }
    }
}