using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.Base.MVP;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.ManageCards.Models;
using myTNB.AndroidApp.Src.MultipleAccountPayment.Models;
using Refit;

namespace myTNB.AndroidApp.Src.MyLearnMoreAboutTnb.MVP
{
    public class MyLearnMoreContract
    {
        public interface IView : IBaseView<IUserActionsListener>
        {
           
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

        }
    }
}
