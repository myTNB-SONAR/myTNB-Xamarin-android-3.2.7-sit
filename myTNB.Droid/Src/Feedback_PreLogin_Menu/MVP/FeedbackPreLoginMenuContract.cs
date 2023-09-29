using myTNB_Android.Src.Base.MVP;
using myTNB_Android.Src.Database.Model;
using Refit;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.Feedback_PreLogin_Menu.MVP
{
    public class FeedbackPreLoginMenuContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
            /// <summary>
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// </summary>
            void HideProgressDialog();

            void ShowBillingPayment();

            void ShowFaultyStreetLamps();

            void ShowOthers();

            void ShowSubmittedFeedback();

            void ShowFeedbackMenu(List<FeedbackCategoryEntity> feedbackCategory);

            void ShowSubmittedFeedbackCount(int count);

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

            void OnCheckBCRMDowntime();
        }

        public interface IUserActionsListener : IBasePresenter
        {

            void OnBillingPayment();

            void OnFaultyStreetLamps();

            void OnOthers();

            void OnRetry();

            void OnSubmittedFeedback();

            void OnResume();

            void GetDownTime();
        }
    }
}