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
using myTNB_Android.Src.Base.Models;
using Refit;

namespace myTNB_Android.Src.SelectSubmittedFeedback.MVP
{
    public class SelectSubmittedFeedbackContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {

            /// <summary>
            /// Show progress dialog
            /// </summary>
            void ShowProgressDialog();

            /// <summary>
            /// Hide progress dialog
            /// </summary>
            void HideProgressDialog();

            /// <summary>
            /// Clear adapter list
            /// </summary>
            void ClearList();

            /// <summary>
            /// Show adapter list
            /// </summary>
            /// <param name="list">List<paramref name="SubmittedFeedback"/></param>
            void ShowList(List<SubmittedFeedback> list);

            /// <summary>
            /// Show bill related feedback details 
            /// </summary>
            /// <param name="submittedFeedback">SubmittedFeedbackDetails</param>
            void ShowFeedbackDetailsBillRelated(SubmittedFeedbackDetails submittedFeedback );

            /// <summary>
            /// Show faulty street lamps feedback details
            /// </summary>
            /// <param name="submittedFeedback">SubmittedFeedbackDetails</param>
            void ShowFeedbackDetailsFaultyLamps(SubmittedFeedbackDetails submittedFeedback);

            /// <summary>
            /// Show others feedback details
            /// </summary>
            /// <param name="submittedFeedback"></param>
            void ShowFeedbackDetailsOthers(SubmittedFeedbackDetails submittedFeedback);

            /// <summary>
            /// Show start loading
            /// </summary>
            void ShowStartLoading();

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
            /// Shows an unknown exception with an option to retry
            /// </summary>
            /// <param name="exception">the returned exception</param>
            void ShowBCRMDownException(String msg);
        }

        public interface IUserActionsListener : IBasePresenter
        {
            /// <summary>
            /// Action to start loading
            /// </summary>
            /// <param name="deviceId">string</param>
            void OnStartShowLoading(string deviceId);

            /// <summary>
            /// Action to select submitted feedback
            /// </summary>
            /// <param name="submittedFeedback">SubmittedFeedback</param>
            void OnSelect(SubmittedFeedback submittedFeedback);
        }
    }
}